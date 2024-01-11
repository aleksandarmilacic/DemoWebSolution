using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace DemoWebSolution.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusProcessor _processor;

        public OrdersController()
        {
            // Initialize Service Bus Client, Sender, and Processor
            string connectionString = "your_service_bus_connection_string";
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender("requestqueue");
            _processor = _client.CreateProcessor("responsequeue");

            // Start listening for responses
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            _processor.StartProcessingAsync();
        }

        [HttpGet("getorders")]
        public async Task<IActionResult> GetOrders(int page, int pageSize)
        {
            var correlationId = Guid.NewGuid().ToString();
            var message = new ServiceBusMessage($"{{ \"CorrelationId\": \"{correlationId}\", \"Page\": {page}, \"PageSize\": {pageSize} }}");

            await _sender.SendMessageAsync(message);

            // For simplicity, we're using a TaskCompletionSource to wait for the response
            // In a real application, you'd use a more robust method to match responses with requests
            var tcs = new TaskCompletionSource<string>();
            RequestResponseHandler.ResponseWaiters[correlationId] = tcs;

            var response = await tcs.Task;

            return Ok(response);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            if (RequestResponseHandler.ResponseWaiters.TryRemove(args.Message.CorrelationId, out var tcs))
            {
                tcs.SetResult(body);
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }

    public static class RequestResponseHandler
    {
        public static ConcurrentDictionary<string, TaskCompletionSource<string>> ResponseWaiters = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
    }
}
}

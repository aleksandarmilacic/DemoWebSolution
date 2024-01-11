using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace DemoWebSolution.OrderService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "your_service_bus_connection_string";
            var client = new ServiceBusClient(connectionString);
            var processor = client.CreateProcessor("requestqueue");
            var sender = client.CreateSender("responsequeue");

            processor.ProcessMessageAsync += async args =>
            {
                var body = args.Message.Body.ToString();
                var request = JsonSerializer.Deserialize<RequestMessage>(body);

                // Simulate fetching data from a database
                var orders = FetchOrdersFromDatabase(request.Page, request.PageSize);

                var responseData = JsonSerializer.Serialize(orders);
                var responseMessage = new ServiceBusMessage(responseData)
                {
                    CorrelationId = args.Message.CorrelationId
                };

                await sender.SendMessageAsync(responseMessage);
                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
            Console.WriteLine("Processor started. Press any key to exit...");
            Console.ReadKey();

            await processor.StopProcessingAsync();
        }

        private static List<Order> FetchOrdersFromDatabase(int page, int pageSize)
        {
            // This is just dummy data.
            List<Order> orders = new List<Order>();
            for (int i = 0; i < pageSize; i++)
            {
                orders.Add(new Order
                {
                    Id = (page - 1) * pageSize + i + 1,
                    Description = $"Order {(page - 1) * pageSize + i + 1}"
                });
            }
            return orders;
        }

        public class RequestMessage
        {
            public string CorrelationId { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}

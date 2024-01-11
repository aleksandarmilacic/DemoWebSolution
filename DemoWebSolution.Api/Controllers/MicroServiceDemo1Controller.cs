using Azure.Messaging.ServiceBus; 
using Microsoft.AspNetCore.Mvc;

namespace DemoWebSolution.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MicroServiceDemo1Controller : ControllerBase
    {
        public MicroServiceDemo1Controller()
        {
        }

        private readonly string connectionString = "<Your Service Bus Connection String>";
        private readonly string queueName = "<Your Queue Name>";

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);

            await sender.SendMessageAsync(new ServiceBusMessage(message));
            return Ok("Message sent");
        }
    }
}

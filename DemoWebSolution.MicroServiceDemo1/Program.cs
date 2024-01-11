using Azure.Messaging.ServiceBus;

namespace DemoWebSolution.MicroServiceDemo1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "<Your Service Bus Connection String>";
            string queueName = "<Your Queue Name>";

            var client = new ServiceBusClient(connectionString);
            var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.WriteLine("Waiting for messages...");
            Console.ReadLine(); // Wait for user input to exit

            await processor.StopProcessingAsync();
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // Process the message here (this is the simulated task)

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error handling message: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}

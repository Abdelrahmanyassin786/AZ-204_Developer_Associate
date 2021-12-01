using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace performancemessagesender
{
    class Program
    {
        const string ServiceBusConnectionString = "<REMOVED>";
        const string TopicName = "salesperformancemessages";

        static void Main(string[] args)
        {

            Console.WriteLine("Sending a message to the Sales Performance topic...");

            SendPerformanceMessageAsync().GetAwaiter().GetResult();

            Console.WriteLine("Message was sent successfully.");

        }

        static async Task SendPerformanceMessageAsync()
        {
            // By leveraging "await using", the DisposeAsync method will be called automatically once the client variable goes out of scope.
            // In more realistic scenarios, you would want to store off a class reference to the client (rather than a local variable) so that it can be used throughout your program.
            await using var client = new ServiceBusClient(ServiceBusConnectionString);

            // Create a sender here
            await using ServiceBusSender sender = client.CreateSender(TopicName);

            // Send messages.
            try
            {
                // Create and send a message here
                string messageBody = "Total sales for Brazil in August: $13m.";
                var message = new ServiceBusMessage(messageBody);
                Console.WriteLine($"Sending message: {messageBody}");
                await sender.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}

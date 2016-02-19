using System;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleServiceBus {
    class Program {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var queueName           = CloudConfigurationManager.GetSetting("QueueName");
            var queueClient         = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var namespaceManager    = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
                Console.WriteLine("Queue didn't exist, so created it");
            }
            
            if (queueClient != null)
            {
                int i = 0;
                while (true) {
                    Console.WriteLine("Enter the Message to send. Blank and enter to Quit...");
                    var message = Console.ReadLine();
                    if (String.IsNullOrEmpty(message)) return;
                    Console.WriteLine("Sending message: {0}", ++i);
                    queueClient.Send(new BrokeredMessage(message));
                    Console.WriteLine("Sent message: {0}", i);
                    
                }

            }
        }
    }
}

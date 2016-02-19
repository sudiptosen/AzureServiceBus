using System;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleServiceBusReader {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("============================================================");
            Console.WriteLine("!!STARTING QUEUE READER!!!");
            Console.WriteLine("============================================================");

            while (true)
            {
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                string queueName = CloudConfigurationManager.GetSetting("QueueName");
                QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, queueName);
                
                NamespaceManager manager = NamespaceManager.CreateFromConnectionString(connectionString);
                if (manager.QueueExists(queueName))
                {
                    OnMessageOptions options    = new OnMessageOptions();
                    options.AutoComplete        = false;
                    options.AutoRenewTimeout    = TimeSpan.FromMinutes(1);
                    Client.OnMessage((message) =>
                    {
                        try
                        {
                            Console.WriteLine("Body: " + message.GetBody<string>());
                            message.Complete();
                        }
                        catch (Exception)
                        {
                            // Indicates a problem, unlock message in queue.
                            message.Abandon();
                        }
                    }, options);

                }
                else
                {
                    Console.WriteLine("Queue doesn't exists");
                }
                System.Threading.Thread.Sleep(2000);

            }
        }
    }
}

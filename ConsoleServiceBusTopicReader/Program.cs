using System;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleServiceBusTopicReader {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("============================================================");
            Console.WriteLine("!!STARTING QUEUE READER!!!");
            Console.WriteLine("============================================================");

            while (true) {
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                string topicName = CloudConfigurationManager.GetSetting("TopicName");
                SubscriptionClient subsClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, CloudConfigurationManager.GetSetting("Subscription"));

                NamespaceManager manager = NamespaceManager.CreateFromConnectionString(connectionString);

                if (manager.TopicExists(topicName)) {

                    OnMessageOptions options = new OnMessageOptions
                    {
                        AutoComplete = false,
                        AutoRenewTimeout = TimeSpan.FromMinutes(1)
                    };

                    subsClient.OnMessage((message) => {
                        try {
                            Console.WriteLine("Body: " + message.GetBody<string>());
                            message.Complete();
                        }
                        catch (Exception) {
                            message.Abandon();
                        }
                    }, options);

                }
                else {
                    Console.WriteLine("Topic doesn't exists.Waiting for topic to come alive...");
                }
                System.Threading.Thread.Sleep(2000);

            }
        }
    }
}

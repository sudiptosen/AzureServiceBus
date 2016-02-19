using System;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleServiceBusTopicWriter {
    class Program {
        static void Main(string[] args) {

            var connectionString    = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var topicName           = CloudConfigurationManager.GetSetting("TopicName");
            var subsList            = CloudConfigurationManager.GetSetting("SubscriptionListCommaSeparated");
            var subscriptions       = subsList != null ? subsList.Split(new[] {","}, StringSplitOptions.None): null;

            if (topicName != null && subscriptions != null)
            {
                Console.WriteLine("============================================================");
                Console.WriteLine("!!WELCOME TO THE TOPIC WRITER!!!");
                Console.WriteLine("============================================================");

                var topicClient     = TopicClient.CreateFromConnectionString(connectionString, topicName);
                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

                // Make sure of the topic
                if (!namespaceManager.TopicExists(topicName)) {
                    namespaceManager.CreateTopic(topicName);
                    Console.WriteLine("Topic didn't exist, so created it");
                }

                // Make sure of the subscription
                if (subscriptions != null)
                {
                    foreach (var subscription in subscriptions)
                    {
                        if (!namespaceManager.SubscriptionExists(topicName, subscription))
                        {
                            namespaceManager.CreateSubscription(topicName, subscription);
                            Console.WriteLine("Created the subscription: {0}", subscription);
                        }
                    }
                }

                if (topicClient != null) {
                    int i = 0;
                    while (true) {
                        Console.WriteLine("Enter the Message to send. Blank and enter to Quit...");
                        var message = Console.ReadLine();
                        if (String.IsNullOrEmpty(message)) return;
                        Console.WriteLine("Sending message: {0}", ++i);
                        topicClient.Send(new BrokeredMessage(message));
                        Console.WriteLine("Sent message: {0}", i);

                    }
                }
            }
            else
            {
                throw new ApplicationException("Error in the Topic configuration");
            }

        }
    }
}

using System;
using RabbitMQ.Client;
using Stienen.RabbitMq;

namespace DataProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultRabbitCore rabbit = new DefaultRabbitCore();
            IModel channel = rabbit.OpenChannel();
            IMessageProcessor processor = new DirectMessageProcessor();
            BasicBinaryMsgConsumer consumer = new BasicBinaryMsgConsumer(channel, "task_queue", processor);
            
            Console.ReadKey();
        }
    }
}

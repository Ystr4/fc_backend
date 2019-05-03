using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Stienen.RabbitMq;

namespace RabbitConsumer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params string[] args)
        {

            if (args.Length > 0 && args[0] == "-console") {
                new RabbitConsumerService().RunConsole(args);
            }
            else {
                ServiceBase.Run(new ServiceBase[] {new RabbitConsumerService()});
            }
        }

        static List<IRabbitMessenger> consumers = new List<IRabbitMessenger>();

        internal static void StartUp(params string[] args)
        {
            DefaultRabbitCore rabbit = new DefaultRabbitCore();
            IModel channel = rabbit.OpenChannel();
            IMessageProcessor processor = new DirectMessageProcessor();
            BasicBinaryMsgConsumer consumer = new BasicBinaryMsgConsumer(channel, "task_queue", processor);
            consumers.Add(consumer);
        }

        internal static void ShutDown()
        {
            foreach (IRabbitMessenger consumer in consumers) {
                consumer.ShutDown();
            }
        }
    }
}

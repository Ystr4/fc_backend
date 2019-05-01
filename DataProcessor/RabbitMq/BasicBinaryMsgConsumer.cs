using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Stienen.Backend {
    public class BasicBinaryMsgConsumer : DefaultRabbitCore {
        private IMessageprocessor _messageProcessor;
        public string ConsumerTag { get; set; }

        public BasicBinaryMsgConsumer(IMessageprocessor processor, string queueName) // , IRabbitConfig config
        {
            _messageProcessor = processor;
            EventingBasicConsumer consumer = new EventingBasicConsumer(base.channel);
            consumer.Received += async (ch, ea) => {
                try {

                    ChangedDataDTO msg = (ChangedDataDTO) DeSerialize(ea.Body);
                    await _messageProcessor.ProcessMessage(msg);

                    base.channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e) {  }
            };

            ConsumerTag = base.channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer, exclusive: true);
        }

        private static Object DeSerialize(byte[] arrBytes)
        {
            using (var memoryStream = new MemoryStream(arrBytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
    }

    public class EventingBasicConsumer {
        public EventingBasicConsumer(IModel channel)
        {
            throw new NotImplementedException();
        }
    }
}
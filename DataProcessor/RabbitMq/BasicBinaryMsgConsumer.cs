using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using fc_backend.DataConverter;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Stienen.Backend {
    public class BasicBinaryMsgConsumer : DefaultRabbitCore {
        private IMessageProcessor _messageProcessor;
        private IModel _channel;
        public string ConsumerTag { get; set; }

        public BasicBinaryMsgConsumer(IModel channel, string queueName, IMessageProcessor processor)
        {
            _channel = channel;
            _messageProcessor = processor;
            EventingBasicConsumer consumer = new EventingBasicConsumer(base.channel);
            consumer.Received += (ch, ea) => {
                try {
                    object obj = DeSerialize(ea.Body);
                    IMessage msg = obj as IMessage;
                    if (msg != null) {
                        _messageProcessor.ProcessMessage(msg);
                    }
                    else {
                        Trace.TraceInformation("Message of unknown type dropped: {0}", obj.GetType());
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex) {
                    Trace.TraceError("Error occurred in: {0}", ex);
                }
            };

            ConsumerTag = channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer, exclusive: true);
        }

        ~BasicBinaryMsgConsumer()
        {
            _channel.Close();
        }

        private static Object DeSerialize(byte[] arrBytes)
        {
            using (var memoryStream = new MemoryStream(arrBytes)) {
                var binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
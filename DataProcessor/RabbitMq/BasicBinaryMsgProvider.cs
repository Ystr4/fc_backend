using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;

namespace Stienen.Backend {
    public class BasicBinaryMsgProvider : DefaultRabbitCore, IRabbitMessenger
    {
        public BasicBinaryMsgProvider() { }

        public void Send<TCommand>(TCommand msg)
            where TCommand : IMessage
        {
            var body = ObjectToByteArray(msg);

            var properties = base.channel.CreateBasicProperties();
            properties.ContentType = "application/object-binary-serialized";
            properties.ContentEncoding = "gzip";
            
            // use default for now
            // properties.Persistent = true; 

            base.channel.BasicPublish(exchange: "",
                                  routingKey: "task_queue",
                                  basicProperties: properties,
                                  body: body);
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) return null;
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
    }
}
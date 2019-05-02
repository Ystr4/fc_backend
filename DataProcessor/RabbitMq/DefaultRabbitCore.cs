using RabbitMQ.Client;

namespace Stienen.Backend {
    public class DefaultRabbitCore {
        private string User { get; set; } // = "guest";
        private string Pass { get; set; } // = "guest";
        private string VHost { get; set; }
        private string Host { get; set; } // = "localhost";
        private int Port { get; set; } // = 5672

        protected IModel channel;
        private IConnection _connection;

        public DefaultRabbitCore(/* NameValueCollection configuration || IRabbitConfig config */)
        {
            var factory = new ConnectionFactory() { };
//                    User = configuration.User.Value;
//                    Host = configuration.Host.Value;
//            };
            _connection = factory.CreateConnection();
            channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "task_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        ~DefaultRabbitCore()
        {
            channel.Close();
            _connection.Close();
        }
    }
}
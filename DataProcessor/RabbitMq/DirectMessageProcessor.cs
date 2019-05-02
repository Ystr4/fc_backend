using System.Threading.Tasks;
using fc_backend.DataConverter;

namespace DataProcessor.RabbitMq {
    public class DirectMessageProcessor : IMessageProcessor {
        public Task ProcessMessage<IMessage>(IMessage msg)
        {
            throw new System.NotImplementedException();
        }
    }
}
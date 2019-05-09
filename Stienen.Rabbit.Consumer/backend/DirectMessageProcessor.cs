using System.Threading.Tasks;
using DataProcessor.RabbitMq;

namespace Stienen.RabbitMq {
    public class DirectMessageProcessor : IMessageProcessor {
        public async Task ProcessMessage(IMessage msg)
        {
            ChangedDataHandler handler = new ChangedDataHandler();
            await handler.Handle(msg);
        }
    }
}
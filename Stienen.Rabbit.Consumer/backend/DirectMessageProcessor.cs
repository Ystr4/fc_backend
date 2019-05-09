using System.Threading.Tasks;
using Stienen.RabbitMq;

namespace Stienen.Rabbit.Consumer {
    public class DirectMessageProcessor : IMessageProcessor {
        public async Task ProcessMessage(IMessage msg)
        {
            ChangedDataHandler handler = new ChangedDataHandler();
            await handler.Handle(msg);
        }
    }
}
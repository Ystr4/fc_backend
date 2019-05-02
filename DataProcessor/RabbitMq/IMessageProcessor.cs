using System.Threading.Tasks;

namespace fc_backend.DataConverter {
    public interface IMessageProcessor {
        Task ProcessMessage<IMessage>(IMessage msg);
    }
}
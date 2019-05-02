using System.Threading.Tasks;

namespace Stienen.Backend {
    public interface IMessage {
        Task ProcessMessage<IMessage>(IMessage msg);
    }
}
using System;

namespace Stienen.Backend {
    public interface IRabbitMessenger {
        void Send<TCommand>(TCommand msg) where TCommand : IMessage;
    }
}
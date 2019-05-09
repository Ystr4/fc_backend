using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Stienen.RabbitMq;
using Stienen.Common;

namespace Stienen.Rabbit.Consumer {
    public class ChangedDataHandler : ICommandHandler {
        public Task Handle(IMessage msg)
        {
            ChangedDataDTO dto = msg as ChangedDataDTO;
            if (dto != null) {
                try {
                    InsertProcs.InsertDeviceData(dto.did, dto.hardware, dto.version, dto.stamp, dto.drift, dto.storeType, dto.data);
                    return Task.CompletedTask;
                }
                catch (Exception ex) {
                    Trace.TraceError("ChangedDataHandler caught exception: {0}", ex);
                    throw;
                }
            }

            throw new Exception("failed to process message");
        }
    }
}
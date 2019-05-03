using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DataProcessor.database;
using Stienen.RabbitMq;

namespace DataProcessor.RabbitMq {
    public class ChangedDataHandler : ICommandHandler {
        public Task Handle(IMessage msg)
        {
            ChangedDataDTO dto = msg as ChangedDataDTO;
            if (dto != null) {
                try {
                    InsertProcs._InsertDeviceData(dto.did, dto.hardware, dto.version, dto.stamp, dto.drift, dto.storeType, dto.data);
                    return Task.CompletedTask;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    Trace.TraceError("ChangedDataHandler exception thrown: {0}", ex);
                    return Task.FromException(new Exception("ChangedDataHandler exception thrown: {0}", ex));
                }
            }

            throw new Exception("ChangedDataHandler received message of unknown type");
        }
    }
}
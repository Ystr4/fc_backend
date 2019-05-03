using System;
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
                    Console.WriteLine("before inserted");
                    InsertProcs._InsertDeviceData(dto.did, dto.hardware, dto.version, dto.stamp, dto.drift, dto.storeType, dto.data);
                    Console.WriteLine("inserted");
                    return Task.CompletedTask;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            throw new Exception("failed to process message");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stienen.Backend {
    [Serializable]
    public class ChangedDataDTO : IMessage {
        public Guid gid;
        public Guid did;
        public int version;
        public int hardware;
        public DateTime stamp;
        public DateTime cStamp;
        public int? drift;
        public List<DataPart> data;
        public StoreType storeType;


        public ChangedDataDTO(DeviceData deviceData, DateTime stamp, DateTime cStamp, int? drift, List<DataPart> data, StoreType storeType)
        {
            this.gid = deviceData.Gid;
            this.did = deviceData.Id;
            this.version = deviceData.Version;
            this.hardware = deviceData.Hardware;

            this.drift = drift;
            this.stamp = stamp;
            this.cStamp = cStamp;
            this.storeType = storeType;
            this.data = data;
        }

        public Task ProcessMessage<IMessage>(IMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
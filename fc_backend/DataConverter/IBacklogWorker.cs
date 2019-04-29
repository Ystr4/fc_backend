using System;
using System.Threading.Tasks;

namespace fc_backend.DataConverter {
    public interface IBacklogWorker {
        Task RequestDeviceBacklog(Uri uri, Guid did, StoreType storeType, DateTime begin, DateTime end, int pageLimit = 100);
    }
}
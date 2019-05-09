using System;
using System.Threading.Tasks;

namespace Stienen.API.DataConverter {
    public interface IBacklogWorker {
        Task RequestDeviceBacklog(Uri uri, Guid did, StoreType storeType, DateTime begin, DateTime end, int pageLimit = 100);
    }
}
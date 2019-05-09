using System;
using System.Threading.Tasks;

namespace Stienen.API.DataConverter {
    public class DefaultBacklogWorker : IBacklogWorker {
        public Task RequestDeviceBacklog(Uri uri, Guid did, StoreType storeType, DateTime begin, DateTime end, int pageLimit = 100)
        {
            // this might just work automaticly because of automatic flow control,
            // otherwise some sort of pageLimit needs to be provided, firing the next 100 only after completion
            // this way it might work more like a background process
            // might work with priority in the queue instead, or setup a seperate queue/consumer just for backlog worker
            throw new NotImplementedException();
        }
    }
}
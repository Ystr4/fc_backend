using System.Threading.Tasks;
using fc_backend.Services;

namespace fc_backend.DataConverter {
    public class DefaultMessageProcessor : IMessageProcessor {
        private IReferenceService _referenceService;
        private IDeviceDataService _deviceDataService;

        public DefaultMessageProcessor(IReferenceService referenceService, IDeviceDataService deviceDataService)
        {
            _referenceService = referenceService;
            _deviceDataService = deviceDataService;
        }


        public async Task ProcessMessage(object msg)
        {
            
        }
    }
}
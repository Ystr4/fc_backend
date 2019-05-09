using System;
using System.Threading.Tasks;
using Data.Postgres.Entities;
using Data.Postgres.Repositories;

namespace Stienen.Backend.Services {
    public class DefaultDeviceDataService : IDeviceDataService {
        private IDeviceDataRepository _deviceDataRepository;
        public DefaultDeviceDataService(IDeviceDataRepository deviceDataRepository)
        {
            _deviceDataRepository = deviceDataRepository;
        }

        public async Task<HistoricDataSet> GetHistoricDeviceData(Guid did, string name, DateTime begin, DateTime end)
        {
            return await _deviceDataRepository.GetHistoricDeviceData(did, name, begin, end);
        }
    }
}
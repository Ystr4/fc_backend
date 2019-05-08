using System;
using System.Threading.Tasks;
using Data.Postgres.Entities;

namespace Stienen.Backend.Services {
    public interface IDeviceDataService {
        Task<HistoricDataSet> GetHistoricDeviceData(Guid did, string name, DateTime begin, DateTime end);
    }
}
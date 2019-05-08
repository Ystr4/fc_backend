using System;
using System.Threading.Tasks;
using Data.Postgres.Entities;

namespace Data.Postgres.Repositories {
    public interface IDeviceDataRepository {
        Task<HistoricDataSet> GetHistoricDeviceData(Guid did, string name, DateTime begin, DateTime end);
    }
}
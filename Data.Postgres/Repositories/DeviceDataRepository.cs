using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Data.Postgres.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace Data.Postgres.Repositories {
    public class DeviceDataRepository : BaseRepository, IDeviceDataRepository {
        private readonly ILogger<DeviceDataRepository> _logger;
        public DeviceDataRepository(IOptions<DatabaseSettings> dbOptions, ILogger<DeviceDataRepository
            > logger)
                : base(dbOptions.Value.ConnectionString) { }

        public Task<HistoricDataSet> GetHistoricDeviceData(Guid did, string name, DateTime begin, DateTime end)
        {
            try {
                Connection.Open();

                using (var cmd = new NpgsqlCommand("public.gethistoricdatabyname", Connection)) {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("did", NpgsqlDbType.Uuid, did);
                    cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, name);
                    cmd.Parameters.AddWithValue("begin", NpgsqlDbType.Timestamp, begin);
                    cmd.Parameters.AddWithValue("end", NpgsqlDbType.Timestamp, end);

                    cmd.Prepare();

                    using (var reader = cmd.ExecuteReader()) {
                        if (!reader.HasRows) return null;

                        HistoricDataSet dataSet = new HistoricDataSet();
                        dataSet.Did = did;
                        dataSet.Name = name;
                        while (reader.Read()) {
                            dataSet.Data.Add(new FormattedDataPoint {
                                    Stamp = Convert.ToDateTime(reader["stamp"]),
                                    Value = Convert.ToDouble(reader["value"])
                            });
                        }
                        Connection.Close();
                        return Task.FromResult(dataSet);
                    }
                }
            }
            catch (Exception ex) {
//                _logger.LogError("GetHistoricDeviceData caught exception: {0}", ex);
                return null;
            }
        }
    }
}
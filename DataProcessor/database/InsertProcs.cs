using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using DataProcessor.RabbitMq;
using Npgsql;
using NpgsqlTypes;
using Stienen.Database;

namespace DataProcessor.database {
    public class InsertProcs {
        public async static void _InsertDeviceData(Guid did, int hardware, int version, DateTime stamp, int drift, StoreType storeType, IEnumerable<DataPart> data)
        {
            try {
                ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[0];
                using (NpgsqlConnection conn = new NpgsqlConnection(css.ConnectionString)) {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("_insertdevicedata", conn)) {
                        cmd.Parameters.AddWithValue("did", NpgsqlDbType.Uuid, did);
                        cmd.Parameters.AddWithValue("hardware", NpgsqlDbType.Integer, hardware);
                        cmd.Parameters.AddWithValue("version", NpgsqlDbType.Integer, version);
                        cmd.Parameters.AddWithValue("stamp", NpgsqlDbType.Timestamp, stamp);
                        cmd.Parameters.AddWithValue("drift", NpgsqlDbType.Integer, drift);
                        cmd.Parameters.AddWithValue("store_type", NpgsqlDbType.Integer, storeType);

                        foreach (DataPart dataPart in data) {
                            cmd.Parameters.AddWithValue("index", NpgsqlDbType.Integer, dataPart.Index);
                            cmd.Parameters.AddWithValue("data", NpgsqlDbType.Bytea, dataPart.Data);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex) {
                Trace.TraceError("Exception occurred during _InsertDeviceData: {0}", ex);
            }
        }
    }
}
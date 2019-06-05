using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Npgsql;
using NpgsqlTypes;
using Stienen.Common;

namespace Stienen.Rabbit.Consumer {
    public static class InsertProcs {
        public static async void InsertDeviceData(Guid did, int hardware, int version, DateTime stamp, int drift, StoreType storeType, IEnumerable<DataPart> data)
        {
            try {
                string connectionString = ConfigurationManager.AppSettings["DeviceData_connectionString"];
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("public._insertdevicedata", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("did", NpgsqlDbType.Uuid, did);
                        cmd.Parameters.AddWithValue("hardware", NpgsqlDbType.Integer, hardware);
                        cmd.Parameters.AddWithValue("version", NpgsqlDbType.Integer, version);
                        cmd.Parameters.AddWithValue("stamp", NpgsqlDbType.Timestamp, stamp);
                        cmd.Parameters.AddWithValue("drift", NpgsqlDbType.Integer, drift);
                        cmd.Parameters.AddWithValue("store_type", NpgsqlDbType.Integer, (int) storeType);
                        cmd.Parameters.Add("index", NpgsqlDbType.Integer);
                        cmd.Parameters.Add("data", NpgsqlDbType.Bytea);

//                        cmd.Prepare(); // not sure this helps in this case

                        foreach (var dataPart in data) {
                            cmd.Parameters["index"].Value = dataPart.Index;
                            cmd.Parameters["data"].Value = dataPart.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex) {
                Trace.TraceError("Exception occurred during InsertDeviceData: {0}", ex);
            }
        }
    }
}
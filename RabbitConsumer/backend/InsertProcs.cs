﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Npgsql;
using NpgsqlTypes;
using Stienen.Common;

namespace DataProcessor.database {
    public static class InsertProcs {
        public static void _InsertDeviceData(Guid did, int hardware, int version, DateTime stamp, int drift, StoreType storeType, IEnumerable<DataPart> data)
        {
            try {
                string connectionString = "Server=localhost;Database=s6_database;Port=5432;User Id=postgres;Password=post;Pooling=true;MinPoolSize=2;MaxPoolSize=30;";
//                ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[0];
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
                    conn.Open();
                    
                    using (var cmd = new NpgsqlCommand("public._insertdevicedata", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("did", NpgsqlDbType.Uuid, did);
                        cmd.Parameters.AddWithValue("hardware", NpgsqlDbType.Integer, hardware);
                        cmd.Parameters.AddWithValue("version", NpgsqlDbType.Integer, version);
                        cmd.Parameters.AddWithValue("stamp", NpgsqlDbType.Timestamp, stamp);
                        cmd.Parameters.AddWithValue("drift", NpgsqlDbType.Integer, drift);
                        cmd.Parameters.AddWithValue("store_type", NpgsqlDbType.Integer, (int)storeType);

                        foreach (DataPart dataPart in data) {
                            cmd.Parameters.AddWithValue("index", NpgsqlDbType.Integer, dataPart.Index);
                            cmd.Parameters.AddWithValue("data", NpgsqlDbType.Bytea, dataPart.Data);
                            cmd.ExecuteNonQuery();
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
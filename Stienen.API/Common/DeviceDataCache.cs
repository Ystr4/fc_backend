using System;
using System.Collections.Generic;

namespace fc_backend.Common {
//    public class DeviceDataCache
//    {
//        private Dictionary<int, Byte> data = new Dictionary<int, byte>();
//        private Object dataGuard = new object();
//        private DateTime latestUpdateTime = new DateTime();
//        private Guid did;
//        private int hardware;
//        private int version;
//
//        public DeviceDataCache(Guid devDid, int devHardware, int devVersion)
//        {
//            did = devDid;
//            hardware = devHardware;
//            version = devVersion;
//        }
//
//        public void ClearCache()
//        {
//            data = new Dictionary<int, byte>();
//        }
//
//        public void UpdateValues(DateTime timeStamp, DataPart[] dps)
//        {
//            // Check if the data array is initial filled
//            lock (dataGuard)
//            {
//                // Ok, we can update the data cache
//                foreach (var dp in dps)
//                {
//                    UpdateValue(timeStamp, dp);
//                }
//            }
//        }
//
//        public void UpdateValues(DateTime timestamp, int index, byte[] dat)
//        {
//            int start = index;
//
//            for (int i = 0; i < dat.Length; i++)
//            {
//                if (data.ContainsKey(start + i))
//                {
//                    data[start + i] = dat[i];
//                }
//                else
//                {
//                    data.Add(start + i, dat[i]);
//                }
//            }
//
//            if (latestUpdateTime < timestamp)
//            {
//                latestUpdateTime = timestamp;
//            }
//        }
//
//        public IEnumWrapper<ValueEx> GetLastValues(Position[] poss, bool cacheDisabled)
//        {
//            List<ValueEx> result = new List<ValueEx>();
//            List<Position> requestFromDatabase = new List<Position>();
//
//            lock (dataGuard)
//            {
//                // Check if the initial data is already received
//                foreach (var pos in poss)
//                {
//                    if (cacheDisabled)
//                    {
//                        requestFromDatabase.Add(pos);
//                    }
//                    else
//                    {
//                        var rslt = GetValue(pos);
//
//                        if (rslt != null)
//                        {
//                            result.Add(rslt);
//                        }
//                        else
//                        {
//                            requestFromDatabase.Add(pos);
//                        }
//                    }
//                }
//
//                // Ok now retrieve the values from the database
//                if (requestFromDatabase.Count > 0)
//                {
//                    var dbResult = ConfigProcs.GetLastValues(did, requestFromDatabase.ToArray());
//
//                    try
//                    {
//                        foreach (var value in dbResult)
//                        {
//                            // Update cache
//                            UpdateValues(value.Stamp, value.Index, value.Data);
//                            // Add to result
//                            result.Add(value);
//                        }
//                    }
//                    catch
//                    {
//                        return null;
//                    }
//                }
//            }
//
//            return new IEnumWrapper<ValueEx>(result);
//        }
//
//        public void UpdateHardwareAndVersion(int devHardware, int devVersion)
//        {
//            if ((hardware != devHardware) || (version != devVersion))
//            {
//                hardware = devHardware;
//                version = devVersion;
//
//                // Clear byte array
//                data = new Dictionary<int, byte>();
//            }
//            else
//            {
//                // Do nothing, hardware / version the same
//            }
//        }
//
//        private void UpdateValue(DateTime timeStamp, DataPart dp)
//        {
//            int start = dp.Index;
//            latestUpdateTime = timeStamp;
//
//            for (int i = 0; i < dp.Data.Length; i++)
//            {
//                if (data.ContainsKey(start + i))
//                {
//                    data[start + i] = dp.Data[i];
//                }
//                else
//                {
//                    data.Add(start + i, dp.Data[i]);
//                }
//            }
//        }
//
//        private ValueEx GetValue(Position pos)
//        {
//            ValueEx result = new ValueEx();
//            result.Data = new Byte[pos.Length];
//            bool retrieveFromDatabase = false;
//
//            for (int i = 0; ((i < result.Data.Length) && (!retrieveFromDatabase)); i++)
//            {
//                // Check if there is data
//                if (data.ContainsKey(i + pos.Index))
//                {
//                    result.Data[i] = data[i + pos.Index];
//
//                    result.Did = did;
//                    result.Index = pos.Index;
//                    result.Stamp = latestUpdateTime;
//                }
//                else
//                {
//                    return null;
//                }
//            }
//
//            return result;
//        }
//    }
}
namespace Stienen.API.Common {
//    public sealed class DataCache
//    {
//        private static volatile DataCache instance;
//        private static object syncRoot = new Object();
//        private bool cacheDisabled = false;
//
//        private Dictionary<Guid, DeviceDataCache> devicesCache = new Dictionary<Guid, DeviceDataCache>();
//
//        private DataCache()
//        {
//            cacheDisabled = Settings.Instance.GetSettingBool("DISABLE_DATA_CACHE");
//        }
//
//        public static DataCache Instance
//        {
//            get {
//                if (instance == null)
//                {
//                    lock (syncRoot)
//                    {
//                        if (instance == null)
//                            instance = new DataCache();
//                    }
//                }
//
//                return instance;
//            }
//        }
//
//        public void UpdateValues(Guid did, DateTime timestamp, DataPart[] dps)
//        {
//            if (!devicesCache.ContainsKey(did))
//            {
//                if (!AddDevice(did))
//                {
//                    return;
//                }
//            }
//
//            devicesCache[did].UpdateValues(timestamp, dps);
//        }
//
//        public IEnumWrapper<ValueEx> GetLastValues(Guid did, Position[] pos)
//        {
//            if (!devicesCache.ContainsKey(did))
//            {
//                // Take from database and fill current device if exists
//                if (!AddDevice(did))
//                {
//                    return new IEnumWrapper<ValueEx>(new List<ValueEx>());
//                }
//            }
//
//            return devicesCache[did].GetLastValues(pos, cacheDisabled);
//        }
//
//        public void UpdateHardwareAndVersion(Guid did, int devHardware, int devVersion)
//        {
//            if (devicesCache.ContainsKey(did))
//            {
//                devicesCache[did].UpdateHardwareAndVersion(devHardware, devVersion);
//            }
//        }
//
//        public void ClearCache(Guid did)
//        {
//            devicesCache[did].ClearCache();
//        }
//
//        public bool AddDevice(Guid did)
//        {
//            // Get the data from the database for the device (if there is any)
//            try
//            {
//                DeviceEx dev = CustomProcs.GetDeviceEx(did);
//                devicesCache.Add(did, new DeviceDataCache(did, dev.Hardware, dev.Version));
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
}
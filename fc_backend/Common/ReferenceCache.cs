using System;
using System.Collections.Concurrent;
using Stienen.Backend.DataAccess.Models;

namespace fc_backend.Common {
    public sealed class ReferenceCache {
        private static volatile ReferenceCache instance;
        private static object syncRoot = new Object();

        private static ConcurrentDictionary<HardwareVersionEntity, HardwareReferenceEntity> 
                References = new ConcurrentDictionary<HardwareVersionEntity, HardwareReferenceEntity>();

        public static ReferenceCache Instance
        {
            get {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ReferenceCache();
                    }
                }

                return instance;
            }
        }

    }
}
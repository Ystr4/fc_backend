using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace Stienen.Backend {
    public class ChannelCache {
        private static volatile ChannelCache instance;

        private static ConcurrentDictionary<Guid, IModel> channels = new ConcurrentDictionary<Guid, IModel>();
        private static object syncRoot = new Object();

        public static ChannelCache Instance
        {
            get {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ChannelCache();
                    }
                }

                return instance;
            }
        }

        public IModel Get(Guid gid)
        {
            IModel channel;
            if (!channels.ContainsKey(gid)) {
                throw new Exception($"No channel available for key: {gid}");
            }
            channels.TryGetValue(gid, out channel);
            return channel;
        }

        public void Add(Guid gid, IModel channel)
        {
            channels.AddOrUpdate(gid, channel, (guid, model) => channel);
        }
    }
}
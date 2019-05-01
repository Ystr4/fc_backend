using System;
using System.Collections.Generic;

namespace fc_backend.DataAccess.Models {
    public class DeviceInfoEntity {
        public Guid Id { get; set; }
        public string Name { get; set; }
//        public Uri OriginServer { get; set; }
        public HardwareVersionEntity CurrentVersion { get; set; }


    //        public bool Deleted { get; set; }
    //        public RemoteComInfoEntity ComInfo{ get; set; }
    //        public LocationInfoEntity LocationInfoEntity { get; set; }
    //        public bool HasVersionHistory {
    //            get { return (this.VersionHistory?.Count > 0); }
    //        }
    //        public List<VersionHistoryEntity> VersionHistory { get; set; }
    }
}
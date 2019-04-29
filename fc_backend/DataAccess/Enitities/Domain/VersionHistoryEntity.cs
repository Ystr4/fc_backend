using System;

namespace fc_backend.DataAccess.Models {
    public class VersionHistoryEntity {
        public HardwareVersionEntity Version { get; set; }
        public DateTime Stamp { get; set; }
    }
}
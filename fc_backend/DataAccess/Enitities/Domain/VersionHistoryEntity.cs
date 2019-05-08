using System;

namespace Stienen.Backend.DataAccess.Models {
    public class VersionHistoryEntity {
        public HardwareVersionEntity Version { get; set; }
        public DateTime Stamp { get; set; }
    }
}
using System.Collections.Generic;

namespace Stienen.Backend.DataAccess.Models {
    public class HardwareReferenceEntity {
        public HardwareReferenceEntity(HardwareVersionEntity version, List<ReferenceEx> references)
        {
            Version = version;
            References = references;
        }
        public HardwareVersionEntity Version { get; set; }

        private List<ReferenceEx> References { get; set; }
    }
}
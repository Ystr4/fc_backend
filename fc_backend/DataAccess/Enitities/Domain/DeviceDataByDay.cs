using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fc_backend.DataAccess.Models {
    public class DeviceDataByDay {
        // public Guid DeviceId { get; set; }
        public DeviceInfoEntity DeviceId { get; set; }

        public DateTime Type { get; set; }

        public ReferenceEx VarReference { get; set; }
        // public string VarName { get; set; }
        // should this be a type: HardwareReference ?
        
        [Column(TypeName = "jsonb")]
        public FormattedDataPoint[] Data { get; set; }
    }
}
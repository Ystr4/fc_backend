using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fc_backend.DataAccess.Models {
    public class DeviceDataByDay {
        public Guid DeviceId { get; set; }

        public DateTime Type { get; set; }

        public string VarName { get; set; }
        // should this be a type: HardwareReference ?
        // public ReferenceEntity VarReference { get; set; }

        [Column(TypeName = "jsonb")]
        public FormattedDataPoint[] Data { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
//using Data.Postgres.Entities;

namespace Stienen.Backend.DataAccess.Models {
    public class DeviceDataByDay {
//        [ForeignKey()]
        public Guid DeviceId { get; set; }
//        public DeviceInfoEntity DeviceId { get; set; }
        public DateTime Day { get; set; }
        public string VarName { get; set; }
//        public ReferenceEx VarReference { get; set; }
        // should this be a type: HardwareReference ?
        
//        [Column(TypeName = "jsonb")]
//        public FormattedDataPoint[] Data { get; set; }
    }
}
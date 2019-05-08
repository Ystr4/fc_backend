using System;

namespace Stienen.Backend.DataAccess.Models.Frontend {
    public class RoundEntity {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Selected { get; set; }
    }
}
using System;

namespace fc_backend.DataAccess.Models {
    public class LocationInfoEntity {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Registered { get; set; }
        public DateTime LastActive { get; set; }
    }
}
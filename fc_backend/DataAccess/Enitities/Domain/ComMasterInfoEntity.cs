using System;

namespace fc_backend.DataAccess.Models {
    public class ComMasterInfoEntity {
        public int RX_Faults { get; set; }
        public int FN_Faults { get; set; }
        public int NumberOfAddressInLoop { get; set; }
        public TimeSpan LoggingInterval { get; set; }
    }
}


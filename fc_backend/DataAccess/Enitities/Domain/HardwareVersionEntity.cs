namespace Stienen.Backend.DataAccess.Models {
    public class HardwareVersionEntity {
        public HardwareVersionEntity(int hardware, int version)
        {
            Hardware = hardware;
            Version = version;
        }
        public int Hardware { get; set; }
        public int Version { get; set; }
    }
}
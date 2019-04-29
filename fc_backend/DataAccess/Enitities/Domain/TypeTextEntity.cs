namespace fc_backend.DataAccess.Models {
    public class TypeTextEntity {
        public HardwareVersionEntity Version { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public string Language { get; set; }
    }
}
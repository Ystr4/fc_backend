namespace fc_backend.DataAccess.Models {
    public class ReferenceEntity {
    
        public HardwareVersionEntity Version { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public int Defop { get; set; }
        public int Mul { get; set; }
        public int Div { get; set; }
        public int Step { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool AckChange { get; set; }
    }
}

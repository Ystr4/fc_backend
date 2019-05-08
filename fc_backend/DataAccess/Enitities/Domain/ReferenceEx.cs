namespace Stienen.Backend.DataAccess.Models {
    public class ReferenceEx {

        //        public HardwareVersionEntity Version { get; set; }
        public int Hardware { get; set; }
        public int Version { get; set; }
        public int Id { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public string Name { get; set; }
        public int Tid { get; set; }
        public string TypeName { get; set; }
        public int Type { get; set; }
        public int Mul { get; set; }
        public int Div { get; set; }
        public float Step { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Text { get; set; }
        public int? Defop { get; set; }
        public bool AcknowledgeChange { get; set; }
    }
}

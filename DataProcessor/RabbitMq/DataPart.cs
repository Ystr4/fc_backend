using System;

namespace DataProcessor.RabbitMq {
    public class DataPart : IComparable<DataPart> {
        public int Index { get; set; }
        public byte[] Data { get; set; }

        public DataPart() { }

        public DataPart(int index, byte[] data)
        {
            Index = index;
            Data = data;
        }

        public int CompareTo(DataPart other)
        {
            return Index.CompareTo(other.Index);
        }
    }
}
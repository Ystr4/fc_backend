using System;

namespace Data.Postgres.Entities {
    public class FormattedDataPoint {
        public FormattedDataPoint() { }

        public FormattedDataPoint(DateTime stamp, double value)
        {
            Stamp = stamp;
            Value = value;
        }
        public DateTime Stamp { get; set; }
        public double Value { get; set; }
    }
}
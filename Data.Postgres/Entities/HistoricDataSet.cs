using System;
using System.Collections.Generic;

namespace Data.Postgres.Entities {
    public class HistoricDataSet {
        public Guid Did { get; set; }
        public string Name { get; set; }
        public List<FormattedDataPoint> Data { get; set; }

        public HistoricDataSet()
        {
            Data = new List<FormattedDataPoint>();
        }
    }
}
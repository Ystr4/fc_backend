using System;

namespace Stienen.Backend.Models {
    public class RequestHistoryParams {
        public string Name { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
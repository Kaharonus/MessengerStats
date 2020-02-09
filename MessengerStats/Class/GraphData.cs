using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    public class GraphData {
        public int Padding { get; set; }
        public string XAxisLabek { get; set; }
        public string YAxisLabel { get; set; }
        public Dictionary<string, List<double>> Data { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

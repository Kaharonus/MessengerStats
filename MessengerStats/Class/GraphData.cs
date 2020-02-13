using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    public class GraphData {
        public int Padding { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public Dictionary<string, (float[] x, float[] y)> Data { get; set; } = new Dictionary<string, (float[] x, float[] y)>();
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GraphData() {

        }

      
    }
}

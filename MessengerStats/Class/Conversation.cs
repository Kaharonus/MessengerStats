using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    public class Conversation {
        public string Name;
        public List<string> People = new List<string>();
        public Dictionary<string, List<float>> Messages = new Dictionary<string, List<float>>();
    }
}

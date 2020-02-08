using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    public class Conversation {
        public string Name { get; set; }
        public int TotalMessages { get; set; }
        public int ActivityPerWeek { get; set; }
        public bool IsActive { get; set; } = false;
        public Message FirstMessage { get; set; } = new Message();
        public Message LastMessage { get; set; } = new Message();
        public HashSet<string> People = new HashSet<string>();
        public Dictionary<string, List<Message>> Messages = new Dictionary<string, List<Message>>();
    }
    public class Message{ 
        public string Sender { get; set; }
        public DateTime Time { get; set; } = DateTime.MinValue;
        public string Content { get; set; }
        public string Type { get; set; }
    }
}

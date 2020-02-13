using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerStats {
    public class ConversationData {

        /// <summary>
        /// Full conversation data. Filled with Load function
        /// </summary>
        public static List<Conversation> Conversations { get; set; } = new List<Conversation>();
        public static string User { get; set; }

        static async Task ParseMessages(Ref<RawConversation> orig, Ref<Conversation> data, DateTime creationTime) {
            await Task.Run(() => {              
              
                data.Value.TotalMessages = orig.Value.messages.Count;
               
                foreach (var message in orig.Value.messages) {
                    Message m = new Message();
                    m.Content = message.content.DecodeString();
                    m.Sender = message.sender_name.DecodeString();
                    m.Time = Helper.ConvertTimeStamp(message.timestamp_ms);
                    m.Type = message.type;
                    if (m.Time.AddDays(7) >= creationTime) {
                        data.Value.ActivityPerWeek++;
                    }
                    if (!data.Value.Messages.ContainsKey(m.Sender)) {
                        data.Value.Messages.Add(m.Sender, new List<Message>());
                    }
                    data.Value.Messages[m.Sender].Add(m);
                }
                data.Value.IsActive = data.Value.ActivityPerWeek > 2;
            });

        }

        static async Task<Conversation> ParseConversation(string dir, EventHandler conversationChange = null) {
            var files = Directory.GetFiles(dir, "message_*.json");
            var conv = new Ref<Conversation>(new Conversation());
            foreach (var file in files) {
                var time = File.GetCreationTime(file);
                using var str = File.OpenRead(file);
                var parsed = new Ref<RawConversation>(await JsonSerializer.DeserializeAsync<RawConversation>(str));
                conv.Value.Name = parsed.Value.title.DecodeString();
                if (conversationChange != null) {
                    conversationChange.Invoke(conv.Value.Name, new EventArgs());
                }
                await ParseMessages(parsed, conv, time);
            }
            var concat = conv.Value.Messages.Values.ToList().SelectMany(x => x);
            var firstOrLast = concat.Max(x => x.Time);
            conv.Value.LastMessage = concat.First(x => x.Time == firstOrLast);

            firstOrLast = concat.Min(x => x.Time);
            conv.Value.FirstMessage = concat.First(x => x.Time == firstOrLast);

            return conv.Value;
        }

        /// <summary>
        /// Assigns activity per week between 0-100 insted of 0-whatever
        /// </summary>
        private static void WeightActivity() {
            var max = Conversations.Max(x => x.ActivityPerWeek);
            for (int i = 0; i < Conversations.Count; i++) {
                Conversations[i].ActivityPerWeek = Conversations[i].ActivityPerWeek * 100.0 / max;
            }
        }

        public static float[] WeightData(float[] arr) {
            var newarr = new float[arr.Length];
            var max = arr.Max(x => x);
            for (int i = 0; i < arr.Length; i++) {
                newarr[i] = arr[i] * 100.0f / max;
            }
            return newarr;
        }

        /// <summary>
        /// Loads Messenger conversation from given path. Gives progress updates by second parameter
        /// </summary>
        /// <param name="path">Directory where facebook data is stored (xx/facebook-xxx/messages/inbox)</param>
        /// <param name="conversationChange">if specified gets fired every time there is a progress update</param>
        public static async Task Load(string path, EventHandler conversationChange = null) {
            Dictionary<string, int> count = new Dictionary<string, int>();
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) {
                var conv = await ParseConversation(dir, conversationChange);
                foreach (var item in conv.Messages.Keys) {
                    if (!count.ContainsKey(item)) {
                        count.Add(item, 0);
                    }
                    count[item]++;
                }
                 Conversations.Add(conv);
            }
            WeightActivity();
            User = count.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        public class RawConversation {
            public List<Message> messages { get; set; }
            public string title { get; set; }
            public bool is_still_participant { get; set; }
            public string thread_type { get; set; }
            public string thread_path { get; set; }     
            public class Message {
                public string sender_name { get; set; }
                public long timestamp_ms { get; set; }
                public string content { get; set; }
                public string type { get; set; }               
            }

        }
    }
}

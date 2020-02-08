using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerStats {
    public class ConversationData {

        /// <summary>
        /// Full conversation data. Filled with Load function
        /// </summary>
        public static List<Conversation> Conversations { get; set; } = new List<Conversation>();

        static async Task ParseMessages(Ref<RawConversation> orig, Ref<Conversation> data, DateTime creationTime) {
            await Task.Run(() => {              
                var firstOrLast = orig.Value.messages[0];
                data.Value.LastMessage = new Message() { Time =  Helper.ConvertTimeStamp(firstOrLast.timestamp_ms), Content = firstOrLast.content.DecodeString(), Sender = firstOrLast.sender_name.DecodeString() };
                firstOrLast = orig.Value.messages[orig.Value.messages.Count-1];
                data.Value.FirstMessage = new Message() { Time = Helper.ConvertTimeStamp(firstOrLast.timestamp_ms), Content = firstOrLast.content.DecodeString(), Sender = firstOrLast.sender_name.DecodeString() };
                data.Value.TotalMessages = orig.Value.messages.Count;
                foreach (var message in orig.Value.messages) {
                    Message m = new Message();
                    m.Content = message.content.DecodeString();
                    m.Sender = message.sender_name.DecodeString();
                    m.Time = Helper.ConvertTimeStamp(message.timestamp_ms);
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
            return conv.Value;
        }

        /// <summary>
        /// Loads Messenger conversation from given path. Gives progress updates by second parameter
        /// </summary>
        /// <param name="path">Directory where facebook data is stored (xx/facebook-xxx/messages/inbox)</param>
        /// <param name="conversationChange">if specified gets fired every time there is a progress update</param>
        public static async Task Load(string path, EventHandler conversationChange = null) {
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) {
                 Conversations.Add(await ParseConversation(dir, conversationChange));
            }
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

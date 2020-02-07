using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerStats {
    public class ConversationData {

        public static List<Conversation> Conversations { get; set; } = new List<Conversation>();

        static async Task<Conversation> ParseConversation(string dir) {
            var files = Directory.GetFiles(dir, "message_*.json");
            Conversation conv = new Conversation();
            foreach (var file in files) {
                var str = File.OpenRead(file);
                //string text = File.ReadAllText(file, Encoding.GetEncoding("ISO-8859-1"));
                var parsed = await JsonSerializer.DeserializeAsync<RawConversation>(str);
                str.Close();
                conv.Name = parsed.title.DecodeString();
                //ParseMessages(ref conv, parsed.messages);
            }
            return conv;
        }

        public static async Task Load(string path) {
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) {
                 Conversations.Add(await ParseConversation(dir));
            }
        }

        public class RawConversation {
            public List<Participant> participants { get; set; }
            public List<Message> messages { get; set; }
            public string title { get; set; }
            public bool is_still_participant { get; set; }
            public string thread_type { get; set; }
            public string thread_path { get; set; }

            public class Participant {
                public string name { get; set; }
            }

            public class Reaction {
                public string reaction { get; set; }
                public string actor { get; set; }
            }

            public class Share {
                public string link { get; set; }
                public string share_text { get; set; }
            }

            public class Photo {
                public string uri { get; set; }
                public int creation_timestamp { get; set; }
            }

            public class File {
                public string uri { get; set; }
                public int creation_timestamp { get; set; }
            }

            public class Gif {
                public string uri { get; set; }
            }

            public class Sticker {
                public string uri { get; set; }
            }

            public class AudioFile {
                public string uri { get; set; }
                public int creation_timestamp { get; set; }
            }

            public class Thumbnail {
                public string uri { get; set; }
            }

            public class Video {
                public string uri { get; set; }
                public int creation_timestamp { get; set; }
                public Thumbnail thumbnail { get; set; }
            }

            public class Message {
                public string sender_name { get; set; }
                public long timestamp_ms { get; set; }
                public string content { get; set; }
                public string type { get; set; }
                public List<Reaction> reactions { get; set; }
                public Share share { get; set; }
                public List<Photo> photos { get; set; }
                public List<File> files { get; set; }
                public List<Gif> gifs { get; set; }
                public Sticker sticker { get; set; }
                public int? call_duration { get; set; }
                public bool? missed { get; set; }
                public List<AudioFile> audio_files { get; set; }
                public List<Video> videos { get; set; }
            }

        }
    }
}

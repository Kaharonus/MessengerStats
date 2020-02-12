using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MessengerStats {
    public class Conversation {
        public string Name { get; set; }
        public int TotalMessages { get; set; }
        public double ActivityPerWeek { get; set; }
        public bool IsActive { get; set; } = false;
        public Message FirstMessage { get; set; } = new Message();
        public Message LastMessage { get; set; } = new Message();
        public HashSet<string> People = new HashSet<string>();
        public Dictionary<string, List<Message>> Messages = new Dictionary<string, List<Message>>();


        public List<GraphData> GenerateData() {
            var graphs = new List<GraphData>();
            var data = new GraphData();
            data.StartDate = FirstMessage.Time;
            data.EndDate = LastMessage.Time;
            data.Name = Name;
            data.Padding = 1;
            var count = (int)(data.EndDate - data.StartDate).TotalDays;
            var arr = new float[count];
            for (int i = 0; i < arr.Count(); i++) {
                arr[i] = i;
            }
            foreach (var person in Messages) {
                var days = person.Value.GroupBy(x => x.Time.Date).ToDictionary(x => x.Key, x => x.ToList());
                var result = new float[count];
                for (int i = 0; i < count; i++) {

                    if (!days.ContainsKey(data.StartDate.Date.AddDays(i))) {
                        result[i] = 0;
                    } else {
                        result[i] = days[data.StartDate.Date.AddDays(i)].Count;
                    }
                }
                //CubicSpline.FitParametric(arr, result, result.Count() * 10, out _, out var reslut);

             
                data.Data.Add(person.Key, result);

            }
            graphs.Add(data);
            return graphs;
        }


        public static List<GraphData> GenerateOverall() {
            var graphs = new List<GraphData>();
            var orderedData = ConversationData.Conversations.Select(x => x.Messages.ContainsKey(ConversationData.User) ? x.Messages[ConversationData.User] : null).Where(x => x != null).SelectMany(x => x).OrderBy(x => x.Time);
            var daily = orderedData.GroupBy(x => x.Time.Date).ToList();
            var hourly = orderedData.GroupBy(x => x.Time.Hour).OrderBy(x => x.Key).ToList();
            var hours = new float[24];
            var hourlyCount = new float[24];
            for (int i = 0; i < 24; i++) {
                hourlyCount[i] = (hourly[i].Count());
                hours[i] = i;
            }
            var dataHourly = new GraphData();
            CubicSpline.FitParametric(hours, hourlyCount, 24 * 10, out _, out var yData);
            dataHourly.Data.Add("Hourly", yData);
            dataHourly.Padding = 10;
            dataHourly.Name = "Hourly data";
            dataHourly.YAxisLabel = "Value";
            dataHourly.XAxisLabel = "Hours";
            graphs.Add(dataHourly);
            return graphs;
        }


    }
    public class Message {
        public string Sender { get; set; }
        public DateTime Time { get; set; } = DateTime.MinValue;
        public string Content { get; set; }
        public string Type { get; set; }

        public Message() {

        }

        public Message(string sender, DateTime time, string content, string type) {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            Time = time;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}

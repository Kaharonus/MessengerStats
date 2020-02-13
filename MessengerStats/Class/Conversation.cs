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

        private GraphData GroupDaily(GraphData data) {
            var newData = new GraphData() {
                EndDate = data.EndDate,
                StartDate = data.StartDate,
                Padding = 10,
                XAxisLabel = data.XAxisLabel,
                YAxisLabel = data.YAxisLabel,

            };
            newData.Name = "Grouped daily";
            foreach (var item in data.Data) {
                var x = new float[item.Value.x.Length];
                var y = new float[item.Value.x.Length];
                float count = 0;
                for (int i = 0; i < y.Length; i++) {
                    x[i] = i;
                    count += item.Value.y[i];
                    y[i] = count;
                }
                CubicSpline.FitParametric(x, y, item.Value.x.Length * 5, out var xS, out var yS);
                newData.Data.Add(item.Key, (xS, RemoveSmallerValues(yS)));

            }
            return newData;
        }


        private float[] RemoveNegativeValues(float[] arr) {
            var newArr = new float[arr.Length];
            for (int i = 1; i < newArr.Length; i++) {
                if (arr[i] < 0) {
                    int offset = 0;
                    while (i + offset < newArr.Length && arr[i + offset] < 0) {
                        newArr[i + offset] = 0;
                        offset++;
                    }
                    i += offset - 1;
                } else {
                    newArr[i] = arr[i];
                }
            }
            return newArr;
        }

        private float[] RemoveSmallerValues(float[] arr) {
            if (arr == null) {
                return new float[0];
            }
            var newArr = new float[arr.Length];
            for (int i = 1; i < newArr.Length; i++) {
                if (arr[i] < arr[i - 1]) {
                    int offset = 0;
                    while (i + offset < newArr.Length && arr[i + offset] < arr[i - 1]) {
                        newArr[i + offset] = arr[i - 1];
                        offset++;
                    }
                    i += offset - 1;
                } else {
                    newArr[i] = arr[i];
                }
            }
            return newArr;
        }

        private GraphData GenerateDaily() {
            var data = new GraphData();
            data.StartDate = FirstMessage.Time;
            data.EndDate = LastMessage.Time;
            data.Name = "Daily data";
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
                data.Data.Add(person.Key, (arr, result));
            }
            return data;
        }

        private GraphData GenerateHourly() {
            var data = new GraphData();
            data.StartDate = FirstMessage.Time;
            data.EndDate = LastMessage.Time;
            data.Name = "Activity";
            var hours = new float[24];
            for (int i = 0; i < 24; i++) {
                hours[i] = i;
            }
            foreach (var item in Messages) {
                var hourlyCount = new float[24];
                var hourly = item.Value.GroupBy(x => x.Time.Hour).OrderBy(x => x.Key).ToList();
                //var max = hourly.Max(x => x.Count());
                foreach (var x in hourly) {
                    //hourlyCount[x.Key] = x.Count() * 100.0f / max;
                    hourlyCount[x.Key] = x.Count();
                }
                CubicSpline.FitParametric(hours, hourlyCount, 24 * 10, out var xs, out var yS);
                data.Data.Add(item.Key, (xs, RemoveNegativeValues(yS)));
            }
            return data;
        }




        public List<GraphData> GenerateData() {
            var graphs = new List<GraphData>();
            var daily = GenerateDaily();
            graphs.Add(GroupDaily(daily));
            graphs.Add(GenerateHourly());
            graphs.Add(daily);
            return graphs;
        }



        public static List<GraphData> GenerateOverall() {
            var graphs = new List<GraphData>();
            var orderedData = ConversationData.Conversations.Select(x => x.Messages.ContainsKey(ConversationData.User) ? x.Messages[ConversationData.User] : null).Where(x => x != null).SelectMany(x => x).OrderBy(x => x.Time);
            //var daily = orderedData.GroupBy(x => x.Time.Date).ToList();
            var hourly = orderedData.GroupBy(x => x.Time.Hour).OrderBy(x => x.Key).ToList();
            var hours = new float[24];
            var hourlyCount = new float[24];
            for (int i = 0; i < 24; i++) {
                hourlyCount[i] = (hourly[i].Count());
                hours[i] = i;
            }
            var dataHourly = new GraphData();
            CubicSpline.FitParametric(hours, hourlyCount, 24 * 10, out var xS, out var yS);
            dataHourly.Data.Add("Hourly", (xS, yS));
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

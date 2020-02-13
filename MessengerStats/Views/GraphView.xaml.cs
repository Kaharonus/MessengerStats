using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerStats.Views {
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl {

        List<GraphData> Data { get; set; }

        public GraphView() {
            InitializeComponent();
        }

        public GraphView(List<GraphData> data) {
            InitializeComponent();
            Data = data;
        }


        public void RenderGraph(GraphData item) {
            PlotPanel.Children.Clear();
            WpfPlot plot = new WpfPlot();
            PlotPanel.Children.Add(plot);
            foreach (var signal in item.Data) {
                if (signal.Value.y.Length == 0) {
                    continue;
                }
                if (item.StartDate != null) {
                    plot.plt.PlotScatter(Array.ConvertAll(signal.Value.x, x => (double)x), Array.ConvertAll(signal.Value.y, x => (double)x), label: signal.Key, markerSize: 0);
                } else {
                    plot.plt.PlotScatter(Array.ConvertAll(signal.Value.x, x => (double)x), Array.ConvertAll(signal.Value.y, x => (double)x), label: signal.Key, markerSize: 0);

                }
            }
            plot.plt.Ticks(dateTimeX: true);
            plot.plt.Legend(true, location: legendLocation.upperLeft);
            plot.plt.XLabel(item.XAxisLabel, enable: true);
            plot.plt.YLabel(item.YAxisLabel, enable: true);
            plot.plt.Style(
                Helper.ConvertColor(brush: (SolidColorBrush)FindResource("BackgroundDark")),
                Helper.ConvertColor((SolidColorBrush)FindResource("Background")),
                ColorTranslator.FromHtml("#888"),
                Helper.ConvertColor((SolidColorBrush)FindResource("TextColor")),
                Helper.ConvertColor((SolidColorBrush)FindResource("TextColor")),
                Helper.ConvertColor((SolidColorBrush)FindResource("TextColor"))
                );
            plot.Render();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            foreach (var item in Data) {
                ComboBoxItem boxItem = new ComboBoxItem();
                boxItem.Content = item.Name;
                GraphsSelect.Items.Add(boxItem);
            }
            GraphsSelect.SelectionChanged += (s, ev) => {
                RenderGraph(Data[GraphsSelect.SelectedIndex]);
            };
            if (Data.Count > 0) {
                GraphsSelect.SelectedIndex = 0;
            }
        }
    }
}

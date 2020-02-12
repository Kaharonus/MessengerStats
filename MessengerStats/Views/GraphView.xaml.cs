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

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            foreach (var item in Data) {
                WpfPlot plot = new WpfPlot();
                PlotPanel.Children.Add(plot);
                foreach (var signal in item.Data) {
                    if (signal.Value.Length == 0) {
                        continue;
                    }
                    if (item.StartDate != null) {
                        plot.plt.PlotSignal(Array.ConvertAll(signal.Value, x => (double)x), item.Padding, label: signal.Key, markerSize: 0, xOffset: item.StartDate.ToOADate());
                    } else {
                        plot.plt.PlotSignal(Array.ConvertAll(signal.Value, x => (double)x), item.Padding, label: signal.Key, markerSize: 0);

                    }
                }
                plot.plt.Ticks(dateTimeX: true);
                plot.plt.Legend(true, location: legendLocation.upperLeft);
                plot.plt.XLabel(item.XAxisLabel, enable: true);
                plot.plt.YLabel(item.YAxisLabel, enable: true);
                plot.plt.Style(
                    Helper.ConvertColor((SolidColorBrush)FindResource("BackgroundDark")), 
                    Helper.ConvertColor((SolidColorBrush)FindResource("Background")),
                    ColorTranslator.FromHtml("#888"),
                    Helper.ConvertColor((SolidColorBrush)FindResource("TextColor")),
                    Helper.ConvertColor((SolidColorBrush)FindResource("TextColor")),
                    Helper.ConvertColor((SolidColorBrush)FindResource("TextColor"))
                    );
                plot.Render();
            }
        }
    }
}

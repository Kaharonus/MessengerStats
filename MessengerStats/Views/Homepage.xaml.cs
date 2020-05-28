using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerStats.Views {
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Page {

        private string Path { get; set; }

        private List<string> searchPatterns = new List<string>();

        public Homepage() {
            InitializeComponent();
        }
        public Homepage(string path) {
            InitializeComponent();
            Path = path;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e) {
            await Task.Delay(300);
            await ConversationData.Load(Path, (s,ev) => {
                ParseName.Text = "Parsing conversation: " + s;
            });
            GraphGrid.Children.Add(new GraphView(Conversation.GenerateOverall()));
            Navigation.FadeOut(LoadingCover, () => { 
                ((Grid)LoadingCover.Parent).Children.Remove(LoadingCover);
            });
            foreach (var item in ConversationData.Conversations) {
                var i = new ConversationItem(item.Name);
                i.MouseLeftButtonUp += (s, ev) => {
                    GraphGrid.Children.Clear();
                    CurrentPersonName.Text = item.Name;
                    GraphGrid.Children.Add(new GraphView(item.GenerateData()));

                };
                ConversationStackPanel.Children.Add(i);
                searchPatterns.Add(item.Name.ToLower());
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Right now this does fuckall, later it will maybe do deep search");
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e) {
            //TODO: Optimize. This is shit and i know it.
            if (SearchText.Text.Length == 0) {
                foreach (ConversationItem item in ConversationStackPanel.Children) {
                    item.Visibility = Visibility.Visible;
                }
                return;
            }
            foreach (ConversationItem item in ConversationStackPanel.Children) {
                item.Visibility = Visibility.Collapsed;
            }
            var search = SearchText.Text.ToLower();
            foreach (var item in searchPatterns.Where(x => x.Contains(search))) {
                ConversationStackPanel.Children[searchPatterns.IndexOf(item)].Visibility = Visibility.Visible;
            }
        }

        private void OverallStats_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            GraphGrid.Children.Clear();
            CurrentPersonName.Text = "Overall stats";
            GraphGrid.Children.Add(new GraphView(Conversation.GenerateOverall()));
        }
    }
}

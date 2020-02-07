using System;
using System.Collections.Generic;
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
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Page {

        private string Path { get; set; }

        public Homepage() {
            InitializeComponent();
        }
        public Homepage(string path) {
            InitializeComponent();
            Path = path;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e) {
            await ConversationData.Load(Path);
            foreach (var item in ConversationData.Conversations) {
                ConversationItem i = new ConversationItem(item.Name);
                ConversationStackPanel.Children.Add(i);
            }
        }
    }
}

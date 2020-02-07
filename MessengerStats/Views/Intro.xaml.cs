using System;
using System.Collections.Generic;
using System.IO;
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
using Ookii.Dialogs.Wpf;
using Path = System.IO.Path;

namespace MessengerStats.Views {
    /// <summary>
    /// Interaction logic for Intro.xaml
    /// </summary>
    public partial class Intro : Page {
        public Intro() {
            InitializeComponent();
        }


        private bool ValidatePath(ref string path) {
            var dir = Path.GetFileName(path);
            if (dir.StartsWith("facebook-")) {
                path += "\\messages\\inbox";
                return ValidatePath(ref path);
            } else if (dir == "messages") {
                path += "\\inbox";
                return ValidatePath(ref path);
            } else if(dir == "inbox") {
                return Directory.Exists(path);
            }
            return false;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e) {
            var browserDialog = new VistaFolderBrowserDialog();
            //I know, I know, however this fuckers return value is bool? not bool and I do not feel like checking it for null so lets just check if its true..
            if (browserDialog.ShowDialog() == true) {
                PathText.Text = browserDialog.SelectedPath;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e) {
            string path = PathText.Text;
            if (!ValidatePath(ref path)) {
                return;
            }
            await Navigation.Navigate(new Homepage(path));
        }
    }
}

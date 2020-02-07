﻿using MessengerStats.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerStats {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e) {
            //Inital setup
            Navigation.Init(this);
            await Navigation.Navigate(new Intro());
        }
    }
}

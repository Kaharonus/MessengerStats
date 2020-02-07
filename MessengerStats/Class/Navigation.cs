using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MessengerStats {
    class Navigation {


        private static MainWindow Window { get; set; }

        public static void Init(MainWindow window) {
            Window = window;
        }

        /// <summary>
        /// Navigates to a given page. Right now isnt really async. Async functionality will be added
        /// </summary>
        /// <param name="page">Page to navigate to</param>
        /// <returns></returns>
        public async static Task Navigate(Page page) {
            Window.MainContent.Navigate(page);
        } 

    }
}

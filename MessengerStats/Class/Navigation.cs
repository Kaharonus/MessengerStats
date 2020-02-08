using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MessengerStats {
    class Navigation {


        private static MainWindow Window { get; set; }
        private static Dictionary<string, Storyboard> Animations { get; set; } = new Dictionary<string, Storyboard>();

        /// <summary>
        /// Gets the entire class ready for operation
        /// </summary>
        /// <param name="window"></param>
        public static void Init(MainWindow window) {
            Window = window;
            Animations.Add("FadeIn", (Storyboard)Application.Current.FindResource("FadeIn"));
            Animations.Add("FadeOut", (Storyboard)Application.Current.FindResource("FadeOut"));

        }

        private static RenderTargetBitmap RenderElement(FrameworkElement element) {
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            double dpi = 96;
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();
            dc.DrawRectangle(new VisualBrush(element), null, new Rect(0, 0, width, height));
            dc.Close();
            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);
            bitmap.Render(visual);
            return bitmap;
        }


        public static void FadeIn(FrameworkElement control, Action continueWith = null) {
            StartAnimation(Animations["FadeIn"], control, continueWith);
        }
        public static void FadeOut(FrameworkElement control, Action continueWith = null) {
            StartAnimation(Animations["FadeOut"], control, continueWith);
        }

        private static void StartAnimation(Storyboard sb, FrameworkElement control, Action continueWith = null) {
            var tmp = sb.Clone();
            if (continueWith != null) {
                tmp.Completed += (s, ev) => continueWith();
            }
            control.BeginStoryboard(tmp);
        }


        /// <summary>
        /// Navigates to a given page. Right now isnt really async. Async functionality will be added
        /// </summary>
        /// <param name="page">Page to navigate to</param>
        /// <returns></returns>
        public async static Task Navigate(Page page) {
            Window.MainContentOld.Source = RenderElement(Window.MainContent);
            Window.MainContent.Navigate(page);
            AnimateLocal(Window.MainContentOld, Window.MainContentOld.Margin, new Thickness(0, Window.ActualHeight * -1, 0, 0), () => {
                Window.MainContentOld.Source = null;
                AnimateLocal(Window.MainContentOld, Window.MainContentOld.Margin, new Thickness(0));
            });
            FadeOut(Window.MainContentOld, async () => {
                await Task.Delay(500);
                FadeIn(Window.MainContentOld);
            });
        }

        /// <summary>
        /// Helper function used for the slide out to the top animation
        /// </summary>
        private static void AnimateLocal(FrameworkElement element, Thickness start, Thickness end, Action completed = null) {
            ThicknessAnimation animation = new ThicknessAnimation {
                From = start,
                To = end,
                Duration = TimeSpan.FromMilliseconds(300),
                AccelerationRatio = 0.5,
                DecelerationRatio = 0.5,
            };
            Timeline.SetDesiredFrameRate(animation, 60);
            if (completed != null) {
                animation.Completed += (s, ev) => completed();
            }
            element.BeginAnimation(Grid.MarginProperty, animation);
        }

    }
}

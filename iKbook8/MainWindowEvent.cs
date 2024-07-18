using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");

        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("OnMainWindowClosing invoked...");

        }
        
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");
            HideScriptErrors(webBrowser, true);

        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private void OnBookTypeSelectChagned(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (txtInitURL != null)
            {
                switch (cmbNovelType.SelectedIndex)
                {
                    case 0:
                        txtInitURL.Text = "https://m.ikbook8.com/book/i116399132/18897986.html";
                        break;
                    case 1:
                        txtInitURL.Text = "https://book.qq.com/book-read/47135031/1";
                        break;
                    case 2:
                        txtInitURL.Text = "https://m.xbiqugew.com/book/50761/32248795.html";
                        break;
                    case 3:
                        txtInitURL.Text = "https://www.xbiqugew.com/book/18927/12811470.html";
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }
    }
}
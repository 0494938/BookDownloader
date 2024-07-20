using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                NovelTyhpeChangeToIndex(cmbNovelType.SelectedIndex);
            }
        }

        private void OnMainWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnMainWindowActivated invoked...");
            if (txtInitURL != null)
            {
                NovelTyhpeChangeToIndex(cmbNovelType.SelectedIndex);
            }
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            Debug.WriteLine("OnMainWindowClosing invoked...");
            datacontext.UnloadPgm = true;
        }

        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
    }
}
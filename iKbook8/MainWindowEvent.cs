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
    }
}
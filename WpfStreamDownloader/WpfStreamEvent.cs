using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window
    {
        private void OnBookTypeSelectChagned(object sender, SelectionChangedEventArgs e)
        {
            if (txtInitURL != null)
            {
                NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, cmbNovelType.SelectedIndex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(cmbNovelType.SelectedIndex <0)
                cmbNovelType.SelectedIndex = 17;
            NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, cmbNovelType.SelectedIndex);
            //BtnClickActions(txtInitURL.Text);

            Task.Run(async delegate
            {
                await Task.Delay(1000);
                this.Dispatcher.Invoke(() => { BtnClickActions(txtInitURL.Text); });
                
            });

        }

        private void OnLoadInBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnLoadInBrowser invoked...");
            StartUrlOnWebBrowser(txtInitURL.Text.Trim());
        }

        private void btnLaunchCurUrlOnWeb_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnLaunchCurUrlOnWeb_Click invoked...");
            StartUrlOnWebBrowser(txtNextUrl.Text.Trim());
        }


        public static void StartUrlOnWebBrowser(string strUrl)
        {
            if (!string.IsNullOrEmpty(strUrl.Trim()))
                //Process.Start("explorer", strUrl.Trim());  
                Process.Start(new ProcessStartInfo { FileName = strUrl.Trim(), UseShellExecute = true });
        }

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            txtCurURL.Text = webBrowser.CoreWebView2.Source.ToString();
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewPagesTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void OnBtnRefreshPage(object sender, RoutedEventArgs e)
        {
            webBrowser.CoreWebView2.Reload();
            txtCurURL.Text = webBrowser.CoreWebView2.Source.ToString();
        }

        private void PagesPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
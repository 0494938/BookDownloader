using System;
using System.Diagnostics;
using System.Windows;

namespace BookDownloaderWpf
{
    public partial class WindowsWPFChrome : Window
    {
        public WindowsWPFChrome()
        {
            InitializeComponent();
        }

        public static void StartUrlOnWebBrowser(string strUrl)
        {
            if (!string.IsNullOrEmpty(strUrl.Trim()))
                //Process.Start("explorer", strUrl.Trim());  
                Process.Start(new ProcessStartInfo { FileName = strUrl.Trim(), UseShellExecute = true });
        }

        private void OnLoadInBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnLoadInBrowser invoked...");
            StartUrlOnWebBrowser(txtInitURL.Text.Trim());
        }

        private void btnLaunchNextUrlOnWeb_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnLaunchNextUrlOnWeb_Click invoked...");
            StartUrlOnWebBrowser(txtCurURL.Text.Trim());
        }

        private void PreviewPagesTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
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

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSyncFromBrowser invoked...");
            txtCurURL.Text = webBrowser.Address.ToString();
        }

    }
}

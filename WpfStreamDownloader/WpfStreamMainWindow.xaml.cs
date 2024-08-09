using BaseBookDownload.WPF;
using BookStreamDownload.WPF;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfStreamDownloader
{
#pragma warning disable IDE0059 // 値の不必要な代入
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
    public partial class WpfStreamMainWindow : Window
    {
        public WpfStreamMainWindow()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //FFmpegBinariesHelper.RegisterFFmpegBinaries();
            //DynamicallyLoadedBindings.Initialize();

            InitializeComponent();
            InitBrowser();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Dispose();
        }

        private void txtInitURL_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtInitURL_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            btnInitURL.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void txtCurURL_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtCurURL_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            if (e.Key == System.Windows.Input.Key.Enter) {
                btnCurURL.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void txtNextUrl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtNextUrl_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnNextPage.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                WndSetting wndSetting = new WndSetting(datacontext)
                {
                    Owner = this
                };
                wndSetting.ShowDialog();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Window_Closed invoked...");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Window_Closing invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null) { datacontext.UnloadPgm = true; }
        }

        private void OnConvertTextToMp3(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            ConvertNovelTextToMp3 wndMp3Convert = new ConvertNovelTextToMp3(datacontext) { Owner = this };
            bool? res = wndMp3Convert.ShowDialog();
        }
    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore IDE0059 // 値の不必要な代入
}
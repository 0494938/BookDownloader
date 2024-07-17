using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");
            ClickBtntnInitURL();

        }

        private void ClickBtntnInitURL()
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                try
                {
                    webBrowser.Navigate(txtInitURL.Text);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                        return;

                    dynamic document = webBrowser.Document;

                    if (document.readyState != "complete")
                        return;
                }
            }
        }

        private void MainFrameWebLoadCompleted(object sender, NavigationEventArgs e)
        {

            Debug.WriteLine("MainFrameWebLoadCompleted invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext!=null))
            {

                var browser = sender as WebBrowser;

                if (browser == null || browser.Document == null)
                    return;

                dynamic document = browser.Document;

                if (document.readyState != "complete")
                    return;

                int nStep = 0;
                try
                {
                    dynamic script = document.createElement("script");
                    nStep = 1;
                    script.type = @"text/javascript";
                    script.text = @"window.onerror = function(msg,url,line){return true;}";
                    document.head.appendChild(script);

                    datacontext.PageLoaded = true;
                    btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

    }
}
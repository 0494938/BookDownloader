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
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public void UpdateNextPageButton() {
            this.btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
        }

        public void UpdateAnalysizedContents(string ? strContents)
        {
            txtAnalysizedContents.Text = strContents;
        }
        
        public void UpdateAggragatedContents(string strContents)
        {
            txtAggregatedContents.Text += strContents;
            txtAggregatedContents.ScrollToEnd();
        }
        
        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            if (txtAggregatedContents.Text.Length > 1024 * 64)
                txtAggregatedContents.Text = strContents;
            else
                txtAggregatedContents.Text += strContents;
            txtAggregatedContents.ScrollToEnd();
        }
        public void UpdateWebBodyOuterHtml(string strBody)
        {
            txtWebContents.Text = strBody;
        }

        public void UpdateNextUrl(string url)
        {
            txtNextUrl.Text = url;
        }

        public void UpdateCurUrl(string url)
        {
            txtCurURL.Text = url;
        }

    }
}
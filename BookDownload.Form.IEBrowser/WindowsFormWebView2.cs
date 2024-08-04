using BaseBookDownloader;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BookDownloadFormApp
{
    [ComVisible(true)]
    public partial class WindowsFormWebView2 : Form, IBaseMainWindow
    {
        public WindowsFormWebView2()
        {
            InitializeComponent();
            InitBrowser();

            txtLog.Height = panelTop.Height - txtLog.Top - 5;
            txtLog.Width = panelTop.Width - txtLog.Left * 2;
        }

        private void WindowsForm_Load(object sender, EventArgs e)
        {
            //cmbNovelType.SelectedIndex = cmbNovelType.FindString("4 无线电子书");
            cmbNovelType.SelectedIndex = 16;

            //browser.LoadUrl(cmbNovelType.Text.Trim());
        }

        public void Test(String message)
        {
            MessageBox.Show(message, "client code");
        }

        public WebView2 webBrowser;

        public async void InitBrowser()
        {
            datacontext.PgmNaviUrl = txtInitURL.Text.Trim();
            webBrowser = new WebView2(
                //txtInitURL.Text.Trim()
            );
            //this.Controls.Add(browser);
            this.scBottom.Panel1.Controls.Add(webBrowser);
            webBrowser.Dock = DockStyle.Fill;
            await webBrowser.EnsureCoreWebView2Async(null);
            webBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
            webBrowser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            //webBrowser.Loaded +=  WebBrowser_Loaded;

            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");

            btnInitURL.PerformClick();
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }


        private void splitContainer1_Panel2_Validated(object sender, EventArgs e)
        {

        }

        private void panelTop_Resize(object sender, EventArgs e)
        {
            txtLog.Height = panelTop.Height - txtLog.Top - 5;
            txtLog.Width = panelTop.Width - txtLog.Left * 2;
            txtInitURL.Width = panelTop.Width - txtInitURL.Left - 20 - 122;
            txtNextUrl.Width = panelTop.Width - txtNextUrl.Left - 20 - 122;
            btnAutoDownload.Left = txtInitURL.Left + txtInitURL.Width + 12;
            txtPages.Left = txtInitURL.Left + txtInitURL.Width + 12;
        }

        private void scResult_Panel1_Resize(object sender, EventArgs e)
        {
            txtHtml.Width = scResult.Panel1.Width - 2 * txtHtml.Top;
            txtContent.Width = scResult.Panel1.Width - 2 * txtContent.Top;
        }

        private void cmbNovelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NovelTypeChangeEvent(datacontext, cmbNovelType.SelectedIndex);
        }


        private void txtPages_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtPages_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPages.Text, "[^0-9]"))
            {
                txtPages.Text = txtPages.Text.Remove(txtPages.Text.Length - 1);
            }
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            txtProgress.Left = statusPanel.Width - (txtProgress.Top + txtProgress.Width) - 5;
            txtStatus.Width  = statusPanel.Width - txtStatus.Left - 5;
        }
    }
}

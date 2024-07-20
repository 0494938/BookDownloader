using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BookDownloader
{
    public class DownloadStatus
    {
        public bool DownloadFinished { get; set; } = false;
        public string? URL { get; set; }
        public string? NextUrl { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int Depth { get; set; } = 0;
        public int ThreadNum { get; set; }
        public static int ThreadMax { get; set; }
        private static StreamWriter? _output_writer = null;
        public static StreamWriter? ContentsWriter
        {
            get
            {
                return _output_writer;
            }
            set
            {
                if (_output_writer != null)
                {
                    _output_writer.Flush();
                    _output_writer.Close();
                }
                _output_writer = value;
            }
        }
    }

    public class WndContextData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool PageLoaded { get; set; } = false;
        public bool ContentsAnalysised { get; set; } = false;
        public bool NextLinkAnalysized { get; set; } = false;
        public BatchQueryNovelContents SiteType { get; set; } = BatchQueryNovelContents.IKBOOK8;
        public string? StartBarMsg { get; set; }
        public int ProcessBarValue { get; set; }
        public Visibility EnabledDbgButtons { get; set; } =
#if DEBUG
#if true
            Visibility.Visible;
#else
            Visibility.Hidden;
#endif
#else
            Visibility.Hidden;
#endif
    }

    public enum BatchQueryNovelContents
    {
        IKBOOK8 = 0,
        QQBOOK = 1,
        BIQUGE = 2,
        BIQUGE2 = 3,
        WXDZH = 4,
        CANGQIONG = 5,
        JINYONG = 6,
        SHUQI = 7,
        FANQIE = 8,
        FANQIE2 = 9,
        HXTX =10,//红袖添香
        XXSB = 11, //新小说吧
        YQXSB = 12,//言情小说吧
        //TOBEDONE = 13,
        //_17K = 14, //17K，Script Over Flow
    }

   
    public partial class MainWindow : Window
    {
        private void NovelTyhpeChangeToIndex(int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                switch (cmbNovelType.SelectedIndex)
                {
                    case (int)BatchQueryNovelContents.IKBOOK8:
                        txtInitURL.Text = "https://m.ikbook8.com/book/i116399132/18897986.html";
                        break;
                    case (int)BatchQueryNovelContents.QQBOOK:
                        txtInitURL.Text = "https://book.qq.com/book-read/47135031/1";
                        break;
                    case (int)BatchQueryNovelContents.BIQUGE:
                        txtInitURL.Text = "https://m.xbiqugew.com/book/50761/32248795.html";
                        break;
                    case (int)BatchQueryNovelContents.BIQUGE2:
                        txtInitURL.Text = "https://www.xbiqugew.com/book/18927/12811470.html";
                        break;
                    case (int)BatchQueryNovelContents.WXDZH:
                        txtInitURL.Text = "https://www.wxdzs.net/wxread/94612_43816524.html";
                        break;
                    case (int)BatchQueryNovelContents.CANGQIONG:
                        txtInitURL.Text = "http://www.cqhhhs.com/book/85756/28421368.html";
                        break;
                    case (int)BatchQueryNovelContents.JINYONG:
                        txtInitURL.Text = "http://www.jinhuaja.com/b/184315/976061.html";
                        break;
                    case (int)BatchQueryNovelContents.SHUQI:
                        txtInitURL.Text = "https://www.shuqi.com/reader?bid=8991909&cid=2589796";
                        break;
                    case (int)BatchQueryNovelContents.FANQIE:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7100359997917397512?enter_from=page";
                        break;
                    case (int)BatchQueryNovelContents.FANQIE2:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7268154919981580800?source=seo_fq_juhe";
                        break;
                    case (int)BatchQueryNovelContents.HXTX:
                        txtInitURL.Text = "https://www.hongxiu.com/chapter/7200532503839703/19328808342308247";
                        break;
                    case (int)BatchQueryNovelContents.XXSB:
                        txtInitURL.Text = "https://book.xxs8.com/677942/94808.html";
                        break;
                    case (int)BatchQueryNovelContents.YQXSB:
                        txtInitURL.Text = "https://www.xs8.cn/chapter/3738025904323901/10686192507259378";
                        break;
                    //case (int)BatchQueryNovelContents._17K:
                    //    txtInitURL.Text = "https://www.17k.com/chapter/3589602/48625472.html";
                    //    break;
                    //case (int)BatchQueryNovelContents.TOBEDONE:
                    //    break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        public void AnalysisHtmlBodyThreadFunc(WndContextData? datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            /*
            if (nDelayMiliSeconds > 0) { 
                Thread.Sleep(nDelayMiliSeconds);
            }
            */

            IFetchNovelContent? fetchNovelContent = null;
            int nMaxRetry = 60; //span is 3s.
            switch (datacontext?.SiteType)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    fetchNovelContent = new IKBook8NovelContent();
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    fetchNovelContent = new OOBookNovelContent();
                    break;
                case BatchQueryNovelContents.WXDZH:
                    fetchNovelContent = new WxdzsBookNovelContent();
                    break;
                case BatchQueryNovelContents.CANGQIONG:
                    fetchNovelContent = new CangQiongBookNovelContent();
                    break;
                case BatchQueryNovelContents.JINYONG:
                    fetchNovelContent = new JinYongBookNovelContent();
                    break;
                case BatchQueryNovelContents.SHUQI:
                    nMaxRetry = 60;
                    fetchNovelContent = new ShuQiBookNovelContent();
                    break;
                case BatchQueryNovelContents.FANQIE:
                case BatchQueryNovelContents.FANQIE2:
                    nMaxRetry = 60;
                    fetchNovelContent = new FanQieBookNovelContent();
                    break;
                case BatchQueryNovelContents.BIQUGE:
                case BatchQueryNovelContents.BIQUGE2:
                    fetchNovelContent = new BiQuGeBookNovelContent();
                    break;
                case BatchQueryNovelContents.HXTX:
                    fetchNovelContent = new HXTXBookNovelContent();
                    break;
                case BatchQueryNovelContents.XXSB:
                    fetchNovelContent = new XXSBBookNovelContent();
                    break;
                case BatchQueryNovelContents.YQXSB:
                    fetchNovelContent = new YQXSBBookNovelContent();
                    break;
                default:
                    break;
            }

            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToString() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum ?? 1 - 1 + 0.5))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToString() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);

            if (fetchNovelContent != null)
            {
                fetchNovelContent.AnalysisHtmlBookBody(this, datacontext, strBody, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No." + status?.ThreadNum ?? 1 + " ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum ?? 1))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No. " + status?.ThreadNum ?? 1 + "...", 100);

            if (bSilenceMode)
            {
                if (status?.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(datacontext, status.NextUrl);
                }
                else
                {
                    DownloadStatus.ContentsWriter = null;
                    UpdateStatusMsg(datacontext, "Finished batch download(Total " + status?.ThreadNum + " Downloaded) ...", 100);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(status?.NextUrl))
                            txtInitURL.Text = status.NextUrl;
                        MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
                    });
                }
            }
        }
    }
}
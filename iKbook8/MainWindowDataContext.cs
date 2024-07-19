using MSHTML;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

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
        TOBEDONE = 8,
        FANQIE = TOBEDONE + 1,
        FANQIE2 = TOBEDONE + 2,
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

                    case (int)BatchQueryNovelContents.TOBEDONE:
                        break;
                    case (int)BatchQueryNovelContents.FANQIE:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7100359997917397512?enter_from=page";
                        break;
                    case (int)BatchQueryNovelContents.FANQIE2:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7268154919981580800?source=seo_fq_juhe";
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }
    }
}
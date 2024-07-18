using MSHTML;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace iKbook8
{
    public class DownloadStatus
    {
        public bool DownloadFinished { get; set; } = false;
        public string URL { get; set; }
        public string NextUrl { get; set; }
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

    public enum BatchQueryNovelContents
    {
        IKBOOK8 = 0,
        QQBOOK = 1,
        BIQUGE = 2,
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
        public string StartBarMsg { get; set; }
        public int ProcessBarValue { get; set; }

    }

}
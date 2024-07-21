using System;
using System.ComponentModel;
using System.IO;

namespace BaseBookDownload
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
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

    public class BaseWndContextData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool BackGroundNotRunning { get; set; } = true;
        bool _pageLoaded = false;
        public bool PageLoaded { get { return _pageLoaded && BackGroundNotRunning; } set { _pageLoaded = value; } }
        bool _contentsAnalysised = false;
        public bool ContentsAnalysised { get { return _contentsAnalysised && BackGroundNotRunning; } set { _contentsAnalysised = value; } }
        bool _nextLinkAnalysized = false;
        public bool NextLinkAnalysized { get { return _nextLinkAnalysized && BackGroundNotRunning; } set { _nextLinkAnalysized = value; } } 
        public BatchQueryNovelContents SiteType { get; set; } = BatchQueryNovelContents.IKBOOK8;
        public string? StartBarMsg { get; set; }
        public int ProcessBarValue { get; set; }
        public string? PgmNaviUrl { get; set; }
        public bool UnloadPgm { get; set; } = false;
    }

    public enum BatchQueryNovelContents
    {
        [EnumCode("爱看书吧")]
        IKBOOK8 = 0,
        [EnumCode("QQ电子书")]
        QQBOOK = 1,
        [EnumCode("笔趣阁")]
        BIQUGE = 2,
        [EnumCode("笔趣阁")]
        BIQUGE2 = 3,
        [EnumCode("无线电子书")]
        WXDZH = 4,
        [EnumCode("苍穹小说")]
        CANGQIONG = 5,
        [EnumCode("金庸小说网")]
        JINYONG = 6,
        [EnumCode("书旗小说网")]
        SHUQI = 7,
        [EnumCode("番茄小说网")]
        FANQIE = 8,
        [EnumCode("番茄小说网")]
        FANQIE2 = 9,
        [EnumCode("红袖添香")]
        HXTX = 10,//红袖添香
        [EnumCode("新小说吧")]
        XXSB = 11, //新小说吧
        [EnumCode("言情小说吧")]
        YQXSB = 12,//言情小说吧
        [EnumCode("17K小说网")]
        _17K = 13, //17K，Script Over Flow
        //TOBEDONE = 13,
    }

    public static class BatchQueryNovelContentsExtensions
    {
        static BatchQueryNovelContentsExtensions()
        {
            EnumCodeExtensions.AddEnumCodeCache<BatchQueryNovelContents>();
        }

        public static string ToCode(this BatchQueryNovelContents enumKey)
        {
            return EnumCodeExtensions.GetEnumCode(enumKey);
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}
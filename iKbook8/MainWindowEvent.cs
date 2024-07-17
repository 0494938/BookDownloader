using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace iKbook8
{
    public enum BatchQueryNovelContents
    {
        IKBOOK8=0,
        XXX=1,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
    }

    public partial class MainWindow : Window
    {
     
        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");

        }

        private void OnMainWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnMainWindowActivated invoked...");
            ClickBtntnInitURL();
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("OnMainWindowClosing invoked...");

        }
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");

        }


    }
}
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iKbook8
{
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
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");

        }
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");

        }


    }
}
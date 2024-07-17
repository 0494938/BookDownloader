using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
//using HtmlElement = System.Windows.Forms.HtmlElement;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bool bWebPageLoadFinished = false;
        public MainWindow()
        {
            InitializeComponent();
            //this.OnClosing += OnMainWindowClosing;
            //this.OnActivated += OnMainWindowActivated;
        }

        private void MainFrameWebLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainFrameWebLoaded invoked...");
        }



        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
        }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    internal interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(ref Guid guidService, ref Guid riid);
    }

}
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TiffViewerViewModel(@"C:\Users\Yury\Desktop\multipage_tif_example.tif");
        }
    }
}
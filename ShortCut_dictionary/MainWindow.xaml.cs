using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShortCut_dictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { this.DragMove(); };
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = (DictClass)(sender as ContentControl).Tag;
            Clipboard.SetText(element.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Srch_txb.Focus();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Srch_txb.Focus();
        }
    }

}

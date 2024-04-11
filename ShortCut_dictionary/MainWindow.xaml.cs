using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

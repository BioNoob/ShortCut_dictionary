using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShortCut_dictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICloseableResult
    {
        public MainWindow()
        {
            InitializeComponent();
            var a = BaseUriHelper.GetBaseUri(this).AbsoluteUri;
            a = a.Substring(0, a.LastIndexOf("/")) + "Resources/Russian_b.dic";
            Settings.base_uri = new Uri(a);
            IList dictionaries = SpellCheck.GetCustomDictionaries(Srch_txb);
            dictionaries.Add(Settings.base_uri);
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

        public void Close(bool state, object result)
        {
            IList dictionaries = SpellCheck.GetCustomDictionaries(Srch_txb);
            dictionaries.Remove(Settings.base_uri);
            Environment.Exit(0);
        }
    }

}

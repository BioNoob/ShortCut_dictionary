using ShortCut_dictionary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            System.Text.Encoding.RegisterProvider(
    System.Text.CodePagesEncodingProvider.Instance);
            a = a.Substring(0, a.LastIndexOf("/")) + "Resources/Russian_b.dic";
            Settings.base_uri = new Uri(a);
            IList dictionaries = SpellCheck.GetCustomDictionaries(Srch_txb);
            dictionaries.Add(Settings.base_uri);
            this.MouseLeftButtonDown += delegate { this.DragMove(); };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Srch_txb.Focus();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            //Srch_txb.Focus();
        }

        public void Close(bool state, object result)
        {
            IList dictionaries = SpellCheck.GetCustomDictionaries(Srch_txb);
            dictionaries.Remove(Settings.base_uri);
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetsPopUp.IsOpen = !SetsPopUp.IsOpen;
            SetBtn.IsEnabled = false;
        }
        private void SetsPopUp_Closed(object sender, EventArgs e)
        {
            SetBtn.IsEnabled = true;
        }

        private void SearchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = DataContext as MainModel;
            if (e.RemovedItems.Count > 0)
                viewmodel.ListOfSelected.RemoveRange(e.RemovedItems.Cast<DictClass>());
            if (e.AddedItems.Count > 0)
                viewmodel.ListOfSelected.AddRange(e.AddedItems.Cast<DictClass>());
            //viewmodel.ListOfSelected.ToList().ForEach(t => System.Diagnostics.Debug.WriteLine(t));
        }
    }

}

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShortCut_dictionary
{
    /// <summary>
    /// Логика взаимодействия для NewRecord.xaml
    /// </summary>
    public partial class NewRecord : Window, ICloseableResult
    {
        public NewRecord()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { this.DragMove(); };
        }
        public object Result { get; set; }

        public void Close(bool state, object result)
        {
            Result = result;
            DialogResult = state;
            if(DataContext as IDisposable != null)
            {
                (DataContext as IDisposable).Dispose();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Short_txb.Focus();
            //SpellCheck.CustomDictionaries
            
        }
    }
}

using System;
using System.Windows;

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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Short_txb.Focus();
        }
    }
}

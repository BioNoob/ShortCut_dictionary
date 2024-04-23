using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ShortCut_dictionary
{
    public static class Settings
    {
        public static Uri base_uri { get; set; }
        private static bool _chk_bx_case;
        private static bool _chk_bx_first_case;
        private static int windowSize_y;
        private static int windowLastPos_y;
        private static int windowSize_x;
        private static int windowLastPos_x;
        private static string _filepath = Directory.GetCurrentDirectory() + "\\dictionary.json";
        private static SeparatorsFormat sep_save;
        private static SeparatorsFormat sep_imp;
        private static EncodingFormat selectedImportEncoding;
        public static EncodingFormat SelectedImportEncoding { get => selectedImportEncoding; set { selectedImportEncoding = value; SetProperty("SelectedImportEncoding"); } }
        public static SeparatorsFormat Selected_sep_imp { get => sep_imp; set { sep_imp = value; SetProperty("Selected_sep_imp"); } }
        public static SeparatorsFormat Selected_sep_save { get => sep_save; set { sep_save = value; SetProperty("Selected_sep_save"); } }
        public static string FilePath { get => _filepath; set { if (_filepath != value) { _filepath = value; SetProperty("FilePath"); } } }
        public static int WindowSise_x { get => windowSize_x; set { if (windowSize_x != value) { windowSize_x = value; SetProperty("WindowSise_x"); } } }
        public static int WindowLastPos_x { get => windowLastPos_x; set { if (windowLastPos_x != value) { windowLastPos_x = value; SetProperty("WindowLastPos_x"); } } }
        public static int WindowSise_y { get => windowSize_y; set { if (windowSize_y != value) { windowSize_y = value; SetProperty("WindowSise_y"); } } }
        public static int WindowLastPos_y { get => windowLastPos_y; set { if (windowLastPos_y != value) { windowLastPos_y = value; SetProperty("WindowLastPos_y"); } } }
        public static bool ChkBxCase { get => _chk_bx_case; set { if (_chk_bx_case != value) { _chk_bx_case = value; SetProperty("ChkBxCase"); } } }
        public static bool ChkBxFirstCase { get => _chk_bx_first_case; set { if (_chk_bx_first_case != value) { _chk_bx_first_case = value; SetProperty("ChkBxFirstCase"); } } }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static string DecodeSepFormat(SeparatorsFormat sep)
        {
            switch (sep)
            {
                case SeparatorsFormat.Semicolon:
                    return ";";
                case SeparatorsFormat.Comma:
                    return ",";
                case SeparatorsFormat.Longdash:
                    return "–";
                case SeparatorsFormat.Dash:
                    return "-";
                case SeparatorsFormat.Tab:
                    return "\t";
                case SeparatorsFormat.Space:
                    return " ";
            }
            return "";
        }

        public static void SetProperty([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        public static void SaveSettings()
        {
            Properties.Settings x = Properties.Settings.Default;
            x.LastPos = new Point(WindowLastPos_x, WindowLastPos_y);
            x.LastSize = new Size(WindowSise_x, WindowSise_y);
            x.NewRec_upper = ChkBxCase;
            x.NewRec_upper_first = ChkBxFirstCase;
            x.Selected_sep_imp = (int)Selected_sep_imp;
            x.Selected_sep_save = (int)Selected_sep_save;
            x.SelectedImportEncoding = (int)SelectedImportEncoding;
            x.Save();
        }
        public static void LoadSettings()
        {
            Properties.Settings x = Properties.Settings.Default;
            WindowSise_x = x.LastSize.Width;
            WindowSise_y = x.LastSize.Height;
            WindowLastPos_x = x.LastPos.X;
            WindowLastPos_y = x.LastPos.Y;
            ChkBxCase = x.NewRec_upper;
            ChkBxFirstCase = x.NewRec_upper_first;
            Selected_sep_imp = (SeparatorsFormat)x.Selected_sep_imp;
            Selected_sep_save = (SeparatorsFormat)x.Selected_sep_save;
            SelectedImportEncoding = (EncodingFormat)x.SelectedImportEncoding;
        }

        public static WpfObservableRangeCollection<DictClass> LoadJson()
        {

            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Close();
            }
            return JsonConvert.DeserializeObject<WpfObservableRangeCollection<DictClass>>(File.ReadAllText(FilePath));
        }

        public static bool SaveJsone(WpfObservableRangeCollection<DictClass> coll)
        {
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(coll));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return true;
        }
    }
}

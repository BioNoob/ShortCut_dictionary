using System;

namespace ShortCut_dictionary.Models
{
    public class NewRecordModel : Proper, IDisposable
    {
        private bool is_rec_to_changed = false;
        public bool Is_rec_to_change { get => is_rec_to_changed; set => SetProperty(ref is_rec_to_changed, value); }
        private DictClass _DictRec;
        private DictClass buff;
        public DictClass DictRec
        {
            get => _DictRec; set
            {
                buff = value;
                if (_DictRec is null)
                    _DictRec = new DictClass();
                if (!(value is null))
                {
                    _DictRec.Full = value.Full;
                    _DictRec.Short = value.Short;
                }
                Check_settings();
            }
        }

        public void SetData(DictClass dt) => DictRec = dt;
        public DictClass GetData() => new DictClass(DictRec.Short, DictRec.Full);

        public NewRecordModel()
        {
            DictRec = new DictClass();
            DictRec.PropertyChanged += _DictRec_PropertyChanged;
            buff = new DictClass();
            Settings.StaticPropertyChanged += Settings_StaticPropertyChanged;
        }

        private void _DictRec_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (DictRec != null)
            {
                if (!DictRec.Equals(buff))
                    Is_rec_to_change = true;
                else
                    Is_rec_to_change = false;
            }
            Check_settings();
        }
        private void Check_settings(string PropertyName = "")
        {
            Check_settings(DictRec, PropertyName);
        }
        public static void Check_settings(DictClass DictRec, string PropertyName = "")
        {
            if (PropertyName == "ChkBxCase" || PropertyName == "")
            {
                if (Settings.ChkBxCase)
                {
                    DictRec.Short = DictRec.Short.ToUpper();
                }
            }
            if (PropertyName == "ChkBxFirstCase" || PropertyName == "")
            {
                if (Settings.ChkBxFirstCase)
                {
                    if (DictRec.Full.Length > 0 && !DictRec.Full.StartsWith(char.ToUpper(DictRec.Full[0])))
                        DictRec.Full = DictRec.Full[1..].Insert(0, char.ToUpper(DictRec.Full[0]).ToString());
                }
            }
        }
        private void Settings_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Check_settings(e.PropertyName);
        }

        public void Dispose()
        {
            buff = null;
            DictRec = null;
            _reject = null;
            _approve = null;
            Settings.StaticPropertyChanged -= Settings_StaticPropertyChanged;
        }

        private CommandHandler _reject;
        private CommandHandler _approve;

        public CommandHandler ApproveCommand
        {
            get
            {
                return _approve ??= new CommandHandler(obj =>
                {
                    var windowFacade = obj as ICloseableResult;
                    if (DictRec.IsValid)
                        windowFacade?.Close(true, DictRec);
                    else
                    {
                        return;
                    }
                },
                (obj) => true
                );
            }
        }
        public CommandHandler RejectCommand
        {
            get
            {
                return _reject ??= new CommandHandler(obj =>
                {
                    var windowFacade = obj as ICloseableResult;
                    DictRec = null;
                    windowFacade?.Close(false, DictRec);
                },
                (obj) => true
                );
            }
        }
    }
}

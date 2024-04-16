using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShortCut_dictionary.Models
{
    public class NewRecordModel : Proper
    {
        DictClass _DictRec;
        public DictClass DictRec { get => _DictRec; set => SetProperty(ref _DictRec, value); }
        public NewRecordModel()
        {
            DictRec = new DictClass();
        }
        private CommandHandler _reject;
        private CommandHandler _approve;
        private CommandHandler _import;
        public CommandHandler ImportCommand
        {
            get
            {
                return _import ??= new CommandHandler(obj =>
                {
                    OpenFileDialog Ofd = new OpenFileDialog();
                    Ofd.Title = "Open csv file";
                    Ofd.InitialDirectory = Directory.GetCurrentDirectory();
                    Ofd.DefaultExt = ".csv";
                    Ofd.Multiselect = false;
                    Ofd.Filter = "All Files (*.*)|*.*" +
                    "|CSV files (*.csv)|*.csv";
                    var windowFacade = obj as ICloseableResult;
                    if (Ofd.ShowDialog() == true)
                    {
                        var buf = File.ReadLines(Ofd.FileName).ToList();
                        List<DictClass> dc = new List<DictClass>();
                        buf.ForEach(t =>
                        {
                            string[] k = t.Split(";");
                            dc.Add(new DictClass(k[0], k[1]));
                        }
                        );
                        windowFacade?.Close(true, dc);
                    }
                    else
                        return;

                },
                (obj) => true
                );
            }
        }
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

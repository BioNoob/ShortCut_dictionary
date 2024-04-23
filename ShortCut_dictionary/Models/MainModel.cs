﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace ShortCut_dictionary.Models
{

    public class MainModel : Proper
    {
        private string search_text;
        private bool is_saved;
        public string Search_text
        {
            get => search_text;
            set
            {
                SetProperty(ref search_text, value);
                Search();
            }
        }
        public bool IsSaved
        {
            get => is_saved;
            set => SetProperty(ref is_saved, value);
        }
        public static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
        private void Search()
        {
            if (ListOfDict != null)
            {
                if (!string.IsNullOrEmpty(search_text) && search_text.Length > 1)
                {
                    if (IsValidRegex(search_text))
                    {
                        var reg = new Regex(@$"{search_text}", RegexOptions.IgnoreCase);
                        FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict.Where(t => reg.IsMatch(t.Full) |
                        reg.IsMatch(t.Short)).OrderBy(t => t.Short));
                    }
                }
                else
                    FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict.OrderBy(t => t.Short));

            }
        }

        private CommandHandler _closewindow;
        private CommandHandler _addnewshcrec;
        private CommandHandler _delhcrec;
        private CommandHandler _loadedcmd;
        private CommandHandler _rejectsearch;
        private CommandHandler _editsearch;
        private CommandHandler _savecmd;
        private CommandHandler _import;
        private CommandHandler _removeduplicate;
        private CommandHandler _copy_clipboard;
        public CommandHandler RemoveduplicateCommand
        {
            get
            {
                return _removeduplicate ??= new CommandHandler(obj =>
                {

                    var a = ListOfDict.NonDistinct(t => t.ToString()).ToList();
                    ListOfDict.RemoveRange(a);
                    Search();
                },
                (obj) => true
                );
            }
        }
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
                    if (Ofd.ShowDialog() == true)
                    {
                        //Добавить форму импорта с выбором кодировки и разделителя.
                        //Добавить копирование чере список выделенных с указанаием разделителя
                        Encoding ec = Encoding.Default;
                        switch (Settings.SelectedImportEncoding)
                        {
                            case EncodingFormat.UTF8:
                                ec = Encoding.UTF8;
                                break;
                            case EncodingFormat.UTF16:
                                ec = Encoding.Unicode;
                                break;
                            case EncodingFormat.Windows1251:
                                ec = Encoding.GetEncoding(1251);
                                break;
                        }
                        var buf = File.ReadLines(Ofd.FileName, ec).ToList();
                        List<DictClass> dc = new List<DictClass>();
                        buf.ForEach(t =>
                        {
                            string[] k = t.Split(Settings.DecodeSepFormat(Settings.Selected_sep_imp));
                            if (k.Count() != 2)
                            {
                                System.Windows.MessageBox.Show("Error recognize import file");
                                return;
                            }
                            var a = new DictClass(k[0].Trim(), k[1].Trim());
                            NewRecordModel.Check_settings(a);
                            dc.Add(a);
                        });
                        ListOfDict.AddRange(dc);
                        Search();
                    }
                    else
                        return;

                },
                (obj) => true
                );
            }
        }
        public CommandHandler SaveJsnCmd
        {
            get
            {
                return _savecmd ??= new CommandHandler(obj =>
                {
                    IsSaved = true;
                    Settings.SaveJsone(ListOfDict);
                    IsSaved = false;
                },
                (obj) => true
                );
            }
        }
        public CommandHandler RejSearchCommand
        {
            get
            {
                return _rejectsearch ??= new CommandHandler(obj =>
                {
                    Search_text = string.Empty;
                },
                (obj) => true
                );
            }
        }
        public CommandHandler EditRecCommand
        {
            get
            {
                return _editsearch ??= new CommandHandler(obj =>
                {
                    NewRecord nw = new NewRecord();
                    nw.Top = Settings.WindowLastPos_y;
                    nw.Left = Settings.WindowLastPos_x + Settings.WindowSise_x + 10;
                    var z = obj as DictClass;
                    (nw.DataContext as NewRecordModel).SetData(z);
                    if (nw.ShowDialog() == true)
                    {
                        var t = ListOfDict.SingleOrDefault(t => t.Full == z.Full && t.Short == z.Short);
                        var buf = (nw.DataContext as NewRecordModel).GetData();
                        t.Full = buf.Full;
                        t.Short = buf.Short;
                        Search();
                    }
                    GC.Collect();
                },
                (obj) => true
                );
            }
        }
        public CommandHandler DelRecCommand
        {
            get
            {
                return _delhcrec ??= new CommandHandler(obj =>
                {
                    ListOfDict.Remove(obj as DictClass);
                    Search();
                },
                (obj) => true
                );
            }
        }
        public CommandHandler AddNewRecordCommand
        {
            get
            {
                return _addnewshcrec ??= new CommandHandler(obj =>
                {
                    NewRecord nw = new NewRecord();
                    nw.Top = Settings.WindowLastPos_y;
                    nw.Left = Settings.WindowLastPos_x + Settings.WindowSise_x + 10;
                    if (nw.ShowDialog() == true)
                    {
                        if (nw.Result is List<DictClass>)
                            ListOfDict.AddRange(nw.Result as List<DictClass>);
                        else
                            ListOfDict.Add(nw.Result as DictClass);
                        Search();
                    }
                    GC.Collect();
                },
                (obj) => true
                );
            }
        }
        //http://blog.functionalfun.net/2009/02/how-to-databind-to-selecteditems.html
        public CommandHandler CloseWindowCommand
        {
            get
            {
                return _closewindow ??= new CommandHandler(obj =>
                {
                    var windowFacade = obj as ICloseableResult;
                    Settings.SaveSettings();
                    Settings.SaveJsone(ListOfDict);
                    windowFacade?.Close(false, null);
                },
                (obj) => true
                );
            }
        }
        public CommandHandler LoadedCommand
        {
            get
            {
                return _loadedcmd ??= new CommandHandler(obj =>
                {
                    Settings.LoadSettings();
                    ListOfDict = Settings.LoadJson();
                    if (ListOfDict is null)
                        ListOfDict = new WpfObservableRangeCollection<DictClass>();
                    Search_text = "";
                },
                (obj) => true
                );
            }
        }
        public CommandHandler CopyClipCommand
        {
            get
            {
                return _copy_clipboard ??= new CommandHandler(obj =>
                {
                    string pit = string.Empty;
                    pit += string.Join("\r\n",ListOfSelected.OrderBy(t=>t.Short));
                    pit += "\r\n";
                    Clipboard.SetText(pit);
                },
                (obj) => true
                );
            }
        }        

        public WpfObservableRangeCollection<DictClass> FilteredListOfDict { get => _filteredListofdict; set => SetProperty(ref _filteredListofdict, value); }
        private WpfObservableRangeCollection<DictClass> _filteredListofdict;


        public WpfObservableRangeCollection<DictClass> ListOfDict { get => _listofdict; set => SetProperty(ref _listofdict, value); }
        private WpfObservableRangeCollection<DictClass> _listofdict;
        public WpfObservableRangeCollection<DictClass> ListOfSelected { get => _listofselected; set => SetProperty(ref _listofselected, value); }
        private WpfObservableRangeCollection<DictClass> _listofselected;
        public MainModel()
        {
            ListOfDict = new WpfObservableRangeCollection<DictClass>();
            FilteredListOfDict = new WpfObservableRangeCollection<DictClass>();
            ListOfSelected = new WpfObservableRangeCollection<DictClass>();
        }
    }
}

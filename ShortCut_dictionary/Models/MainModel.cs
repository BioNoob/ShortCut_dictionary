﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                WpfObservableRangeCollection<DictClass> res = new WpfObservableRangeCollection<DictClass>();
                if (!string.IsNullOrEmpty(search_text) && search_text.Length > 1)
                {
                    if (IsValidRegex(search_text))
                    {
                        var reg = new Regex(@$"{search_text}", RegexOptions.IgnoreCase);
                        FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict.Where(t => reg.IsMatch(t.Full) |
                        reg.IsMatch(t.Short)).ToList());
                    }
                }
                else
                    FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict);
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
                        var buf = File.ReadLines(Ofd.FileName).ToList();
                        List<DictClass> dc = new List<DictClass>();
                        buf.ForEach(t =>
                        {
                            string[] k = t.Split(";");
                            if (k.Count() != 2)
                            {
                                k = t.Split(",");
                                if (k.Count() != 2)
                                {
                                    k = t.Split(" ");
                                    if (k.Count() != 2)
                                    {
                                        k = t.Split("\t");
                                        if (k.Count() != 2)
                                        {
                                            System.Windows.MessageBox.Show("Error recognize import file");
                                            return;
                                        }
                                    }
                                }
                            }
                            dc.Add(new DictClass(k[0], k[1]));
                        });
                        ListOfDict.AddRange(dc);
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

        public CommandHandler CloseWindowCommand
        {
            get
            {
                return _closewindow ??= new CommandHandler(obj =>
                {
                    Settings.SaveSettings();
                    Settings.SaveJsone(ListOfDict);
                    Environment.Exit(0);
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

        public WpfObservableRangeCollection<DictClass> FilteredListOfDict { get => _filteredListofdict; set => SetProperty(ref _filteredListofdict, value); }
        private WpfObservableRangeCollection<DictClass> _filteredListofdict;


        public WpfObservableRangeCollection<DictClass> ListOfDict { get => _listofdict; set => SetProperty(ref _listofdict, value); }
        private WpfObservableRangeCollection<DictClass> _listofdict;
        public MainModel()
        {
            ListOfDict = new WpfObservableRangeCollection<DictClass>();
            FilteredListOfDict = new WpfObservableRangeCollection<DictClass>();


            //Sets = new Settings();
        }
    }
}

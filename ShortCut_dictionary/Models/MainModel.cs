using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ShortCut_dictionary.Models
{
    public class CommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public CommandHandler(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
    public class MainModel : Proper
    {
        private const uint _filterStartedCounter = 1;

        private string search_text;
        public string Search_text
        {
            get => search_text;
            set
            {
                SetProperty(ref search_text, value);
                Search();
            }
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

            //if (search_text.Trim().Length < _filterStartedCounter)
            //{
            //    FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict);
            //    return;
            //}
            //try
            //{
            //    Regex r = new Regex(search_text);
            //    FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(
            //        ListOfDict.Where(str => r.Matches(str.Short).ToList().Count != 0 |
            //        r.Matches(str.Full).ToList().Count != 0));
            //    return;
            //}
            //catch
            //{
            //    FilteredListOfDict = new WpfObservableRangeCollection<DictClass>(ListOfDict);
            //    return;
            //}
        }

        private CommandHandler _closewindow;
        private CommandHandler _addnewshcrec;
        private CommandHandler _delhcrec;
        private CommandHandler _loadedcmd;
        private CommandHandler _rejectsearch;
        private CommandHandler _editsearch;
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
                    nw.Top = this.Sets.WindowLastPos.Y;
                    nw.Left = this.Sets.WindowLastPos.X + this.Sets.WindowSise.Y + 10;
                    (nw.DataContext as NewRecordModel).ChkBxCase = Sets.ChkBxCase;
                    var z = obj as DictClass;
                    var buf = new DictClass(z.Short, z.Full);
                    (nw.DataContext as NewRecordModel).DictRec = buf;
                    if (nw.ShowDialog() == true)
                    {
                        var t = ListOfDict.SingleOrDefault(t => t.Full == z.Full && t.Short == z.Short);
                        t.Full = buf.Full;
                        t.Short = buf.Short;
                        Search();
                    }
                    Sets.ChkBxCase = (nw.DataContext as NewRecordModel).ChkBxCase;
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
                    nw.Top = this.Sets.WindowLastPos.Y;
                    nw.Left = this.Sets.WindowLastPos.X + this.Sets.WindowSise.Y + 10;
                    (nw.DataContext as NewRecordModel).ChkBxCase = Sets.ChkBxCase;
                    if (nw.ShowDialog() == true)
                    {
                        if (nw.Result is List<DictClass>)
                            ListOfDict.AddRange(nw.Result as List<DictClass>);
                        else
                            ListOfDict.Add(nw.Result as DictClass);
                        Search();
                    }
                    Sets.ChkBxCase = (nw.DataContext as NewRecordModel).ChkBxCase;
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
                    Sets.SaveSettings();
                    Sets.SaveJsone(ListOfDict);
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
                    Sets.LoadSettings();
                    ListOfDict = Sets.LoadJson();
                    if (ListOfDict is null)
                        ListOfDict = new WpfObservableRangeCollection<DictClass>();
                    Search_text = "";
                },
                (obj) => true
                );
            }
        }
        private Settings sets;
        public Settings Sets { get => sets; set => SetProperty(ref sets, value); }

        public WpfObservableRangeCollection<DictClass> FilteredListOfDict { get => _filteredListofdict; set => SetProperty(ref _filteredListofdict, value); }
        private WpfObservableRangeCollection<DictClass> _filteredListofdict;


        public WpfObservableRangeCollection<DictClass> ListOfDict { get => _listofdict; set => SetProperty(ref _listofdict, value); }
        private WpfObservableRangeCollection<DictClass> _listofdict;
        public MainModel()
        {
            ListOfDict = new WpfObservableRangeCollection<DictClass>();
            FilteredListOfDict = new WpfObservableRangeCollection<DictClass>();


            Sets = new Settings();
        }
    }
}

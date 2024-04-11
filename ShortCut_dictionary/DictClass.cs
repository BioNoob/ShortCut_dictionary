using Newtonsoft.Json;

namespace ShortCut_dictionary
{
    public class DictClass : Proper
    {
        private string _Short;
        private string _Full;
        private bool _isvalid;
        public string Short { get => _Short; set { SetProperty(ref _Short, value); IsValid = value.Length > 1 && !string.IsNullOrWhiteSpace(Full); } }
        public string Full { get => _Full; set { SetProperty(ref _Full, value); IsValid = Short.Length > 1 && !string.IsNullOrWhiteSpace(value); } }
        [JsonIgnore]
        public bool IsValid { get => _isvalid; set => SetProperty(ref _isvalid, value); }
        public DictClass()
        {
            Short = "";
            Full = "";
            IsValid = false;
        }
        public DictClass(string _short, string _full)
        {
            Short = _short;
            Full = _full;
        }
        public override string ToString()
        {
            return Short + " " + Full;
        }
    }
}

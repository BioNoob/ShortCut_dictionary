using Newtonsoft.Json;

namespace ShortCut_dictionary
{
    public class DictClass : Proper
    {
        private string _Short;
        private string _Full;
        private bool _isvalid;
        private bool _isupper;
        public string Short { get => _Short; set { SetProperty(ref _Short, value); IsValid = value.Length > 1 && !string.IsNullOrWhiteSpace(Full); } }
        public string Full
        {
            get => _Full; set
            {
                if (IsFirstUpper)
                    if (value.Length > 0 && !value.StartsWith(char.ToUpper(value[0])))
                        value = value.Substring(1).Insert(0, char.ToUpper(value[0]).ToString());
                SetProperty(ref _Full, value); IsValid = Short.Length > 1 && !string.IsNullOrWhiteSpace(value);
            }
        }
        [JsonIgnore]
        public bool IsValid { get => _isvalid; set => SetProperty(ref _isvalid, value); }
        [JsonIgnore]
        public bool IsFirstUpper { get => _isupper; set { SetProperty(ref _isupper, value); Full = _Full; } }
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

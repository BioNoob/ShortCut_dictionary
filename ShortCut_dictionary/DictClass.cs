using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return Short + Settings.DecodeSepFormat(Settings.Selected_sep_save) + Full;
        }
        public override bool Equals(object objj)
        {
            if (objj is DictClass)
            {
                var obj = objj as DictClass;
                return Full == obj.Full && Short == obj.Short;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return Short.GetHashCode() + Full.GetHashCode();
        }
    }
}

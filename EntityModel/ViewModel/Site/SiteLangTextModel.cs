using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SiteLangTextModel
    {
        public SiteLangTextModel()
        {
            Group1 = new Dictionary<string, string>();
            Group2 = new Dictionary<string, string>();
            Group3 = new Dictionary<string, string>();
            Group4 = new Dictionary<string, string>();
            Group5 = new Dictionary<string, string>();
            Group6 = new Dictionary<string, string>();
            GroupKey1 = new Dictionary<string, string>();
            GroupKey2 = new Dictionary<string, string>();
            GroupKey3 = new Dictionary<string, string>();
            GroupKey4 = new Dictionary<string, string>();
            GroupKey5 = new Dictionary<string, string>();
            GroupKey6 = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Group1 { get; set; }
        public Dictionary<string, string> Group2 { get; set; }
        public Dictionary<string, string> Group3 { get; set; }
        public Dictionary<string, string> Group4 { get; set; }
        public Dictionary<string, string> Group5 { get; set; }
        public Dictionary<string, string> Group6 { get; set; }
        public Dictionary<string, string> GroupKey1 { get; set; }
        public Dictionary<string, string> GroupKey2 { get; set; }
        public Dictionary<string, string> GroupKey3 { get; set; }
        public Dictionary<string, string> GroupKey4 { get; set; }
        public Dictionary<string, string> GroupKey5 { get; set; }
        public Dictionary<string, string> GroupKey6 { get; set; }
    }
}

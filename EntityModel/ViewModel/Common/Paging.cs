using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Paging<T> where T : class
    {
        public Paging() {
            total = 0;
            rows = new List<T>();
        }
        public int total { get; set; }
        public List<T> rows { get; set; }
    }

    public class PagingInfo<T> where T : class
    {
        public PagingInfo()
        {
            total = 0;
            rows = new List<T>();
        }
        public int total { get; set; }
        public List<T> rows { get; set; }
        public string Info { get; set; }
    }
}

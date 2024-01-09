using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
   public  class EmptySearchModel : SearchModelBase
    {
        public EmptySearchModel()
        {
            Name = "";
        }
        public string Name { get; set; }
    }
}

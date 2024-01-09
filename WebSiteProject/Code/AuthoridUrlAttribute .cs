using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteProject.Code
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AuthoridUrlAttribute : Attribute
    {
        private string _url;
        private string _parameter;
        public AuthoridUrlAttribute(string url,string parameter)
        {
            _url = url;
            _parameter = parameter;
        }
        public string GetUrl() { return _url; }
        public string GetParameter() { return _parameter; }
    }
}



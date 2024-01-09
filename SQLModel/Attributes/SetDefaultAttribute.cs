using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class SetDefaultAttribute : Attribute
    {
        private object defaultvalue;
        public SetDefaultAttribute(object defaultValue)
        { this.defaultvalue = defaultValue; }
        public object GetDefault() { return defaultvalue; }
    }
}

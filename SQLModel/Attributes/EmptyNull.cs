using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class EmptyNull : Attribute
    {
        public EmptyNull()
        { }
    }
}

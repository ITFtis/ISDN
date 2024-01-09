using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IsSequenceAttribute : Attribute
    {
        //private bool issequence;
        //public IsSequenceAttribute(bool IsSequence)
        //{
        //    this.issequence = IsSequence;
        //}
        public IsSequenceAttribute()
        { }
    }
}

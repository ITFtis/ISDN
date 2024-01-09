using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SetTableNameAttribute : Attribute
    {
        private string tablename;
        public SetTableNameAttribute(string tableName)
        { this.tablename = tableName; }
        public string GetTableName() { return tablename; }
    }
}

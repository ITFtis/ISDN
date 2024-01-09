using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ObjectTool
    {
        public static string GetDifferentString<D, S>(D newobj, S oldobj,string[] skipProperties)
        {
            var dstr = new System.Text.StringBuilder();
            var appentstr = new List<string>();
            var PropertiesOldObj = oldobj.GetType().GetProperties();
            var PropertiesNewObj = newobj.GetType().GetProperties();
            foreach (var p in PropertiesNewObj)
            {
                if (skipProperties.Contains(p.Name)) { continue; }
                var dp = PropertiesOldObj.Where(v => v.Name == p.Name);
                if (dp.Count() > 0)
                {
                    var valueold = dp.First().GetValue(oldobj, null)==null?"": dp.First().GetValue(oldobj, null).ToString();
                    var value = p.GetValue(newobj, null) == null ? "" : p.GetValue(newobj, null).ToString();
                    if (valueold!= value)
                    {
                        if (string.IsNullOrEmpty(valueold)) { valueold = "空白"; }
                        appentstr.Add(p.Name + "從" + valueold + "修改為" + value);
                    }
                }
                else
                {
                    appentstr.Add(p.Name + "設定為空值");
                }
            }
            return string.Join(",", appentstr);
        }

        public static S CreateDiffObj<S>(S newobj, S oldobj, S returnobj, string[] skipProperties)
        {
            var PropertiesOldObj = oldobj.GetType().GetProperties();
            var PropertiesNewObj = newobj.GetType().GetProperties();
            var PropertiesReObj = returnobj.GetType().GetProperties();
            foreach (var p in PropertiesNewObj)
            {
                if (skipProperties.Contains(p.Name)) { continue; }
                var dp = PropertiesOldObj.Where(v => v.Name == p.Name);
                var valueold = dp.First().GetValue(oldobj, null) == null ? "" : dp.First().GetValue(oldobj, null).ToString();
                var value = p.GetValue(newobj, null) == null ? "" : p.GetValue(newobj, null).ToString();
                if (valueold.Equals(value)==false)
                {
                    var rp = PropertiesReObj.Where(v => v.Name == p.Name);
                    rp.First().SetValue(returnobj, p.GetValue(newobj, null));
                }
            }
            return returnobj;
        }
    }
}

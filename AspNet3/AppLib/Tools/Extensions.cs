using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Tools
{
    public static class Extensions
    {
        /// <summary>
        /// Returns public instance properties of and object as comma seperated string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string PropertyNames<T>(T obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string commaSeparatedPropertyNames = props
                //.Where(p => (p.CustomAttributes.Count() == 0 || p.CustomAttributes.Count() > 0 && (p.CustomAttributes.Any(a => a.AttributeType.FullName != "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))))
                .ToList()
                .Aggregate<PropertyInfo, string, string>(
                    string.Empty, // seed value
                    (str, s) => str += s.Name + ",", // returns result using seed value, String.Empty goes to lambda expression as str
                    str => str.Substring(0, str.Length - 1) // result selector that removes last comma
                );

            return commaSeparatedPropertyNames;

            //Dictionary<string, object> dict = new Dictionary<string, object>();
            //foreach (PropertyInfo prp in props)
            //{
            //    object value = prp.GetValue(atype, new object[] { });
            //    dict.Add(prp.Name, value);
            //}
            //return dict;
        }
    }
}

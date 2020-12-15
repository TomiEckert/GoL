using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gol
{
    static class Utils
    {
        public static List<T> Clone<T>(this List<T> list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add(item);
            }

            return result;
        }
    }
}

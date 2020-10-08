using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Extensions.Net
{
    public static partial class EnumExtensions
    {

        public static List<(string, EnumType)> ToList<EnumType>(this Enum i)
        {
            return i.ToTuple<EnumType>().ToList();
        }

        public static IEnumerable<(string, EnumType)> ToTuple<EnumType>(this Enum i)
        {
            return Enum.GetValues(typeof(EnumType)).Cast<EnumType>().Select(s => (s.ToString(), s));
        }

    }
}

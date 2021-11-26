using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ZarlosExtensions.Net.EnumEx
{
    public static partial class EnumExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (string, EnumType)[] ToArray<EnumType>(this Enum i)
        {
            return i.ToList<EnumType>().ToArray();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<(string, EnumType)> ToList<EnumType>(this Enum i)
        {
            return i.ToTuple<EnumType>().ToList();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(string, EnumType)> ToTuple<EnumType>(this Enum i)
        {
            return Enum.GetValues(typeof(EnumType)).Cast<EnumType>().Select(s => (s.ToString(), s));
        }

    }
}

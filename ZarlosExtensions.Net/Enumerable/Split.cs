using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZarlosExtensions.Net.Attributes;

namespace ZarlosExtensions.Net.EnumerableEx
{
    [AutotNullException]
    public static partial class EnumerableExtensions
    {

        public static (IEnumerable<T>, IEnumerable<T>) Split<T>(this IEnumerable<T> array, int index)
        {
            array.Split(index, out var one, out var two);
            return (one, two);
        }

        public static void Split<T>(this IEnumerable<T> array, int index, out IEnumerable<T> item1, out IEnumerable<T> item2)
        {
            item1 = array.Take(index - 1);
            item2 = array.Skip(index);
        }

    }
}

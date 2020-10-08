using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.Net
{
    public static partial class EnumerableExtensions
    {

        public static (IEnumerable<T>, IEnumerable<T>) Split<T>(this IEnumerable<T> array, int index)
        {
            return (array.Take(index - 1), array.Skip(index));
        }

        public static void Split<T>(this IEnumerable<T> array, int index, out IEnumerable<T> item1, out IEnumerable<T> item2)
        {
            item1 = array.Take(index - 1);
            item2 = array.Skip(index);
        }

        public static IEnumerable<T> Split<T>(this IEnumerable<T> array, int index, int count)
        {
            return array.Skip(index).Take(count);
        }


    }
}

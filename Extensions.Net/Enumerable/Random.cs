using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.Net.Enumerable
{
    public static partial class EnumerableExtensions
    {

        public static T Random<T>(this IEnumerable<T> array)
        {
            return array.Random(new Random());
        }

        public static T Random<T>(this IEnumerable<T> array, Random random)
        {
            int max = array.Count() - 1;

            int index = random.Next(0, max);

            return array.ElementAt(index);

        }

    }
}

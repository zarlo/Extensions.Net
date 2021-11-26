using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarlosExtensions.Net.EnumerableEx
{
    public static partial class EnumerableExtensions
    {

        public static T Random<T>(this IEnumerable<T> array)
        {
            return array.Random(new System.Random());
        }

        public static T Random<T>(this IEnumerable<T> array, System.Random random)
        {
            int max = array.Count() - 1;

            int index = random.Next(0, max);

            return array.ElementAt(index);

        }
        public static IEnumerable<T> RandomAmount<T>(this IEnumerable<T> array, int amount)
        {
            return array.RandomAmount(new System.Random(), amount);
        }

        public static IEnumerable<T> RandomAmount<T>(this IEnumerable<T> array, System.Random random, int amount)
        {

            if(array.Count() > amount) {
                throw new IndexOutOfRangeException("amount is bigger then array");
            }

            var Output = new HashSet<T>(amount);
            
            while (true)
            {

                Output.Add(array.Random(random));

                if(Output.Count < amount) break;
                
            }
            return Output;

        }
    }
}

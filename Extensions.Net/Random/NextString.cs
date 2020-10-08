using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.Net
{
    public static partial class RandomExtensions
    {

        public static string NextString(this Random i, string chars)
        {
            return i.NextString(16, chars);
        }

        public static string NextString(this Random i, int length, string chars)
        {
            return System.Linq.Enumerable.Repeat(chars, length).Select(s => s[i.Next(s.Length)]).ToArray().ToUnicode();
        }

        public static string NextString(this Random i)
        {
            return i.NextString(16);
        }

        public static string NextString(this Random i, int length)
        {
            return i.NextString(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        }

    }
}

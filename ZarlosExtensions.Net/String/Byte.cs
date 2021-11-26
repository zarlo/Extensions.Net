using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ZarlosExtensions.Net.StringEx
{
    public static partial class StringExtensions
    {

        public static byte[] ToASCIIBytes(this string s)
        {            
            return Encoding.ASCII.GetBytes(s);
        }

        public static byte[] ToUTF8Bytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] ToUTF32Bytes(this string s)
        {
            return Encoding.UTF32.GetBytes(s);
        }

        public static byte[] ToUTF7Bytes(this string s)
        {
            return Encoding.UTF7.GetBytes(s);
        }

        public static byte[] ToUnicodeBytes(this string s)
        {
            return Encoding.Unicode.GetBytes(s);
        }

    }
}

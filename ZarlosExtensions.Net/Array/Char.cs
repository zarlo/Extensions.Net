using System;
using System.Linq;
using ZarlosExtensions.Net.ArrayEx;
using ZarlosExtensions.Net.StringEx;

namespace ZarlosExtensions.Net.ArrayEx
{
    public static partial class ArrayExtensions
    {

        public static byte[] ToBytes(this char[] arrBytes)
        {            
            return arrBytes.SelectMany(i =>  char.ToString(i).ToUTF8Bytes()).ToArray();
        }



    }
}

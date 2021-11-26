using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ZarlosExtensions.Net.Object
{
    public static partial class ObjectExtensions
    {
        
        public static MemoryStream ToMemoryStream(this object i)
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, i);
            return ms;

        }

        public static byte[] ToArray(this object i)
        {
            return i.ToMemoryStream().ToArray();

        }

    }
}

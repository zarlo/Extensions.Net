using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Extensions.Net
{
    public static partial class ObjectExtensions
    {

        public static byte[] ToArray(this object i)
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, i);
            return ms.ToArray();

        }

    }
}

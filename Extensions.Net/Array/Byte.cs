using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Extensions.Net
{
    public static partial class ArrayExtensions
    {

        public static string ToASCII(this byte[] arrBytes)
        {
            return Encoding.ASCII.GetString(arrBytes);
        }

        public static string ToUTF8(this byte[] arrBytes)
        {
            return Encoding.UTF8.GetString(arrBytes);
        }

        public static string ToUTF32(this byte[] arrBytes)
        {
            return Encoding.UTF32.GetString(arrBytes);
        }

        public static string ToUTF7(this byte[] arrBytes)
        {
            return Encoding.UTF7.GetString(arrBytes);
        }

        public static string ToUnicode(this byte[] arrBytes)
        {
            return Encoding.Unicode.GetString(arrBytes);
        }

        public static object ToObject(this byte[] arrBytes)
        {
            using var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }

        public static T ToObject<T>(this byte[] arrBytes)
        {
            return (T)arrBytes.ToObject();
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public static class Utils
    {
        public static bool IsBitSet(long b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static string DisplayException(Exception e)
        {
            string temp = "";
            temp += e.Message + "\n" + e.StackTrace + "\n";
            if (e.InnerException != null)
                temp += "-----\n" + DisplayException(e.InnerException);
            return temp;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        private static TimeSpan LOCKTIMEOUT = TimeSpan.FromSeconds(10);
        public static void AcquireLock(ref bool lockObject)
        {
            DateTime start = DateTime.Now;
            while (lockObject)
            {
                if (DateTime.Now >= (start + LOCKTIMEOUT))
                {
                    throw new TimeoutException("Timed out acquiring lock");
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            lockObject = true;
        }

        public static void ReleaseLock(ref bool lockObject)
        {
            lockObject = false;
        }
    }
}

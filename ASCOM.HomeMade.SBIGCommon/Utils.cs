/**
 * ASCOM.HomeMade.SBIGCamera - SBIG camera driver
 * Copyright (C) 2021 Cedric Raguenaud [cedric@raguenaud.earth]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static TimeSpan LOCKTIMEOUT = TimeSpan.FromSeconds(20);
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

        public static Array RotateMatrixCounterClockwiseAndConvertToInt(UInt16[,] oldMatrix)
        {
            Array newMatrix = Array.CreateInstance(typeof(int), oldMatrix.GetLength(1), oldMatrix.GetLength(0));
            //int[,] newMatrix = new int[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = 0; oldColumn < oldMatrix.GetLength(1); oldColumn++)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix.SetValue(oldMatrix[oldRow, oldColumn], newRow, newColumn);
                    newColumn++;
                }
                newRow++;
            }
            return newMatrix;
        }

        public static uint BCDToUInt(uint bcd)
        {
            uint result = 0;
            uint multiplier = 1;
            for (int i = 0; i < 8; ++i)
            {
                var nextDigit = (bcd >> (4 * i)) & 0xF;
                result += nextDigit * multiplier;
                multiplier *= 10;
            }
            return result;
        }

        /*
        private static int[][] SaveImageData(int width, int height, int[] data)
        {
            int[][] bimg = new int[height][];

            for (int y = 0; y < height; y++)
            {
                bimg[y] = new int[width];

                for (int x = 0; x < width; x++)
                {
                    bimg[y][x] = (int)Math.Max((int)0, data[x + (height - y - 1) * width]);
                }
            }

            return bimg;
        }

        public static void SaveFitsFrame(int frameNo, string fileName, int width, int height, int[] framePixels, DateTime timeStamp, float exposureSeconds)
        {
            SaveFitsFrame(frameNo, fileName, width, height, framePixels, timeStamp, exposureSeconds, timeStamp.ToLongTimeString(), new Dictionary<string, Tuple<string, string>>());
        }

        public static void SaveFitsFrame(
            int frameNo,
            string fileName,
            int width,
            int height,
            int[] framePixels,
            DateTime timeStamp,
            float exposureSeconds,
            string timeStampComment,
            Dictionary<string, Tuple<string, string>> additionalFileHeaders)
        {
            Fits f = new Fits();

            object data = (object)SaveImageData(width, height, framePixels);

            BasicHDU imageHDU = Fits.MakeHDU(data);

            nom.tam.fits.Header hdr = imageHDU.Header;
            hdr.AddValue("SIMPLE", "T", null);

            hdr.AddValue("BITPIX", 32, null);

            hdr.AddValue("BZERO", 32768, null);
            hdr.AddValue("BSCALE", 1, null);

            hdr.AddValue("NAXIS", 2, null);
            hdr.AddValue("NAXIS1", width, null);
            hdr.AddValue("NAXIS2", height, null);

            hdr.AddValue("EXPOSURE", exposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), "Exposure, seconds");

            hdr.AddValue("DATE-OBS", timeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture), timeStampComment);

            hdr.AddValue("FRAMENO", frameNo, null);

            foreach (var kvp in additionalFileHeaders)
            {
                hdr.AddValue(kvp.Key, kvp.Value.Item1, kvp.Value.Item2);
            }

            hdr.AddValue("END", null, null);

            f.AddHDU(imageHDU);

            // Write a FITS file.
            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                f.Write(bf);
                bf.Flush();
            }
        }

        public static int[] To1DArray(int[,] input)
        {
            // Step 1: get total size of 2D array, and allocate 1D array.
            int size = input.Length;
            int[] result = new int[size];

            // Step 2: copy 2D array elements into a 1D array.
            int write = 0;
            for (int z = input.GetUpperBound(1); z >= 0; z--)
            {
                for (int i = 0; i <= input.GetUpperBound(0); i++)
                {
                    result[write++] = input[i, z];
                }
            }
            // Step 3: return the new array.
            return result;
        }
        */
    }
}

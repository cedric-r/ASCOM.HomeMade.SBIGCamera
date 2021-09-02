using System;
using System.IO;
using CSharpFitsTests.util;
using NUnit.Framework;
using nom.tam.util;

namespace nom.tam.fits
{
    /// <summary>
    /// Test the ImageHDU, ImageData and ImageTiler classes.
    ///   - multiple HDU's in a single file
    ///   - deferred input of HDUs
    ///   - creating and reading arrays of all permitted types.
    ///   - Tiles of 1, 2 and 3 dimensions
    ///       - from a file
    ///       - from internal data
    ///   - Multiple tiles extracted from an image.
    /// </summary>
    [TestFixture]
    public class ImageTest
    {
        byte[][] bimg;
        Array[] simg, iimg, limg, fimg, dimg;
        int[][][] img3;
        double[] img1;

        [Test]
        public void TestImage()
        {
            MakeFile();
            ReadFile();
        }

        // Initialize Objects
        public void Initialize()
        {
            bimg = new byte[40][];
            for (int i = 0; i < bimg.Length; i++)
            {
                bimg[i] = new byte[40];
            }
            for (int i = 10; i < 30; i += 1)
            {
                for (int j = 10; j < 30; j += 1)
                {
                    bimg[i][j] = (byte) (i + j);
                }
            }

            simg = (Array[]) ArrayFuncs.ConvertArray(bimg, typeof (short));
                // Array.ConvertAll<byte, short>(bimg, Convert.ToInt16);
            iimg = (Array[]) ArrayFuncs.ConvertArray(bimg, typeof (int));
                // Array.ConvertAll<byte, int>(bimg, Convert.ToInt32);
            limg = (Array[]) ArrayFuncs.ConvertArray(bimg, typeof (long));
                // Array.ConvertAll<byte, long>(bimg, Convert.ToInt64); ;
            fimg = (Array[]) ArrayFuncs.ConvertArray(bimg, typeof (float));
                // Array.ConvertAll<byte, float>(bimg, Convert.ToSingle); ;
            dimg = (Array[]) ArrayFuncs.ConvertArray(bimg, typeof (double));
                // Array.ConvertAll<byte, double>(bimg, Convert.ToDouble); ;

            img3 = new int[10][][];
            for (int i = 0; i < img3.Length; i++)
            {
                img3[i] = new int[20][];
            }
            for (int i = 0; i < img3.Length; i++)
            {
                for (int j = 0; j < img3[i].Length; j++)
                {
                    img3[i][j] = new int[30];
                }
            }

            for (int i = 0; i < 10; i += 1)
            {
                for (int j = 0; j < 20; j += 1)
                {
                    for (int k = 0; k < 30; k += 1)
                    {
                        img3[i][j][k] = i + j + k;
                    }
                }
            }

            img1 = (double[]) ArrayFuncs.Flatten(dimg);

        }

        // Make FITS file
        public void MakeFile()
        {
            Initialize();

            Fits f = new Fits();

            // Make HDUs of various types.
            f.AddHDU(Fits.MakeHDU(bimg));
            f.AddHDU(Fits.MakeHDU(simg));
            f.AddHDU(Fits.MakeHDU(iimg));
            f.AddHDU(Fits.MakeHDU(limg));
            f.AddHDU(Fits.MakeHDU(fimg));
            f.AddHDU(Fits.MakeHDU(dimg));
            f.AddHDU(Fits.MakeHDU(img3));
            f.AddHDU(Fits.MakeHDU(img1));

            Assert.AreEqual(f.NumberOfHDUs, 8);

            // Write a FITS file.
            BufferedFile bf = new BufferedFile(
                TestFileSetup.GetTargetFilename("image1.fits"),
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
            f.Write(bf);
            bf.Flush();
            bf.Close();
            bf.Dispose();
            f.Close();
        }

        // Read a FITS file
        public void ReadFile()
        {
            Initialize();

            Fits f = null;

            try
            {
                f = new Fits(TestFileSetup.GetTargetFilename("image1.fits"));
                BasicHDU[] hdus = f.Read();

                Assert.AreEqual(f.NumberOfHDUs, 8);
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bimg, hdus[0].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(simg, hdus[1].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(iimg, hdus[2].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(limg, hdus[3].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(fimg, hdus[4].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(dimg, hdus[5].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(img3, hdus[6].Data.Kernel));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(img1, hdus[7].Data.Kernel));
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }
    }
}

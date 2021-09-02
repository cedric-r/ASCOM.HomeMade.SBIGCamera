using System;
using System.IO;
using CSharpFitsTests.util;
using NUnit.Framework;
using nom.tam.image;
using nom.tam.util;


/** This class tests the ImageTiler.  It
 *  first creates a FITS file and then reads
 *  it back and allows the user to select
 *  tiles.  The values of the corner and center
 *  pixels for the selected tile are displayed.
 *  Both file and memory tiles are checked.
 */

namespace nom.tam.fits
{
    [TestFixture]
    public class TilerTest
    {
        void doTile(String test,
            float[][] data,
            ImageTiler t,
            int x, int y, int nx, int ny)
        {
            float[] tile = new float[nx * ny];
            t.GetTile(tile, new int[] { y, x }, new int[] { ny, nx });

            float sum0 = 0;
            float sum1 = 0;

            for (int i = 0; i < nx; i += 1)
            {
                for (int j = 0; j < ny; j += 1)
                {
                    sum0 += tile[i + j * nx];
                    sum1 += data[j + y][i + x];
                }
            }

            Assert.AreEqual(sum0, sum1);
        }

        [Test]
        public void test()
        {
            float[][] data = new float[300][];
            for (int i = 0; i < 300; i++)
                data[i] = new float[300];

            for (int i = 0; i < 300; i += 1)
            {
                for (int j = 0; j < 300; j += 1)
                {
                    data[i][j] = 1000*i + j;
                }
            }

            Fits f = null;

            try
            {
                f = new Fits();

                BufferedFile bf = new BufferedFile(TestFileSetup.GetTargetFilename("tiler1.fits"), FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.AddHDU(Fits.MakeHDU(data));
                f.Write(bf);
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("tiler1.fits"));
                ImageHDU h = (ImageHDU) f.ReadHDU();

                ImageTiler t = h.Tiler;
                doTile("t1", data, t, 200, 200, 50, 50);
                doTile("t2", data, t, 133, 133, 72, 26);

                Object o = h.Data.Kernel;
                doTile("t3", data, t, 200, 200, 50, 50);
                doTile("t4", data, t, 133, 133, 72, 26);
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

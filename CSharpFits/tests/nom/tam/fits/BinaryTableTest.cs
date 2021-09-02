using System;
using nom.tam.util;
using System.IO;
using CSharpFitsTests.util;
using NUnit.Framework;

namespace nom.tam.fits
{
    [TestFixture]
    public class BinaryTableTester
    {
        byte[] bytes = new byte[50];
        byte[][] bits = new byte[50][];
        bool[] bools = new bool[50];
        short[][] shorts = new short[50][];
        int[] ints = new int[50];
        float[][][] floats = new float[50][][];
        double[] doubles = new double[50];
        long[] longs = new long[50];
        String[] strings = new String[50];
        float[][] vf = new float[50][];
        short[][] vs = new short[50][];
        double[][] vd = new double[50][];
        bool[][] vbool = new bool[50][];

        [OneTimeSetUp]
        public void Initialize()
        {
            TestFileSetup.ClearAndCopyToTarget();

            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = new byte[2];
            }

            for (int i = 0; i < shorts.Length; i++)
            {
                shorts[i] = new short[3];
            }

            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = new float[4][];
            }

            for (int i = 0; i < floats.Length; i++)
            {
                for (int j = 0; j < floats[i].Length; j++)
                {
                    floats[i][j] = new float[4];
                }
            }

            for (int i = 0; i < bytes.Length; i += 1)
            {
                bytes[i] = (byte) (2*i);
                bits[i][0] = bytes[i];
                bits[i][1] = (byte) (~bytes[i]);
                bools[i] = (bytes[i]%8) == 0 ? true : false;

                shorts[i][0] = (short) (2*i);
                shorts[i][1] = (short) (3*i);
                shorts[i][2] = (short) (4*i);

                ints[i] = i*i;
                for (int j = 0; j < 4; j += 1)
                {
                    for (int k = 0; k < 4; k += 1)
                    {
                        floats[i][j][k] = (float) (i + j*Math.Exp(k));
                    }
                }
                doubles[i] = 3*Math.Sin(i);
                longs[i] = i*i*i*i;
                strings[i] = "abcdefghijklmnopqrstuvwxzy".Substring(0, i%20);

                vf[i] = new float[i + 1];
                vf[i][i/2] = i*3;
                vs[i] = new short[i/10 + 1];
                vs[i][i/10] = (short) -i;
                vd[i] = new double[i%2 == 0 ? 1 : 2];
                vd[i][0] = 99.99;
                vbool[i] = new bool[i/10];
                if (i >= 10)
                {
                    vbool[i][0] = i%2 == 1;
                }
            }
        }

        [Test]
        public void TestSimpleIO()
        {
            Fits f = null;

            try
            {
                FitsFactory.UseAsciiTables = false;

                f = new Fits();
                Object[] data = new Object[]
                {
                    bytes, bits, bools, shorts, ints,
                    floats, doubles, longs, strings
                };
                f.AddHDU(Fits.MakeHDU(data));

                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);
                bhdu.SetColumnName(0, "bytes", null);
                bhdu.SetColumnName(1, "bits", "bits later on");
                bhdu.SetColumnName(6, "doubles", null);
                bhdu.SetColumnName(5, "floats", "4 x 4 array");

                BufferedFile bf =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt1.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bf);
                bf.Flush();
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt1.fits"));
                f.Read();

                Assert.AreEqual(2, f.NumberOfHDUs);

                BinaryTableHDU thdu = (BinaryTableHDU) f.GetHDU(1);
                Header hdr = thdu.Header;

                Assert.AreEqual(9, hdr.GetIntValue("TFIELDS"));
                Assert.AreEqual(2, hdr.GetIntValue("NAXIS"));
                Assert.AreEqual(8, hdr.GetIntValue("BITPIX"));
                Assert.AreEqual("BINTABLE", hdr.GetStringValue("XTENSION"));
                Assert.AreEqual("bytes", hdr.GetStringValue("TTYPE1"));
                Assert.AreEqual("doubles", hdr.GetStringValue("TTYPE7"));

                for (int i = 0; i < data.Length; i += 1)
                {
                    Object col = thdu.GetColumn(i);
                    if (i == 8)
                    {
                        String[] st = (String[]) col;

                        for (int j = 0; j < st.Length; j += 1)
                        {
                            st[j] = st[j].Trim();
                        }
                    }
                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(data[i], col));
                }
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void TestRowDelete()
        {
            Fits f = null;

            try
            {
                f = new Fits(
                    TestFileSetup.GetTargetFilename(
                        TestFileSetup.GetTargetFilename("bt1.fits")));
                f.Read();

                BinaryTableHDU thdu = (BinaryTableHDU) f.GetHDU(1);

                Assert.AreEqual(50, thdu.NRows);
                thdu.DeleteRows(10, 20);
                Assert.AreEqual(30, thdu.NRows);

                double[] dbl = (double[]) thdu.GetColumn(6);
                Assert.AreEqual(dbl[9], doubles[9]);
                Assert.AreEqual(dbl[10], doubles[30]);

                BufferedFile bf =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt1x.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);

                f.Write(bf);
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt1x.fits"));
                f.Read();
                thdu = (BinaryTableHDU) f.GetHDU(1);
                dbl = (double[]) thdu.GetColumn(6);
                Assert.AreEqual(30, thdu.NRows);
                Assert.AreEqual(9, thdu.NCols);
                Assert.AreEqual(dbl[9], doubles[9]);
                Assert.AreEqual(dbl[10], doubles[30]);

                thdu.DeleteRows(20);
                Assert.AreEqual(20, thdu.NRows);
                dbl = (double[]) thdu.GetColumn(6);
                Assert.AreEqual(20, dbl.Length);
                Assert.AreEqual(dbl[0], doubles[0]);
                Assert.AreEqual(dbl[19], doubles[39]);
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void TestVar()
        {
            Fits f = null;

            try
            {
                Object[] data = new Object[] {floats, vf, vs, vd, shorts, vbool};
                f = new Fits();
                f.AddHDU(Fits.MakeHDU(data));

                BufferedFile bdos =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt2.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);

                f.Write(bdos);
                bdos.Close();
                bdos.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt2.fits"), FileAccess.Read);
                f.Read();
                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);
                Header hdr = bhdu.Header;

                Assert.AreEqual(true, hdr.GetIntValue("PCOUNT") > 0);
                Assert.AreEqual(6, hdr.GetIntValue("TFIELDS"));

                for (int i = 0; i < data.Length; i += 1)
                {
                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(data[i], bhdu.GetColumn(i)));
                }
            }
            finally
            {
                f.Close();
            }

        }

        [Test]
        public void TestSet()
        {
            Fits f = null;
            try
            {
                f = new Fits(TestFileSetup.GetTargetFilename("bt2.fits"), FileAccess.Read);
                f.Read();

                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);
                Header hdr = bhdu.Header;

                // Check the various set methods on variable length data.
                float[] dta = (float[]) bhdu.GetElement(4, 1);
                dta = new float[] {22, 21, 20};
                bhdu.SetElement(4, 1, dta);

                BufferedFile bdos =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt2a.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bdos);
                bdos.Close();
                bdos.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt2a.fits"));
                bhdu = (BinaryTableHDU) f.GetHDU(1);
                float[] xdta = (float[]) bhdu.GetElement(4, 1);

                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(dta, xdta));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(3, 1), vf[3]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(5, 1), vf[5]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), dta));

                float[] tvf = new float[] {101, 102, 103, 104};
                vf[4] = tvf;

                bhdu.SetColumn(1, vf);
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(3, 1), vf[3]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), vf[4]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(5, 1), vf[5]));

                bdos = new BufferedFile(
                    TestFileSetup.GetTargetFilename("bt2b.fits"),
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.Write(bdos);
                bdos.Close();
                bdos.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt2b.fits"));
                bhdu = (BinaryTableHDU) f.GetHDU(1);
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(3, 1), vf[3]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), vf[4]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(5, 1), vf[5]));

                Object[] rw = (Object[]) bhdu.GetRow(4);

                float[] trw = new float[] {-1, -2, -3, -4, -5, -6};
                rw[1] = trw;

                bhdu.SetRow(4, rw);
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(3, 1), vf[3]));
                Assert.AreEqual(false, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), vf[4]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), trw));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(5, 1), vf[5]));

                // bdos = new BufferedDataStream(new FileStream("bt2c.fits",FileMode.Open));
                bdos = new BufferedFile(
                    TestFileSetup.GetTargetFilename("bt2c.fits"),
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.Write(bdos);
                bdos.Close();
                bdos.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt2c.fits"));
                bhdu = (BinaryTableHDU) f.GetHDU(1);
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(3, 1), vf[3]));
                Assert.AreEqual(false, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), vf[4]));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(4, 1), trw));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(bhdu.GetElement(5, 1), vf[5]));
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void BuildByColumn()
        {
            Fits f = null;

            try
            {
                BinaryTable btab = new BinaryTable();

                btab.AddColumn(floats);
                btab.AddColumn(vf);
                btab.AddColumn(strings);
                btab.AddColumn(vbool);
                btab.AddColumn(ints);

                f = new Fits();
                f.AddHDU(Fits.MakeHDU(btab));

                BufferedFile bdos =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt3.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bdos);
                bdos.Close();
                bdos.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt3.fits"));
                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);
                btab = (BinaryTable) bhdu.Data;

                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(floats, bhdu.GetColumn(0)));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(vf, bhdu.GetColumn(1))); // problem is here only

                String[] col = (String[]) bhdu.GetColumn(2);
                for (int i = 0; i < col.Length; i += 1)
                {
                    col[i] = col[i].Trim();
                }

                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(strings, col));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(vbool, bhdu.GetColumn(3)));
                Assert.AreEqual(true, ArrayFuncs.ArrayEquals(ints, bhdu.GetColumn(4)));
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void BuildByRow()
        {
            Fits f = null;

            try
            {
                f = new Fits(
                    TestFileSetup.GetTargetFilename("bt2.fits"), 
                    FileAccess.Read);
                f.Read();

                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);
                Header hdr = bhdu.Header;

                BinaryTable btab = (BinaryTable) bhdu.Data;
                for (int i = 0; i < 50; i += 1)
                {
                    Object[] row = (Object[]) btab.GetRow(i);
                    float[] qx = (float[]) row[1];
                    Array[] p = (Array[]) row[0];
                    float[] pt = (float[]) p.GetValue(0);
                    pt[0] = (float) (i * Math.Sin(i));
                    btab.AddRow(row);
                }
                f.Close();

                f = new Fits();
                f.AddHDU(Fits.MakeHDU(btab));

                BufferedFile bf =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt4.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bf);

                bf.Flush();
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(
                    TestFileSetup.GetTargetFilename("bt4.fits"),
                    FileAccess.Read);

                btab = (BinaryTable) f.GetHDU(1).Data;
                Assert.AreEqual(100, btab.NRows);

                // Try getting data before we Read in the table.
                Array[] xf = (Array[]) btab.GetColumn(0);
                Array[] xft = (Array[]) xf.GetValue(50);
                float[] xftt = (float[]) xft.GetValue(0);

                Assert.AreEqual((float) 0, (float) xftt[0]);

                xft = (Array[]) xf.GetValue(99);
                xftt = (float[]) xft.GetValue(0);
                Assert.AreEqual((float) (49 * Math.Sin(49)), (float) xftt[0]);

                for (int i = 0; i < xf.Length; i += 3)
                {
                    bool[] ba = (bool[]) btab.GetElement(i, 5);
                    float[] fx = (float[]) btab.GetElement(i, 1);

                    int trow = i % 50;

                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(ba, vbool[trow])); // prob 1
                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(fx, vf[trow]));
                }

                // Fill the table.
                Data data = f.GetHDU(1).Data;

                xf = (Array[]) btab.GetColumn(0);
                xft = (Array[]) xf.GetValue(50);
                xftt = (float[]) xft.GetValue(0);
                Assert.AreEqual(0F, (float) xftt[0]);
                xft = (Array[]) xf.GetValue(99);
                xftt = (float[]) xft.GetValue(0);
                Assert.AreEqual((float) (49 * Math.Sin(49)), (float) xftt[0]);

                for (int i = 0; i < xf.Length; i += 3)
                {
                    bool[] ba = (bool[]) btab.GetElement(i, 5);
                    float[] fx = (float[]) btab.GetElement(i, 1);

                    int trow = i % 50;

                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(ba, vbool[trow])); // prob 2
                    Assert.AreEqual(true, ArrayFuncs.ArrayEquals(fx, vf[trow]));
                }
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void TestObj()
        {
            FitsFactory.UseAsciiTables = false;

            /*** Create a binary table from an Object[][] array */
            Object[][] x = new Object[5][];
            for (int i = 0; i < 5; i += 1)
            {
                x[i] = new Object[3];

                x[i][0] = new float[] {i};

                string temp = string.Concat("AString", i);
                x[i][1] = new string[] {temp};

                int[][] t = new int[2][];
                for (int j = 0; j < 2; j++)
                {
                    t[j] = new int[2];
                    t[j][0] = j * i;
                    t[j][1] = (j + 2) * i;
                }
                x[i][2] = t;
            }

            Fits f = null;

            try
            {
                f = new Fits();
                BasicHDU hdu = Fits.MakeHDU(x);
                f.AddHDU(hdu);

                BufferedFile bf =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt5.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bf);
                bf.Close();
                bf.Dispose();

                /* Now get rid of some columns */
                BinaryTableHDU xhdu = (BinaryTableHDU) hdu;

                // First column
                Assert.AreEqual(3, xhdu.NCols);
                xhdu.DeleteColumnsIndexOne(1, 1);
                Assert.AreEqual(2, xhdu.NCols);

                xhdu.DeleteColumnsIndexZero(1, 1);
                Assert.AreEqual(1, xhdu.NCols);

                bf = new BufferedFile(
                    TestFileSetup.GetTargetFilename("bt6.fits"),
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.Write(bf);
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt6.fits"));
                xhdu = (BinaryTableHDU) f.GetHDU(1);
                Assert.AreEqual(1, xhdu.NCols);
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void TestDegenerate()
        {
            Fits f = null;

            try
            {
                String[] sa = new String[10];
                int[,] ia = new int[10, 0];

                f = new Fits();

                for (int i = 0; i < sa.Length; i += 1)
                {
                    sa[i] = "";
                }

                Object[] data = new Object[] {sa, ia};
                BinaryTableHDU bhdu = (BinaryTableHDU) Fits.MakeHDU(data);
                Header hdr = bhdu.Header;
                f.AddHDU(bhdu);

                BufferedFile bf =
                    new BufferedFile(
                        TestFileSetup.GetTargetFilename("bt7.fits"),
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                f.Write(bf);
                bf.Close();
                bf.Dispose();

                Assert.AreEqual(2, hdr.GetIntValue("TFIELDS"));
                Assert.AreEqual(10, hdr.GetIntValue("NAXIS2"));
                Assert.AreEqual(0, hdr.GetIntValue("NAXIS1"));

                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt7.fits"));
                bhdu = (BinaryTableHDU) f.GetHDU(1);
                hdr = bhdu.Header;
                Assert.AreEqual(2, hdr.GetIntValue("TFIELDS"));
                Assert.AreEqual(10, hdr.GetIntValue("NAXIS2"));
                Assert.AreEqual(0, hdr.GetIntValue("NAXIS1"));
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }

        [Test]
        public void TestDegen2()
        {
            FitsFactory.UseAsciiTables = false;

            Object[] data = new Object[]
            {
                new String[] {"a", "b", "c", "d", "e", "f"},
                new int[] {1, 2, 3, 4, 5, 6},
                new float[] {1f, 2f, 3f, 4f, 5f, 6f},
                new String[] {"", "", "", "", "", ""},
                new String[] {"a", "", "c", "", "e", "f"},
                new String[] {"", "b", "c", "d", "e", "f"},
                new String[] {"a", "b", "c", "d", "e", ""},
                new String[] {null, null, null, null, null, null},
                new String[] {"a", null, "c", null, "e", "f"},
                new String[] {null, "b", "c", "d", "e", "f"},
                new String[] {"a", "b", "c", "d", "e", null}
            };

            Fits f = null;

            try
            {
                f = new Fits();
                f.AddHDU(Fits.MakeHDU(data));
                BufferedFile ff = new BufferedFile(TestFileSetup.GetTargetFilename("bt8.fits"), FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.Write(ff);
                ff.Flush();
                ff.Close();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("bt8.fits"));
                BinaryTableHDU bhdu = (BinaryTableHDU) f.GetHDU(1);

                Assert.AreEqual("e", bhdu.GetElement(4, data.Length - 1));
                Assert.AreEqual("", bhdu.GetElement(5, data.Length - 1));

                String[] col = (String[]) bhdu.GetColumn(0);
                Assert.AreEqual("a", col[0]);
                Assert.AreEqual("f", col[5]);

                col = (String[]) bhdu.GetColumn(3);
                Assert.AreEqual("", col[0]);
                Assert.AreEqual("", col[5]);

                col = (String[]) bhdu.GetColumn(7); // All nulls
                Assert.AreEqual("", col[0]);
                Assert.AreEqual("", col[5]);

                col = (String[]) bhdu.GetColumn(8);

                Assert.AreEqual("a", col[0]);
                Assert.AreEqual("", col[1]);
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

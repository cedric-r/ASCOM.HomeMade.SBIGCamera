using System.IO;
using CSharpFitsTests.util;
using NUnit.Framework;
using System;
using nom.tam.util;

namespace nom.tam.fits
{
    [TestFixture]
    public class FitsReaderTester
    {
        [Test]
        public void TestFits()
        {
            string filename = TestFileSetup.GetTargetFilename("test.fits");

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                Fits f = new Fits(fs, false);

                Console.Out.WriteLine("FitsReader called.");

                int i = 0;
                BasicHDU h;

                do
                {
                    h = f.ReadHDU();
                    if (h != null)
                    {
                        if (i == 0)
                        {
                            Console.Out.WriteLine("\n\nPrimary header:\n");
                        }
                        else
                        {
                            Console.Out.WriteLine("\n\nExtension " + i + ":\n");
                        }
                        i += 1;
                        h.Info();
                    }
                } while (h != null);
            }
        }

        [Test]
        public void TestReadBuffered()
        {
            string filename = TestFileSetup.GetTargetFilename("test.fits");
            BufferedFile bf =
                new BufferedFile(
                    filename,
                    FileAccess.Read,
                    FileShare.None);

            Header h = Header.ReadHeader(bf);

            long n = h.DataSize;
            int naxes = h.GetIntValue("NAXIS");
            int lastAxis = h.GetIntValue("NAXIS" + naxes);
            
            HeaderCard hnew = new HeaderCard("NAXIS", naxes - 1, "this is header card with naxes");
            h.AddCard(hnew);
            
            float[] line = new float[h.DataSize];
            
            for (int i = 0; i < lastAxis; i += 1)
            {
                Console.Out.WriteLine("read");
                bf.Read(line);
            }

            bf.Close();
            bf.Dispose();
        }
    }
}

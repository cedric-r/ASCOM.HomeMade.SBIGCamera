using System;
using System.IO;
using CSharpFitsTests.util;
using nom.tam.util;
using NUnit.Framework;

namespace nom.tam.fits
{
    [TestFixture]
    public class FitsCopyTester
    {
        [Test]
        public void TestFitsCopy()
        {
            String file = TestFileSetup.GetTargetFilename("test_dup.fits");

            Fits f = new Fits(file);
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

            BufferedFile bf = new BufferedFile(
                TestFileSetup.GetTargetFilename("gbfits3.fits"),
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
            f.Write(bf);
            bf.Close();
            bf.Dispose();
            f.Close();
        }
    }
}
using System;
using System.Collections;
using System.IO;
using CSharpFitsTests.util;
using NUnit.Framework;
using nom.tam.util;

namespace nom.tam.fits
{
    /// <summary> summary description for HeaderTest.</summary>
    [TestFixture]
    public class HeaderTest
    {
        [SetUp]
        public void Setup()
        {
            TestFileSetup.ClearAndCopyToTarget();
        }

        /// <summary> Check out header manipulation.</summary>
        [Test]
        public void TestSimpleImages()
        {
            float[][] img = new float[300][];
            for (int i = 0; i < 300; i++)
                img[i] = new float[300];

            Fits f = null;

            try
            {
                f = new Fits();

                ImageHDU hdu = (ImageHDU) Fits.MakeHDU(img);
                BufferedFile bf = new BufferedFile(
                    TestFileSetup.GetTargetFilename("ht1.fits"),
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                f.AddHDU(hdu);
                f.Write(bf);
                bf.Close();
                bf.Dispose();
                f.Close();

                f = new Fits(TestFileSetup.GetTargetFilename("ht1.fits"));
                hdu = (ImageHDU) f.GetHDU(0);
                Header hdr = hdu.Header;

                Assert.AreEqual(2, hdr.GetIntValue("NAXIS"));
                Assert.AreEqual(300, hdr.GetIntValue("NAXIS1"));
                Assert.AreEqual(300, hdr.GetIntValue("NAXIS2"));
                Assert.AreEqual(300, hdr.GetIntValue("NAXIS2", -1));
                Assert.AreEqual(-1, hdr.GetIntValue("NAXIS3", -1));

                Assert.AreEqual(-32, hdr.GetIntValue("BITPIX"));

                Cursor c = hdr.GetCursor();
                c.MoveNext();
                HeaderCard hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("SIMPLE", hc.Key);

                c.MoveNext();
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("BITPIX", hc.Key);

                c.MoveNext();
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("NAXIS", hc.Key);

                c.MoveNext();
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("NAXIS1", hc.Key);

                c.MoveNext();
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("NAXIS2", hc.Key);
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
        public void TestCursor()
        {
            Fits f = null;

            try
            {
                f = new Fits(TestFileSetup.GetTargetFilename("ht1.fits"));
                ImageHDU hdu = (ImageHDU) f.GetHDU(0);
                Header hdr = hdu.Header;
                Cursor c = hdr.GetCursor();

                c.Key = "XXX";
                c.Add("CTYPE1", new HeaderCard("CTYPE1", "GLON-CAR", "Galactic Longitude"));
                c.Add("CTYPE2", new HeaderCard("CTYPE2", "GLAT-CAR", "Galactic Latitude"));

                c.Key = "CTYPE1"; // Move before CTYPE1
                c.Add("CRVAL1", new HeaderCard("CRVAL1", 0f, "Longitude at reference"));

                c.Key = "CTYPE2"; // Move before CTYPE2
                c.Add("CRVAL2", new HeaderCard("CRVAL2", -90f, "Latitude at reference"));

                c.Key = "CTYPE1"; // Just practicing moving around!!
                c.Add("CRPIX1", new HeaderCard("CRPIX1", 150.0, "Reference Pixel X"));

                c.Key = "CTYPE2";
                c.Add("CRPIX2", new HeaderCard("CRPIX2", 0f, "Reference pixel Y"));
                c.Add("INV2", new HeaderCard("INV2", true, "Invertible axis"));
                c.Add("SYM2", new HeaderCard("SYM2", "YZ SYMMETRIC", "Symmetries..."));

                Assert.AreEqual("GLON-CAR", hdr.GetStringValue("CTYPE1"));
                Assert.AreEqual(0f, hdr.GetDoubleValue("CRPIX2", -2f));
                
                c.Key = "CRVAL1";
                HeaderCard hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CRVAL1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CRPIX1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CTYPE1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CRVAL2", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CRPIX2", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("INV2", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("SYM2", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CTYPE2", hc.Key);
                
                hdr.FindCard("CRPIX1");
                hdr.AddValue("INTVAL1", 1, "An integer value");
                hdr.AddValue("LOG1", true, "A true value");
                hdr.AddValue("LOGB1", false, "A false value");
                hdr.AddValue("FLT1", 1.34, "A float value");
                hdr.AddValue("FLT2", -1.234567890e-134, "A very long float");
                hdr.AddValue("COMMENT", null, "Comment after flt2");
                
                c.Key = "INTVAL1";

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("INTVAL1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("LOG1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("LOGB1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("FLT1", hc.Key);

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("FLT2", hc.Key);

                c.MoveNext(); // Skip comment
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                c.MoveNext();
                Assert.AreEqual("CRPIX1", hc.Key);

                Assert.AreEqual(1.34, hdr.GetDoubleValue("FLT1", 0));

                c.Key = "FLT1";
                c.Remove();
                Assert.AreEqual(0f, hdr.GetDoubleValue("FLT1", 0));

                c.Key = "LOGB1";
                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("LOGB1", hc.Key);
                c.MoveNext();

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("FLT2", hc.Key);
                c.MoveNext();

                hc = (HeaderCard) ((DictionaryEntry) c.Current).Value;
                Assert.AreEqual("Comment after flt2", hc.Comment);
                c.MoveNext();
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
        public void TestBadHeader()
        {
            Fits f = null;

            try
            {
                f = new Fits(TestFileSetup.GetTargetFilename("ht1.fits"));
                ImageHDU hdu = (ImageHDU) f.GetHDU(0);
                Header hdr = hdu.Header;
                Cursor c = hdr.GetCursor();

                c = hdr.GetCursor();
                c.MoveNext();
                c.MoveNext();
                c.Remove();
                bool thrown = false;
                try
                {
                    hdr.Rewrite();
                }
                catch (Exception e)
                {
                    thrown = true;
                }

                Assert.AreEqual(true, thrown);
            }
            finally
            {
                f.Close();
            }
        }

        [Test]
        public void TestRewrite()
        {
            Fits f = null;

            try
            {
                // Should be rewriteable until we add enough cards to
                // start a new block.
                f = new Fits(TestFileSetup.GetTargetFilename("ht1.fits"));
                ImageHDU hdu = (ImageHDU) f.GetHDU(0);
                Header hdr = hdu.Header;
                Cursor c = hdr.GetCursor();
                c.MoveNext();

                int nc = hdr.NumberOfCards;
                int nb = (nc - 1) / 36;

                while (hdr.Rewriteable)
                {
                    int nbx = (hdr.NumberOfCards - 1) / 36;
                    Assert.AreEqual(nb == nbx, hdr.Rewriteable);
                    c.Add(new HeaderCard("DUMMY" + nbx, null, null));
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
    }
}

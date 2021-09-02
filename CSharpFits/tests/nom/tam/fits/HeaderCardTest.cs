using System;
using NUnit.Framework;

namespace nom.tam.fits
{
    [TestFixture]
    public class HeaderCardTest
    {
        [Test]
        public void Test1()
        {

            HeaderCard p;
            p = new HeaderCard("SIMPLE  =                     T");

            Assert.AreEqual("SIMPLE", p.Key);
            Assert.AreEqual("T", p.Value);
            Assert.IsNull(p.Comment);

            p = new HeaderCard("VALUE   =                   123");
            Assert.AreEqual("VALUE", p.Key);
            Assert.AreEqual("123", p.Value);
            Assert.IsNull(p.Comment);

            p = new HeaderCard("VALUE   =    1.23698789798798E23 / Comment ");
            Assert.AreEqual("VALUE", p.Key);
            Assert.AreEqual("1.23698789798798E23", p.Value);
            Assert.AreEqual("Comment", p.Comment);

            String lng = "111111111111111111111111111111111111111111111111111111111111111111111111";
            p = new HeaderCard("COMMENT " + lng);
            Assert.AreEqual("COMMENT", p.Key);
            Assert.IsNull(p.Value);
            Assert.AreEqual(lng, p.Comment);

            bool thrown = false;
            try
            {
                //
                p = new HeaderCard("VALUE   = '   ");
            }
            catch (Exception e)
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);

            p = new HeaderCard("COMMENT " + lng + lng);
            Assert.AreEqual(lng, p.Comment);
        }

        [Test]
        public void Test3()
        {

            HeaderCard p = new HeaderCard("KEY", "VALUE", "COMMENT");
            Assert.AreEqual(
                "KEY     = 'VALUE   '           / COMMENT                                        ",
                    p.ToString());

            p = new HeaderCard("KEY", 123, "COMMENT");
            Assert.AreEqual(
                "KEY     =                  123 / COMMENT                                        ",
                    p.ToString());

            p = new HeaderCard("KEY", 1.23, "COMMENT");
            Assert.AreEqual(
                "KEY     =                 1.23 / COMMENT                                        ",
                    p.ToString());

            p = new HeaderCard("KEY", true, "COMMENT");
            Assert.AreEqual(
                "KEY     =                    T / COMMENT                                        ",
                    p.ToString());

            bool thrown = false;
            try
            {
                p = new HeaderCard("LONGKEYWORD", 123, "COMMENT");
            }
            catch (Exception e)
            {
                thrown = true;
            }
            Assert.AreEqual(true, thrown);

            thrown = false;
            String lng = "00000000001111111111222222222233333333334444444444555555555566666666667777777777";
            try
            {
                p = new HeaderCard("KEY", lng, "COMMENT");
            }
            catch (Exception e)
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);
        }

        [Test]
        public void TestHierarch()
        {

            HeaderCard hc;
            String key = "HIERARCH.TEST1.TEST2.INT";
            bool thrown = false;

            try
            {
                hc = new HeaderCard(key, 123, "Comment");
            }
            catch (Exception e)
            {
                thrown = true;
            }

            Assert.AreEqual(true, thrown);

            String card = "HIERARCH TEST1 TEST2 INT=           123 / Comment                               ";
            hc = new HeaderCard(card);
            Assert.AreEqual("HIERARCH", hc.Key);
            Assert.IsNull(hc.Value);
            Assert.AreEqual("TEST1 TEST2 INT=           123 / Comment", hc.Comment);

            FitsFactory.UseHierarch = true;

            hc = new HeaderCard(key, 123, "Comment");
            Assert.AreEqual(key, hc.Key);
            Assert.AreEqual("123", hc.Value);
            Assert.AreEqual("Comment", hc.Comment);

            hc = new HeaderCard(card);
            Assert.AreEqual(key, hc.Key);
            Assert.AreEqual("123", hc.Value);
            Assert.AreEqual("Comment", hc.Comment);
        }

        [Test]
        public void TestLongDoubles()
        {
            // Check to see if we make long double values fit in the recommended space.
            HeaderCard hc = new HeaderCard("TEST", -1.234567890123456789E-123, "dummy");
            String val = hc.Value;

            Assert.AreEqual(val.Length, 20);
        }
    }
}

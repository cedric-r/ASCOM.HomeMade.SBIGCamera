using System;
using NUnit.Framework;

namespace nom.tam.fits
{
    [TestFixture]
    public class TestDate
    {
        [Test]
        public void DateTest()
        {
            Assert.AreEqual(true, TestArg("20/09/79"));
            Assert.AreEqual(true, TestArg("1997-07-25"));
            Assert.AreEqual(true, TestArg("1987-06-05T04:03:02.01"));
            Assert.AreEqual(true, TestArg("1998-03-10T16:58:34"));
            Assert.AreEqual(true, TestArg(null));
            Assert.AreEqual(true, TestArg("        "));

            Assert.AreEqual(false, TestArg("20/09/"));
            Assert.AreEqual(false, TestArg("/09/79"));
            Assert.AreEqual(false, TestArg("09//79"));
            Assert.AreEqual(false, TestArg("20/09/79/"));

            Assert.AreEqual(false, TestArg("1997-07"));
            Assert.AreEqual(false, TestArg("-07-25"));
            Assert.AreEqual(false, TestArg("1997--07-25"));
            Assert.AreEqual(false, TestArg("1997-07-25-"));

            Assert.AreEqual(false, TestArg("5-Aug-1992"));
            Assert.AreEqual(false, TestArg("28/02/91 16:32:00"));
            Assert.AreEqual(false, TestArg("18-Feb-1993"));
            Assert.AreEqual(false, TestArg("nn/nn/nn"));
        }

        bool TestArg(String arg)
        {
            try
            {
                FitsDate fd = new FitsDate(arg);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}

namespace nom.tam.util
{
    using System;
    using nom.tam.util;
    using NUnit.Framework;


    [TestFixture]
    public class ByteFormatParseTester
    {

        static byte[] buffer = new byte[100000];
        ByteFormatter bf = new ByteFormatter();
        ByteParser bp = new ByteParser(buffer);
        int offset = 0;
        int cnt = 0;

        [Test]
        public void testInt()
        {

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            bf.Align = true;
            bf.TruncationThrow = false;

            int[] tint = new int[100];

            tint[0] = int.MinValue;
            tint[1] = int.MaxValue;
            tint[2] = 0;
            Random random = new Random();

            for (int i = 0; i < tint.Length; i += 1)
            {
                tint[i] = (int) (int.MaxValue*(2*(random.NextDouble() - 0.5)));
            }

            cnt = 0;
            offset = 0;
            // Write 100 numbers

            int colSize = 12;
            while (cnt < tint.Length)
            {

                offset = bf.format(tint[cnt], buffer, offset, colSize);
                cnt += 1;
                if (cnt%8 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }

            // Now see if we can get them back
            bp.Offset = 0;
            bp = new ByteParser(buffer);
            for (int i = 0; i < tint.Length; i += 1)
            {
                Console.WriteLine("i= " + i);
                int chk = bp.GetInt(colSize);

                Assert.AreEqual(chk, tint[i]);
                if ((i + 1)%8 == 0)
                {
                    bp.Skip(1);
                }
            }

            // Now do it with left-aligned numbers.
            bf.Align = false;
            bp.FillFields = true;
            offset = 0;
            colSize = 12;
            cnt = 0;
            offset = 0;
            while (cnt < tint.Length)
            {
                int oldOffset = offset;
                offset = bf.format(tint[cnt], buffer, offset, colSize);
                int nb = colSize - (offset - oldOffset);
                if (nb > 0)
                {
                    offset = bf.alignFill(buffer, offset, nb);
                }
                cnt += 1;
                if (cnt%8 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }

            // Now see if we can get them back
            bp.Offset = 0;
            for (int i = 0; i < tint.Length; i += 1)
            {

                int chk = bp.GetInt(colSize);

                Assert.AreEqual(chk, tint[i]);
                if ((i + 1)%8 == 0)
                {
                    bp.Skip(1);
                }
            }

            offset = 0;
            colSize = 12;
            cnt = 0;
            offset = 0;
            while (cnt < tint.Length)
            {
                offset = bf.format(tint[cnt], buffer, offset, colSize);
                cnt += 1;
                if (cnt%8 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }
            String myStr = null;
            sbyte[] sbytes = new sbyte[100000];
            Buffer.BlockCopy(buffer, 0, sbytes, 0, buffer.Length);
            unsafe
            {
                // Instruct the Garbage Collector not to move the memory
                fixed (sbyte* buffBytes = sbytes)
                {
                    myStr = new String(buffBytes, 0, offset);

                }

            }
            Assert.AreEqual(-1, myStr.IndexOf(" "));
            bf.Align = false;

            offset = 0;
            colSize = 12;
            cnt = 0;
            offset = 0;
            while (cnt < tint.Length)
            {
                offset = bf.format(tint[cnt], buffer, offset, colSize);
                offset = bf.format(" ", buffer, offset, 1);
                cnt += 1;
            }
            String myStr2 = null;
            sbyte[] sbytes2 = new sbyte[100000];
            Buffer.BlockCopy(buffer, 0, sbytes2, 0, buffer.Length);
            unsafe
            {
                // Instruct the Garbage Collector not to move the memory
                fixed (sbyte* buffBytes2 = sbytes2)
                {
                    myStr2 = new String(buffBytes2, 0, offset);

                }

            }
            String[] array = myStr2.Split(' ');

            Assert.AreEqual(100, array.Length - 1);

            for (int i = 0; i < array.Length - 1; i += 1)
            {
                Assert.AreEqual(tint[i], int.Parse(array[i]));
            }


            bf.TruncationThrow = false;

            int val = 1;
            //Arrays.fill(buffer, (byte)' ');

// array is used, values set, etc.
//Array.Clear(buffer, ' ', buffer.Length-1);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte) ' ';
            for (int i = 0; i < 10; i += 1)
            {
                offset = bf.format(val, buffer, 0, 6);
                String test = (val + "      ").Substring(0, 6);
                sbyte[] sbytes1 = new sbyte[100000];
                Buffer.BlockCopy(buffer, 0, sbytes1, 0, buffer.Length);
                unsafe
                {
                    // Instruct the Garbage Collector not to move the memory
                    fixed (sbyte* buffBytes1 = sbytes1)
                    {

                        if (i < 6)
                        {
                            Assert.AreEqual(test, new String(buffBytes1, 0, 6));
                        }
                        else
                        {
                            Assert.AreEqual("******", new String(buffBytes1, 0, 6));
                        }
                        val *= 10;
                    }
                }
            }

            bf.TruncationThrow = true;
            val = 1;
            for (int i = 0; i < 10; i += 1)
            {
                bool thrown = false;
                try
                {
                    offset = bf.format(val, buffer, 0, 6);
                }
                catch (TruncationException e)
                {
                    thrown = true;
                }
                if (i < 6)
                {
                    Assert.AreEqual(false, thrown);
                }
                else
                {
                    Assert.AreEqual(true, thrown);
                }
                val *= 10;
            }
        }

        [Test]
        public void testLong()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            long[] lng = new long[100];
            Random random = new Random();
            for (int i = 0; i < lng.Length; i += 1)
            {
                lng[i] = (long) (long.MaxValue*(2*(random.Next() - 0.5)));
            }


            lng[0] = long.MaxValue;
            lng[1] = long.MinValue;
            lng[2] = 0;

            bf.TruncationThrow = false;
            bp.FillFields = true;
            bf.Align = true;
            offset = 0;
            for (int i = 0; i < lng.Length; i += 1)
            {
                offset = bf.format(lng[i], buffer, offset, 20);
                if ((i + 1)%4 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }

            bp.Offset = 0;

            for (int i = 0; i < lng.Length; i += 1)
            {
                Assert.AreEqual(lng[i], bp.GetLong(20));
                if ((i + 1)%4 == 0)
                {
                    bp.Skip(1);
                }
            }
        }

        [Test]
        public void testFloat()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            Random random = new Random();
            float[] flt = new float[100];
            for (int i = 6; i < flt.Length; i += 1)
            {
                flt[i] = (float) (2*(random.NextDouble() - 0.5)*Math.Pow(10, 60*(random.NextDouble() - 0.5)));
            }

            flt[0] = float.MaxValue;
            flt[1] = float.MinValue;
            flt[2] = 0;
            flt[3] = float.NaN;
            flt[4] = float.PositiveInfinity;
            flt[5] = float.NegativeInfinity;

            bf.TruncationThrow = false;
            bf.Align = true;

            offset = 0;
            cnt = 0;

            while (cnt < flt.Length)
            {
                offset = bf.format(flt[cnt], buffer, offset, 24);
                cnt += 1;
                if (cnt%4 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }

            bp.Offset = 0;

            for (int i = 0; i < flt.Length; i += 1)
            {

                float chk = bp.GetFloat(24);

                float dx = Math.Abs(chk - flt[i]);
                if (flt[i] != 0)
                {
                    dx = dx/Math.Abs(flt[i]);
                }
                if (float.IsNaN(flt[i]))
                {
                    Assert.AreEqual(true, float.IsNaN(chk));
                }
                else if (float.IsInfinity(flt[i]))
                {
                    Assert.AreEqual(flt[i], chk);
                }
                else
                {
                    Assert.AreEqual(0F, dx, 1E-6);
                }
                if ((i + 1)%4 == 0)
                {
                    bp.Skip(1);
                }
            }
        }

        [Test]
        public void testDouble()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            Random random = new Random();
            double[] dbl = new double[100];
            for (int i = 6; i < dbl.Length; i += 1)
            {
                dbl[i] = 2*(random.Next() - 0.5)*Math.Pow(10, 60*(random.Next() - 0.5));
            }

            dbl[0] = Double.MaxValue;
            dbl[1] = Double.MaxValue;
            dbl[2] = 0;
            dbl[3] = Double.NaN;
            dbl[4] = Double.PositiveInfinity;
            dbl[5] = Double.NegativeInfinity;

            bf.TruncationThrow = false;
            bf.Align = true;
            offset = 0;
            cnt = 0;
            while (cnt < dbl.Length)
            {
                offset = bf.format(dbl[cnt], buffer, offset, 25);
                cnt += 1;
                if (cnt%4 == 0)
                {
                    offset = bf.format("\n", buffer, offset, 1);
                }
            }

            bp.Offset = 0;
            for (int i = 0; i < dbl.Length; i += 1)
            {
                double chk = bp.GetDouble(25);

                double dx = Math.Abs(chk - dbl[i]);
                if (dbl[i] != 0)
                {
                    dx = dx/Math.Abs(dbl[i]);
                }
                if (Double.IsNaN(dbl[i]))
                {
                    Assert.AreEqual(true, Double.IsNaN(chk));
                }
                else if (Double.IsInfinity(dbl[i]))
                {
                    Assert.AreEqual(dbl[i], chk);
                }
                else
                {
                    Assert.AreEqual(0F, dx, 1e-14);
                }

                if ((i + 1)%4 == 0)
                {
                    bp.Skip(1);
                }
            }
        }

        // [Test]
        public void testBoolean()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            Random random = new Random();
            bool[] btst = new bool[100];
            for (int i = 0; i < btst.Length; i += 1)
            {
                //   bool val = random.Next > 0.5;
                // btst[i] =val;
            }
            offset = 0;
            bf.Align = false;
            bf.TruncateOnOverflow = true;
            for (int i = 0; i < btst.Length; i += 1)
            {
                offset = bf.format(btst[i], buffer, offset, 1);
                offset = bf.format(" ", buffer, offset, 1);
            }

            bp.Offset = 0;
            for (int i = 0; i < btst.Length; i += 1)
            {
                Assert.AreEqual(btst[i], bp.GetBoolean(btst.Length));
            }
        }

        [Test]
        public void testString()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            offset = 0;
            String bigStr = "abcdefghijklmnopqrstuvwxyz";
            bf.Align = false;
            bf.TruncateOnOverflow = true;
            for (int i = 0; i < 100; i += 1)
            {
                offset = bf.format(bigStr.Substring(i%27), buffer, offset, 13);
                offset = bf.format(" ", buffer, offset, 1);
            }

            bp.Offset = 0;
            for (int i = 0; i < 100; i += 1)
            {
                int ind = i%27;
                if (ind > 13)
                {
                    ind = 13;
                }
                String want = bigStr.Substring(i%27);
                if (want.Length > 13)
                {
                    want = want.Substring(0, 13);
                }
                String s = bp.GetString(want.Length);
                Assert.AreEqual(want, s);
                bp.Skip(1);
            }
        }
    }
}
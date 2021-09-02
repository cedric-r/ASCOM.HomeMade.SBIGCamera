using System;
using NUnit.Framework;

namespace nom.tam.util
{
    [TestFixture]
    public class ArrayFuncsTester
    {
        /// <summary>Test and demonstrate the ArrayFuncs methods.</summary>
        [Test]
        public void TestArrayFuncs()
        {
            // Check GetBaseClass(), GetBaseLength() and ComputeSize() methods
            int[,,] test1 = new int[10, 9, 8];
            bool[][] test2 = new bool[4][];
            test2[0] = new bool[5];
            test2[1] = new bool[4];
            test2[2] = new bool[3];
            test2[3] = new bool[2];

            double[,] test3 = new double[10, 20];
            System.Text.StringBuilder[,] test4 = new System.Text.StringBuilder[3, 2];

            Assert.AreEqual(typeof (int), ArrayFuncs.GetBaseClass(test1));
            Assert.AreEqual(4, ArrayFuncs.GetBaseLength(test1));
            Assert.AreEqual(4 * 8 * 9 * 10, ArrayFuncs.ComputeSize(test1));

            Assert.AreEqual(typeof (bool), ArrayFuncs.GetBaseClass(test2));
            Assert.AreEqual(1, ArrayFuncs.GetBaseLength(test2));
            Assert.AreEqual(1 * (2 + 3 + 4 + 5), ArrayFuncs.ComputeSize(test2));

            Assert.AreEqual(typeof (double), ArrayFuncs.GetBaseClass(test3));
            Assert.AreEqual(8, ArrayFuncs.GetBaseLength(test3));
            Assert.AreEqual(8 * 10 * 20, ArrayFuncs.ComputeSize(test3));

            Assert.AreEqual(typeof (System.Text.StringBuilder), ArrayFuncs.GetBaseClass(test4));
            Assert.AreEqual(-1, ArrayFuncs.GetBaseLength(test4));
            Assert.AreEqual(0, ArrayFuncs.ComputeSize(test4));


            Object[] agg = new Object[4];
            agg[0] = test1;
            agg[1] = test2;
            agg[2] = test3;
            agg[3] = test4;

            Assert.AreEqual(typeof (Object), ArrayFuncs.GetBaseClass(agg));
            Assert.AreEqual(-1, ArrayFuncs.GetBaseLength(agg));


            // Add up all the primitive arrays and ignore the objects.
            Assert.AreEqual(2880 + 14 + 1600 + 0, ArrayFuncs.ComputeSize(agg));

            for (int i = 0; i < ((Array) test1).GetLength(0); i += 1)
            {
                for (int j = 0; j < ((Array) test1).GetLength(1); j += 1)
                {
                    for (int k = 0; k < ((Array) test1).GetLength(2); k += 1)
                    {
                        test1[i, j, k] = i + j + k;
                    }
                }
            }

            /*
            // Check DeepClone() method: Does not work for multi-dimension Array.
            int[,,] test5 = (int[,,]) ArrayFuncs.DeepClone(test1);
        	
	        Assert.AreEqual("deepClone()", true, ArrayFuncs.ArrayEquals(test1, test5));
	        test5[1,1,1] = -3;
	        Assert.AreEqual("arrayEquals()", false, ArrayFuncs.ArrayEquals(test1, test5));
            */

            // Check Flatten() method
            int[] dimsOrig = ArrayFuncs.GetDimensions(test1);
            int[] test6 = (int[]) ArrayFuncs.Flatten(test1);

            int[] dims = ArrayFuncs.GetDimensions(test6);

            Assert.AreEqual(3, dimsOrig.Length);
            Assert.AreEqual(10, dimsOrig[0]);
            Assert.AreEqual(9, dimsOrig[1]);
            Assert.AreEqual(8, dimsOrig[2]);
            Assert.AreEqual(1, dims.Length);


            // Check Curl method
            int[] newdims = {8, 9, 10};
            Array[] test7 = (Array[]) ArrayFuncs.Curl(test6, newdims);
            int[] dimsAfter = ArrayFuncs.GetDimensions(test7);

            Assert.AreEqual(3, dimsAfter.Length);
            Assert.AreEqual(8, dimsAfter[0]);
            Assert.AreEqual(9, dimsAfter[1]);
            Assert.AreEqual(10, dimsAfter[2]);


            /*
            // Check Convert Array method: Implemented in Java Package
            byte[,,] xtest1 = (byte[,,]) ArrayFuncs.convertArray(test1, typeof(byte));
        	
            Assert.AreEqual("convertArray(toByte)", typeof(byte), ArrayFuncs.GetBaseClass(xtest1));
            Assert.AreEqual("convertArray(tobyte)", test1[3,3,3], (int)xtest1[3,3,3]);

            double[,,] xtest2 = (double[,,]) ArrayFuncs.convertArray(test1, typeof(double));
            Assert.AreEqual("convertArray(toByte)", typeof(double), ArrayFuncs.GetBaseClass(xtest2));
            Assert.AreEqual("convertArray(tobyte)", test1[3,3,3], (int)xtest2[3,3,3]);
            */

            // Check NewInstance method
            int[] xtest3 = (int[]) ArrayFuncs.NewInstance(typeof (int), 20);
            int[] xtd = ArrayFuncs.GetDimensions(xtest3);
            Assert.AreEqual(1, xtd.Length);
            Assert.AreEqual(20, xtd[0]);
            Array[] xtest4 = (Array[]) ArrayFuncs.NewInstance(typeof (double), new int[] {5, 4, 3, 2});

            xtd = ArrayFuncs.GetDimensions(xtest4);
            Assert.AreEqual(4, xtd.Length);
            Assert.AreEqual(5, xtd[0]);
            Assert.AreEqual(4, xtd[1]);
            Assert.AreEqual(3, xtd[2]);
            Assert.AreEqual(2, xtd[3]);
            Assert.AreEqual(120, ArrayFuncs.CountElements(xtest4));

            /*
            // Check TestPattern method: Implemented in Java package, not in C#.
            ArrayFuncs.TestPattern(xtest4, (byte)-1);
        	
            Assert.AreEqual("testPattern()", (double) -1,  xtest4[0,0,0,0]);
            Assert.AreEqual("testPattern()", (double) 118, xtest4[4,3,2,1]);
            double[] xtest4x = (double[])ArrayFuncs.GetBaseArray(xtest4);
        	
            Assert.AreEqual("getBaseArray()", 2, xtest4x.Length);
            */


            // Check ArrayEquals method
            double[] x = {1, 2, 3, 4, 5};
            double[] y = new double[x.Length];
            for (int i = 0; i < y.Length; i += 1)
            {
                y[i] = x[i] + 1E-10;
            }

            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(x, y));
            Assert.AreEqual(true, ArrayFuncs.ArrayEquals(x, y, 0d, 1e-9));
            Assert.AreEqual(true, ArrayFuncs.ArrayEquals(x, y, 1E-5, 1e-9));
            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(x, y, 0d, 1e-11));
            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(x, y, 1E-5, 0d));

            float[] fx = {1, 2, 3, 4, 5};
            float[] fy = new float[fx.Length];
            for (int i = 0; i < fy.Length; i += 1)
            {
                fy[i] = fx[i] + 1E-5F;
            }

            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(fx, fy));
            Assert.AreEqual(true, ArrayFuncs.ArrayEquals(fx, fy, 1E-4, 0d));
            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(fx, fy, 1E-6, 0d));
            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(fx, fy, 0d, 0d));
            Assert.AreEqual(false, ArrayFuncs.ArrayEquals(fx, fy, 0d, 1E-4));
        }
    }
}
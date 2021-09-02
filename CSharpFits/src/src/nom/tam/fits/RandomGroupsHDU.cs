namespace nom.tam.fits
{
  /*
   * Copyright: Thomas McGlynn 1997-2007.
   * Many thanks to David Glowacki (U. Wisconsin) for substantial
   * improvements, enhancements and bug fixes.
   * The CSharpFITS package is a C# port of Tom McGlynn's
   * nom.tam.fits Java package, initially ported by  Samuel Carliles
   *
   * Copyright: 2007 Virtual Observatory - India.   
   *
   * Use is subject to license terms
   */
    using System;
    using System.Collections;
	using nom.tam.util;
	
    /// <summary>Random groups HDUs.  Note that the internal storage of random
	/// groups is a Object[ngroup][2] array.  The first element of
	/// each group is the parameter data from that group.  The second element
	/// is the data.  The parameters should be a one dimensional array
	/// of the primitive types byte, short, int, long, float or double.
	/// The second element is a n-dimensional array of the same type.
	/// When analyzing group data structure only the first group is examined,
	/// but for a valid FITS file all groups must have the same structure.
    /// </summary>
	public class RandomGroupsHDU:BasicHDU
	{
        /// <summary>Indicate that a RandomGroupsHDU can come at the beginning of a FITS file.</summary>
        internal override bool CanBePrimary
        {
          get
          {
            return true;
          }
        }
    		
        /// <summary>Move a RandomGroupsHDU to or from the beginning
		/// of a FITS file.  Note that the FITS standard only
		/// supports Random Groups data at the beginning
		/// of the file, but we allow it within Image extensions.
		/// </summary>
		override internal bool PrimaryHDU
		{
			set
			{
				try
				{
					base.PrimaryHDU = value;
				}
				catch(FitsException)
				{
					Console.Error.WriteLine("Unreachable catch in RandomGroupsHDU");
				}
				if (value)
				{
					myHeader.Simple = true;
				}
				else
				{
					myHeader.Xtension = "IMAGE";
				}
			}
		}
		/// <summary>Check that this HDU has a valid header.</summary>
		/// <returns> <CODE>true</CODE> if this HDU has a valid header.</returns>
		virtual public bool HasHeader
		{
			get
			{
				return IsHeader(myHeader);
			}
		}

        //internal Object dataArray;

        /// <summary>Create an HDU from the given header and data</summary>
		public RandomGroupsHDU(Header h, Data d)
		{
			myHeader = h;
			myData = d;
		}
		
		/// <summary>Make a header point to the given object.</summary>
		/// <param name="odata">The random groups data the header should describe.</param>
		internal static Header ManufactureHeader(Data d)
		{
			if (d == null)
			{
				throw new FitsException("Attempt to create null Random Groups data");
			}
			Header h = new Header();
			d.FillHeader(h);
			return h;
		}
		
		/// <summary>Is this a random groups header?</summary>
		/// <param name="myHeader">The header to be tested.</param>
		public static new bool IsHeader(Header hdr)
		{
			if (hdr.GetBooleanValue("SIMPLE"))
			{
				return hdr.GetBooleanValue("GROUPS");
			}
			
			String s = hdr.GetStringValue("XTENSION");
			if (s.Trim().Equals("IMAGE"))
			{
				return hdr.GetBooleanValue("GROUPS");
			}
			
			return false;
		}

        /// <summary>Check if this data is compatible with Random Groups structure.
		/// Must be an Object[ngr][2] structure with both elements of each
		/// group having the same base type and the first element being
		/// a simple primitive array.  We do not check anything but
		/// the first row.</summary>
		public static bool IsData(Object oo)
		{
            if (oo is Object[][]) //ArrayFuncs.CountDimensions(oo) == 2)
			{
				//Object[][] o = (Object[][]) oo;

				//if(o.Length > 0)
                if(ArrayFuncs.IsArrayOfArrays(oo))
                {
                  try
                  {
                    Array a = (Array)oo;
                    Type t1 = ArrayFuncs.GetBaseClass(((Array)a.GetValue(0)).GetValue(0));
                    Type t2 = ArrayFuncs.GetBaseClass(((Array)a.GetValue(0)).GetValue(1));

                    return ((Array)a.GetValue(0)).Length == 2 &&
                      t1.Equals(t2) && !t1.Equals(typeof(char)) && !t1.Equals(typeof(bool));
                  }
                  catch(Exception)
                  {
                    return false;
                  }
                }
                else
                {
                  try
                  {
                    Array a = (Array)oo;
                    Type t1 = a.GetValue(new int[]{0, 0}).GetType();
                    Type t2 = a.GetValue(new int[]{0, 1}).GetType();

                    return a.GetLength(1) == 2 &&
                      t1.Equals(t2) && !t1.Equals(typeof(char)) && !t1.Equals(typeof(bool));
                  }
                  catch(Exception)
                  {
                    return false;
                  }
                }
                /*
		        {
			        if(o[0].Length == 2)
			        {
				        if (ArrayFuncs.getBaseClass(o[0][0]) == ArrayFuncs.getBaseClass(o[0][1]))
				        {
					        System.String cn = o[0][0].GetType().FullName;
					        if (cn.Length == 2 && cn[1] != 'Z' || cn[1] != 'C')
					        {
						        return true;
					        }
				        }
			        }
		        }
                */
			}

            return false;
		}
		
		/// <summary>Create a FITS Data object corresponding to this HDU header.</summary>
		internal override Data ManufactureData()
		{
			return ManufactureData(myHeader);
		}
		
		/// <summary>Create FITS data object corresponding to a given header.</summary>
		public static Data ManufactureData(Header hdr)
		{
			int gcount = hdr.GetIntValue("GCOUNT", - 1);
			int pcount = hdr.GetIntValue("PCOUNT", - 1);
			
			if (!hdr.GetBooleanValue("GROUPS") || hdr.GetIntValue("NAXIS1", - 1) != 0 || gcount < 0 || pcount < 0 || hdr.GetIntValue("NAXIS") < 2)
			{
				throw new FitsException("Invalid Random Groups Parameters");
			}
			
			// Allocate the object.
			Object[][] dataArray;
			
			if (gcount > 0)
			{
				dataArray = new Object[gcount][];
				for (int i = 0; i < gcount; i++)
				{
					dataArray[i] = new Object[2];
				}
			}
			else
			{
				dataArray = new Object[0][];
			}
			
			Object[] sampleRow = GenerateSampleRow(hdr);
			for (int i = 0; i < gcount; i += 1)
			{
				((Object[][]) dataArray)[i][0] = ((Object[]) ArrayFuncs.DeepClone(sampleRow))[0];
				((Object[][]) dataArray)[i][1] = ((Object[]) ArrayFuncs.DeepClone(sampleRow))[1];
			}
			return new RandomGroupsData(dataArray);
		}
		
		internal static Object[] GenerateSampleRow(Header h)
		{
			int ndim = h.GetIntValue("NAXIS", 0) - 1;
			int[] dims = new int[ndim];
			
			int bitpix = h.GetIntValue("BITPIX", 0);
			
			Type baseClass;
			
			switch (bitpix)
			{
				case 8:
					baseClass = typeof(byte);
                      break;
				case 16:
					baseClass = typeof(short);
                      break;
				case 32:
					baseClass = typeof(int);
                      break;
				case 64:
					baseClass = typeof(long);
                      break;
				case - 32:
					baseClass = typeof(float);
                      break;
				case - 64:
					baseClass = typeof(double);
                      break;
				default: 
					throw new FitsException("Invalid BITPIX:" + bitpix);
			}
			
			// Note that we have to invert the order of the axes
			// for the FITS file to get the order in the array we
			// are generating.  Also recall that NAXIS1=0, so that
			// we have an 'extra' dimension.
			for (int i = 0; i < ndim; i += 1)
			{
				long cdim = h.GetIntValue("NAXIS" + (i + 2), 0);
				if (cdim < 0)
				{
					throw new FitsException("Invalid array dimension:" + cdim);
				}
				dims[ndim - i - 1] = (int) cdim;
			}
			
			Object[] sample = new Object[2];
			sample[0] = ArrayFuncs.NewInstance(baseClass, h.GetIntValue("PCOUNT"));
			sample[1] = ArrayFuncs.NewInstance(baseClass, dims);
			
			return sample;
		}
		
		public static Data Encapsulate(Object o)
		{
            if (o is Object[][])// ArrayFuncs.CountDimensions(o) == 2)
			{
				//return new RandomGroupsData((System.Object[][]) o);
                return new RandomGroupsData((Array)o);
			}
			else
			{
				throw new FitsException("Attempt to encapsulate invalid data in Random Group");
			}
		}
		
		
		/// <summary>Display structural information about the current HDU.</summary>
		public override void Info()
		{
			Console.Out.WriteLine("Random Groups HDU");
			if (myHeader != null)
			{
				Console.Out.WriteLine("   HeaderInformation:");
				Console.Out.WriteLine("     Ngroups:" + myHeader.GetIntValue("GCOUNT"));
				Console.Out.WriteLine("     Npar:   " + myHeader.GetIntValue("PCOUNT"));
				Console.Out.WriteLine("     BITPIX: " + myHeader.GetIntValue("BITPIX"));
				Console.Out.WriteLine("     NAXIS:  " + myHeader.GetIntValue("NAXIS"));
				for (int i = 0; i < myHeader.GetIntValue("NAXIS"); i += 1)
				{
					Console.Out.WriteLine("      NAXIS" + (i + 1) + "= " + myHeader.GetIntValue("NAXIS" + (i + 1)));
				}
			}
			else
			{
				Console.Out.WriteLine("    No Header Information");
			}

//      Object[][] data = null;
        Array data = null;
			if (myData != null)
			{
				try
				{
					//data = (Object[][]) myData.DataArray;
                    data = (Array)myData.DataArray;
				}
				catch(FitsException)
				{
					data = null;
				}
			}
			
			//if(data == null || data.Length < 1 || data[0].Length != 2)
            if(data == null || data.Length < 1 || data.GetLength(1) != 2)
			{
				Console.Out.WriteLine("    Invalid/unreadable data");
			}
			else
			{
				Console.Out.WriteLine("    Number of groups:" + data.Length);
                Console.Out.WriteLine("    Parameters: " + ArrayFuncs.ArrayDescription(data.GetValue(new int[]{0, 0})));
                Console.Out.WriteLine("    Data:" + ArrayFuncs.ArrayDescription(data.GetValue(new int[]{0, 1})));
			}
		}
	}
}

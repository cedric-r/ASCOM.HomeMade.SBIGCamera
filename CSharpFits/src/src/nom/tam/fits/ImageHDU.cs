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
	using nom.tam.util;
	using nom.tam.image;
    /// <summary>FITS image header/data unit.The ImageTiler class allows
    /// users to extract subimages from a FITS primary image or image extension.</summary>
	public class ImageHDU:BasicHDU
	{
        /// <summary>Indicate that Images can appear at the beginning of a FITS dataset</summary>
        internal override bool CanBePrimary
        {
          get
          {
            return true;
          }
        }
    		
        /// <summary>Change the Image from/to primary</summary>
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
					Console.Error.WriteLine("Impossible exception in ImageData");
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
        /// <summary>
        /// returns ImageTiler instance
        /// </summary>
		virtual public ImageTiler Tiler
		{
			get
			{
				return ((ImageData) myData).Tiler;
			}
		}
		
		/// <summary>Build an image HDU using the supplied data.</summary>
		/// <param name="obj">the data used to build the image.</param>
        /// <exception cref="FitsException">If there was a problem with the data.</exception>
		public ImageHDU(Header h, Data d)
		{
			myData = d;
			myHeader = h;
		}
		
		/// <summary>Check that this HDU has a valid header for this type.</summary>
		/// <returns> <CODE>true</CODE> if this HDU has a valid header.</returns>
		public static new bool IsHeader(Header hdr)
		{
			bool found = false;
			found = hdr.GetBooleanValue("SIMPLE");
			if (!found)
			{
				String s = hdr.GetStringValue("XTENSION");
				if (s != null)
				{
					if (s.Trim().Equals("IMAGE") || s.Trim().Equals("IUEIMAGE"))
					{
						found = true;
					}
				}
			}
			if (!found)
			{
				return false;
			}
			return !hdr.GetBooleanValue("GROUPS");
		}
		
		/// <summary>Check if this object can be described as a FITS image.</summary>
		/// <param name="o">The Object being tested.</param>
		public static bool IsData(Object o)
		{
            // Removed multidimension Array check logic: "(typeof(Array).Equals(o.GetType()) || o.GetType().FullName.IndexOf("[")!=-1)"

            // return (o != null) && o.GetType().IsArray && (!ArrayFuncs.GetBaseClass(o).Equals(typeof(bool)));

           return (o!=null) && o.GetType().IsArray && (ArrayFuncs.GetBaseClass(o).IsPrimitive && !(ArrayFuncs.GetBaseClass(o).Equals(typeof(bool))));
        }

        /// <summary>Create a Data object to correspond to the header description.</summary>
		/// <returns> An unfilled Data object which can be used to read in the data for this HDU.</returns>
		/// <exception cref=""> FitsException if the image extension could not be created.</exception>
		internal override Data ManufactureData()
		{
			return ManufactureData(myHeader);
		}
		
		public static Data ManufactureData(Header hdr)
		{
			return new ImageData(hdr);
		}
		
		/// <summary>Create a  header that describes the given image data.</summary>
		/// <param name="o">The image to be described.</param>
		/// <exception cref=""> FitsException if the object does not contain valid image data.</exception>
		public static Header ManufactureHeader(Data d)
		{
			if (d == null)
			{
				return null;
			}
			
			Header h = new Header();
			d.FillHeader(h);
			
			return h;
		}
		
		/// <summary>Encapsulate an object as an ImageHDU.</summary>
		public static Data Encapsulate(Object o)
		{
			return new ImageData(o);
		}

        /// <summary>Print out some information about this HDU.</summary>
		public override void Info()
		{
			if(IsHeader(myHeader))
			{
				Console.Out.WriteLine("  Image");
			}
			else
			{
				Console.Out.WriteLine("  Image (bad header)");
			}
			
			Console.Out.WriteLine("      Header Information:");
			Console.Out.WriteLine("         BITPIX=" + myHeader.GetIntValue("BITPIX", - 1));
			int naxis = myHeader.GetIntValue("NAXIS", - 1);
			Console.Out.WriteLine("         NAXIS=" + naxis);
			for (int i = 1; i <= naxis; i += 1)
			{
				Console.Out.WriteLine("         NAXIS" + i + "=" + myHeader.GetIntValue("NAXIS" + i, - 1));
			}
			
			Console.Out.WriteLine("      Data information:");
			try
			{
				if(myData.DataArray == null)
				{
					Console.Out.WriteLine("        No Data");
				}
				else
				{
					Console.Out.WriteLine("         " + ArrayFuncs.ArrayDescription(myData.DataArray));
				}
			}
			catch(Exception)
			{
				System.Console.Out.WriteLine("      Unable to get data");
			}
		}
	}
}

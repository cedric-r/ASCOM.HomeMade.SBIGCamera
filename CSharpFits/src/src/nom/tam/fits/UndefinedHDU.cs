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
	using ArrayFuncs = nom.tam.util.ArrayFuncs;
    /// <summary>Holder for unknown data types. 
    /// </summary>
	public class UndefinedHDU:BasicHDU
	{
		
		/// <summary>Build an image HDU using the supplied data.</summary>
		/// <param name="obj">the data used to build the image.</param>
		/// <exception cref=""> FitsException if there was a problem with the data.</exception>
		public UndefinedHDU(Header h, Data d)
		{
			myData = d;
			myHeader = h;
		}
		
        /// <summary>Check if we can find the length of the data for this header.</summary>
        /// <returns><CODE>true</CODE> if this HDU has a valid header.</returns>
		public static new bool IsHeader(Header hdr)
		{
			if(hdr.GetStringValue("XTENSION") != null && hdr.GetIntValue("NAXIS", - 1) >= 0)
			{
				return true;
			}
			return false;
		}
		
		/// <summary>Check if we can use the following object as
		/// in an Undefined FITS block.  We allow this
		/// so long as computeSize can get a size.  Note
		/// that computeSize may be wrong!
		/// </summary>
		/// <param name="o">The Object being tested.</param>
		public static bool IsData(System.Object o)
		{
			if (ArrayFuncs.ComputeSize(o) > 0)
			{
				return true;
			}
			return false;
		}
		
		
		/// <summary>Create a Data object to correspond to the header description.</summary>
		/// <returns> An unfilled Data object which can be used to read
		/// in the data for this HDU.
		/// </returns>
		/// <exception cref=""> FitsException if the image extension could not be created.</exception>
		internal override Data ManufactureData()
		{
			return ManufactureData(myHeader);
		}
		/// <summary>
        /// Create a UndefinedData object to correspond to the header description.
		/// </summary>
		/// <param name="hdr"></param>
		/// <returns></returns>
		public static Data ManufactureData(Header hdr)
		{
			return new UndefinedData(hdr);
		}
		
		/// <summary>Create a  header that describes the given
		/// image data.
		/// </summary>
		/// <param name="o">The image to be described.
		/// </param>
		/// <exception cref=""> FitsException if the object does not contain
		/// valid image data.
		/// 
		/// </exception>
		public static Header ManufactureHeader(Data d)
		{
			Header h = new Header();
			d.FillHeader(h);
			
			return h;
		}
		
		/// <summary>Encapsulate an object as an ImageHDU.</summary>
		public static Data Encapsulate(System.Object o)
		{
			return new UndefinedData(o);
		}
		
		
		
		/// <summary>Print out some information about this HDU.</summary>
		public override void Info()
		{
			
			System.Console.Out.WriteLine("  Unhandled/Undefined/Unknown Type");
			System.Console.Out.WriteLine("  XTENSION=" + myHeader.GetStringValue("XTENSION").Trim());
			System.Console.Out.WriteLine("  Apparent size:" + myData.TrueSize);
		}
	}
}
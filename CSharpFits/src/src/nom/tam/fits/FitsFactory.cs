namespace nom.tam.fits
{
    /*
  * Copyright: Thomas McGlynn 1997-2007.
  * 
  * The CSharpFITS package is a C# port of Tom McGlynn's
  * nom.tam.fits Java package, initially ported by  Samuel Carliles
  *
  * Copyright: 2007 Virtual Observatory - India.   
  *
  * Use is subject to license terms
  */
    using System;

    /// <summary>This class contains the code which
	/// associates particular FITS types with header
	/// and data configurations.  It comprises
	/// a set of Factory methods which call
	/// appropriate methods in the HDU classes.
	/// If -- God forbid -- a new FITS HDU type were
	/// created, then the XXHDU, XXData classes would
	/// need to be added and this file modified but
	/// no other changes should be needed in the FITS libraries.
	/// </summary>
	
	public class FitsFactory
	{
		/// <summary>Indicate whether ASCII tables should be used where feasible.</summary>
		public static bool UseAsciiTables
		{
            // should be internal
            get
            {
                return useAsciiTables;
            }

			set
			{
				useAsciiTables = value;
			}			
		}
	
		/// <summary>Are we processing HIERARCH style keywords</summary>
		/// <summary>Enable/Disable hierarchical keyword processing.</summary>
		public static bool UseHierarch
		{
			get
			{
				return useHierarch;
			}
			
			set
			{
				useHierarch = value;
			}
			
		}
		
		private static bool useAsciiTables = true;
		private static bool useHierarch = false;

        // change suggested in .99.5 version: Method made public from protected.
        /// <summary>Given a Header return an appropriate datum.</summary>
		public static Data DataFactory(Header hdr)
		{
			if (ImageHDU.IsHeader(hdr))
			{
				return ImageHDU.ManufactureData(hdr);
			}
			else if (RandomGroupsHDU.IsHeader(hdr))
			{
				return RandomGroupsHDU.ManufactureData(hdr);
			}
			else if (useAsciiTables && AsciiTableHDU.IsHeader(hdr))
			{
				return AsciiTableHDU.ManufactureData(hdr);
			}
			else if (BinaryTableHDU.IsHeader(hdr))
			{
				return BinaryTableHDU.ManufactureData(hdr);
			}
			else if (UndefinedHDU.IsHeader(hdr))
			{
				return UndefinedHDU.ManufactureData(hdr);
			}
			else
			{
				throw new FitsException("Unrecognizable header in dataFactory");
			}
		}

        // change suggested in .99.5 version: Method made public from protected.
        /// <summary>Given an object, create the appropriate FITS header to describe it.</summary>
		/// <param name="o">The object to be described.</param>
		public static BasicHDU HDUFactory(Object o)
		{
			Data d;
			Header h;

			if(ImageHDU.IsData(o))
			{
				d = ImageHDU.Encapsulate(o);
				h = ImageHDU.ManufactureHeader(d);
			}
			else if (RandomGroupsHDU.IsData(o))
			{
				d = RandomGroupsHDU.Encapsulate(o);
				h = RandomGroupsHDU.ManufactureHeader(d);
			}
			else if (useAsciiTables && AsciiTableHDU.IsData(o))
			{
				d = AsciiTableHDU.Encapsulate(o);
				h = AsciiTableHDU.ManufactureHeader(d);
			}
			else if (BinaryTableHDU.IsData(o))
			{
				d = BinaryTableHDU.Encapsulate(o);
				h = BinaryTableHDU.ManufactureHeader(d);
			}
			else if (UndefinedHDU.IsData(o))
			{
				d = UndefinedHDU.Encapsulate(o);
				h = UndefinedHDU.ManufactureHeader(d);
			}
			else
			{
                Console.WriteLine();
				throw new FitsException("Invalid data presented to HDUFactory");
			}
			
			return HDUFactory(h, d);
		}

        // change suggested in .99.5 version: Method made public from protected.
        /// <summary>Given Header and data objects return the appropriate type of HDU.</summary>
		public static BasicHDU HDUFactory(Header hdr, Data d)
		{
			if (d is ImageData)
			{
				return new ImageHDU(hdr, d);
			}
			else if (d is RandomGroupsData)
			{
				return new RandomGroupsHDU(hdr, d);
			}
			else if (d is AsciiTable)
			{
				return new AsciiTableHDU(hdr, d);
			}
			else if (d is BinaryTable)
			{
				return new BinaryTableHDU(hdr, d);
			}
			else if (d is UndefinedData)
			{
				return new UndefinedHDU(hdr, d);
			}
			
			return null;
		}
	}
}

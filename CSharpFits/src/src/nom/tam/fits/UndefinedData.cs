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
    using System.IO;
	using nom.tam.util;

	/// <summary>This class provides a simple holder for data which is
	/// not handled by other classes.
	/// </summary>
	public class UndefinedData:Data
	{
    #region Properties
		/// <summary>Get the size in bytes of the data</summary>
		override internal int TrueSize
		{
			get
			{
				return (int) byteSize;
			}
			
		}
		/// <summary>Return the actual data.
		/// Note that this may return a null when
		/// the data is not readable.  It might be better
		/// to throw a FitsException, but this is
		/// a very commonly called method and we prefered
		/// not to change how users must invoke it.
		/// </summary>
		override public Object DataArray
		{
			get
			{
				if(data == null)
				{
					try
					{
						//FitsUtil.Reposition(input, fileOffset);
            input.Seek(fileOffset, SeekOrigin.Begin);
						input.Read(data);
					}
					catch(Exception)
					{
						return null;
					}
				}
				
				return data;
			}
		}
    #endregion

		/// <summary>The size of the data 
		/// </summary>
		internal long byteSize;

		internal byte[] data;

    #region Constructors
        /// <summary>
        /// Create an UndefinedData object using the specified header.
        /// </summary>
        /// <param name="h"></param>
		public UndefinedData(Header h)
		{
			
			int size = 1;
			for (int i = 0; i < h.GetIntValue("NAXIS"); i += 1)
			{
				size *= h.GetIntValue("NAXIS" + (i + 1));
			}
			size += h.GetIntValue("PCOUNT");
			if (h.GetIntValue("GCOUNT") > 1)
			{
				size *= h.GetIntValue("GCOUNT");
			}
			size *= System.Math.Abs(h.GetIntValue("BITPIX") / 8);
			
			data = new byte[size];
			byteSize = size;
		}

    /// <summary>Create an UndefinedData object using the specified object.</summary>
		public UndefinedData(Object x)
		{
			
			byteSize = ArrayFuncs.ComputeSize(x);
			data = new byte[(int) byteSize];
		}
    #endregion

		/// <summary>Fill header with keywords that describe data.
		/// </summary>
		/// <param name="head">The FITS header
		/// 
		/// </param>
		internal override void FillHeader(Header head)
		{
			try
			{
				head.Xtension = "UNKNOWN";
				head.Bitpix = 8;
				head.Naxes = 1;
				head.AddValue("NAXIS1", byteSize, " Number of Bytes ");
				head.AddValue("PCOUNT", 0, null);
				head.AddValue("GCOUNT", 1, null);
				head.AddValue("EXTEND", true, "Extensions are permitted"); // Just in case!
			}
			catch (HeaderCardException e)
			{
				System.Console.Error.WriteLine("Unable to create unknown header:" + e);
			}
		}
        /// <summary>
        /// Method to read the ArrayDataIO
        /// </summary>
        /// <param name="i"></param>
        public override void Read(ArrayDataIO i)
		{
			SetFileOffset(i);
			
			if (i is RandomAccess)
			{
				try
				{
					//BinaryReader temp_BinaryReader;
					System.Int64 temp_Int64;
					//temp_BinaryReader = i;
					temp_Int64 = i.Position; //temp_BinaryReader.BaseStream.Position;
					temp_Int64 = i.Seek((int)byteSize) - temp_Int64;  //temp_BinaryReader.BaseStream.Seek((int) byteSize, SeekOrigin.Current) - temp_Int64;
					int generatedAux = (int)temp_Int64;
				}
				catch (IOException e)
				{
					throw new FitsException("Unable to skip over data:" + e);
				}
			}
			else
			{
				try
				{
                    i.Read(data);
				}
				catch (IOException e)
				{
					throw new FitsException("Unable to read unknown data:" + e);
				}
			}
			
			int pad = FitsUtil.Padding(TrueSize);
			try
			{
				//BinaryReader temp_BinaryReader2;
				System.Int64 temp_Int65;
				//temp_BinaryReader2 = i;
				temp_Int65 = i.Position;  //temp_BinaryReader2.BaseStream.Position;
				temp_Int65 = i.Seek(pad) - temp_Int65;  //temp_BinaryReader2.BaseStream.Seek(pad, SeekOrigin.Current) - temp_Int65;
			//	if (temp_Int65 != pad)
                if (i.Seek(pad) != pad)
				{
					throw new FitsException("Error skipping padding");
				}
			}
			catch (IOException e)
			{
				throw new FitsException("Error reading unknown padding:" + e);
			}
		}
		/// <summary>
		/// Method Write ArrayDataIO
		/// </summary>
		/// <param name="o"></param>
		public override void Write(ArrayDataIO o)
		{
			if (data == null)
			{
				Object generatedAux = DataArray;
			}
			
			if (data == null)
			{
				throw new FitsException("Null unknown data");
			}
			
			try
			{
				o.Write(data);
			}
			catch (IOException e)
			{
				throw new FitsException("IO Error on unknown data write" + e);
			}
			
			byte[] padding = new byte[FitsUtil.Padding(TrueSize)];
			try
			{
				o.Write(padding);
			}
			catch (IOException e)
			{
				throw new FitsException("Error writing padding: " + e);
			}
		}
	}
}

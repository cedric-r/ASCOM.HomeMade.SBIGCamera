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
	/// <summary>This abstract class is the parent of all HDU types.
	/// It provides basic functionality for an HDU.
	/// </summary>
	public abstract class BasicHDU : FitsElement
	{
    
    #region Properties
        /// <summary>Indicate whether HDU can be primary HDU.
        /// This method must be overriden in HDU types which can
        /// appear at the beginning of a FITS file.</summary>
        internal virtual bool CanBePrimary
        {
            get
            {
                return false;
            }
        }

        /// <summary>Get the associated header</summary>
		virtual public Header Header
		{
			get
			{
				return myHeader;
			}
		}
		/// <summary>Get the starting offset of the HDU</summary>
		virtual public long FileOffset
		{
			get
			{
				return myHeader.FileOffset;
			}
		}
		/// <summary>Get the associated Data object</summary>
		virtual public Data Data
		{
			get
			{
				return myData;
			}
		}

        /// <summary>Get the non-FITS data object</summary>
		virtual public Object Kernel
		{
			get
			{
				try
				{
					return myData.Kernel;
				}
				catch(FitsException)
				{
					return null;
				}
			}
		}

        /// <summary>Get the total size in bytes of the HDU.</summary>
		/// <returns>The size in bytes.</returns>
		virtual public long Size
		{
			get
			{
				int size = 0;
				
				if (myHeader != null)
				{
					size = (int) (size + myHeader.Size);
				}
				if (myData != null)
				{
					size = (int) (size + myData.Size);
				}
				return size;
			}
		}
        /// <summary>
        /// Returnd the BitPix value
        /// </summary>
        /// <exception cref="FitsException"./>
        virtual public int BitPix
		{
			get
			{
				int bitpix = myHeader.GetIntValue("BITPIX", - 1);
				switch (bitpix)
				{
					case BITPIX_BYTE: 
					case BITPIX_SHORT: 
					case BITPIX_INT: 
					case BITPIX_FLOAT: 
					case BITPIX_DOUBLE: 
						break;
					default: 
						throw new FitsException("Unknown BITPIX type " + bitpix);
				}
				
				return bitpix;
			}
		}
        /// <summary>
        /// Return the number of axes in the associated data array
        /// </summary>
        /// <exception cref="FitsException"./>
        virtual public int[] Axes
		{
			get
			{
				int nAxis = myHeader.GetIntValue("NAXIS", 0);
				if (nAxis < 0)
				{
					throw new FitsException("Negative NAXIS value " + nAxis);
				}
				if (nAxis > 999)
				{
					throw new FitsException("NAXIS value " + nAxis + " too large");
				}
				
				if (nAxis == 0)
				{
					return null;
				}
				
				int[] axes = new int[nAxis];
				for (int i = 1; i <= nAxis; i++)
				{
					axes[nAxis - i] = myHeader.GetIntValue("NAXIS" + i, 0);
				}
				
				return axes;
			}
		}
        /// <summary>
        /// Returns the value oF PCOUNT from HDU
        /// </summary>
        virtual public int ParameterCount
		{
			get
			{
				return myHeader.GetIntValue("PCOUNT", 0);
			}
		}
        /// <summary>
        /// Returns the value oF GCOUNT from HDU
        /// </summary>
		virtual public int GroupCount
		{
			get
			{
				return myHeader.GetIntValue("GCOUNT", 1);
			}
		}
        /// <summary>
        /// Returns the value oF BSCALE from HDU
        ///The value field shall contain a floating point number.The default value for this keyword os 1.0
        /// </summary>
        virtual public double BScale
		{
			get
			{
				return myHeader.GetDoubleValue("BSCALE", 1.0);
			}
		}
        /// <summary>
        /// Returns the value oF BZero from HDU
        /// This keyword shall be used, along with the BSCALE keyword
        /// The default value for this keyword is 0.0.
        /// </summary>
        virtual public double BZero
		{
			get
			{
				return myHeader.GetDoubleValue("BZERO", 0.0);
			}
		}
        /// <summary>
        /// The value shall contain a character String, describing the physical
        /// units in which the quantities in the array, after application of BSCALE and BZERO,are expressed.
        /// </summary>
        virtual public String BUnit
		{
			get
			{
				return GetTrimmedString("BUNIT");
			}
		}
        /// <summary>
        /// Returns value for BLANK keyword if exists
        /// </summary>
        /// <exception cref="FitsException" If BLANK is undefined. />
        virtual public int BlankValue
		{
			get
			{
				if (!myHeader.ContainsKey("BLANK"))
				{
					throw new FitsException("BLANK undefined");
				}
				return myHeader.GetIntValue("BLANK");
			}
		}

        /// <summary> Get the FITS file creation date as a <CODE>Date</CODE> object.</summary>
		/// <returns>	either <CODE>null</CODE> or a Date object</returns>
		virtual public DateTime CreationDate
		{
			get
			{
                Object result = null;

                try
				{
					result = new FitsDate(myHeader.GetStringValue("DATE")).ToDate();
				}
				catch(FitsException)
				{
					result = null;
				}

                return (DateTime)result;
			}
		}
		
        /// <summary> Get the FITS file observation date as a <CODE>Date</CODE> object.</summary>
		/// <returns>	either <CODE>null</CODE> or a Date object</returns>
		virtual public DateTime ObservationDate
		{
			get
			{
                Object result = null;

				try
				{
					result = new FitsDate(myHeader.GetStringValue("DATE-OBS")).ToDate();
				}
				catch (FitsException)
				{
					result = null;
				}

                return (DateTime)result;
			}
		}

        /// <summary> Get the name of the organization which created this FITS file.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Origin
		{
			get
			{
				return GetTrimmedString("ORIGIN");
			}
		}

        /// <summary> Get the name of the telescope which was used to acquire the data in this FITS file.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Telescope
		{
			get
			{
				return GetTrimmedString("TELESCOP");
			}
		}

        /// <summary> Get the name of the instrument which was used to acquire the data in this FITS file.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Instrument
		{
			get
			{
				return GetTrimmedString("INSTRUME");
			}
		}

        /// <summary>Get the name of the person who acquired the data in this FITS file.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Observer
		{
			get
			{
				return GetTrimmedString("OBSERVER");
			}
		}

        /// <summary> Get the name of the observed object in this FITS file.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Object
		{
			get
			{
				return GetTrimmedString("OBJECT");
			}
		}

        /// <summary> Get the equinox in years for the celestial coordinate system in which
		/// positions given in either the header or data are expressed.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public double Equinox
		{
			get
			{
				return myHeader.GetDoubleValue("EQUINOX", - 1.0);
			}
		}

        /// <summary> Get the equinox in years for the celestial coordinate system in which
		/// positions given in either the header or data are expressed.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		/// <deprecated>	Replaced by getEquinox</deprecated>
		/// <seealso cref="">#getEquinox()</seealso>
		virtual public double Epoch
		{
			get
			{
				return myHeader.GetDoubleValue("EPOCH", - 1.0);
			}
		}

        /// <summary> Return the name of the person who compiled the information in
		/// the data associated with this header.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Author
		{
			get
			{
				return GetTrimmedString("AUTHOR");
			}
		}

        /// <summary> Return the citation of a reference where the data associated with
		/// this header are published.</summary>
		/// <returns>	either <CODE>null</CODE> or a String object</returns>
		virtual public String Reference
		{
			get
			{
				return GetTrimmedString("REFERENC");
			}
		}

        /// <summary> Return the minimum valid value in the array.</summary>
		/// <returns>	minimum value.</returns>
		virtual public double MaximumValue
		{
			get
			{
				return myHeader.GetDoubleValue("DATAMAX");
			}
		}

        /// <summary> Return the minimum valid value in the array.</summary>
		/// <returns>	minimum value.</returns>
		virtual public double MinimumValue
		{
			get
			{
				return myHeader.GetDoubleValue("DATAMIN");
			}
		}

        /// <summary>Indicate that an HDU is the first element of a FITS file.</summary>
		virtual internal bool PrimaryHDU
		{
			set
			{
				if(value && !this.CanBePrimary)
				{
					throw new FitsException("Invalid attempt to make HDU of type:" + this.GetType().FullName + " primary.");
				}
				else
				{
					this.isPrimary = value;
				}
				
				// Some FITS readers don't like the PCOUNT and GCOUNT keywords
				// in a primary array or they EXTEND keyword in extensions.
				
				if(isPrimary && !myHeader.GetBooleanValue("GROUPS", false))
				{
					myHeader.DeleteKey("PCOUNT");
					myHeader.DeleteKey("GCOUNT");
				}
				
				if(isPrimary)
				{
					HeaderCard card = myHeader.FindCard("EXTEND");
					if(card == null)
					{
                        // .97 changes:
                        int[] a = Axes; // Leaves the iterator pointing to the last NAXISn card.
						myHeader.NextCard();

						myHeader.AddValue("EXTEND", true, "Allow extensions");
					}
				}
				
				if(!isPrimary)
				{
					Cursor c = myHeader.GetCursor();
					
					int pcount = myHeader.GetIntValue("PCOUNT", 0);
					int gcount = myHeader.GetIntValue("GCOUNT", 1);
					int naxis = myHeader.GetIntValue("NAXIS", 0);
					myHeader.DeleteKey("EXTEND");
					//HeaderCard card;
					HeaderCard pcard = myHeader.FindCard("PCOUNT");
					HeaderCard gcard = myHeader.FindCard("GCOUNT");
					
					myHeader.GetCard(2 + naxis);
					if (pcard == null)
					{
						myHeader.AddValue("PCOUNT", pcount, "Required value");
					}
					if (gcard == null)
					{
						myHeader.AddValue("GCOUNT", gcount, "Required value");
					}
					c = myHeader.GetCursor();
				}
			}
		}
        /// <summary>
        /// Returns DummyHDu instance
        /// </summary>
        public static BasicHDU DummyHDU
		{
			get
			{
				try
				{
					return FitsFactory.HDUFactory(new int[0]);
				}
				catch(FitsException fe)
				{
					Console.Error.WriteLine("Impossible exception in GetDummyHDU");
					return null;
				}
			}
		}

        /// <summary>Is the HDU rewriteable</summary>
        public virtual bool Rewriteable
        {
            get
            {
                return myHeader.Rewriteable && myData.Rewriteable;
            }
        }
	#endregion

    #region Class Variables
        /// <summary>
        /// Constant for BitPix byte value
        /// </summary>
        public const int BITPIX_BYTE = 8;
        /// <summary>
        /// Constant for BitPix short value
        /// </summary>
		public const int BITPIX_SHORT = 16;
        /// <summary>
        /// Constant for BitPix integer value
        /// </summary>
		public const int BITPIX_INT = 32;
        /// <summary>
        /// Constant for BitPix long value
        /// </summary>
		public const int BITPIX_LONG = 64;
        /// <summary>
        /// Constant for BitPix float value
        /// </summary>
		public const int BITPIX_FLOAT = - 32;
        /// <summary>
        /// Constant for BitPix double value
        /// </summary>
		public const int BITPIX_DOUBLE = - 64;
    #endregion

    #region Instance Variables
        /// <summary>The associated header.</summary>
		protected internal Header myHeader = null;
		
		/// <summary>The associated data unit.</summary>
		protected internal Data myData = null;
		
		/// <summary>Is this the first HDU in a FITS file?</summary>
		protected internal bool isPrimary = false;
    #endregion

		/// <summary>Create a Data object to correspond to the header description.</summary>
		/// <returns> An unfilled Data object which can be used to read in the data for this HDU.</returns>
        /// <exception cref="FitsException"> FitsException if the Data object could not be created
		/// from this HDU's Header</exception>
		internal abstract Data ManufactureData();
		
		/// <summary>Skip the Data object immediately after the given Header object on
		/// the given stream object.</summary>
		/// <param name="stream">the stream which contains the data.</param>
        /// <param name="hdr">template indicating length of Data section</param>
        /// <exception cref="IOException"> IOException if the Data object could not be skipped.</exception>
		public static void SkipData(ArrayDataIO stream, Header hdr)
		{
			//System.IO.BinaryReader temp_BinaryReader;
			Int64 temp_Int64;
			//temp_BinaryReader = stream;
			temp_Int64 = stream.Position; //temp_BinaryReader.BaseStream.Position;
			temp_Int64 = stream.Seek((int)hdr.DataSize) - temp_Int64; //temp_BinaryReader.BaseStream.Seek((int) hdr.DataSize, System.IO.SeekOrigin.Current) - temp_Int64;
			int generatedAux = (int)temp_Int64;
		}
		
		/// <summary>Skip the Data object for this HDU.</summary>
		/// <param name="stream">the stream which contains the data.</param>
        /// <exception cref="IOException"> IOException if the Data object could not be skipped.</exception>
		public virtual void SkipData(ArrayDataIO stream)
		{
			SkipData(stream, myHeader);
		}
		
		/// <summary>Read in the Data object for this HDU.</summary>
		/// <param name="stream">the stream from which the data is read.</param>
        /// <exception cref="FitsException"> FitsException if the Data object could not be created from this HDU's Header</exception>
		public virtual void ReadData(ArrayDataIO stream)
		{
			myData = null;
			try
			{
				myData = ManufactureData();
			}
			finally
			{
				// if we cannot build a Data object, skip this section
				if (myData == null)
				{
					try
					{
						SkipData(stream, myHeader);
					}
					catch(Exception)
					{
					}
				}
			}
			
			myData.Read(stream);
		}

        /// <summary>Check that this is a valid header for the HDU.</summary>
		/// <param name="header">to validate.</param>
		/// <returns> <CODE>true</CODE> if this is a valid header.</returns>
		public static bool IsHeader(Header header)
		{
			return false;
		}
		
		/// <summary>Print out some information about this HDU.</summary>
		public abstract void Info();
		
		/// <summary>Check if a field is present and if so print it out.</summary>
		/// <param name="name">The header keyword.</param>
		/// <returns>Was it found in the header?</returns>
		internal virtual bool CheckField(String name)
		{
			String value_Renamed = myHeader.GetStringValue(name);
			if (value_Renamed == null)
			{
				return false;
			}
			
			return true;
		}
		
        /// <summary>Read out the HDU from the data stream.  This
		/// will overwrite any existing header and data components.
        /// </summary>
		public virtual void Read(ArrayDataIO stream)
		{
			myHeader = Header.ReadHeader(stream);
			myData = myHeader.MakeData();
			myData.Read(stream);
		}
		
        /// <summary>Write out the HDU.</summary>
		public virtual void Write(ArrayDataIO stream)
		{
			if (myHeader != null)
			{
				myHeader.Write(stream);
			}
			if (myData != null)
			{
				myData.Write(stream);
			}
			try
			{
				stream.Flush();
			}
			catch(IOException e)
			{
				throw new FitsException("Error flushing at end of HDU: " + e.Message);
			}
		}
		
		/// <summary>Rewrite the HDU.</summary>
		public virtual void Rewrite()
		{
			if(this.Rewriteable)
			{
				myHeader.Rewrite();
				myData.Rewrite();
			}
			else
			{
				throw new FitsException("Invalid attempt to rewrite HDU");
			}
		}
       
      /// <summary>
     /// Get the String value associated with <CODE>keyword</CODE>
      /// </summary>
      /// <param name="keyword"></param>
      /// <returns>either <CODE>null</CODE> or a String with leading/trailing blanks stripped.</returns>
		public virtual String GetTrimmedString(String keyword)
		{
			String s = myHeader.GetStringValue(keyword);
			if (s != null)
			{
				s = s.Trim();
			}
			return s;
		}

        /// <summary>Add boolean information to the header</summary>
		public virtual void AddValue(String key, bool val, String comment)
		{
			myHeader.AddValue(key, val, comment);
		}
        /// <summary>Add int information to the header</summary>
		public virtual void AddValue(String key, int val, String comment)
		{
			myHeader.AddValue(key, val, comment);
		}
        /// <summary>Add double information to the header</summary>
		public virtual void AddValue(String key, double val, String comment)
		{
			myHeader.AddValue(key, val, comment);
		}
        /// <summary>Add String information to the header</summary>
		public virtual void AddValue(String key, String val, String comment)
		{
			myHeader.AddValue(key, val, comment);
		}
	}
}

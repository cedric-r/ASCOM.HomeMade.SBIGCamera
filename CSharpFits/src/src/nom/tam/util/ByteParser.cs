namespace nom.tam.util
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
	
	/// <summary>This class provides routines
	/// for efficient parsing of data stored in a byte array.
	/// This routine is optimized (in theory at least!) for efficiency
	/// rather than accuracy.  The values read in for doubles or floats
	/// may differ in the last bit or so from the standard input
	/// utilities, especially in the case where a float is specified
	/// as a very long string of digits (substantially longer than
	/// the precision of the type).
	/// <p>
	/// The get methods generally are available with or without a length
	/// parameter specified.  When a length parameter is specified only
	/// the bytes with the specified range from the current offset will
	/// be search for the number.  If no length is specified, the entire
	/// buffer from the current offset will be searched.
	/// <p>
	/// The getString method returns a string with leading and trailing
	/// white space left intact.  For all other get calls, leading
	/// white space is ignored.  If fillFields is set, then the get
	/// methods check that only white space follows valid data and a
	/// FormatException is thrown if that is not the case.  If
	/// fillFields is not set and valid data is found, then the
	/// methods return having read as much as possible.  E.g., for
	/// the sequence "T123.258E13", a getBoolean, getInteger and
	/// getFloat call would return true, 123, and 2.58e12 when
	/// called in succession.
	/// </p>
    /// </p>
    /// </summary>
	public class ByteParser
	{
    #region Properties
    /// <summary>Get the buffer being used by the parser</summary>
		/// <summary>Set the buffer for the parser</summary>
		virtual public byte[] Buffer
		{
			get
			{
				return input;
			}
			
			set
			{
				this.input = value;
				this.offset = 0;
			}
		}
        /// <summary>Get the current offset.Set the offset into the array.</summary>
		/// <returns>The current offset within the buffer.</returns>
		virtual public int Offset
		{
            
			get
			{
				return offset;
			}
			
			set
			{
				this.offset = value;
			}
		}
		/// <summary>Do we require a field to completely fill up the specified
		/// length (with optional leading and trailing white space.
		/// </summary>
		virtual public bool FillFields
		{
			set
			{
				fillFields = value;
			}
		}
		/// <summary>Get the number of characters used to parse the previous
		/// number (or the length of the previous String returned).
		/// </summary>
		virtual public int NumberLength
		{
			get
			{
				return numberLength;
			}
		}
		/// <summary>Read in the buffer until a double is read.  This will read
		/// the entire buffer if fillFields is set.
		/// </summary>
		/// <returns> The value found.
		/// 
		/// </returns>
		virtual public double Double
		{
			get
			{
				return GetDouble(input.Length - offset);
			}
		}
		/// <summary>Get a floating point value from the buffer.  (see getDouble(int())
		/// </summary>
		virtual public float Float
		{
			get
			{
				return (float) GetDouble(input.Length - offset);
			}
			
		}
		/// <summary>Look for an integer at the beginning of the buffer 
		/// </summary>
		virtual public int Int
		{
			get
			{
				return GetInt(input.Length - offset);
			}
			
		}
		/// <summary>Get a boolean value from the beginning of the buffer 
		/// </summary>
		virtual public bool Boolean
		{
			get
			{
				return GetBoolean(input.Length - offset);
			}
			
		}
		
    #endregion

    #region Variables
		/// <summary>Array being parsed</summary>
		private byte[] input;
		
		/// <summary>Current offset into input.</summary>
		private int offset;
		
		/// <summary>Length of last parsed value</summary>
		private int numberLength;
		
		/// <summary>Did we find a sign last time we checked?</summary>
		private bool foundSign;
		
		/// <summary>Do we fill up fields?</summary>
		private bool fillFields = false;
    #endregion

		/// <summary>Construct a parser.</summary>
		/// <param name="input">byte array to be parsed.
		/// Note that the array can be re-used by
		/// refilling its contents and resetting the offset.</param>
		public ByteParser(byte[] input)
		{
			this.input = input;
			this.offset = 0;
		}

    /// <summary>Look for a double in the buffer.  Leading spaces are ignored.</summary>
		/// <param name="length">The maximum number of characters used to parse this number.
		/// If fillFields is specified then exactly only whitespace may follow
		///  a valid double value.</param>
		public virtual double GetDouble(int length)
		{
		//	System.out.println("Checking: "+new String(input, offset, length));
			
			int startOffset = offset;
			
			bool error = true;
			
			double number = 0;
//			int i = 0;
			
			// Skip initial blanks.
			length -= SkipWhite(length);
			if (length == 0)
			{
				return 0;
			}
			
			double mantissaSign = CheckSign();
			if (foundSign)
			{
				length -= 1;
			}

            // suggested in .99.5 version:
            // Look for the special strings NaN, Inf,
	        if (length >= 3 &&
                (input[offset] == 'n' || input[offset] == 'N') &&
                (input[offset + 1] == 'a' || input[offset+1] == 'A') &&
                (input[offset + 2] == 'n' || input[offset+2] == 'N'))
            {
        	
	            number = Double.NaN;
	            length -= 3;
	            offset += 3;

	        }
            // suggested in .99.2 version:
            // Look for the longer string first then try the shorter.
            else if (length >= 8 &&
                (input[offset] == 'i' || input[offset] == 'I') &&
                (input[offset + 1] == 'n' || input[offset+1] == 'N') &&
                (input[offset + 2] == 'f' || input[offset+2] == 'F') &&
                (input[offset + 3] == 'i' || input[offset+3] == 'I') &&
                (input[offset + 4] == 'n' || input[offset+4] == 'N') &&
                (input[offset + 5] == 'i' || input[offset+5] == 'I') &&
                (input[offset + 6] == 't' || input[offset+6] == 'T') &&
                (input[offset + 7] == 'y' || input[offset+7] == 'Y'))
            {
	            number = Double.PositiveInfinity;
	            length -= 8;
	            offset += 8;
        	    
	        }
            else if (length >= 3 &&
             (input[offset] == 'i' || input[offset] == 'I') &&
             (input[offset+1] == 'n' || input[offset+1] == 'N') &&
             (input[offset+2] == 'f' || input[offset+2] == 'F'))
            {
                number = Double.PositiveInfinity;
                length -= 3;
                offset += 3;

            }
            else
            {
                number = GetBareInteger(length); // This will update offset
                length -= numberLength; // Set by getBareInteger

                if (numberLength > 0)
                {
                    error = false;
                }

                // Check for fractional values after decimal
                if (length > 0 && input[offset] == '.')
                {

                    offset += 1;
                    length -= 1;

                    double numerator = GetBareInteger(length);
                    if (numerator > 0)
                    {
                        number += numerator / System.Math.Pow(10.0, numberLength);
                    }
                    length -= numberLength;
                    if (numberLength > 0)
                    {
                        error = false;
                    }
                }

                if (error)
                {
                    offset = startOffset;
                    numberLength = 0;
                    throw new FormatException("Invalid real field");
                }

                // Look for an exponent
                if (length > 0)
                {

                    // Our Fortran heritage means that we allow 'D' for the exponent indicator.
                    if (input[offset] == 'e' || input[offset] == 'E' ||
                        input[offset] == 'd' || input[offset] == 'D')
                    {
                        offset += 1;
                        length -= 1;
                        if (length > 0)
                        {
                            int sign = CheckSign();
                            if (foundSign)
                            {
                                length -= 1;
                            }

                            //UPGRADE_WARNING: Narrowing conversions may produce unexpected results in C#. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042"'
                            int exponent = (int)GetBareInteger(length);

                            // suggested in .99.5 version:
                            // For very small numbers we try to miminize
			                // effects of denormalization.
			                if (exponent*sign > -300)
                            {
		                            number *= Math.Pow(10f,exponent*sign);
			                }
                            else
                            {
                                number = Math.E - 300 * (number * Math.Pow(10f, exponent * sign + 300));
			                }
		                    length -= numberLength;
                        }
                    }
                }
            }
			if (fillFields && length > 0)
			{
				if (IsWhite(length))
				{
					offset += length;
				}
				else
				{
					numberLength = 0;
					offset = startOffset;
					throw new FormatException("Non-blanks following real.");
				}
			}
			
			numberLength = offset - startOffset;
			return mantissaSign * number;
            
		}

        /// <summary>Get a floating point value in a region of the buffer</summary>
		public virtual float GetFloat(int length)
		{
			return (float) GetDouble(length);
		}
		
		/// <summary>Convert a region of the buffer to an integer</summary>
		public virtual int GetInt(int length)
		{
			int startOffset = offset;
			
			length -= SkipWhite(length);
			
			int number = 0;
			bool error = true;
			
			int sign = CheckSign();
			if (foundSign)
			{
				length -= 1;
			}
          	while (length > 0 && input[offset] >= '0' && input[offset] <= '9')
			{
				number = number * 10 + input[offset] - '0';
				offset += 1;
				length -= 1;
				error = false;
			}
            
			if (error)
			{
				numberLength = 0;
				offset = startOffset;
				throw new FormatException("Invalid Integer");
			}
			
			if (length > 0 && fillFields)
			{
				if (IsWhite(length))
				{
					offset += length;
				}
				else
				{
					numberLength = 0;
					offset = startOffset;
					throw new FormatException("Non-white following integer");
				}
			}
			
			numberLength = offset - startOffset;
			return sign * number;
		}

        /// <summary>Look for a long in a specified region of the buffer</summary>
		public virtual long GetLong(int length)
		{
			int startOffset = offset;
			
			// Skip white space.
			length -= SkipWhite(length);
			
			long number = 0;
			bool error = true;
			
			long sign = CheckSign();
			if (foundSign)
			{
				length -= 1;
			}

            while (length > 0 && input[offset] >= '0' && input[offset] <= '9')
			{
				number = number * 10 + input[offset] - '0';
				error = false;
				offset += 1;
				length -= 1;
			}
			
			if (error)
			{
				numberLength = 0;
				offset = startOffset;
				throw new FormatException("Invalid long number");
			}
			
			if (length > 0 && fillFields)
			{
				if (IsWhite(length))
				{
					offset += length;
				}
				else
				{
					offset = startOffset;
					numberLength = 0;
					throw new FormatException("Non-white following long");
				}
			}
			numberLength = offset - startOffset;
			return sign * number;
		}
		
		/// <summary>Get a string</summary>
		/// <param name="length">The length of the string.</param>
		public virtual String GetString(int length)
		{
			//char[] tmpChar;
			//tmpChar = new char[input.Length];
			//input.CopyTo(tmpChar, 0);
            // TODO: FIGURE OUT WHY THIS HANGS ALL THE TIME
			//String s = new String(tmpChar, offset, length);
            char[] tmpChar = new char[length];
            Array.Copy(input, offset, tmpChar, 0, length);
            String s = new String(tmpChar);//new String(input, offset, length);
			offset += length;
			numberLength = length;
			return s;
		}

        /// <summary>Get a boolean value from a specified region of the buffer</summary>
		public virtual bool GetBoolean(int length)
		{
			int startOffset = offset;
			length -= SkipWhite(length);
			if (length == 0)
			{
				throw new FormatException("Blank boolean field");
			}

            bool value_Renamed = false;
            if (input[offset] == 'T' || input[offset] == 't')
			{
				value_Renamed = true;
			}
            else if (input[offset] != 'F' && input[offset] != 'f')
			{
				numberLength = 0;
				offset = startOffset;
				throw new FormatException("Invalid boolean value");
			}
			offset += 1;
			length -= 1;
			
			if (fillFields && length > 0)
			{
				if (IsWhite(length))
				{
					offset += length;
				}
				else
				{
					numberLength = 0;
					offset = startOffset;
					throw new FormatException("Non-white following boolean");
				}
			}
			numberLength = offset - startOffset;
			return value_Renamed;
		}
		
		/// <summary>Skip bytes in the buffer 
		/// </summary>
		public virtual void Skip(int nBytes)
		{
			offset += nBytes;
		}
		
		
		/// <summary>Get the integer value starting at the current position.
		/// This routine returns a double rather than an int/long
		/// to enable it to read very long integers (with reduced
		/// precision) such as 111111111111111111111111111111111111111111.
		/// Note that this routine does set numberLength.</summary>
		/// <param name="length">The maximum number of characters to use.</param>
		private double GetBareInteger(int length)
		{
			int startOffset = offset;
			double number = 0;

            while (length > 0 && input[offset] >= '0' && input[offset] <= '9')
			{
				
				number *= 10;
				number += input[offset] - '0';
				offset += 1;
				length -= 1;
			}
			numberLength = offset - startOffset;
			return number;
		}
		
		/// <summary>Skip white space.  This routine skips with space in
		/// the input and returns the number of character skipped.
		/// White space is defined as ' ', '\t', '\n' or '\r'
		/// </summary>
		/// <param name="length">The maximum number of characters to skip.
		/// 
		/// </param>
		public virtual int SkipWhite(int length)
		{
			int i;
			for (i = 0; i < length; i += 1)
			{
                if (input[offset+i] != ' ' && input[offset+i] != '\t' &&
                    input[offset+i] != '\n' && input[offset+i] != '\r')
				{
					break;
				}
			}
			
			offset += i;
			return i;
		}
		
		/// <summary>Find the sign for a number .
		/// This routine looks for a sign (+/-) at the current location
		/// and return +1/-1 if one is found, or +1 if not.
		/// The foundSign boolean is set if a sign is found and offset is
		/// incremented.
		/// </summary>
		private int CheckSign()
		{
			
			foundSign = false;

            if (input[offset] == '+')
			{
				foundSign = true;
				offset += 1;
				return 1;
			}
            else if (input[offset] == '-')
			{
				foundSign = true;
				offset += 1;
				return - 1;
			}
			
			return 1;
		}
		
		
		/// <summary>Is a region blank?</summary>
		/// <param name="length">The length of the region to be tested</param>
		private bool IsWhite(int length)
		{
			int oldOffset = offset;
			bool value_Renamed = SkipWhite(length) == length;
			offset = oldOffset;
			return value_Renamed;
		}
	}
}
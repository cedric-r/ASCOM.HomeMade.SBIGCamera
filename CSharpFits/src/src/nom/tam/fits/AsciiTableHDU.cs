namespace nom.tam.fits
{
   /*
    * Copyright: Thomas McGlynn 1997-2007.
    * Many thanks to David Glowacki (U. Wisconsin) for substantial
    * improvements, enhancements and bug fixes.
    *
    * The CSharpFITS package is a C# port of Tom McGlynn's
    * nom.tam.fits Java package, initially ported by  Samuel Carliles
    *
    * Copyright: 2007 Virtual Observatory - India. 
    * 
    * Use is subject to license terms
    */
    using System;
	using nom.tam.util;
	
	/// <summary> FITS ASCII table header/data unit
	/// </summary>
	public class AsciiTableHDU:TableHDU
	{
		/// <summary> Check that this HDU has a valid header.</summary>
		/// <returns> <CODE>true</CODE> if this HDU has a valid header.</returns>
		virtual public bool HasHeader
		{
			get
			{
				return IsHeader(myHeader);
			}
			
		}
        /// <summary>
        /// Returns data object of the AsciiTableHDU
        /// </summary>
        override public Data Data
		{
			get
			{
				return data;
			}
		}
		
		/// <summary>Just a copy of myData with the correct type</summary>
		internal AsciiTable data;

        /// Suggested in .97 version
        /// <summary>
        /// The standard column stems for an ASCII table.
        /// Note that TBCOL is not included here -- it needs to 
        /// be handled specially since it does not simply shift.
        /// </summary>
        private String[] keyStems = { "TFORM", "TZERO", "TNULL", "TTYPE", "TUNIT" };
        
		/// <summary>
        /// Create an ascii table header/data unit
		/// </summary>
        /// <param name="h">The template specifying the ascii table.</param>
        /// <param name="d"> The FITS data structure containing the table data.</param>
        ///  <exception cref="FitsException"> FitsException if there was a problem with the header.</exception>
        public AsciiTableHDU(Header h, Data d):base((TableData) d)
		{
			myHeader = h;
			data = (AsciiTable) d;
			myData = d;
		}
		
		
		/// <summary> Check that this is a valid ascii table header.</summary>
		/// <param name="header">to validate.</param>
		/// <returns> <CODE>true</CODE> if this is an ascii table header.</returns>
		public static new bool IsHeader(Header header)
		{
			return header.GetStringValue("XTENSION").Trim().Equals("TABLE");
		}

        /// <summary>Check if this data is usable as an ASCII table.</summary>
		public static bool IsData(Object o)
		{
            if(ArrayFuncs.CountDimensions(o) != 2)
            {
                return false;
            }

        /*  Type t = ArrayFuncs.GetBaseClass(o);
                return t != null && (t.Equals(typeof(String)) || t.Equals(typeof(int)) || t.Equals(typeof(long)) ||
            t.Equals(typeof(float)) || t.Equals(typeof(double)));
        */  
            if(o is Object[])
			{
				System.Object[] oo = (System.Object[]) o;
				for (int i = 0; i < oo.Length; i += 1)
				{
					if (oo[i] is System.String[] || oo[i] is int[] || oo[i] is long[] || oo[i] is float[] || oo[i] is double[])
					{
						continue;
					}
					return false;
				}
				return true;
			}
			else
			{
				return false;
			}
      
		}
		
		/// <summary> Create a Data object to correspond to the header description.</summary>
		/// <returns> An unfilled Data object which can be used to read in the data for this HDU.</returns>
        /// <exception cref="FitsException"> FitsException if the Data object could not be created from this HDU's Header</exception>
		public static Data ManufactureData(Header hdr)
		{
			return new AsciiTable(hdr);
		}

        /// <summary>Create an empty data structure corresponding to the input header.</summary>
        internal override Data ManufactureData()
		{
			return ManufactureData(myHeader);
		}
		
		/// <summary>Create a header to match the input data.</summary>
        public static Header ManufactureHeader(Data d)
		{
			Header hdr = new Header();
			d.FillHeader(hdr);
			Cursor c = hdr.GetCursor();
			return hdr;
		}
		
        /// <summary>
        /// Create a ASCII table data structure from 
        /// an array of objects representing the columns.
        /// </summary>
		public static Data Encapsulate(Object o)
		{
          if(o != null && o.GetType().IsArray)
          {
            if(ArrayFuncs.IsArrayOfArrays(o))
            {
                Array oo = (Array)o;
                AsciiTable d = new AsciiTable();
                for (int i = 0; i < oo.Length; i += 1)
                {
                    d.AddColumn(oo.GetValue(i));
                }
                return d;
            }
            else
            {
              throw new Exception("OOPS.  FIX AsciiTableHDU.Encapsulate(Object o).");
            }
          }

          return null;
		}
		
		/// <summary> Skip the ASCII table and throw an exception.</summary>
		/// <param name="stream">the stream from which the data is read.</param>
		public override void ReadData(ArrayDataIO stream)
		{
			myData.Read(stream);
		}

        /// <summary>Mark an entry as null.</summary>
		public virtual void SetNull(int row, int col, bool flag)
		{
			if(flag)
			{
				String nullStr = myHeader.GetStringValue("TNULL" + (col + 1));
				if (nullStr == null)
				{
					SetNullString(col, "NULL");
				}
			}
			data.SetNull(row, col, flag);
		}
		
		/// <summary>See if an element is null</summary>
		public virtual bool IsNull(int row, int col)
		{
			return data.IsNull(row, col);
		}
		
		/// <summary>Set the null string for a column</summary>
		public virtual void SetNullString(int col, String newNull)
		{
			myHeader.PositionAfterIndex("TBCOL", col + 1);
			try
			{
				myHeader.AddValue("TNULL" + (col + 1), newNull, null);
			}
			catch(HeaderCardException e)
			{
				Console.Error.WriteLine("Impossible exception in setNullString" + e);
			}

            data.SetNullString(col, newNull);
		}
		
		/// <summary>Add a column</summary>
		public override int AddColumn(Object newCol)
		{
			data.AddColumn(newCol);
			
			// Move the cursor to point after all the data describing
			// the previous column.
			
			Cursor c = myHeader.PositionAfterIndex("TBCOL", data.NCols);
			
			int rowlen = data.AddColInfo(NCols, c);
			int oldRowlen = myHeader.GetIntValue("NAXIS1");
			myHeader.SetNaxis(1, rowlen + oldRowlen);
			
			int oldTfields = myHeader.GetIntValue("TFIELDS");
			try
			{
				myHeader.AddValue("TFIELDS", oldTfields + 1, null);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("Impossible exception at addColumn:" + e);
			}
			return NCols;
		}
		
		/// <summary> Print a little information about the data set.</summary>
		public override void Info()
		{
			Console.Out.WriteLine("ASCII Table:");
			Console.Out.WriteLine("  Header:");
			Console.Out.WriteLine("    Number of fields:" + myHeader.GetIntValue("TFIELDS"));
			Console.Out.WriteLine("    Number of rows:  " + myHeader.GetIntValue("NAXIS2"));
			Console.Out.WriteLine("    Length of row:   " + myHeader.GetIntValue("NAXIS1"));
			Console.Out.WriteLine("  Data:");
			Array data = (Array)Kernel;
			for (int i = 0; i < NCols; i += 1)
			{
				System.Console.Out.WriteLine("      " + i + ":" + ArrayFuncs.ArrayDescription(data.GetValue(i)));
			}
		}

        /// <summary>Return the FITS data structure associated with this HDU.</summary>
        public Data GetData()
        {
            return data;
        }

        /// Suggested in .97 version
        /// <summary>Return the keyword column stems for an ASCII table.</summary>
        public override string[] ColumnKeyStems
        {
            get
            {
                return keyStems;
            }
        }

    }
}

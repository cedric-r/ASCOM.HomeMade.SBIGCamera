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
	/// <summary>FITS binary table header/data unit</summary>
	public class BinaryTableHDU:TableHDU
    {

    #region Instance Variables
        private BinaryTable table;

        // change suggested in .99.1 version
        /// <summary> The standard column keywords for a binary table.</summary>
        private String[] keyStems = { "TTYPE", "TFORM", "TUNIT", "TNULL", "TSCAL", "TZERO", "TDISP", "TDIM" };
    #endregion


    #region Properties
        /// <summary>Check that this HDU has a valid header.</summary>
		/// <returns> <CODE>true</CODE> if this HDU has a valid header.</returns>
		virtual public bool HasHeader
		{
			get
			{
				return IsHeader(myHeader);
			}
		}

        // change suggested in .99.2 version
        /// <summary>
        /// Returns the standars column keywords
        /// </summary>
        public override string[] ColumnKeyStems
        {
            get
            {
                return keyStems;
            }
        }
   #endregion


    #region Constructor
        /// <summary>
        /// Consructor initislizing header,data
        /// </summary>
        /// <param name="hdr"></param>
        /// <param name="datum"></param>
        public BinaryTableHDU(Header hdr, Data datum)
            : base((TableData)datum)
		{
			myHeader = hdr;
			myData = datum;
			table = (BinaryTable) datum;
        }
    #endregion


        /// <summary>Create data from a binary table header.</summary>
		/// <param name="header">the template specifying the binary table.</param>
		/// <exception cref=""> FitsException if there was a problem with the header.</exception>
		public static Data ManufactureData(Header header)
		{
			return new BinaryTable(header);
		}
		
		internal override Data ManufactureData()
		{
			return ManufactureData(myHeader);
		}
		
		/// <summary>Build a binary table HDU from the supplied data.</summary>
		/// <param name="data">the array used to build the binary table.</param>
        /// <exception cref="FitsException"> If there was a problem with the data.</exception>
		public static Header ManufactureHeader(Data data)
		{
			Header hdr = new Header();
			data.FillHeader(hdr);
			return hdr;
		}
		
		/// <summary>Encapsulate data in a BinaryTable data type</summary>
		public static Data Encapsulate(Object o)
		{
			if(o is ColumnTable)
			{
				return new BinaryTable((ColumnTable) o);
			}
			else if (o is Object[][])
			{
				return new BinaryTable((Object[][]) o);
			}
			else if (o is Object[])
			{
				return new BinaryTable((Object[]) o);
			}
			else
			{
				throw new FitsException("Unable to encapsulate object of type:" + o.GetType().FullName + " as BinaryTable");
			}
		}
		
		/// <summary>Check that this is a valid binary table header.</summary>
		/// <param name="header">to validate.</param>
		/// <returns> <CODE>true</CODE> if this is a binary table header.</returns>
		public static new bool IsHeader(Header header)
		{
			String xten = header.GetStringValue("XTENSION");
			if (xten == null)
			{
				return false;
			}
			xten = xten.Trim();
			if (xten.Equals("BINTABLE") || xten.Equals("A3DTABLE"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
        /// <summary>Check if this data object is consistent with a binary table.  There
		/// are three options:  a column table object, an Object[][], or an Object[].
		/// This routine doesn't check that the dimensions of arrays are properly
		/// consistent.</summary>
		public static bool IsData(Object o)
		{
			if (o is ColumnTable || o is Object[][] || o is Object[])
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		/// <summary>Add a column without any associated header information.</summary>
		/// <param name="data">The column data to be added.  Data should be an Object[] where
		/// type of all of the constituents is identical.  The length
		/// of data should match the other columns.  <b> Note:</b> It is
		/// valid for data to be a 2 or higher dimensionality primitive
		/// array.  In this case the column index is the first 
		/// index of the array.  E.g., if called with int[30][20][10], the
		/// number of rows in the table should be 30 and this column
		/// will have elements which are 2-d integer arrays with TDIM = (10,20).
		/// </param>
		/// <exception cref=""> FitsException the column could not be added.</exception>
		public override int AddColumn(Object data)
		{
			int col = table.AddColumn(data);
			table.PointToColumn(NCols - 1, myHeader);
			return col;
		}
		
        /// <summary>Need to tell header about the Heap before writing.</summary>
		public override void Write(ArrayDataIO ado)
		{
			int oldSize = myHeader.GetIntValue("PCOUNT");
			if (oldSize != table.HeapSize)
			{
				myHeader.AddValue("PCOUNT", table.HeapSize, "Includes Heap");
			}
			
			if (myHeader.GetIntValue("PCOUNT") == 0)
			{
				myHeader.DeleteKey("THEAP");
			}
			else
			{
				myHeader.GetIntValue("TFIELDS");
				int offset = myHeader.GetIntValue("NAXIS1") * myHeader.GetIntValue("NAXIS2") + table.HeapOffset;
				myHeader.AddValue("THEAP", offset, "");
			}
			
			base.Write(ado);
		}
		
		/// <summary>Print out some information about this HDU.</summary>
		public override void Info()
		{
			BinaryTable myData = (BinaryTable) this.myData;

            Console.Out.WriteLine("  Binary Table");
			Console.Out.WriteLine("      Header Information:");
			
			int nhcol = myHeader.GetIntValue("TFIELDS", - 1);
			int nrow = myHeader.GetIntValue("NAXIS2", - 1);
			int rowsize = myHeader.GetIntValue("NAXIS1", - 1);
			
			Console.Out.Write("          " + nhcol + " fields");
			Console.Out.WriteLine(", " + nrow + " rows of length " + rowsize);
			
			for (int i = 1; i <= nhcol; i += 1)
			{
				Console.Out.Write("           " + i + ":");
				PrtField("Name", "TTYPE" + i);
				PrtField("Format", "TFORM" + i);
				PrtField("Dimens", "TDIM" + i);
				Console.Out.WriteLine(" ");
			}
			
			Console.Out.WriteLine("      Data Information:");
			if (myData == null || table.NRows == 0 || table.NCols == 0)
			{
				Console.Out.WriteLine("         No data present");
				if (table.HeapSize > 0)
				{
					Console.Out.WriteLine("         Heap size is: " + table.HeapSize + " bytes");
				}
			}
			else
			{
				Console.Out.WriteLine("          Number of rows=" + table.NRows);
				Console.Out.WriteLine("          Number of columns=" + table.NCols);
				if (table.HeapSize > 0)
				{
					Console.Out.WriteLine("          Heap size is: " + table.HeapSize + " bytes");
				}
				Object[] cols = table.FlatColumns;
				for (int i = 0; i < cols.Length; i += 1)
				{
					Console.Out.WriteLine("           " + i + ":" + ArrayFuncs.ArrayDescription(cols[i]));
				}
			}
		}

        private void PrtField(String type, String field)
        {
	        String val = myHeader.GetStringValue(field);
	        if (val != null)
            {
	            System.Console.Out.WriteLine(type+'='+val+"; ");
	        }
        }

    }
}

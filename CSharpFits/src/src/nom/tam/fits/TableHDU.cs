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
    
    /// <summary>This class allows FITS binary and ASCII tables to
	/// be accessed via a common interface.
    /// </summary>
	
	public abstract class TableHDU:BasicHDU
	{
		/// <summary>Get the number of columns for this table</summary>
		/// <returns> The number of columns in the table.</returns>
		virtual public int NCols
		{
			get
			{
				return table.NCols;
			}
			
		}
		/// <summary>Get/set the number of rows for this table</summary>
		/// <returns> The number of rows in the table.</returns>
		virtual public int NRows
		{
			get
			{
				return table.NRows;
			}
			
		}
		virtual public int CurrentColumn
		{
			set
			{
				myHeader.PositionAfterIndex("TFORM", (value + 1));
			}
			
		}
		
		private TableData table;
		//private int currentColumn;
		
		
		internal TableHDU(TableData td)
		{
			table = td;
		}
		
		public virtual Array GetRow(int row)
		{
			return table.GetRow(row);
		}
		
		public virtual Object GetColumn(String colName)
		{
			return GetColumn(FindColumn(colName));
		}
		
		public virtual Object GetColumn(int col)
		{
			return table.GetColumn(col);
		}
		
		public virtual Object GetElement(int row, int col)
		{
			return table.GetElement(row, col);
		}
		
		public virtual void SetRow(int row, Array newRow)
		{
			table.SetRow(row, newRow);
		}
		
		public virtual void SetColumn(String colName, Object newCol)
		{
			SetColumn(FindColumn(colName), newCol);
		}
		
		public virtual void SetColumn(int col, Object newCol)
		{
			table.SetColumn(col, newCol);
		}
		
		public virtual void SetElement(int row, int col, Object element)
		{
			table.SetElement(row, col, element);
		}
		
		public virtual int AddRow(Array newRow)
		{
			int row = table.AddRow(newRow);
			myHeader.AddValue("NAXIS2", row, null);
			return row;
		}
		
		public virtual int FindColumn(String colName)
		{
			for (int i = 0; i < NCols; i += 1)
			{
				String val = myHeader.GetStringValue("TTYPE" + (i + 1));
				if (val != null && val.Trim().Equals(colName))
				{
					return i;
				}
			}
			return - 1;
		}
		
		public abstract int AddColumn(Object data);

    /// <summary>Get the name of a column in the table.</summary>
		/// <param name="index">The 0-based column index.</param>
		/// <returns> The column name.</returns>
		/// <exception cref=""> FitsException if an invalid index was requested.</exception>
		public virtual String GetColumnName(int index)
		{
			String ttype = myHeader.GetStringValue("TTYPE" + (index + 1));
			if (ttype != null)
			{
				ttype = ttype.Trim();
			}
			return ttype;
		}
		
		public virtual void SetColumnName(int index, String name, String comment)
		{
			if (NCols > index && index >= 0)
			{
				myHeader.PositionAfterIndex("TFORM", index + 1);
				myHeader.AddValue("TTYPE" + (index + 1), name, comment);
			}
		}
		
		/// <summary>Get the FITS type of a column in the table.</summary>
		/// <returns> The FITS type.</returns>
		/// <exception cref=""> FitsException if an invalid index was requested.</exception>
		public virtual String GetColumnFormat(int index)
		{
			int flds = myHeader.GetIntValue("TFIELDS", 0);
			if (index < 0 || index >= flds)
			{
				throw new FitsException("Bad column index " + index + " (only " + flds + " columns)");
			}
			
			return myHeader.GetStringValue("TFORM" + (index + 1)).Trim();
		}

        // suggested in .99.1 version:
        /// <summary>
        /// Remove all rows from the table starting at some specific index from the table.
        /// Inspired by a routine by R. Mathar but re-implemented using the DataTable and
        /// changes to AsciiTable so that it can be done easily for both Binary and ASCII tables.
        /// </summary>
        /// <param name="row">The (0-based) index of the first row to be deleted.</param>
        public virtual void DeleteRows(int row)
        {
		    DeleteRows(row, NRows-row) ;
        }

        // suggested in .99.1 version:
        /// <summary>
        /// Remove a number of adjacent rows from the table.  This routine
        /// was inspired by code by R.Mathar but re-implemented using changes
        /// in the ColumnTable class abd AsciiTable so that we can do
        /// it for all FITS tables.
        /// <param name="firstRow">The (0-based) index of the first row to be deleted.
       	/// This is zero-based indexing: 0<=firstrow< number of rows.</param>
        /// <param name="nRow">The total number of rows to be deleted.</param>
        public virtual void DeleteRows(int firstRow, int nRow)
        {
            // Just ignore invalid requests.
            if (nRow <= 0 || firstRow >= NRows || nRow <= 0)
            {
                return;
            }

            /* correct if more rows are requested than available */
            if (nRow > NRows - firstRow)
            {
                nRow = NRows - firstRow;
            }

            table.DeleteRows(firstRow, nRow);
            myHeader.SetNaxis(2, NRows);
        }

        // suggested in .99.1 version:
        /// <summary>Delete a set of columns from a table.</summary>
        public void DeleteColumnsIndexOne(int column, int len)
        {
	        DeleteColumnsIndexZero(column-1, len);
        }


        // suggested in .99.1 version:
        /// <summary>Delete a set of columns from a table.</summary>
        public void DeleteColumnsIndexZero(int column, int len)
        {
	        DeleteColumnsIndexZero(column, len, ColumnKeyStems);
        }

        // suggested in .99.1 version:
        /// <summary>Delete a set of columns from a table.</summary>
        /// <param name="column">The one-indexed start column.</param>
        /// <param name="len">The number of columns to delete.</param>
        /// <param name="fields">Stems for the header fields to be removed for the table.</param>
        public void DeleteColumnsIndexOne(int column, int len, String[] fields)
        {
	        DeleteColumnsIndexZero(column-1, len, fields);
        }

        // suggested in .99.1 version:
        /// <summary>Delete a set of columns from a table.</summary>
        /// <param name="column">The zero-indexed start column.</param>
        /// <param name="len">The number of columns to delete.</param>
        /// <param name="fields">Stems for the header fields to be removed for the table.</param>
        public void DeleteColumnsIndexZero(int column, int len, String[] fields)
        {
        	
	        if (column < 0 || len < 0 || column+len > NCols)
            {
	            throw new FitsException("Illegal columns deletion request- Start:"+column+" Len:"+len+" from table with "+NCols+ " columns");
	        }
        	
	        if (len == 0)
            {
	            return;
	        }
        	
	        int ncol = NCols;
	        table.DeleteColumns(column, len);
        	
        	
	        // Get rid of the keywords for the deleted columns
	        for (int col=column; col<column+len; col += 1)
            {
	            for (int fld=0; fld<fields.Length; fld += 1)
                {
		            String key = fields[fld] + (col+1);
		            myHeader.DeleteKey(key);
	            }
	        }
        	
	        // Shift the keywords for the columns after the deleted columns
	        for (int col=column+len ; col<ncol ; col += 1)
            {
	            for (int fld=0; fld<fields.Length; fld += 1)
                {
		            String oldKey = fields[fld] + (col+1);
		            String newKey = fields[fld] + (col+1-len);
		            if (myHeader.ContainsKey(oldKey))
                    {
		                myHeader.ReplaceKey(oldKey, newKey);
		            }
	            }
	        }
	        // Update the number of fields.
	        myHeader.AddValue("TFIELDS", NCols, "Number of table fields");
        	
	        // Give the data sections a chance to update the header too.
	        table.UpdateAfterDelete(ncol, myHeader);
        }

        // suggested in .99.1 version:
        /// <summary>
        /// Get the stems of the keywords that are associated 
        /// with table columns. Users can supplement this 
        /// with their own and call the appropriate deleteColumns fields.
        /// </summary>
        public abstract String[] ColumnKeyStems
        {
            get;
        }
        
	}
}

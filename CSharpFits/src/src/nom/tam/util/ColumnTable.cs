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
    /// <summary>A data table is conventionally considered to consist of rows and
	/// columns, where the structure within each column is constant, but
	/// different columns may have different structures.  I.e., structurally
	/// columns may differ but rows are identical.
	/// Typically tabular data is usually stored in row order which can
	/// make it extremely difficult to access efficiently using Java.
	/// This class provides efficient
	/// access to data which is stored in row order and allows users to
	/// get and set the elements of the table.
	/// The table can consist only of arrays of primitive types.
	/// Data stored in column order can
	/// be efficiently read and written using the
	/// BufferedDataXputStream classes.
	/// *
	/// The table is represented entirely as a set of one-dimensional primitive
	/// arrays.  For a given column, a row consists of some number of
	/// contiguous elements of the array.  Each column is required to have
	/// the same number of rows.
    /// </summary>
	
	public class ColumnTable : DataTable
	{
    #region Properties
		/// <summary>Get the number of rows in the table.</summary>
		virtual public int NRows
		{
			get
			{
				return nrow;
			}
		}

    /// <summary>Get the number of columns in the table.</summary>
		virtual public int NCols
		{
			get
			{
				return arrays.Length;
			}
		}

    /// <summary>Get the base classes of the columns.</summary>
		/// <returns>An array of Class objects, one for each column.</returns>
		virtual public Type[] Bases
		{
			get
			{
				return bases;
			}
		}
/*
    /// <summary>Get the characters describing the base classes of the columns.
		/// </summary>
		/// <returns> An array of char's, one for each column.
		/// 
		/// </returns>
		virtual public char[] Types
		{
			get
			{
				return types;
			}
		}
    */
		/// <summary>Get the actual data arrays</summary>
		virtual public Object[] Columns
		{
			get
			{
				return arrays;
			}
		}

        virtual public int[] Sizes
		{
			get
			{
				return sizes;
			}
		}
    #endregion		

    #region Instance Variables
		/// <summary>The columns to be read/written</summary>
		private Object[] arrays;
		
		/// <summary>The number of elements in a row for each column</summary>
		private int[] sizes;
		
		/// <summary>The number of rows</summary>
		private int nrow;
		
		/// <summary>The number or rows to read/write in one I/O.</summary>
		private int chunk;
		
		/// <summary>The size of a row in bytes</summary>
		private int rowSize;
		
		/// <summary>The base type of each row (using the second character
		/// of the [x class names of the arrays.</summary>
		//private char[] types;
		private Type[] bases;
		
		// The following arrays are used to avoid having to check casts during the I/O loops.
		// They point to elements of arrays.
		private byte[][] bytePointers;
		private short[][] shortPointers;
		private int[][] intPointers;
		private long[][] longPointers;
		private float[][] floatPointers;
		private double[][] doublePointers;
		private char[][] charPointers;
		private bool[][] booleanPointers;
    #endregion

		/// <summary>Create the object after checking consistency.</summary>
		/// <param name="arrays"> An array of one-d primitive arrays representing columns.</param>
		/// <param name="sizes">  The number of elements in each row
		/// for the corresponding column</param>
		public ColumnTable(Object[] arrays, int[] sizes)
		{
			Setup(arrays, sizes);
		}
		
		/// <summary>Actually perform the initialization.</summary>
		protected internal virtual void Setup(Object[] arrays, int[] sizes)
		{
			CheckArrayConsistency(arrays, sizes);
			GetNumberOfRows();
			InitializePointers();
		}

		/// <summary>Get a particular column.</summary>
		/// <param name="col">The column desired.</param>
		/// <returns> an object containing the column data desired.
		/// This will be an instance of a 1-d primitive array.</returns>
		public virtual Object GetColumn(int col)
		{
			return arrays[col];
		}
		
		/// <summary>Set the values in a particular column.
		/// The new values must match the old in length but not necessarily in type.
		/// </summary>
		/// <param name="col">The column to modify.</param>
		/// <param name="newColumn">The new column data.  This should be a primitive array.</param>
		/// <exception cref=""> TableException Thrown when the new data is not commenserable with
		/// informaiton in the table.</exception>
		public virtual void SetColumn(int col, Object newColumn)
		{
            // suggested in .99.2 version:
            bool reset = newColumn.GetType() != arrays[col].GetType() ||
                        ((Array)newColumn).Length != ((Array) arrays[col]).Length;
			arrays[col] = newColumn;
			if(reset)
			{
				Setup(arrays, sizes);
			}
            else
            {
                // This is required, because otherwise the typed pointer may point to the old
                // array, which has been replaced by newColumn. Added by Jeroen de Jong, 1 Aug 2006
                InitializePointers();
            }
		}
		
		/// <summary>Add a column.</summary>
		public virtual void AddColumn(Array newColumn, int size)
		{
			nrow = CheckColumnConsistency(newColumn, nrow, size);
			
			rowSize += nrow * ArrayFuncs.GetBaseLength(newColumn);
			
			GetNumberOfRows();
			
			int ncol = arrays.Length;
			
			Object[] newArrays = new Object[ncol + 1];
			int[] newSizes = new int[ncol + 1];
			Type[] newBases = new Type[ncol + 1];
//			char[] newTypes = new char[ncol + 1];
			
			Array.Copy(arrays, 0, newArrays, 0, ncol);
			Array.Copy(sizes, 0, newSizes, 0, ncol);
			Array.Copy(bases, 0, newBases, 0, ncol);
//			Array.Copy(types, 0, newTypes, 0, ncol);
			
			arrays = newArrays;
			sizes = newSizes;
			bases = newBases;
//			types = newTypes;
			
			arrays[ncol] = newColumn;
			sizes[ncol] = size;
			bases[ncol] = ArrayFuncs.GetBaseClass(newColumn);
//			types[ncol] = classname[1];
			AddPointer(newColumn);
		}
		
		/// <summary>Add a row to the table.  This method is very inefficient
		/// for adding multiple rows and should be avoided if possible.
		/// </summary>
        /// <exception cref="TableException"> TableException Thrown when the new data
        /// is not of the same type as the data it replaces.</exception>
		public virtual void AddRow(Object[] row)
		{
			if(arrays.Length == 0)
			{
				for (int i = 0; i < row.Length; i += 1)
				{
					AddColumn((Array)row[i], ((Array)row[i]).Length);
				}
			}
			else
			{
				if (row.Length != arrays.Length)
				{
					throw new TableException("Row length mismatch");
				}
				
				for (int i = 0; i < row.Length; i += 1)
				{
					if (row[i].GetType() != arrays[i].GetType() || ((Array) row[i]).Length != sizes[i])
					{
						throw new TableException("Row column mismatch at column:" + i);
					}
					Object xarray = ArrayFuncs.NewInstance(bases[i], (nrow + 1) * sizes[i]);
					Array.Copy((Array)arrays[i], 0, (Array)xarray, 0, nrow * sizes[i]);
					Array.Copy((Array)row[i], 0, (Array)xarray, nrow * sizes[i], sizes[i]);
					arrays[i] = xarray;
				}
				InitializePointers();
				nrow += 1;
			}
		}
		
		/// <summary>Get a element of the table.</summary>
		/// <param name="row">The row desired.</param>
		/// <param name="col">The column desired.</param>
		/// <returns> A primitive array containing the information.  Note
		/// that an array will be returned even if the element
		/// is a scalar.</returns>
		public virtual Object GetElement(int row, int col)
		{
			Array x = ArrayFuncs.NewInstance(bases[col], sizes[col]);
			Array.Copy((Array)arrays[col], sizes[col] * row, x, 0, sizes[col]);
			return x;
		}
		
		/// <summary>Modify an element of the table.</summary>
		/// <param name="row">The row containing the element.</param>
		/// <param name="col">The column containing the element.</param>
		/// <param name="x">  The new datum.  This should be 1-d primitive array.</param>
        /// <exception cref="TableException"> TableException Thrown when the new data
		/// is not of the same type as the data it replaces.</exception>
		public virtual void SetElement(int row, int col, Object x)
		{
			//String classname = x.GetType().FullName;
			
			//if(!classname.Equals("[" + types[col]))
            if(!(ArrayFuncs.CountDimensions(x) == 1 &&
                bases[col].Equals(ArrayFuncs.GetBaseClass(x))))
			{
				throw new TableException("setElement: Incompatible element type");
			}
			
			if(((Array)x).Length != sizes[col])
			{
				throw new TableException("setElement: Incompatible element size");
			}
			
			Array.Copy((Array)x, 0, (Array)arrays[col], sizes[col] * row, sizes[col]);
		}
		
		/// <summary>Get a row of data.</summary>
		/// <param name="The">row desired.</param>
		/// <returns> An array of objects each containing a primitive array.</returns>
		public virtual Object GetRow(int row)
		{
			Object[] x = new Object[arrays.Length];
			for (int col = 0; col < arrays.Length; col += 1)
			{
				x[col] = GetElement(row, col);
			}
			return x;
		}
		
		/// <summary>Modify a row of data.</summary>
		/// <param name="row">The row to be modified.</param>
		/// <param name="x">  The data to be modified.  This should be an
		/// array of objects.  It is described as an Object
		/// here since other table implementations may
		/// use other methods to store the data (e.g.,</param>
        /// <exception cref="TableException"> TableException Thrown when the new data
        /// is not of the same type as the data it replaces.</exception>
        /// <seealso cref="">ColumnTable.getColumn.</seealso>
		public virtual void SetRow(int row, Object x)
		{
            if (!(x is Object[])) // ArrayFuncs.CountDimensions(x) != 1
			{
				throw new TableException("setRow: Incompatible row");
			}
			
			for (int col = 0; col < arrays.Length; col += 1)
			{
				SetElement(row, col, ((Array)x).GetValue(col));
			}
		}
		
		/// <summary>Check that the columns and sizes are consistent.
		/// Inconsistencies include:
		/// * arrays and sizes have different lengths.
		/// * an element of arrays is not a primitive array.
		/// * the size of an array is not divisible by the sizes entry.
		/// * the number of rows differs for the columns.
		/// </summary>
		/// <param name="arrays">The arrays defining the columns.</param>
		/// <param name="sizes">The number of elements in each row for the column.</param>
        /// <exception cref="TableException"> TableException Thrown when the new data
        /// is not of the same type as the data it replaces.</exception>
        protected internal virtual void CheckArrayConsistency(Object[] arrays, int[] sizes)
		{
			// This routine throws an error if it detects an inconsistency
			// between the arrays being read in.
			
			// First check that the lengths of the two arrays are the same.
			if(arrays.Length != sizes.Length)
			{
				throw new TableException("readArraysAsColumns: Incompatible arrays and sizes.");
			}
			
			// Now check that that we fill up all of the arrays exactly.
			int ratio = 0;
			int rowSize = 0;
			
			//this.types = new char[arrays.Length];
			this.bases = new Type[arrays.Length];
			
			// Check for a null table.
//			bool nullTable = true;
			
			for(int i = 0; i < arrays.Length; i += 1)
			{
				ratio = CheckColumnConsistency((Array)arrays[i], ratio, sizes[i]);

				rowSize += sizes[i] * ArrayFuncs.GetBaseLength(arrays[i]);
				//types[i] = classname[1];
				bases[i] = ArrayFuncs.GetBaseClass(arrays[i]);
			}
			
			this.nrow = ratio;
			this.rowSize = rowSize;
			this.arrays = arrays;
			this.sizes = sizes;
		}
		
		private int CheckColumnConsistency(Array data, int ratio, int size)
		{
			//if (classname[0] != '[' || classname.Length != 2)
            if(ArrayFuncs.CountDimensions(data) != 1 || !ArrayFuncs.GetBaseClass(data).IsPrimitive)
			{
				throw new TableException("Non-primitive array for column");
			}
			
			int thisSize = data.Length;

            // suggested in .99.2 version: change 
            if (thisSize == 0 && size != 0 && ratio != 0 || thisSize != 0 && size == 0)
			{
				throw new TableException("Size mismatch in column");
			}

			// The row size must evenly divide the size of the array.
            if (size != 0 && thisSize % size != 0)
			{
				throw new TableException("Row size does not divide array for column");
			}
			
			// Finally the ratio of sizes must be the same for all columns -- this
			// is the number of rows in the table.
			int thisRatio = 0;
			if (size > 0)
			{
				thisRatio = thisSize / size;
				
				if (ratio != 0 && (thisRatio != ratio))
				{
					throw new TableException("Different number of rows in different columns");
				}
			}
			if (thisRatio > 0)
			{
				return thisRatio;
			}
			else
			{
				return ratio;
			}
		}
		
		/// <summary>Calculate the number of rows to read/write at a time.</summary>
		protected internal virtual void GetNumberOfRows()
		{
			int bufSize = 65536;
			
			// If a row is larger than bufSize, then read one row at a time.
			if(rowSize == 0)
			{
				this.chunk = 0;
			}
			else if(rowSize > bufSize)
			{
				this.chunk = 1;
				
				// If the entire set is not too big, just read it all.
			}
			else if(bufSize / rowSize >= nrow)
			{
				this.chunk = nrow;
			}
			else
			{
				this.chunk = bufSize / rowSize + 1;
			}
		}
		
		/// <summary>Set the pointer arrays for the eight primitive types
		/// to point to the appropriate elements of arrays.</summary>
		protected internal virtual void InitializePointers()
		{
			int nbyte, nshort, nint, nlong, nfloat, ndouble, nchar, nboolean;

			// Count how many of each type we have.
			nbyte = 0; nshort = 0; nint = 0; nlong = 0;
			nfloat = 0; ndouble = 0; nchar = 0; nboolean = 0;

			for(int col = 0; col < arrays.Length; col += 1)
			{
                Type t = bases[col];

                if(typeof(byte).Equals(t))// || typeof(sbyte).Equals(t))
                {
                  ++nbyte;
                }
                else if(typeof(bool).Equals(t))
                {
                  ++nboolean;
                }
                else if(typeof(char).Equals(t))
                {
                  ++nchar;
                }
                else if(typeof(short).Equals(t))
                {
                  ++nshort;
                }
                else if(typeof(int).Equals(t))
                {
                  ++nint;
                }
                else if(typeof(long).Equals(t))
                {
                  ++nlong;
                }
                else if(typeof(float).Equals(t))
                {
                  ++nfloat;
                }
                else if(typeof(double).Equals(t))
                {
                  ++ndouble;
                }
                else
                {
                    throw new Exception("None of the type is matching to the standard ones.");
                }
                #region old crap
                /*
                switch (types[col])
				        {
					        case 'B': 
						        nbyte += 1;
						        break;
					        case 'S': 
						        nshort += 1;
						        break;
					        case 'I': 
						        nint += 1;
						        break;
					        case 'J': 
						        nlong += 1;
						        break;
					        case 'F': 
						        nfloat += 1;
						        break;
					        case 'D': 
						        ndouble += 1;
						        break;
					        case 'C': 
						        nchar += 1;
						        break;
					        case 'Z': 
						        nboolean += 1;
						        break;
					        }
                  */
                #endregion
			}
			
			// Allocate the pointer arrays.  Note that many will be
			// zero-length.
			
			//bytePointers = new sbyte[nbyte][];
            bytePointers = new byte[nbyte][];
			shortPointers = new short[nshort][];
			intPointers = new int[nint][];
			longPointers = new long[nlong][];
			floatPointers = new float[nfloat][];
			doublePointers = new double[ndouble][];
			charPointers = new char[nchar][];
			booleanPointers = new bool[nboolean][];
			
			// Now set the pointers.
			nbyte = 0; nshort = 0; nint = 0; nlong = 0;
			nfloat = 0; ndouble = 0; nchar = 0; nboolean = 0;
			
			for(int col = 0; col < arrays.Length; col += 1)
			{
                Type t = bases[col];

                if(typeof(byte).Equals(t))
                {
                  bytePointers[nbyte] = (byte[])arrays[col];
                  ++nbyte;
                }
                else if(typeof(bool).Equals(t))
                {
                  booleanPointers[nboolean] = (bool[])arrays[col];
                  ++nboolean;
                }
                else if(typeof(char).Equals(t))
                {
                  charPointers[nchar] = (char[])arrays[col];
                  ++nchar;
                }
                else if(typeof(short).Equals(t))
                {
                  shortPointers[nshort] = (short[])arrays[col];
                  ++nshort;
                }
                else if(typeof(int).Equals(t))
                {
                  intPointers[nint] = (int[])arrays[col];
                  ++nint;
                }
                else if(typeof(long).Equals(t))
                {
                  longPointers[nlong] = (long[])arrays[col];
                  ++nlong;
                }
                else if(typeof(float).Equals(t))
                {
                  floatPointers[nfloat] = (float[])arrays[col];
                  ++nfloat;
                }
                else if(typeof(double).Equals(t))
                {
                  doublePointers[ndouble] = (double[])arrays[col];
                  ++ndouble;
                }

                #region other old crap
                /*
                switch (types[col])
				        {
					        case 'B': 
						        bytePointers[nbyte] = (sbyte[]) arrays[col];
						        nbyte += 1;
						        break;
					        case 'S': 
						        shortPointers[nshort] = (short[]) arrays[col];
						        nshort += 1;
						        break;
					        case 'I': 
						        intPointers[nint] = (int[]) arrays[col];
						        nint += 1;
						        break;
					        case 'J': 
						        longPointers[nlong] = (long[]) arrays[col];
						        nlong += 1;
						        break;
					        case 'F': 
						        floatPointers[nfloat] = (float[]) arrays[col];
						        nfloat += 1;
						        break;
					        case 'D': 
						        doublePointers[ndouble] = (double[]) arrays[col];
						        ndouble += 1;
						        break;
					        case 'C': 
						        charPointers[nchar] = (char[]) arrays[col];
						        nchar += 1;
						        break;
					        case 'Z': 
						        booleanPointers[nboolean] = (bool[]) arrays[col];
						        nboolean += 1;
						        break;
				        }
                  */
                #endregion
			}
		}
		
		/// <summary>
        ///  Add a pointer in the pointer lists.
		/// </summary>
		/// <param name="data"></param>

		protected internal virtual void AddPointer(Object data)
		{
            // String classname = data.GetType().FullName;
            // char type = classname[1];
	        Type t = data.GetType();

            // Get the base Type, removing the dimension brackets
            if (t.ToString().IndexOf("[") != -1)
                t = Type.GetType( t.ToString().Substring(0, t.ToString().IndexOf("[")) );

              if(typeof(byte).Equals(t))
              {
                byte[][] xb = new byte[bytePointers.Length + 1][];
                Array.Copy(bytePointers, 0, xb, 0, bytePointers.Length);
                xb[bytePointers.Length] = (byte[]) data;
                bytePointers = xb;
              }
              else if(typeof(bool).Equals(t))
              {
                bool[][] xb = new bool[booleanPointers.Length + 1][];
                Array.Copy(booleanPointers, 0, xb, 0, booleanPointers.Length);
                xb[booleanPointers.Length] = (bool[]) data;
                booleanPointers = xb;
              }
              else if(typeof(char).Equals(t))
              {
                char[][] xb = new char[charPointers.Length + 1][];
                Array.Copy(charPointers, 0, xb, 0, charPointers.Length);
                xb[charPointers.Length] = (char[]) data;
                charPointers = xb;
              }
              else if(typeof(short).Equals(t))
              {
                short[][] xb = new short[shortPointers.Length + 1][];
                Array.Copy(shortPointers, 0, xb, 0, shortPointers.Length);
                xb[shortPointers.Length] = (short[]) data;
                shortPointers = xb;
              }
              else if(typeof(int).Equals(t))
              {
                int[][] xb = new int[intPointers.Length + 1][];
                Array.Copy(intPointers, 0, xb, 0, intPointers.Length);
                xb[intPointers.Length] = (int[]) data;
                intPointers = xb;
              }
              else if(typeof(long).Equals(t))
              {
                long[][] xb = new long[longPointers.Length + 1][];
                Array.Copy(longPointers, 0, xb, 0, longPointers.Length);
                xb[longPointers.Length] = (long[]) data;
                longPointers = xb;
              }
              else if(typeof(float).Equals(t))
              {
                float[][] xb = new float[floatPointers.Length + 1][];
                Array.Copy(floatPointers, 0, xb, 0, floatPointers.Length);
                xb[floatPointers.Length] = (float[]) data;
                floatPointers = xb;
              }
              else if(typeof(double).Equals(t))
              {
                double[][] xb = new double[doublePointers.Length + 1][];
                Array.Copy(doublePointers, 0, xb, 0, doublePointers.Length);
                xb[doublePointers.Length] = (double[]) data;
                doublePointers = xb;
              }
              else
              {
                throw new TableException("Invalid type for added column:" + t.FullName);//classname);
              }

      #region more old crap
/*
			switch (type)
			{
				case 'B':  {
						sbyte[][] xb = new sbyte[bytePointers.Length + 1][];
						Array.Copy(SupportClass.ToByteArray(bytePointers), 0, SupportClass.ToByteArray(xb), 0, bytePointers.Length);
						xb[bytePointers.Length] = (sbyte[]) data;
						bytePointers = xb;
						break;
					}
				
				case 'Z':  {
						bool[][] xb = new bool[booleanPointers.Length + 1][];
						Array.Copy(booleanPointers, 0, xb, 0, booleanPointers.Length);
						xb[booleanPointers.Length] = (bool[]) data;
						booleanPointers = xb;
						break;
					}
				
				case 'S':  {
						short[][] xb = new short[shortPointers.Length + 1][];
						Array.Copy(shortPointers, 0, xb, 0, shortPointers.Length);
						xb[shortPointers.Length] = (short[]) data;
						shortPointers = xb;
						break;
					}
				
				case 'C':  {
						char[][] xb = new char[charPointers.Length + 1][];
						Array.Copy(charPointers, 0, xb, 0, charPointers.Length);
						xb[charPointers.Length] = (char[]) data;
						charPointers = xb;
						break;
					}
				
				case 'I':  {
						int[][] xb = new int[intPointers.Length + 1][];
						Array.Copy(intPointers, 0, xb, 0, intPointers.Length);
						xb[intPointers.Length] = (int[]) data;
						intPointers = xb;
						break;
					}
				
				case 'J':  {
						long[][] xb = new long[longPointers.Length + 1][];
						Array.Copy(longPointers, 0, xb, 0, longPointers.Length);
						xb[longPointers.Length] = (long[]) data;
						longPointers = xb;
						break;
					}
				
				case 'F':  {
						float[][] xb = new float[floatPointers.Length + 1][];
						Array.Copy(floatPointers, 0, xb, 0, floatPointers.Length);
						xb[floatPointers.Length] = (float[]) data;
						floatPointers = xb;
						break;
					}
				
				case 'D':  {
						double[][] xb = new double[doublePointers.Length + 1][];
						Array.Copy(doublePointers, 0, xb, 0, doublePointers.Length);
						xb[doublePointers.Length] = (double[]) data;
						doublePointers = xb;
						break;
					}
				
				default: 
					throw new TableException("Invalid type for added column:" + classname);
				
			}
      */
      #endregion
		}

		/// <summary>Read a table.</summary>
		/// <param name="is">The input stream to read from.</param>
		public virtual int Read(ArrayDataIO is_Renamed)
		{
			// While we have not finished reading the table..
			for (int row = 0; row < nrow; row += 1)
			{
				int ibyte = 0;
				int ishort = 0;
				int iint = 0;
				int ilong = 0;
				int ichar = 0;
				int ifloat = 0;
				int idouble = 0;
				int iboolean = 0;
				
				// Loop over the columns within the row.
				for (int col = 0; col < arrays.Length; col += 1)
				{
					int arrOffset = sizes[col] * row;
					int size = sizes[col];
					
                      Type t = bases[col];
                      if(typeof(int).Equals(t))
                      {
                        int[] ia = intPointers[iint];
                        iint += 1;
                        is_Renamed.Read(ia, arrOffset, size);
                      }
                      else if(typeof(short).Equals(t))
                      {
                        short[] s = shortPointers[ishort];
                        ishort += 1;
                        is_Renamed.Read(s, arrOffset, size);
                      }
                      else if(typeof(byte).Equals(t))
                      {
                        byte[] b = bytePointers[ibyte];
                        ibyte += 1;
                        is_Renamed.Read(b, arrOffset, size);
                      }
                      else if(typeof(float).Equals(t))
                      {
                        float[] f = floatPointers[ifloat];
                        ifloat += 1;
                        is_Renamed.Read(f, arrOffset, size);
                      }
                      else if(typeof(double).Equals(t))
                      {
                        double[] d = doublePointers[idouble];
                        idouble += 1;
                        is_Renamed.Read(d, arrOffset, size);
                      }
                      else if(typeof(char).Equals(t))
                      {
                        char[] c = charPointers[ichar];
                        ichar += 1;
                        is_Renamed.Read(c, arrOffset, size);
                      }
                      else if(typeof(long).Equals(t))
                      {
                        long[] l = longPointers[ilong];
                        ilong += 1;
                        is_Renamed.Read(l, arrOffset, size);
                      }
                      else if(typeof(bool).Equals(t))
                      {
                        bool[] bool_Renamed = booleanPointers[iboolean];
                        iboolean += 1;
                        is_Renamed.Read(bool_Renamed, arrOffset, size);
                      }
				}
			}
			
			// All done if we get here...
			return rowSize * nrow;
		}
		
		/// <summary>Write a table.</summary>
		/// <param name="os">the output stream to write to.</param>
		public virtual int Write(ArrayDataIO os)
		{
			if (rowSize == 0)
			{
				return 0;
			}
			
      for (int row = 0; row < nrow; row += 1)
			{
        int ibyte = 0;
        int ishort = 0;
        int iint = 0;
        int ilong = 0;
        int ichar = 0;
        int ifloat = 0;
        int idouble = 0;
        int iboolean = 0;
				
        // Loop over the columns within the row.
				for (int col = 0; col < arrays.Length; col += 1)
				{
					int arrOffset = sizes[col] * row;
					int size = sizes[col];
					
          Type t = bases[col];
          if(typeof(int).Equals(t))
          {
            int[] ia = intPointers[iint];
            iint += 1;
            os.Write(ia, arrOffset, size);
          }
          else if(typeof(short).Equals(t))
          {
            short[] s = shortPointers[ishort];
            ishort += 1;
            os.Write(s, arrOffset, size);
          }
          else if(typeof(byte).Equals(t))
          {
            byte[] b = bytePointers[ibyte];
            ibyte += 1;
            os.Write(b, arrOffset, size);
          }
          else if(typeof(float).Equals(t))
          {
            float[] f = floatPointers[ifloat];
            ifloat += 1;
            os.Write(f, arrOffset, size);
          }
          else if(typeof(double).Equals(t))
          {
            double[] d = doublePointers[idouble];
            idouble += 1;
            os.Write(d, arrOffset, size);
          }
          else if(typeof(char).Equals(t))
          {
            char[] c = charPointers[ichar];
            ichar += 1;
            os.Write(c, arrOffset, size);
          }
          else if(typeof(long).Equals(t))
          {
            long[] l = longPointers[ilong];
            ilong += 1;
            os.Write(l, arrOffset, size);
          }
          else if(typeof(bool).Equals(t))
          {
            bool[] bool_Renamed = booleanPointers[iboolean];
            iboolean += 1;
            os.Write(bool_Renamed, arrOffset, size);
          }
				}
			}
			
			// All done if we get here...
			return rowSize * nrow;
		}

        // suggested in .99.1 version:
        /// <summary>Delete a row from the table.</summary>
        /// <param name="row">The row (0-indexed) to be deleted.</param>
        public void DeleteRow(int row)
        {
	        DeleteRows(row, 1);
        }

        // suggested in .99.1 version:
        /// Delete a contiguous set of rows from the table.
        /// <param name="row">The row (0-indexed) to be deleted.</param>
        /// <param name="length">The number of rows to be deleted.</param>
        /// <exception cref="TableException"> TableException Thrown when the new data
        /// is not of the same type as the data it replaces.</exception>
        public void DeleteRows(int row, int length)
        {
        	
	        if (row < 0  || length < 0 || row+length > nrow)
            {
	            throw new TableException("Invalid request to delete rows start: "+row+" length:"+length+
				             " for table with "+nrow+" rows.");
	        }
        	
	        if (length == 0)
            {
	            return;
	        }
        	
	        for (int col=0; col<arrays.Length; col += 1)
            {
	            int sz = sizes[col];
	            int newSize = sz*(nrow-length);
	            Object newArr  = ArrayFuncs.NewInstance(bases[col], newSize);
        	    
	            // Copy whatever comes before the deletion
                Array.Copy((System.Array)arrays[col], 0, (System.Array)newArr, 0, row * sz);
        	    
	            // Copy whatever comes after the deletion
                Array.Copy((System.Array)arrays[col], (row + length) * sz, 
                            (System.Array)newArr, row * sz, (nrow - row - length) * sz);
	            arrays[col] = newArr;
	        }
	        nrow -= length;
	        InitializePointers();
        }

        // suggested in .99.1 version:
        /// Delete a contiguous set of columns from the table.
        /// <param name="start">The column (0-indexed) to be deleted.</param>
        /// <param name="len">The number of rows to be deleted.</param>
        /// <exception cref="TableException"> TableException Thrown when the new data
        /// is not of the same type as the data it replaces.</exception>
        public int DeleteColumns(int start, int len)
        {
        	
	        int ncol = arrays.Length;
        	
	        if (start < 0  || len < 0 || start+len > ncol)
            {
	            throw new TableException("Invalid request to delete columns start: "+start+" length:"+len+
				             " for table with "+ncol+" columns.");
	        }
        	
	        if (len == 0)
            {
	            return rowSize;
	        }
        	
	        for (int i=start; i<start+len; i += 1)
            {
	             rowSize -= sizes[i]*ArrayFuncs.GetBaseLength(arrays[i]);
	        }
        	    
	        int ocol = ncol;
	        ncol -= len;
        	
	        Object[] newArrays  = new Object[ncol];
            int[] newSizes      = new int   [ncol];
	        Type[] newBases     = new Type  [ncol];
	        char[]  newTypes    = new char  [ncol];
        	
	        Array.Copy(arrays, 0, newArrays, 0, start);
            Array.Copy(sizes, 0, newSizes, 0, start);
            Array.Copy(bases, 0, newBases, 0, start);
        //    Array.Copy(types, 0, newTypes, 0, start);
        	
	        int rem = ocol-(start+len);

            Array.Copy(arrays, start + len, newArrays, start, rem);
            Array.Copy(sizes, start + len, newSizes, start, rem);
            Array.Copy(bases, start + len, newBases, start, rem);
        //    Array.Copy(types, start + len, newTypes, start, rem);
        	
        	
	        arrays = newArrays;
	        sizes  = newSizes;
	        bases  = newBases;
	    //    types  = newTypes;
        	
	        InitializePointers();
	        return rowSize;
        }

	}
}

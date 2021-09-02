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
    using System.IO;
	using nom.tam.util;
    /// <summary>This class provides methods to access the data segment of an HDU.</summary>
	
	public abstract class Data : FitsElement
	{
    #region Properties
        /// <summary>
        /// Returns whether input stream is rewritable
        /// </summary>
        public virtual bool Rewriteable
        {
          get
          {
            if(input == null || fileOffset < 0 || (TrueSize + 2879) / 2880 != (dataSize + 2879) / 2880)
            {
              return false;
            }
            else
            {
              return true;
            }
          }
        }
    		
        /// <summary>Get the file offset</summary>
		virtual public long FileOffset
		{
			get
			{
				return fileOffset;
			}
        }

		internal abstract int TrueSize{get;}
		/// <summary>Get the size of the data element in bytes</summary>
		virtual public long Size
		{
			get
			{
				return FitsUtil.AddPadding(TrueSize);
			}
		}

        /// <summary>Return the data array object.</summary>
		public abstract Object DataArray{get;}

		/// <summary>Return the non-FITS data object</summary>
		virtual public Object Kernel
		{
			get
			{
				return DataArray;
			}
		}
    #endregion

		/*
		 <summary>This is the object which contains the actual data for the HDU.
		 <ul>
		 <li> For images and primary data this is a simple (but possibly
		 multi-dimensional) primitive array.  When group data is
		 supported it will be a possibly multidimensional array
		 of group objects.
		 <li> For ASCII data it is a two dimensional Object array where
		 each of the constituent objects is a primitive array of length 1.
		 <li> For Binary data it is a two dimensional Object array where
		 each of the constituent objects is a primitive array of arbitrary
		 (more or less) dimensionality.
		 </ul>
		 </summary>
		*/

    #region Instance Variables
		/// <summary>The starting location of the data when last read</summary>
		protected internal long fileOffset = - 1;
		/// <summary>The size of the data when last read</summary>
		protected internal int dataSize;
		/// <summary>The inputstream used.</summary>
		protected internal RandomAccess input;
    #endregion

        protected void SetFileOffset(Object o)
        {
          if(o is RandomAccess)
          {
            fileOffset = FitsUtil.FindOffset(o);
            dataSize = TrueSize;
            input = (RandomAccess)o;
          }
        }

		/// <summary>Write the data -- including any buffering needed</summary>
		/// <param name="o"> The output stream on which to write the data.</param>
		public abstract void Write(ArrayDataIO o);
		
		/// <summary>Read a data array into the current object and if needed position
		/// to the beginning of the next FITS block.</summary>
		/// <param name="i">The input data stream</param>
		public abstract void Read(ArrayDataIO i);
		
		public virtual void Rewrite()
		{
			if(this.Rewriteable)
			{
				throw new FitsException("Illegal attempt to rewrite data");
			}
			
			//FitsUtil.Reposition(input, fileOffset);
            input.Seek(fileOffset, SeekOrigin.Begin);
			Write(input);
			try
			{
				input.Flush();
			}
			catch(IOException e)
			{
				throw new FitsException("Error in Rewrite Flush: " + e);
			}
		}
		
		/// <summary>Modify a header to point to this data</summary>
		internal abstract void FillHeader(Header head);
	}
}

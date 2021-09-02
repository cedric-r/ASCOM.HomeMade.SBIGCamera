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
    using System.IO;
	using nom.tam.util;
    /// <summary>This class supports the FITS heap.  This
	/// is currently used for variable length columns
    /// in binary tables.</summary>
	
	public class FitsHeap : FitsElement
	{
    #region Properties
        /// <summary>
        /// Returns if stream is rewritable
        /// </summary>
        public virtual bool Rewriteable
        {
          get
          {
              // change suggested in .99.1 version: 
              return ((fileOffset >= 0) && (input is ArrayDataIO) && (!expanded));
          }
        }

        /// <summary>Get the current offset within the heap.</summary>
		virtual public int Offset
		{
			get
			{
				return heapOffset;
			}
			
		}

        /// <summary>Return the size of the heap using the more bean compatbile format.</summary>
        virtual public long Size
		{
			get
			{
				return GetSize();
			}
		}

        /// <summary>Get the file offset of the heap.</summary>
        virtual public long FileOffset
		{
			get
			{
				return fileOffset;
			}
		}
    #endregion

    #region Instance Variables
    /// <summary>The storage buffer</summary>
		private byte[] heap;
		
		/// <summary>The current used size of the buffer <= heap.length</summary>
		private int heapSize;
		
		/// <summary>The offset within a file where the heap begins</summary>
		private long fileOffset = - 1;

        // change suggested in .99.1 version: 
        /// <summary> Has the heap ever been expanded?</summary>
        private bool expanded = false;

        /// <summary>The stream the last read used</summary>
		private ArrayDataIO input;
		
		/// <summary>Our current offset into the heap.  When we read from 
        /// the heap we use a byte array input stream.  So long 
        /// as we continue to read further into the heap, we can 
        /// continue to use the same stream, but we need to 
        /// recreate the stream whenever we skip backwards.</summary>
		private int heapOffset = 0;
		
		/// <summary>A stream used to read the heap data</summary>
		private BufferedDataStream bstr;
    #endregion

		/// <summary>Create a heap of a given size.</summary>
		internal FitsHeap(int size)
		{
			heap = new byte[size];
			heapSize = size;
		}
		
		/// <summary>Read the heap</summary>
		public virtual void Read(ArrayDataIO str)
		{
			if (str is RandomAccess)
			{
				fileOffset = FitsUtil.FindOffset(str);
				input = str;
			}
			
			if (heap != null)
			{
				try
				{
					str.Read(heap, 0, heapSize);
				}
				catch(IOException e)
				{
					throw new FitsException("Error reading heap:" + e);
				}
			}
            
            // change suggested in .99.1 version: 
            bstr = null;
		}
		
		/// <summary>Write the heap</summary>
		public virtual void Write(ArrayDataIO str)
		{
			try
			{
				str.Write(heap, 0, heapSize);
			}
			catch(IOException e)
			{
				throw new FitsException("Error writing heap:" + e);
			}
		}
		
		/// <summary>Attempt to rewrite the heap with the current contents.
		/// Note that no checking is done to make sure that the
		/// heap does not extend past its prior boundaries.</summary>
		public virtual void Rewrite()
		{
			if(this.Rewriteable)
			{
				//ArrayDataIO str = (ArrayDataIO) input;
				//FitsUtil.Reposition(input, fileOffset);
                input.Seek(fileOffset, SeekOrigin.Begin);
				Write(input);
			}
			else
			{
				throw new FitsException("Invalid attempt to rewrite FitsHeap");
			}
		}
		
		/// <summary>Get data from the heap.</summary>
		/// <param name="offset">The offset at which the data begins.</param>
		/// <param name="array"> The array to be extracted.</param>
		public virtual void GetData(int offset, Object array)
		{
            // Can we reuse the existing byte stream?
            try
			{
				if (bstr == null || heapOffset > offset)
				{
					heapOffset = 0;
					bstr = new BufferedDataStream(new MemoryStream(heap));
				}
				
				//System.IO.BinaryReader temp_BinaryReader;
				System.Int64 temp_Int64;
				//temp_BinaryReader = bstr;
				temp_Int64 = bstr.Position;  //temp_BinaryReader.BaseStream.Position;
				temp_Int64 = bstr.Seek(offset - heapOffset) - temp_Int64;  //temp_BinaryReader.BaseStream.Seek(offset - heapOffset, System.IO.SeekOrigin.Current) - temp_Int64;
				int generatedAux = (int)temp_Int64;
				heapOffset = offset;
				heapOffset += bstr.ReadArray(array);
			}
			catch(IOException e)
			{
				throw new FitsException("Error decoding heap area at offset=" + offset + ".  Exception: Exception " + e);
			}
		}
		
		/// <summary>Check if the Heap can accommodate a given requirement. If not expand the heap.</summary>
		internal virtual void ExpandHeap(int need)
		{
            // change suggested in .99.1 version: 
            // Invalidate any existing input stream to the heap.
            bstr = null;

            if (heapSize + need > heap.Length)
			{
                // change suggested in .99.1 version: 
                expanded = true;

				int newlen = (heapSize + need) * 2;
				if (newlen < 16384)
				{
					newlen = 16384;
				}
				byte[] newHeap = new byte[newlen];
				Array.Copy(heap, 0, newHeap, 0, heapSize);
				heap = newHeap;
			}
		}
		
		/// <summary>Add some data to the heap.</summary>
		internal virtual int PutData(Object data)
		{
			int size = ArrayFuncs.ComputeSize(data);
			ExpandHeap(size);
			MemoryStream bo = new MemoryStream(size);
			
			try
			{
				BufferedDataStream o = new BufferedDataStream(bo);
				o.WriteArray(data);
				o.Flush();
				o.Close();
			}
			catch(IOException)
			{
				throw new FitsException("Unable to write variable column length data");
			}
			
			Array.Copy(bo.ToArray(), 0, heap, heapSize, size);
			int oldOffset = heapSize;
			heapSize += size;

            // change suggested in .99.1 version: 
            heapOffset = heapSize;
			
			return oldOffset;
		}

        /// <summary>Return the size of the Heap.</summary>
        public virtual int GetSize()
		{
			return heapSize;
		}
	}
}

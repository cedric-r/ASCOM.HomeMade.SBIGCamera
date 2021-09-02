namespace nom.tam.image
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

    /// <summary>The ImageTiler class allows users to extract subimages from a FITS primary image 
    /// or image extension.
    /// This class provides a subset of an N-dimensional image.
	/// Modified May 2, 2000 by T. McGlynn to permit
	/// tiles that go off the edge of the image.
    /// </summary>
	public class ImageTiler
	{
		/// <summary>See if we can get the image data from memory.
		/// This may be overriden by other classes, notably
		/// in nom.tam.fits.ImageData.</summary>
		virtual public Array MemoryImage
		{
			get
			{
				return null;
			}
		}

		/// <summary>Read the entire image into a multidimensional array.</summary>
		virtual public Object CompleteImage
		{
			get
			{
				if (f == null)
				{
					throw new IOException("Attempt to read from null file");
				}
				//long currentOffset = f.FilePointer;
                long currentOffset = f.Position;
				Object o = ArrayFuncs.NewInstance(base_Renamed, dims);
				f.Seek(fileOffset, SeekOrigin.Begin);
				f.ReadArray(o);
				f.Seek(currentOffset,SeekOrigin.Begin);
                return o;
			}
		}

		//internal RandomAccess f;
        internal ArrayDataIO f;
		internal long fileOffset;
		
		internal int[] dims;
		internal Type base_Renamed;
		
		/// <summary>Create a tiler.</summary>
		/// <param name="f">The random access device from which image data may be read.
		/// This may be null if the tile information is available from memory.</param>
		/// <param name="fileOffset">The file offset within the RandomAccess device at which
		/// the data begins.</param>
		/// <param name="dims">The actual dimensions of the image.</param>
		/// <param name="base_Renamed">base class (should be a primitive type) of the image.</param>
		public ImageTiler(RandomAccess f, long fileOffset, int[] dims, Type base_Renamed)
		{
			this.f = f;
			this.fileOffset = fileOffset;
			this.dims = dims;
			this.base_Renamed = base_Renamed;
		}

        /// <summary>Get a subset of the image.  An image tile is returned
		/// as a one-dimensional array although the image will
		/// normally be multi-dimensional.</summary>
		/// <param name="corners">starting corner (using 0 as the start) for the image.</param>
		/// <param name="lenghts">length requested in each dimension.</param>
		public virtual Array GetTile(int[] corners, int[] lengths)
		{
			if (corners.Length != dims.Length || lengths.Length != dims.Length)
			{
				throw new IOException("Inconsistent sub-image request");
			}
			
			int arraySize = 1;
			for (int i = 0; i < dims.Length; i += 1)
			{
				if (corners[i] < 0 || lengths[i] < 0 || corners[i] + lengths[i] > dims[i])
				{
					throw new IOException("Sub-image not within image");
				}
				
				arraySize *= lengths[i];
			}
			
			Array outArray = ArrayFuncs.NewInstance(base_Renamed, arraySize);
			
			GetTile(outArray, corners, lengths);
			return outArray;
		}
		
		/// <summary>Get a tile, filling in a prespecified array.
		/// This version does not check that the user hase
		/// entered a valid set of corner and length arrays.
		/// ensure that out matches the
		/// length implied by the lengths array.</summary>
		/// <param name="outArray">The output tile array.  A one-dimensional
		/// array. Data not within the valid limits of the image will
		/// be left unchanged.  The length of this array should be the
		/// product of lengths.</param>
		/// <param name="corners">The corners of the tile.</param>
		/// <param name="lengths">The dimensions of the tile.</param>
		public virtual void GetTile(Array outArray, int[] corners, int[] lengths)
		{
			Array data = MemoryImage;
			
			if(data == null && f == null)
			{
				throw new IOException("No data source for tile subset");
			}

            FillTile(data, outArray, dims, corners, lengths);
		}
		

		/// <summary>Fill the subset.</summary>
		/// <param name="data">The memory-resident data image.
		/// This may be null if the image is to be read from a file.  This should
		/// be a multi-dimensional primitive array.</param>
		/// <param name="o">The tile to be filled.  This is a simple primitive array.</param>
		/// <param name="dims">The dimensions of the full image.</param>
		/// <param name="corners">The indices of the corner of the image.</param>
		/// <param name="lengths">The dimensions of the subset.</param>
		protected internal virtual void FillTile(Array data, Array o, int[] dims, int[] corners, int[] lengths)
		{
			int n = dims.Length;
			int[] posits = new int[n];
            int baseLength = ArrayFuncs.GetBaseLength(o);
            int segment = lengths[n - 1];
			
			Array.Copy(corners, 0, posits, 0, n);
			long currentOffset = 0;
			if (data == null)
			{
				//currentOffset = f.FilePointer;

                currentOffset = f.Position;
			}
			
			int outputOffset = 0;
            if(data != null && data.GetType().Equals(typeof(Array)) && !ArrayFuncs.IsArrayOfArrays(data))
            {
                int[] index = new int[posits.Length];
                Array.Copy(posits, 0, index, 0, n);
                index[index.Length - 1] -= 1;
                for(int i = outputOffset; ArrayFuncs.NextIndex(index, posits, lengths); ++i)
                {
                  o.SetValue(data.GetValue(index), i);
                }

                return;
            }
  
			do 
			{
				// This implies there is some overlap in the last index
                // (in conjunction with other tests)
				int mx = dims.Length - 1;
				bool validSegment = posits[mx] + lengths[mx] >= 0 && posits[mx] < dims[mx];
				
				
				// Don't do anything for the current segment if anything but the
				// last index is out of range.
				if (validSegment)
				{
					for (int i = 0; i < mx; i += 1)
					{
						if (posits[i] < 0 || posits[i] >= dims[i])
						{
							validSegment = false;
							break;
						}
					}
				}
				
				if (validSegment)
				{
					if (data != null)
					{
                        FillMemData(data, posits, segment, o, outputOffset, 0);
					}
					else
					{
						int offset = GetOffset(dims, posits) * baseLength;
						
						// Point to offset at real beginning
						// of segment
						int actualLen = segment;
						int actualOffset = offset;
						int actualOutput = outputOffset;
						if (posits[mx] < 0)
						{
							actualOffset -= posits[mx] * baseLength;
							actualOutput -= posits[mx];
							actualLen += posits[mx];
						}
						if (posits[mx] + segment > dims[mx])
						{
							actualLen -= posits[mx] + segment - dims[mx];
						}
						FillFileData(o, actualOffset, actualOutput, actualLen);
					}
				}
				outputOffset += segment;
			}
			while(IncrementPosition(corners, posits, lengths));
			if (data == null)
			{
				f.Seek(currentOffset,SeekOrigin.Begin);
			}
		}

        /// <summary>Fill a single segment from memory.
		/// This routine is called recursively to handle multi-dimensional
		/// arrays.  E.g., if data is three-dimensional, this will
		/// recurse two levels until we get a call with a single dimensional
		/// datum.  At that point the appropriate data will be copied
		/// into the output.</summary>
		/// <param name="data">The in-memory image data.</param>
		/// <param name="posits">The current position for which data is requested.</param>
		/// <param name="length">The size of the segments.</param>
		/// <param name="output">The output tile.</param>
		/// <param name="outputOffset">The current offset into the output tile.</param>
		/// <param name="dim">The current dimension being</param>
		protected internal virtual void FillMemData(Array data, int[] posits, int length,
                                                Array output, int outputOffset, int dim)
		{
            // FIX THIS n-D crap
			//if(data is Object[])
            if(ArrayFuncs.CountDimensions(data) > 1)
			{
                if(ArrayFuncs.IsArrayOfArrays(data))
                {
                    //Object[] xo = (Object[]) data;
                    //FillMemData((Array)xo[posits[dim]], posits, length, output, outputOffset, dim + 1);
                    FillMemData((Array)((Array)data).GetValue(posits[dim]), posits, length, output, outputOffset, dim + 1);
                }
                else
                {
                    throw new Exception("Called FillMemData with multi-dimensional array.");
                }
			}
			else
			{
				// Adjust the spacing for the actual copy.
				int startFrom = posits[dim];
				int startTo = outputOffset;
				int copyLength = length;
				
				if(posits[dim] < 0)
				{
					startFrom -= posits[dim];
					startTo -= posits[dim];
					copyLength += posits[dim];
				}
				if(posits[dim] + length > dims[dim])
				{
					copyLength -= (posits[dim] + length - dims[dim]);
				}
				
				Array.Copy(data, startFrom, output, startTo, copyLength);
			}
		}

		/// <summary>File a tile segment from a file.</summary>
		/// <param name="output">The output tile.</param>
		/// <param name="delta">The offset from the beginning of the image in bytes.</param>
		/// <param name="outputOffset">The index into the output array.</param>
		/// <param name="segment">The number of elements to be read for this segment.</param>
		protected internal virtual void FillFileData(Array output, int delta, int outputOffset, int segment)
		{
			f.Seek(fileOffset + delta,SeekOrigin.Begin);
			
			if (base_Renamed == typeof(float))
			{
				f.Read((float[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(int))
			{
				f.Read((int[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(short))
			{
				f.Read((short[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(double))
			{
				f.Read((double[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(byte))
			{
				f.Read((byte[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(char))
			{
				f.Read((char[]) output, outputOffset, segment);
			}
			else if (base_Renamed == typeof(long))
			{
				f.Read((long[]) output, outputOffset, segment);
			}
			else
			{
				throw new IOException("Invalid type for tile array");
			}
		}

        /// <summary>Increment the offset within the position array.
		/// Note that we never look at the last index since
		/// we copy data a block at a time and not byte by byte.</summary>
		/// <param name="start">starting corner values.</param>
		/// <param name="current">current offsets.</param>
		/// <param name="lengths">The desired dimensions of the subset.</param>
		protected internal static bool IncrementPosition(int[] start, int[] current, int[] lengths)
		{
			for (int i = start.Length - 2; i >= 0; i -= 1)
			{
				if (current[i] - start[i] < lengths[i] - 1)
				{
					current[i] += 1;
					for (int j = i + 1; j < start.Length - 1; j += 1)
					{
						current[j] = start[j];
					}
					return true;
				}
			}
			return false;
		}
		
		
		/// <summary>Get the offset of a given position.</summary>
		/// <param name="dims"> The dimensions of the array.</param>
		/// <param name="pos">  The index requested.</param>
		public static int GetOffset(int[] dims, int[] pos)
		{
			int offset = 0;
			for (int i = 0; i < dims.Length; i += 1)
			{
				if (i > 0)
				{
					offset *= dims[i];
				}
				offset += pos[i];
			}
			return offset;
		}
	}
}

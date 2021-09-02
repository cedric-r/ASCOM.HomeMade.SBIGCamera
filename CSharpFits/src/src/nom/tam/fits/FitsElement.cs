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
	using nom.tam.util;
    /// <summary>This inteface describes allows uses to easily perform
    /// basic I/O operations
    /// on a FITS element.
    /// </summary>
	public interface FitsElement
	{
        /// <summary>Get the byte at which this element begins.
		/// This is only available if the data is originally read from
		/// a random access medium.
        /// </summary>
		long FileOffset
		{
			get;				
		}
		/// <summary>The size of this element in bytes</summary>
		long Size
		{
			get;
		}
        bool Rewriteable
        {
            get;
        }

		/// <summary>Read the contents of the element from an input source.</summary>
		/// <param name="in_Renamed">input source.</param>
		void Read(ArrayDataIO in_Renamed);
		/// <summary>Write the contents of the element to a data sink.</summary>
		/// <param name="out_Renamed">The data sink.</param>
		void Write(ArrayDataIO out_Renamed);
		/// <summary>Rewrite the contents of the element in place.
		/// The data must have been orignally read from a random
		/// access device, and the size of the element may not have changed.
		/// </summary>
		void Rewrite();
	}
}

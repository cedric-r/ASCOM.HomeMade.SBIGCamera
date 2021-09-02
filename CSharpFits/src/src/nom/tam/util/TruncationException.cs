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
	
	public class TruncationException:Exception
	{
        /// <summary>
        /// Truncation Exception handling class
        /// </summary>
		public TruncationException():base()
		{
		}
		public TruncationException(String msg):base(msg)
		{
		}
	}
}
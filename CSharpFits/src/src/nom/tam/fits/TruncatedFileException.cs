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
	/// <summary>This exception is thrown when an EOF is detected in the middle
	/// of an HDU.
    /// </summary>
   	public class TruncatedFileException:FitsException
	{
		/// <summary>
		/// Constructor taking no arguments
		/// </summary>
        public TruncatedFileException():base()
		{
		}
        /// <summary>
        /// Constructor taking string argument
        /// </summary>
        /// <param name="msg"></param>
		public TruncatedFileException(String msg):base(msg)
		{
		}
	}
}

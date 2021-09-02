namespace nom.tam.fits
{
	using System;
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

	/// <summary>This exception indicates that an error
	/// was detected while parsing a FITS header record.
	/// </summary>
	public class BadHeaderException:FitsException
	{
		/// <summary>
		/// Constructor without arguments
		/// </summary>
        public BadHeaderException():base()
		{
		}
        /// <summary>
        /// Constructor with String argument for exception message
        /// </summary>
        /// <param name="msg"></param>
		public BadHeaderException(String msg):base(msg)
		{
		}
	}
}

namespace nom.tam.fits
{
	using System;

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

	/* This class was contributed by David Glowacki */
	/// <summary>
	/// This Class handles header card exception.It extends FitsException
	/// </summary>
	public class HeaderCardException:FitsException
	{
        /// <summary>
        /// Constructor without any paramteres
        /// </summary>
		public HeaderCardException():base()
		{
		}
		/// <summary>
		/// Constructor taking String as paramter
		/// </summary>
		/// <param name="s"></param>

		public HeaderCardException(String s):base(s)
		{
		}
	}
}

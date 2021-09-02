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
    public class FitsException : Exception
    {
        public FitsException()
            : base()
        {
        }

        public FitsException(String msg)
            : base(msg)
        {
        }
    }
}

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

    /// <Remarks>These packages define the methods which indicate that
    /// an i/o stream may be accessed in arbitrary order.
    /// The method signatures are taken from RandomAccessFile
    /// though that class does not implement this interface.
    /// </Remarks>
    //public interface RandomAccess : ArrayDataIO
    public abstract class RandomAccess : ArrayDataIO
    {
        // <summary>Get the current position in the stream</summary>
        /*
        public abstract long FilePointer
        {
            get;
        }
    */
    }
}

namespace nom.tam.util
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

    /// <summary>summary description for ArrayDataIO.</summary>
	//public interface ArrayDataIO
  public abstract class ArrayDataIO : Stream
	{
    /*
    long Position
    {
      get;
    }
*/
    protected abstract Stream BaseStream
    {
      get;
    }

    public abstract bool ReadBoolean();
    // ***** CHECK READBYTE CALLS *****
    //public abstract override byte ReadByte();
    public abstract sbyte ReadSByte();
    public abstract char ReadChar();
    public abstract short ReadInt16();
    public abstract int ReadInt32();
    public abstract long ReadInt64();
    public abstract float ReadSingle();
    public abstract double ReadDouble();

    /// <summary>Read a generic (possibly multidimenionsional) primitive array.
    /// An  Object[] array is also a legal argument if each element
    /// of the array is a legal.
    /// <p>
    /// The ArrayDataIO classes do not support String input since
    /// it is unclear how one would read in an Array of strings.
    /// </summary>
    /// <param name="o">  A [multidimensional] primitive (or Object) array.
    /// 
    /// </param>
    public abstract int ReadArray(System.Object o);
    /// <summary>
    /// Read a complete primitive array
    /// </summary>
    /// <param name="buf"></param>
    /// <returns></returns>
    public abstract int Read(byte[] buf);
    public abstract int Read(sbyte[] buf);
    public abstract int Read(bool[] buf);
    public abstract int Read(short[] buf);
    public abstract int Read(char[] buf);
    public abstract int Read(int[] buf);
    public abstract int Read(long[] buf);
    public abstract int Read(float[] buf);
    public abstract int Read(double[] buf);
    //public abstract int Read(byte[] buf, int offset, int size);
   /// <summary>
    ///  Read a segment of a primitive array.
   /// </summary>
   /// <param name="buf"></param>
   /// <param name="offset"></param>
   /// <param name="size"></param>
   /// <returns></returns>
    public abstract int Read(sbyte[] buf, int offset, int size);
    public abstract int Read(bool[] buf, int offset, int size);
    public abstract int Read(char[] buf, int offset, int size);
    public abstract int Read(short[] buf, int offset, int size);
    public abstract int Read(int[] buf, int offset, int size);
    public abstract int Read(long[] buf, int offset, int size);
    public abstract int Read(float[] buf, int offset, int size);
    public abstract int Read(double[] buf, int offset, int size);
    /* Skip (forward) in a file */
    public abstract long Seek(long distance);
    //public abstract long Seek(long distance, SeekOrigin origin);
    /* Close the file. */
    //void Close();

    /// <summary>Write a generic (possibly multi-dimenionsional) primitive or String
    /// array.  An array of Objects is also allowed if all
    /// of the elements are valid arrays.
    /// <p>
    /// This routine is not called 'write' to avoid possible compilation
    /// errors in routines which define only some of the other methods
    /// of the interface (and defer to the superclass on others).
    /// In that case there is an ambiguity as to whether to
    /// call the routine in the current class but convert to
    /// Object, or call the method from the super class with
    /// the same type argument.
    /// </summary>
    /// <param name="o	The">primitive or String array to be written.
    /// @throws IOException if the argument is not of the proper type
    /// 
    /// </param>
    public abstract void  WriteArray(System.Object o);
    public abstract void  Write(byte b);
    public abstract void  Write(sbyte sb);
    public abstract void  Write(bool b);
    public abstract void  Write(short s);
    public abstract void  Write(char c);
    public abstract void  Write(int i);
    public abstract void  Write(long l);
    public abstract void  Write(float f);
    public abstract void  Write(double d);
    /* Write a complete array */
    public abstract void  Write(byte[] buf);
    public abstract void  Write(sbyte[] buf);
    public abstract void  Write(bool[] buf);
    public abstract void  Write(short[] buf);
    public abstract void  Write(char[] buf);
    public abstract void  Write(int[] buf);
    public abstract void  Write(long[] buf);
    public abstract void  Write(float[] buf);
    public abstract void  Write(double[] buf);

      /// <summary>
    /// Write an array of Strings 
      /// </summary>
      /// <param name="buf"></param>
    public abstract void  Write(System.String[] buf);
    
    //public abstract void  Write(byte[] buf, int offset, int size);
    /// <summary>
    /// Write a segment of a primitive array.
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public abstract void  Write(sbyte[] buf, int offset, int size);
    public abstract void  Write(bool[] buf, int offset, int size);
    public abstract void  Write(char[] buf, int offset, int size);
    public abstract void  Write(short[] buf, int offset, int size);
    public abstract void  Write(int[] buf, int offset, int size);
    public abstract void  Write(long[] buf, int offset, int size);
    public abstract void  Write(float[] buf, int offset, int size);
    public abstract void  Write(double[] buf, int offset, int size);

    /// <summary>
    ///  Write some of an array of Strings 
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public abstract void  Write(System.String[] buf, int offset, int size);
    /* Flush the output buffer */
    //void  Flush();
  }
}

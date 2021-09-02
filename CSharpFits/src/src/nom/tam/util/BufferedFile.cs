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
    using System.IO;

  /// <summary>summary description for BufferedFile.</summary>
  //public class BufferedFile : BufferedDataStream, RandomAccess
  public class BufferedFile : BufferedDataStream
  {
    #region Constructors
    /// <summary>Create a read-only buffered file</summary>
    public BufferedFile(String filename):this(filename, FileAccess.Read, FileShare.Read,32768)
    {
    }
	
    /// <summary>Create a buffered file with the given mode.</summary>
    /// <param name="filename">The file to be accessed.</param>
    /// <param name="access">Read/write</param>
    public BufferedFile(String filename, FileAccess access,FileShare share):this(filename, access,share, 32768)
    {
    }
	
    /// <summary> Create a buffered file from a File descriptor.</summary>
    public BufferedFile(FileInfo file):this(file, FileAccess.Read,FileShare.Read,2768)
    {
    }
    
    /// <summary> Create a buffered file from a File descriptor.</summary>
      public BufferedFile(FileInfo file, FileAccess mode,FileShare share):this(file, mode,FileShare.ReadWrite, 32768)
    {
    }
    /// <summary>
    ///  Create a buffered file from a FileAccess and FileShare descriptor ,Bufer size
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="access"></param>
    /// <param name="share"></param>
    /// <param name="bufferSize"></param>
    public BufferedFile(String filename, FileAccess access, FileShare share,int bufferSize)
        : this(new FileInfo(filename), access, share,bufferSize)
    {
    }
    
    /// <summary>Create a buffered file with the given mode and a specified
    /// buffer size.
    /// </summary>
    /// <param name="filename">The file to be accessed.</param>
    /// <param name="mode">Read/write</param>
    /// <param name="buffer">The buffer size to be used. This should be
    /// substantially larger than 100 bytes and
    /// defaults to 32768 bytes in the other
    /// constructors.</param>
    public BufferedFile(FileInfo filename, FileAccess access,FileShare share, int bufferSize)
      //  : base(new FileStream(filename.FullName, FileMode.OpenOrCreate, access, FileShare.Read, bufferSize))
      : base(new FileStream(filename.FullName, FileMode.OpenOrCreate, access, share, bufferSize))
        
    {
    }
    #endregion

    #region RandomAccess Members
    /// <summary>Get the current position in the stream</summary>
    public long FilePointer
    {
      get
      {
        return _s.Position;
      }
    }
    #endregion
  }
}

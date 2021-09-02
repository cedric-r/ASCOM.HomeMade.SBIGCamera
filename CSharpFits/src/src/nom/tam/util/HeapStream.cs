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
  using System.IO;
  using System;

  /// <summary>
  /// summary description for HeapStream.
    /// </summary>
  public class HeapStream : ActualBufferedStream
  {
    public override long Position
    {
      get
      {
        return _pos;
      }
    }

    public HeapStream(Stream s) : base(s)
    {
    }
   
    public override void Write(byte[] buffer, int offset, int count)
    {
      base.Write(buffer, offset, count);
      //_s.Write(buffer, offset, count);
      _pos += count;
    }
    
    public override void WriteByte(byte val)
    {
      base.WriteByte(val);
      ++_pos;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      _nRead = base.Read(buffer, offset, count);
      _pos += _nRead;

      return _nRead;
    }

    public override int ReadByte()
    {
      _byte = base.ReadByte();

      if(_byte != -1)
      {
        ++_pos;
      }

      return _byte;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      _pos = base.Seek(offset, origin);

      return _pos;
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    protected long _pos = 0;
    protected int _nRead = 0;
    protected int _byte = 0;
  }
}

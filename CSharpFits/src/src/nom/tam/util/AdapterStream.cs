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

    /// <summary>
	/// When writing new Streams, I'm tired of having to forward most of the methods
	/// to the underlying Stream.  So this is the default behavior in this class, and
	/// subclasses are free to override any methods they see fit.
    /// </summary>
	public class AdapterStream : Stream
	{
    #region properties
    public virtual Stream TargetStream
    {
      get
      {
        return _s;
      }
      set
      {
        _s = value;
      }
    }

    public override bool CanRead
    {
      get
      {
        return _s.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return _s.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return _s.CanWrite;
      }
    }

    public override long Length
    {
      get
      {
        return _s.Length;
      }
    }

    public override long Position
    {
      get
      {
        return _s.Position;
      }
      set
      {
        _s.Position = value;
      }
    }
    #endregion

		public AdapterStream(Stream targetStream)
		{
      TargetStream = targetStream;
		}

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return _s.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return _s.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close()
    {
      _s.Close();
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      return _s.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      _s.EndWrite(asyncResult);
    }

    public override void Flush()
    {
      _s.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return _s.Read(buffer, offset, count);
    }

    public override int ReadByte()
    {
      return _s.ReadByte();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return _s.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      _s.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      _s.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      _s.WriteByte(value);
    }

    protected Stream _s;
	}
}

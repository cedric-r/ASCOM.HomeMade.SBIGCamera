using System;
using System.IO;

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

	/// <summary>
	///   Converts Strings to byte arrays and writes them to the underlying stream.
	///   Characters are written as 8-bit ASCII, not 16-bit unicode!!!
	/// </summary>
	public class StringToByteStream : AdapterStream
	{
    public static readonly int DEFAULT_BYTE_BUFFER_SIZE = 4096;

		public StringToByteStream(Stream s) : base(s)
		{
      _byteBuf = new byte[_byteBufferSize];
		}

    public void Write(String s)
    {
      if(s == null)
      {
        return;
      }

      Write(s.ToCharArray());
    }

    public void Write(char[] c)
    {
      if(c == null)
      {
        return;
      }

      int j = 0;
      for(int i = 0; i < c.Length;)
      {
        for(j = 0; i + j < c.Length && j < _byteBufferSize; ++j)
        {
          _byteBuf[j] = (byte)c[i + j];
        }
        _s.Write(_byteBuf, 0, j);
        i += j;
      }
    }

    protected int _byteBufferSize = DEFAULT_BYTE_BUFFER_SIZE;
    protected byte[] _byteBuf = null;
	}
}

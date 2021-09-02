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

    /// <summary>
    /// summary description for ConfigStream.
	/// </summary>
  public class ConfigStream : AdapterStream
  {
    public override bool CanSeek
    {
      get
      {
        if(_canSeek)
        {
          return base.CanSeek;
        }

        return false;
      }
    }

    public ConfigStream(Stream s) : base(s)
    {
    }

    public void SetCanSeek(bool canSeek)
    {
      _canSeek = canSeek;
    }

    bool _canSeek;
  }
}

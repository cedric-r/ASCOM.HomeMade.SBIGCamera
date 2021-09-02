using System;

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
	///   To replace C# non-nullable bool struct.
	/// </summary>
	public class Troolean
	{
		public Troolean() : this(false, false)
		{
		}

    public Troolean(bool val) : this(val, false)
    {
    }

    public Troolean(bool val, bool isNull)
    {
      _val = val;
      _isNull = isNull;
    }

    public bool Val
    {
      get
      {
        return _val;
      }
      set
      {
        _val = value;
      }
    }

    public bool IsNull
    {
      get
      {
        return _isNull;
      }
      set
      {
        _isNull = value;
      }
    }

    protected bool _val;
    protected bool _isNull;
	}
}

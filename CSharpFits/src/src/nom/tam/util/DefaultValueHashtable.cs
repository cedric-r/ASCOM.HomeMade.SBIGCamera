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
    using System.Collections;

    /// <summary>
	/// summary description for DefaultValueHashtable.
    /// </summary>
  public class DefaultValueHashtable : Hashtable
  {
    public override Object this[Object key]
    {
      get
      {
        _result = base[key];
        if(_result == null)
        {
          _result = DefaultValue;
        }

        return _result;
      }

      set
      {
        if(key == null)
        {
          DefaultValue = value;
        }
        else
        {
          base[key] = value;
        }
      }
    }

    public Object DefaultValue
    {
      get
      {
        return _defaultValue;
      }
      set
      {
        _defaultValue = value;
      }
    }

    public DefaultValueHashtable() : this(null)
    {
    }

    public DefaultValueHashtable(Object defaultValue) : base()
    {
      DefaultValue = defaultValue;
    }

    protected Object _result = null;
    protected Object _defaultValue = null;
  }
}

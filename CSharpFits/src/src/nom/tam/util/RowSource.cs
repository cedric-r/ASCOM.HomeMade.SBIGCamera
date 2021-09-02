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
   using System.Data;

    /// <Remarks>
	/// summary description for RowSource.
    /// </Remarks>
	public abstract class RowSource
	{
    public const int NA = -1;

    public abstract int NRows
    {
      get;
    }

    public abstract String[] ColumnNames
    {
      get;
    }

    public abstract Array[] ModelRow
    {
      get;
    }

    public abstract Object[] TNULL
    {
      get;
    }

    public abstract Array[] GetNextRow(ref Array[] row);
	}

  public abstract class RowAdapter : RowSource
  {
    #region RowSource Members
    public override int NRows
    {
      get
      {
        return _nrows;
      }
    }

    public override String[] ColumnNames
    {
      get
      {
        return _columnNames;
      }
    }

    public override Array[] ModelRow
    {
      get
      {
        return _modelRow;
      }
    }

    public override Object[] TNULL
    {
      get
      {
        return _tnull;
      }
    }
    #endregion

    protected RowAdapter(int nrows, String[] columnNames, Array[] modelRow, Object[] tnull)
    {
      _nrows = nrows;
      _columnNames = columnNames;
      _modelRow = modelRow;
      _tnull = tnull;
    }

    protected int _nrows;
    protected String[] _columnNames;
    protected Array[] _modelRow;
    protected Object[] _tnull;
  }

  public class DataReaderAdapter : RowAdapter
  {
    public DataReaderAdapter(IDataReader reader) : this(reader, RowSource.NA)
    {
    }

    public DataReaderAdapter(IDataReader reader, int nrows) :
      base(nrows, MakeColumnNames(reader), MakeModelRow(reader), MakeTNULL(reader))
    {
      _haveRow = reader.Read();
      _reader = reader;
      _rowStuffers = new RowStuffer[reader.FieldCount];
      //_rowStuffers = new RowStuffer[reader.GetSchemaTable().Columns.Count];

      for(int i = 0; i < _rowStuffers.Length; ++i)
      {
        Type t = reader.GetFieldType(i);
        //_rowStuffers[i] = RowStuffer.GetRowStuffer(reader.GetFieldType(i));
        try
        {
          _rowStuffers[i] = RowStuffer.GetRowStuffer(t);
        }
        catch(Exception e)
        {
          throw e;
        }
        if(reader.GetFieldType(i) == typeof(String))
        {
            _rowStuffers[i] =
              //new RowStuffer.StringRowStuffer(reader.GetSchemaTable().Columns[i].MaxLength);
              new RowStuffer.StringRowStuffer(30);
        }
      }
    }

    public override Array[] GetNextRow(ref Array[] row)
    {
      if(!_haveRow)
      {
        return null;
      }

      if(row == null || row.Length != _reader.FieldCount)
      {
        Type t = null;
        row = new Array[_reader.FieldCount];
        for(int i = 0; i < row.Length; ++i)
        {
          t = GetFieldType(_reader, i);
          t = t == typeof(decimal) ? typeof(long) : t;
          row[i] = Array.CreateInstance(t, 1);
        }
      }

      for(int i = 0; i < row.Length; ++i)
      {
        row[i] = _rowStuffers[i].Stuff(_reader[i]);
      }
      _haveRow = _reader.Read();

      return row;
    }

    protected bool _haveRow = false;
    protected static String[] MakeColumnNames(IDataReader reader)
    {
      String[] result = new String[reader.FieldCount];

      for(int i = 0; i < result.Length; ++i)
      {
        result[i] = reader.GetName(i);
        if("".Equals(result[i]))
        {
          result[i] = "Column" + i;
        }
      }

      return result;
    }

    protected static Array[] MakeModelRow(IDataReader reader)
    {
      Array[] result = new Array[reader.FieldCount];
      Type t = null;

      for(int i = 0; i < result.Length; ++i)
      {
        t = GetFieldType(reader, i);
        t = t == typeof(decimal) ? typeof(long) : t; // to handle mono bug where long => decimal!
        result[i] = Array.CreateInstance(t, 1);

        if(t == typeof(String))
        {
          result[i].SetValue(" ", 0);
        }
      }

      return result;
    }

    protected static Object[] MakeTNULL(IDataReader reader)
    {
      Object[] result = new Object[reader.FieldCount];

      for(int i = 0; i < result.Length; ++i)
      {
        result[i] = RowStuffer.GetDefaultNullValue(reader.GetFieldType(i));
        if(reader.GetFieldType(i) == typeof(String))
        {
          result[i] = new String(' ', 1);
        }
      }

      return result;
    }

    protected static Type GetFieldType(IDataReader r, int field)
    {
      Type result = r.GetFieldType(field);
      result = result == typeof(bool) ? typeof(Troolean) : result;
      return result;
    }

    protected IDataReader _reader;
    protected RowStuffer[] _rowStuffers;
  }

  public abstract class RowStuffer
  {
    public abstract Array Stuff(Object o);
    public abstract RowStuffer GetRowStuffer();
    protected abstract Object TNull
    {
      get;
    }

    protected Object CheckValue(Object o)
    {
      if(o == null || o.GetType() == typeof(DBNull))
      {
        return TNull;
      }

      return o;
    }

    public static RowStuffer GetRowStuffer(Type t)
    {
      return ((RowStuffer)_rowStuffers[t]).GetRowStuffer();
    }

    public class DefaultRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        return _b;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new DefaultRowStuffer();
      }

      protected byte[] _b = new byte[0];
      protected override Object TNull
      {
        get
        {
          return null;
        }
      }
    }

    public class ByteRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _b[0] = (byte)o;
        return _b;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new ByteRowStuffer();
      }

      protected byte[] _b = new byte[1];
      protected override Object TNull
      {
        get
        {
          return (byte)0;
        }
      }
    }

    public class TrooleanRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _b[0] = (Troolean)o;
        return _b;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new TrooleanRowStuffer();
      }

      protected Troolean[] _b = new Troolean[1];
      protected Troolean _null = new Troolean(false, true);
      protected override Object TNull
      {
        get
        {
          return _null;
        }
      }
    }

    public class CharRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _c[0] = (char)o;
        return _c;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new CharRowStuffer();
      }

      protected char[] _c = new char[1];
      protected override Object TNull
      {
        get
        {
          return '\0';
        }
      }
    }

    public class ShortRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _s[0] = (short)o;
        return _s;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new ShortRowStuffer();
      }

      protected short[] _s = new short[1];
      protected override Object TNull
      {
        get
        {
          return (short)-99;
        }
      }
    }

    public class IntRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _i[0] = (int)o;
        return _i;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new IntRowStuffer();
      }

      protected int[] _i = new int[1];
      protected override Object TNull
      {
        get
        {
          return -99;
        }
      }
    }

    public class FloatRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _f[0] = (float)o;
        return _f;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new FloatRowStuffer();
      }

      protected float[] _f = new float[1];
      protected override Object TNull
      {
        get
        {
          return float.NaN;
        }
      }
    }

    public class LongRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _l[0] = (long)o;
        return _l;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new LongRowStuffer();
      }

      protected long[] _l = new long[1];
      protected override Object TNull
      {
        get
        {
          return (long)-99;
        }
      }
    }

    public class DoubleRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _d[0] = (double)o;
        return _d;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new DoubleRowStuffer();
      }

      protected double[] _d = new double[1];
      protected override Object TNull
      {
        get
        {
          return double.NaN;
        }
      }
    }

    public class DecimalRowStuffer : RowStuffer
    {
      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _l[0] = Decimal.ToInt64((decimal)o);
        return _l;
      }

      public override RowStuffer GetRowStuffer()
      {
        return new DecimalRowStuffer();
      }

      protected long[] _l = new long[1];
      protected override Object TNull
      {
        get
        {
          return (long)-99;
        }
      }
    }

    public class StringRowStuffer : RowStuffer
    {
      public StringRowStuffer() : this(-1)
      {
      }

      public StringRowStuffer(int arrayLength) : this(arrayLength, ' ', true)
      {
      }

      public StringRowStuffer(int arrayLength, char padChar, bool padLeft)
      {
        _arrayLength = arrayLength;
        _padChar = padChar;
        _padLeft = padLeft;
        _tnull = _arrayLength > 0 ? new String(_padChar, _arrayLength) : "";
      }

      public override Array Stuff(Object o)
      {
        o = CheckValue(o);
        _s[0] = o == null ? "" : (String)o;
        return _s;
        /*
        String s = o == null ? "" : (String)o;
        if(s.Length < _arrayLength)
        {
          s = _padLeft ? s.PadLeft(_arrayLength, _padChar) : s.PadRight(_arrayLength, _padChar);
        }

        return s.ToCharArray();
        */
      }

      public override RowStuffer GetRowStuffer()
      {
        return new StringRowStuffer();
      }

      protected String[] _s = new String[1];
      protected bool _padLeft;
      protected char _padChar;
      protected int _arrayLength;
      protected Object _tnull;
      protected override Object TNull
      {
        get
        {
          return _tnull;
        }
      }
    }

    public static Object GetDefaultNullValue(Type t)
    {
      return _defaultNullValues[t];
    }

    protected static Hashtable _rowStuffers;
    protected static Hashtable _defaultNullValues;

    static RowStuffer()
    {
      _rowStuffers = new DefaultValueHashtable(new DefaultRowStuffer());
      _rowStuffers[typeof(byte)] = new ByteRowStuffer();
      _rowStuffers[typeof(bool)] = new TrooleanRowStuffer();
      _rowStuffers[typeof(char)] = new CharRowStuffer();
      _rowStuffers[typeof(short)] = new ShortRowStuffer();
      _rowStuffers[typeof(int)] = new IntRowStuffer();
      _rowStuffers[typeof(float)] = new FloatRowStuffer();
      _rowStuffers[typeof(long)] = new LongRowStuffer();
      _rowStuffers[typeof(double)] = new DoubleRowStuffer();
      _rowStuffers[typeof(decimal)] = new DecimalRowStuffer();
      _rowStuffers[typeof(String)] = new StringRowStuffer();

      _defaultNullValues = new Hashtable();
      _defaultNullValues[typeof(byte)] = (byte)0;
      _defaultNullValues[typeof(bool)] = new Troolean(false, true);
      _defaultNullValues[typeof(char)] = '\0';
      _defaultNullValues[typeof(short)] = (short)-99;
      _defaultNullValues[typeof(int)] = -99;
      _defaultNullValues[typeof(float)] = float.NaN;
      _defaultNullValues[typeof(long)] = (long)-99;
      _defaultNullValues[typeof(double)] = double.NaN;
      _defaultNullValues[typeof(decimal)] = (long)-99;
      _defaultNullValues[typeof(String)] = "";
    }
  }
}

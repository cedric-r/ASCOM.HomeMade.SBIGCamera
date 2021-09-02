namespace nom.tam.fits
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
    using System.IO;

    using nom.tam.util;

    /// <summary>
	/// summary description for StreamedBinaryTableHDU.
    /// </summary>
	public class StreamedBinaryTableHDU : BasicHDU
	{
        public enum StringWriteMode {TRUNCATE, HEAP, PAD};

        public static readonly int DEFAULT_BUFFER_SIZE = 4096;
        public static readonly int DEFAULT_STRING_TRUNCATION_LENGTH = 128;
        public static readonly StringWriteMode DEFAULT_STRING_WRITE_MODE = StringWriteMode.PAD;

        #region properties
        public StringWriteMode WriteMode
        {
          get
          {
            return _writeMode;
          }
          set
          {
            _writeMode = value;
            SetupRenderers();
          }
        }

        // make this fix things
        public int StringTruncationLength
        {
          get
          {
            return _stringTruncationLength;
          }
          set
          {
            _stringTruncationLength = value;
            SetupRenderers();
          }
        }

        public bool PadLeft
        {
          get
          {
            return _padLeft;
          }
          set
          {
            _padLeft = value;
            SetupRenderers();
          }
        }

        public char PadChar
        {
          get
          {
            return _padChar;
          }
          set
          {
            _padChar = value;
            SetupRenderers();
          }
        }
        #endregion

        #region constructors
        public StreamedBinaryTableHDU(RowSource rs) : this(rs, DEFAULT_BUFFER_SIZE)
        {
        }

        public StreamedBinaryTableHDU(RowSource rs, int bufSize) :
          this(rs, bufSize, DEFAULT_STRING_WRITE_MODE, DEFAULT_STRING_TRUNCATION_LENGTH)
        {
        }

        public StreamedBinaryTableHDU(RowSource rs, int bufSize, StringWriteMode writeMode,
          int stringTruncationLength) : this(rs, bufSize, writeMode, stringTruncationLength, true, ' ')
        {
        }

        public StreamedBinaryTableHDU(RowSource rs, int bufSize, StringWriteMode writeMode,
          int stringTruncationLength, bool padLeft, char padChar)
		    {
          _writerMap = CreateWriterMap();
          _rs = rs;
          _buf = new byte[bufSize];
          _byteRenderers = new ByteRenderer[_rs.ModelRow.Length];
          _writeMode = writeMode;
          _stringTruncationLength = stringTruncationLength;
          _padLeft = padLeft;
          _padChar = padChar;
          SetupRenderers();
		    }
        #endregion

        protected void SetupRenderers()
        {
          Array[] modelRow = ReplaceTroolean(_rs.ModelRow);
          Array[] modelRow2 = null;
          switch(_writeMode)
          {
            case StringWriteMode.HEAP:
            case StringWriteMode.PAD:
              modelRow2 = CopyModelRowReplaceStrings(modelRow, new int[2]);
              modelRow2 = CopyModelRowStripUnknowns(modelRow2, new byte[1]);
    //          _rowSizeInBytes = ArrayFuncs.ComputeSize(CopyModelRowReplaceStrings(modelRow, new int[2]));
              _rowSizeInBytes = ArrayFuncs.ComputeSize(modelRow2);
              modelRow2 = CopyModelRowStripUnknowns(modelRow, new byte[1]);
              //myHeader = ManufactureHeader(modelRow, _rs.ColumnNames, _rs.TNULL, _rs.NRows);
              myHeader = ManufactureHeader(modelRow2, _rs.ColumnNames, _rs.TNULL, _rs.NRows);
              _stringArrayRenderer = ByteRenderer.STRING_ARRAY_RENDERER_HEAP;
              break;
            case StringWriteMode.TRUNCATE:
              modelRow2 = CopyModelRowReplaceStrings(modelRow, new String[]{new String(' ', _stringTruncationLength)});
              modelRow2 = CopyModelRowStripUnknowns(modelRow2, new byte[1]);
              _rowSizeInBytes = ArrayFuncs.ComputeSize(modelRow2);
              myHeader = ManufactureHeader(modelRow2, _rs.ColumnNames, _rs.TNULL, _rs.NRows);
              _stringArrayRenderer =
                new ByteRenderer.StringArrayByteRendererTruncate(_stringTruncationLength, _padChar, _padLeft, false);
              break;
          }
          
          _hasStrings = false;
          for(int i = 0; i < _rs.ModelRow.Length; ++i)
          {
            _byteRenderers[i] = ByteRenderer.GetByteRenderer(_rs.ModelRow[i].GetType());
            
            if(_rs.ModelRow[i] is String[])
            {
              _byteRenderers[i] = _stringArrayRenderer;
              _hasStrings = true;
            }

            if(_byteRenderers[i].GetType() == typeof(ByteRenderer.NullByteRenderer))
            {
              myHeader.AddComment("COLUMN " + (i + 1) + " NULL DUE TO UNKNOWN TYPE.");
            }
          }
        }

        protected static Array[] CopyModelRowStripUnknowns(Array[] a, Array a2)
        {
          Array[] result = new Array[a.Length];

          for(int i = 0; i < a.Length; ++i)
          {
            if(a[i] == null || (a[i].GetType() != typeof(String[]) && ByteRenderer.GetByteRenderer(a[i].GetType()).GetType() == typeof(ByteRenderer.NullByteRenderer)))
            {
              result[i] = a2;
            }
            else
            {
              result[i] = a[i];
            }
          }

          return result;
        }

        protected static Array[] CopyModelRowReplaceStrings(Array[] a, Array a2)
        {
          Array[] result = new Array[a.Length];

          for(int i = 0; i < a.Length; ++i)
          {
            if(a[i] is String[])
            {
              result[i] = a2;
            }
            else
            {
              result[i] = a[i];
            }
          }

          return result;
        }

        protected static String CreateTempFilename()
        {
          return Fits.TempDirectory + "\\" + DateTime.Now.Ticks;
        }

        protected static Array[] ReplaceTroolean(Array[] row)
        {
          Array[] result = new Array[row.Length];

          for(int i = 0; i < result.Length; ++i)
          {
            result[i] = row[i];
            if(row[i].GetType() == typeof(Troolean[]))
            {
              int[] dims = new int[row[i].Rank];
              for(int j = 0; j < dims.Length; ++j)
              {
                dims[j] = row[i].GetLength(j);
              }
              result[i] = Array.CreateInstance(typeof(bool), dims);
            }
          }

          return result;
        }

        protected static Header ManufactureHeader(Array[] row, String[] columnNames,
                                                  Object[] tnull, int nRows)
        {
          Header hdr = new Header();
          Object[][] table = new Object[1][];
          table[0] = row;
          new BinaryTable(table).FillHeader(hdr);

          if(columnNames == null)
          {
            columnNames = new String[row.Length];
            for(int i = 0; i < columnNames.Length; ++i)
            {
              columnNames[i] = "Column" + (i + 1);
            }
          }

          for(HeaderCard c = hdr.NextCard(); c != null; c = hdr.NextCard());

          Type t = null;
          for(int i = 0; i < columnNames.Length; ++i)
          {
            if(!hdr.ContainsKey("TTYPE" + (i + 1)))
            {
              hdr.AddLine(new HeaderCard("TTYPE" + (i + 1), columnNames[i], null));
            }

            t = row[i].GetType();
            if(t == typeof(short[]))
            {
              hdr.AddLine(new HeaderCard("TNULL" + (i + 1), (short)tnull[i], null));
            }
            else if(t == typeof(int[]))
            {
              hdr.AddLine(new HeaderCard("TNULL" + (i + 1), (int)tnull[i], null));
            }
            else if(t == typeof(long[]))
            {
              hdr.AddLine(new HeaderCard("TNULL" + (i + 1), (long)tnull[i], null));
            }
          }

          hdr.RemoveCard("NAXIS2");
          hdr.SetNaxis(2, nRows);

          return hdr;
        }


        #region write methods
        public override void Write(ArrayDataIO s)
        {
          System.Threading.Monitor.Enter(this);
          //((Writer)_writerMap[new Config(_rs.NRows != RowSource.NA, _hasStrings, s.CanSeek, _writeMode)]).Write(s);
          Config c = new Config(_rs.NRows != RowSource.NA, _hasStrings, s.CanSeek, _writeMode);
          ((Writer)_writerMap[c]).Write(s);
          s.Flush();
          System.Threading.Monitor.Exit(this);
        }

        protected Hashtable CreateWriterMap()
        {
          Hashtable result = new Hashtable();
          Writer w1 = new OnePassWriter(this);
          Writer w2 = new FixWriter(this);
          Writer w3 = new HeapWriterWithTempTable(this);
          Writer w4 = new HeapWriter(this);
          Writer w5 = new HeapWriterWithTempTable(this);
          Writer w6 = new PadWriter(this);

          // have nrows, have strings, destination seekable, write mode
          result.Add(new Config(true, true, true, StringWriteMode.TRUNCATE), w1);
          result.Add(new Config(true, true, true, StringWriteMode.HEAP), w4);
          result.Add(new Config(true, true, true, StringWriteMode.PAD), w6);
          result.Add(new Config(true, true, false, StringWriteMode.TRUNCATE), w1);
          result.Add(new Config(true, true, false, StringWriteMode.HEAP), w5);
          result.Add(new Config(true, true, false, StringWriteMode.PAD), w6);

          result.Add(new Config(true, false, true, StringWriteMode.TRUNCATE), w1);
          result.Add(new Config(true, false, true, StringWriteMode.HEAP), w1);
          result.Add(new Config(true, false, true, StringWriteMode.PAD), w1);
          result.Add(new Config(true, false, false, StringWriteMode.TRUNCATE), w1);
          result.Add(new Config(true, false, false, StringWriteMode.HEAP), w1);
          result.Add(new Config(true, false, false, StringWriteMode.PAD), w1);

          result.Add(new Config(false, true, true, StringWriteMode.TRUNCATE), w2);
          result.Add(new Config(false, true, true, StringWriteMode.HEAP), w4);
          result.Add(new Config(false, true, true, StringWriteMode.PAD), w6);
          result.Add(new Config(false, true, false, StringWriteMode.TRUNCATE), w3);
          result.Add(new Config(false, true, false, StringWriteMode.HEAP), w5);
          result.Add(new Config(false, true, false, StringWriteMode.PAD), w6);

          result.Add(new Config(false, false, true, StringWriteMode.TRUNCATE), w2);
          result.Add(new Config(false, false, true, StringWriteMode.HEAP), w2);
          result.Add(new Config(false, false, true, StringWriteMode.PAD), w2);
          result.Add(new Config(false, false, false, StringWriteMode.TRUNCATE), w3);
          result.Add(new Config(false, false, false, StringWriteMode.HEAP), w3);
          result.Add(new Config(false, false, false, StringWriteMode.PAD), w3);

          return result;
        }

        protected class Config
        {
          public Config(bool haveNRows, bool haveStrings, bool seekable, StringWriteMode mode)
          {
            _haveNRows = haveNRows;
            _haveStrings = haveStrings;
            _seekable = seekable;
            _mode = mode;
          }

          public override bool Equals(object obj)
          {
            return obj is Config &&
              ((Config)obj)._haveNRows == _haveNRows &&
              ((Config)obj)._haveStrings == _haveStrings &&
              ((Config)obj)._seekable == _seekable &&
              ((Config)obj)._mode == _mode;
          }

          public override int GetHashCode()
          {
            return (_haveNRows + "" + _haveStrings + "" + _seekable + "" + _mode).GetHashCode();
          }

          public override string ToString()
          {
            return "_haveNRows = " + _haveNRows + " _haveStrings = " + _haveStrings +
              " _seekable = " + _seekable + " _mode = " + _mode;
          }

          bool _haveNRows;
          bool _haveStrings;
          bool _seekable;
          StringWriteMode _mode;
        }

        protected abstract class Writer
        {
          public abstract void Write(ArrayDataIO s);
          protected StreamedBinaryTableHDU _table;
        }

        protected class OnePassWriter : Writer
        {
          public OnePassWriter(StreamedBinaryTableHDU table)
          {
            _table = table;
          }

          #region Writer Members
          public override void Write(ArrayDataIO s)
          {
            _table.WriteOnePass(s);
          }
          #endregion
        }

        protected class FixWriter : Writer
        {
          public FixWriter(StreamedBinaryTableHDU table)
          {
            _table = table;
          }

          #region Writer Members
          public override void Write(ArrayDataIO s)
          {
            _table.WriteThenFix(s);
          }
          #endregion
        }

        protected class HeapWriter : Writer
        {
          public HeapWriter(StreamedBinaryTableHDU table)
          {
            _table = table;
          }

          #region Writer Members
          public override void Write(ArrayDataIO s)
          {
            _table.WriteHeapOutputWithTempHeapFile(s);
          }
          #endregion
        }

        protected class HeapWriterWithTempTable : Writer
        {
          public HeapWriterWithTempTable(StreamedBinaryTableHDU table)
          {
            _table = table;
          }

          #region Writer Members
          public override void Write(ArrayDataIO s)
          {
            _table.WriteHeapOutputWithTempTableAndHeapFiles(s);
          }
          #endregion
        }

        protected class PadWriter : Writer
        {
          public PadWriter(StreamedBinaryTableHDU table)
          {
            _table = table;
          }

          #region Writer Members
          public override void Write(ArrayDataIO s)
          {
            _table.WritePadOutput(s);
          }
          #endregion
        }

        protected int WriteTable(ArrayDataIO s)
        {
          myHeader.Write(s);

          // write the table
          int nRows = 0;
          for(Array[] els = _rs.GetNextRow(ref _row); els != null;)
          {
            ++nRows;
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Write(els[col], s);
            }

            els = _rs.GetNextRow(ref _row);
          }

          // pad
          s.Write(new byte[FitsUtil.Padding((long)nRows * (long)_rowSizeInBytes)]);

          return nRows;
        }

        /// <summary>
        ///   Writes this binary table in one pass.
        ///   Requires foreknowledge of nRows and
        ///   either no strings or truncated string output.
        /// </summary>
        /// <param name="s">The destination stream.</param>
        protected void WriteOnePass(ArrayDataIO s)
        {
          WriteTable(s);
        }

        /// <summary>
        ///   Writes this binary table in one pass,
        ///   then seeks back to fix NAXIS2.  Requires
        ///   s to be seekable and
        ///   either no strings or truncated string output.
        /// </summary>
        /// <param name="s">The destination stream.</param>
        protected void WriteThenFix(ArrayDataIO s)
        {
          long headerMark = s.Position;
          int nRows = WriteTable(s);
          long endMark = s.Position;

          myHeader.RemoveCard("NAXIS2");
          myHeader.SetNaxis(2, nRows);
          myHeader.InsertComment("No comment.", "THEAP");
          myHeader.RemoveCard("THEAP");
          s.Seek(headerMark, SeekOrigin.Begin);
          myHeader.Write(s);
          s.Seek(endMark, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Writes this binary table with heap temp file if necessary,
        ///   then fixing nRows and PCOUNT in header,
        ///   then if necessary copying heap file to destination stream.
        /// </summary>
        /// <param name="s">The destination stream.</param>
        /// steps:
        /// 1) write the header to the main stream
        ///    write the table to the main stream
        ///    byterenderers write data to the heap if necessary
        ///    byterenderers return heap positions and lengths if necessary
        ///    these are returned as a byte sequence like any other data
        ///    and are written to the table like any other data
        /// 2) fix the header
        /// 3) write the heap tempfile to the main stream
        /// what a pain
        protected void WriteHeapOutputWithTempHeapFile(ArrayDataIO s)
        {
          String heapFilename = CreateTempFilename() + "heap.tmp";
          HeapStream heapS = null;
          int[] maxColWidths = null;

          if(_hasStrings)
          {
            maxColWidths = new int[_byteRenderers.Length];
            heapS = new HeapStream(new FileStream(heapFilename, FileMode.Create));
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Heap = heapS;
              maxColWidths[col] = -1;
            }
          }

          #region 1) write the header and the table
          // prep header to make sure it will line up properly with the table later on.
          // since we made the header, we know that anything we add later
          // (except THEAP, accounted for here) will already have been there,
          // so we're not inflating the header
          myHeader.RemoveCard("THEAP");
          if(_hasStrings && _writeMode == StringWriteMode.HEAP)
          {
            myHeader.AddValue("THEAP", 0, null);
          }
          long headerMark = s.Position;
          myHeader.Write(s);

          int nRows = 0;
          for(Array[] els = _rs.GetNextRow(ref _row); els != null;)
          {
            ++nRows;
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Write(els[col], s);
              if(els[col] is String[])
              {
                maxColWidths[col] = maxColWidths[col] < ((String[])els[col])[0].Length ?
                  ((String[])els[col])[0].Length : maxColWidths[col];
              }
            }

            els = _rs.GetNextRow(ref _row);
          }

          // pad the table.  if there's a heap, pad the heap instead
          if(!_hasStrings)
          {
            int pad = FitsUtil.Padding((long)nRows * (long)_rowSizeInBytes);
            s.Write(new byte[pad], 0, pad);
          }
          s.Flush();
          #endregion

          #region 2) fix the header and write it to the main stream
          myHeader.RemoveCard("NAXIS2");
          myHeader.SetNaxis(2, nRows);
          // shoehorn correct heap information into header
          // PCOUNT, THEAP, and TFORMn
          // fix NAXIS1
          if(_hasStrings)
          {
            long theap = (long)nRows * (long)_rowSizeInBytes;
            int pad = FitsUtil.Padding(theap + heapS.Position);
            int pcount = (int)heapS.Position + pad;
            // here we correct for swapping out actual strings with heap indices/lengths
            myHeader.RemoveCard("NAXIS1");
            myHeader.InsertCard(new HeaderCard("NAXIS1", _rowSizeInBytes, null), "NAXIS2");
            myHeader.RemoveCard("PCOUNT");
            myHeader.InsertCard(new HeaderCard("PCOUNT", pcount, "Length of heap area in bytes"),
              "GCOUNT");
            myHeader.RemoveCard("THEAP");
            // can't fit a long in here!
            if(pcount > 0 && _writeMode == StringWriteMode.HEAP)
            {
              myHeader.AddValue("THEAP", (int)theap, "Position of heap wrt start of binary table");
            }

            // fix the TFORMn entries for string columns
            IEnumerator ie = null;
            bool found = false;
            for(int i = 0; i < maxColWidths.Length; ++i, found = false)
            {
              if(maxColWidths[i] > -1)
              {
                ie = myHeader.GetEnumerator();
                ie.MoveNext();
                for(int j = 0; !found && ie.Current != null; ++j, ie.MoveNext())
                {
                  if(("TFORM" + (i + 1)).Equals(((DictionaryEntry)ie.Current).Key))
                  {
                    myHeader.RemoveCard(j);
                    myHeader.
                      InsertCard(new HeaderCard("TFORM" + (i + 1),
                      "1PA(" + maxColWidths[i] + ")",
                      null), j);
                    found = true;
                  }
                }
              }
            }
          }
          // rewrite the header
          long heapMark = s.Position;
          s.Seek(headerMark, SeekOrigin.Begin);
          myHeader.Write(s);
          #endregion

          #region 3) write the heap tempfile to the main stream
          if(_hasStrings)
          {
            // calculate the pad
            int pad = FitsUtil.Padding((long)nRows * (long)_rowSizeInBytes + heapS.Position);

            s.Seek(heapMark, SeekOrigin.Begin);

            // write heap to the main stream
            heapS.Seek(0, SeekOrigin.Begin);
            for(int nRead = heapS.Read(_buf, 0, _buf.Length); nRead > 0;)
            {
              s.Write(_buf, 0, nRead);
              nRead = heapS.Read(_buf, 0, _buf.Length);
            }
            heapS.Close();
            File.Delete(heapFilename);

            // pad the file
            s.Write(new byte[pad], 0, pad);
          }
          #endregion
        }

        /// <summary>
        ///   Writes this binary table with data first going
        ///   to a temp file (and heap file if necessary),
        ///   then with header going to destination stream,
        ///   then copying data from temp file to destination stream,
        ///   then if necessary copying heap file to destination stream.
        /// </summary>
        /// <param name="s">The destination stream.</param>
        /// steps:
        /// 1) write the table to a tempfile
        ///    byterenderers write data to the heap if necessary
        ///    byterenderers return heap positions and lengths if necessary
        ///    these are returned as a byte sequence like any other data
        ///    and are written to the table like any other data
        /// 2) fix the header
        ///    write the header to the main stream
        /// 3) write the table tempfile to the main stream
        /// 4) write the heap tempfile to the main stream
        /// what a pain
        protected void WriteHeapOutputWithTempTableAndHeapFiles(ArrayDataIO s)
        {
          String tempFilename = CreateTempFilename() + "temp.tmp";
          String heapFilename = CreateTempFilename() + "heap.tmp";
          Stream tempS = new ActualBufferedStream(new FileStream(tempFilename, FileMode.Create));
          HeapStream heapS = null;
          int[] maxColWidths = null;
          bool _doHeap = _hasStrings && _writeMode != StringWriteMode.TRUNCATE;

          if(_doHeap)
          {
            maxColWidths = new int[_byteRenderers.Length];
            heapS = new HeapStream(new FileStream(heapFilename, FileMode.Create));
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Heap = heapS;
              maxColWidths[col] = -1;
            }
          }

          #region 1) write the table
          int nRows = 0;
          for(Array[] els = _rs.GetNextRow(ref _row); els != null;)
          {
            ++nRows;
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Write(els[col], tempS);
              if(_doHeap && els[col] is String[])
              {
                maxColWidths[col] = maxColWidths[col] < ((String[])els[col])[0].Length ?
                  ((String[])els[col])[0].Length : maxColWidths[col];
              }
            }

            els = _rs.GetNextRow(ref _row);
          }
          tempS.Flush();
          #endregion

          #region 2) fix the header and write it to the main stream
          myHeader.RemoveCard("NAXIS2");
          myHeader.SetNaxis(2, nRows);
          // shoehorn correct heap information into header
          // PCOUNT, THEAP, and TFORMn
          // fix NAXIS1
          if(_doHeap)
          {
            heapS.Flush();
            int theap = (nRows * _rowSizeInBytes);
            int pad = FitsUtil.Padding((long)theap + heapS.Position);
            int pcount = (int)heapS.Position + pad;
            // here we correct for swapping out actual strings with heap indices/lengths
            myHeader.RemoveCard("NAXIS1");
            myHeader.InsertCard(new HeaderCard("NAXIS1", _rowSizeInBytes, null), "NAXIS2");
            myHeader.RemoveCard("PCOUNT");
            myHeader.InsertCard(new HeaderCard("PCOUNT", pcount, "Length of heap area in bytes"),
              "GCOUNT");
            myHeader.RemoveCard("THEAP");
            myHeader.AddValue("THEAP", theap, "Position of heap wrt start of binary table");
          }

          // fix the TFORMn entries for string columns
          IEnumerator ie = null;
          bool found = false;
          //for(int i = 0; i < maxColWidths.Length; ++i, found = false)
          for(int i = 0; i < _rs.ModelRow.Length; ++i, found = false)
          {
            if(_rs.ModelRow[i] is String[])
            {
              ie = myHeader.GetEnumerator();
              ie.MoveNext();
              for(int j = 0; !found && ie.Current != null; ++j, ie.MoveNext())
              {
                if(("TFORM" + (i + 1)).Equals(((DictionaryEntry)ie.Current).Key))
                {
                  if(_doHeap)
                  {
                    myHeader.RemoveCard(j);
                    myHeader.
                      InsertCard(new HeaderCard("TFORM" + (i + 1),
                      "1PA(" + maxColWidths[i] + ")",
                      null), j);
                    found = true;
                  }
                  else
                  {
                    myHeader.RemoveCard(j);
                    myHeader.
                      InsertCard(new HeaderCard("TFORM" + (i + 1),
                      _stringTruncationLength + "A",
                      null), j);
                    found = true;
                  }
                }
              }
            }
          }
          myHeader.Write(s);
          #endregion

          #region 3) write the table tempfile to the main stream
          tempS.Seek(0, SeekOrigin.Begin);
          for(int nRead = tempS.Read(_buf, 0, _buf.Length); nRead > 0;)
          {
            s.Write(_buf, 0, nRead);
            nRead = tempS.Read(_buf, 0, _buf.Length);
          }
          tempS.Close();
          File.Delete(tempFilename);

          // if there's a heap, pad the heap instead of the table
          if(!_doHeap)
          {
            int pad = FitsUtil.Padding((long)nRows * (long)_rowSizeInBytes);
            s.Write(new byte[pad], 0, pad);
          }
          #endregion

          #region 4) write the heap tempfile to the main stream
          if(_doHeap)
          {
            // calculate the pad
            int pad = FitsUtil.Padding((long)nRows * (long)_rowSizeInBytes + heapS.Position);

            // write to the main stream
            heapS.Seek(0, SeekOrigin.Begin);
            for(int nRead = heapS.Read(_buf, 0, _buf.Length); nRead > 0;)
            {
              s.Write(_buf, 0, nRead);
              nRead = heapS.Read(_buf, 0, _buf.Length);
            }
            heapS.Close();
            File.Delete(heapFilename);

            // pad the file
            s.Write(new byte[pad], 0, pad);
          }
          #endregion
        }

        /// <summary>
        ///   Writes this binary table with data first going
        ///   to a temp file (and heap file if necessary),
        ///   then with header going to destination stream,
        ///   then merging heap with table data if necessary
        ///   and copying these to destination stream.
        /// </summary>
        /// <param name="s">The destination stream.</param>
        /// steps:
        /// 1) write the table to a tempfile
        ///    byterenderers write data to the heap if necessary
        ///    byterenderers return heap positions and lengths if necessary
        ///    these are returned as a byte sequence like any other data
        ///    and are written to the table like any other data
        /// 2) fix the header
        ///    write the header to the main stream
        /// 3) write the table tempfile to the main stream, merging heap if necessary
        /// what a pain
        protected void WritePadOutput(ArrayDataIO s)
        {
          String tempFilename = CreateTempFilename() + "temp.tmp";
          String heapFilename = CreateTempFilename() + "heap.tmp";
          Stream tempS = new ActualBufferedStream(new FileStream(tempFilename, FileMode.Create));
          Stream heapS = null;
          //Stream tempS = new BufferedStream(new FileStream(tempFilename, FileMode.Create), 4096);
          int[] maxColWidths = null;
          int[] stringIndices = GetStringIndices(_rs.ModelRow);
          int[] byteWidths = ComputeByteWidths(CopyModelRowStripUnknowns(ReplaceTroolean(_rs.ModelRow), new byte[1]));
          int nRows = 0;
          int maxColWidth = 0;

          if(_hasStrings)
          {
            maxColWidths = new int[_byteRenderers.Length];
            heapS = new HeapStream(new FileStream(heapFilename, FileMode.Create));
            //heapS = new BufferedStream(new FileStream(heapFilename, FileMode.Create));
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Heap = heapS;
              maxColWidths[col] = -1;
            }
          }

          #region 1) write the table
          for(Array[] els = _rs.GetNextRow(ref _row); els != null;)
          {
            ++nRows;
            for(int col = 0; col < _byteRenderers.Length; ++col)
            {
              _byteRenderers[col].Write(els[col], tempS);
              if(els[col] is String[] && maxColWidths[col] < ((String[])els[col])[0].Length)
              {
                maxColWidths[col] = ((String[])els[col])[0].Length;

                if(maxColWidth < maxColWidths[col])
                {
                  maxColWidth = maxColWidths[col];
                }
              }
            }

            els = _rs.GetNextRow(ref _row);
          }
          tempS.Flush();
          heapS.Flush();
          #endregion

          #region 2) fix the header and write it to the main stream
          if(_hasStrings)
          {
            // fix NAXIS1, NAXIS2
            Array[] modelRow2 = CopyModelRowReplaceStrings(ReplaceTroolean(_rs.ModelRow), null);
            //modelRow2 = CopyModelRowStripUnknowns(modelRow2, new byte[1]);
            for(int i = 0; i < modelRow2.Length; ++i)
            {
              if(modelRow2[i] == null)
              {
                modelRow2[i] = new String[]{new String(' ', maxColWidths[i])};
                myHeader.RemoveCard("TFORM" + (i + 1));
                myHeader.InsertValue("TFORM" + (i + 1), maxColWidths[i] + "A", null, "TDIM" + (i + 1));
              }
            }
            modelRow2 = CopyModelRowStripUnknowns(modelRow2, new byte[1]);
            myHeader.RemoveCard("NAXIS1");
            myHeader.InsertValue("NAXIS1", ArrayFuncs.ComputeSize(modelRow2), "row width in bytes", "NAXIS2");
            myHeader.RemoveCard("NAXIS2");
            myHeader.InsertValue("NAXIS2", nRows, "number of rows", "PCOUNT");
            myHeader.RemoveCard("THEAP");
          }
          myHeader.Write(s);
          #endregion

          #region 3) write the table tempfile to the main stream
          tempS.Seek(0, SeekOrigin.Begin);
          heapS.Seek(0, SeekOrigin.Begin);
          // man, if you can't even fit a row into memory, I give up
          byte[] row = new byte[_rowSizeInBytes]; // this is the old size
          byte[] padBuf = SupportClass.ToByteArray(new String(_padChar, maxColWidth));
          int len = 0;
          int off = 0;
          for(int nRead = tempS.Read(row, 0, row.Length), rowOffset = 0; nRead > 0; rowOffset = 0)
          {
            for(int i = 0; i < byteWidths.Length; ++i)
            {
              if(stringIndices[i] != -1)
              {
                Array.Reverse(row, stringIndices[i], 4); // fix the length bytes
                Array.Reverse(row, stringIndices[i] + 4, 4); // fix the pos bytes
                len = BitConverter.ToInt32(row, stringIndices[i]);
                off = BitConverter.ToInt32(row, stringIndices[i] + 4);
                if(_padLeft)
                {
                  s.Write(padBuf, 0, maxColWidths[i] - len);
                  heapS.Seek(off, SeekOrigin.Begin);
                  int bufread = heapS.Read(_buf, 0, len);
                  s.Write(_buf, 0, len);
                }
                else
                {
                  heapS.Seek(off, SeekOrigin.Begin);
                  heapS.Read(_buf, 0, len);
                  s.Write(_buf, 0, len);
                  s.Write(padBuf, 0, maxColWidths[i] - len);
                }
                rowOffset += 8; // advance 2 ints into the row
              }
              else
              {
                // s better be buffered, or this is going to be slow.  But since s is ArrayDataIO,
                // and the only current concrete ArrayDataIO implementations are buffered,
                // I think we're good.
                // **** MAKE SURE BUFFEREDSTREAM USED BY BUFFEREDDATASTREAM IS GOOD *****
                s.Write(row, rowOffset, byteWidths[i]);
                rowOffset += byteWidths[i];
              }
            }
            nRead = tempS.Read(row, 0, row.Length);
          }
          tempS.Close();
          heapS.Close();
          File.Delete(tempFilename);
          File.Delete(heapFilename);

          // pad the table
          int tableWidth = 0;
          for(int i = 0; i < byteWidths.Length; ++i)
          {
            if(stringIndices[i] != -1)
            {
              tableWidth += maxColWidths[i];
            }
            else
            {
              tableWidth += byteWidths[i];
            }
          }
          int pad = FitsUtil.Padding((long)nRows * (long)tableWidth);
          s.Write(new byte[pad], 0, pad);
          #endregion
        }

        #endregion

        protected static int[] ComputeByteWidths(Array[] a)
        {
          int[] result = new int[a.Length];

          for(int i = 0; i < a.Length; ++i)
          {
            result[i] = ArrayFuncs.ComputeSize(a[i]);
          }

          return result;
        }

        protected static int[] GetStringIndices(Array[] a)
        {
          int[] result = new int[a.Length];
          for(int i = 0, offset = 0; i < a.Length; ++i)
          {
            result[i] = -1;
            if(a[i] is String[])
            {
              result[i] = offset;
              offset += 8; // two ints, position & length
            }
            else if(a[i] is Troolean[])
            {
              offset += 1; // one char, 'T', 'F', or null
            }
            else
            {
              offset += ArrayFuncs.ComputeSize(a[i]);
            }
          }

          return result;
        }

        public override void Info()
        {
        }

        internal override Data ManufactureData()
        {
          return null;
        }

        protected RowSource _rs;
        protected Array[] _row;
        protected int _rowSizeInBytes;
        protected byte[] _buf;
        protected ByteRenderer[] _byteRenderers;
        protected bool _hasStrings;
        protected ByteRenderer _stringArrayRenderer;
        protected Hashtable _writerMap;

        protected StringWriteMode _writeMode;
        protected int _stringTruncationLength;
        protected bool _padLeft = true;
        protected char _padChar = ' ';
    }
}

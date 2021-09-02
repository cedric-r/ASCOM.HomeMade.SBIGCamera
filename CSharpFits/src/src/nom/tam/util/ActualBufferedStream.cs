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
    ///  The comparison between inline buffering versus System.IO.BufferedStream versus
    ///   unbuffered output, and System.IO.BufferedStream shows no improvement
    ///   whatsoever over unbuffered output, whereas hand-implemented buffered
    ///   output with the same buffer size settings show drastic improvement.
    ///   Hence this class.
    /// </summary>
    public class ActualBufferedStream : AdapterStream
    {
        public static readonly int DEFAULT_BUFFER_SIZE = 4096;

        #region properties
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                _bufferSize = value;
                if (_writeBuf != null)
                {
                    _s.Write(_writeBuf, 0, _writeBufPos);
                }
                _writeBuf = new byte[_bufferSize];
                _readBuf = new byte[_bufferSize];
                _readBufStart = Math.Min(_readBufStart, _bufferSize - 1);
                _readBufEnd = Math.Min(_readBufEnd, _bufferSize - 1);
            }
        }
        #endregion

        public ActualBufferedStream(Stream targetStream)
            : this(targetStream, DEFAULT_BUFFER_SIZE)
        {
        }

        public ActualBufferedStream(Stream targetStream, int bufferSize)
            : base(targetStream)
        {
            BufferSize = bufferSize;
        }

        public override void Flush()
        {
            _s.Write(_writeBuf, 0, _writeBufPos);
            _writeBufPos = 0;
            _s.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (_bufferSize - _writeBufPos < count)
            {
                _s.Write(_writeBuf, 0, _writeBufPos);

                if (count < _bufferSize)
                {
                    _writeTemp = count;
                }
                else
                {
                    _writeTemp = _bufferSize;
                }

                Array.Copy(buffer, offset, _writeBuf, 0, _writeTemp);
                count -= _writeTemp;

                if (count < 0)
                {
                    count = 0;
                }
                offset += _writeTemp;
                _writeBufPos = _writeTemp;
            }

            Array.Copy(buffer, offset, _writeBuf, _writeBufPos, count);
            _writeBufPos += count;
        }

        public override void WriteByte(byte val)
        {
            if (_writeBuf.Length - _writeBufPos < 1)
            {
                _s.Write(_writeBuf, 0, _writeBufPos);
                _writeBufPos = 0;
            }
            _writeBuf[_writeBufPos++] = val;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Flush();
            ResetReadBuffer();

            return _s.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (_readTemp = _readBufEnd - _readBufStart, _readEOS = false; _readTemp < count && !_readEOS; )
            {
                // _readTemp is what's left in the buffer
                Array.Copy(_readBuf, _readBufStart, buffer, offset, _readTemp);
                count -= _readTemp;
                offset += _readTemp;
                _readBufStart = 0;
                _readBufEnd = _s.Read(_readBuf, _readBufStart, _bufferSize);
                _readTemp = _readBufEnd;
                if (_readBufEnd == 0)
                {
                    _readEOS = true;
                }
            }
            _readTemp = Math.Min(count, _readTemp);
            Array.Copy(_readBuf, _readBufStart, buffer, offset, _readTemp);
            _readBufStart += _readTemp;

            return offset + _readTemp;
        }

        public override int ReadByte()
        {
            if (_readBufEnd - _readBufStart < 1)
            {
                _readBufStart = 0;
                _readBufEnd = _s.Read(_readBuf, _readBufStart, _bufferSize);
            }

            if (_readBufEnd - _readBufStart > 0)
            {
                return _readBuf[_readBufStart++];
            }

            return -1;
        }

        protected void ResetReadBuffer()
        {
            _readBufStart = 0;
            _readBufEnd = 0;
        }

        protected int _bufferSize = DEFAULT_BUFFER_SIZE;
        protected byte[] _writeBuf = null;
        protected int _writeBufPos = 0;
        protected byte[] _readBuf = null;
        protected int _readBufStart = 0;
        protected int _readBufEnd = 0;
        protected int _writeTemp = 0;
        protected int _readTemp = 0;
        protected bool _readEOS = false;
    }
}

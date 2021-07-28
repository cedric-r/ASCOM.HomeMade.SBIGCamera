// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{

    /// <summary>
    /// BCD (Binary Coded Decimals) is a common way to encode integer
    /// numbers in a way that makes human-readable output somewhat
    /// simpler. In this encoding scheme, every decimal digit is encoded as
    /// either a single byte (8 bits), or a nibble (half of a byte, 4
    /// bits). This obviously wastes a lot of bits, but it makes translation
    /// into human-readable string much easier than traditional
    /// binary-to-decimal conversion process, which includes lots of
    /// divisions by 10.
    /// 
    /// For example, encoding integer 31337 in 8-digit, 8 bits per digit,
    /// big endian order of digits BCD format yields
    /// 
    /// ```
    /// 00 00 00 03 01 03 03 07
    /// ```
    /// 
    /// Encoding the same integer as 8-digit, 4 bits per digit, little
    /// endian order BCD format would yield:
    /// 
    /// ```
    /// 73 31 30 00
    /// ```
    /// 
    /// Using this type of encoding in Kaitai Struct is pretty
    /// straightforward: one calls for this type, specifying desired
    /// encoding parameters, and gets result using either `as_int` or
    /// `as_str` attributes.
    /// </summary>
    public partial class Bcd : KaitaiStruct
    {
        public Bcd(byte p_numDigits, byte p_bitsPerDigit, bool p_isLe, KaitaiStream p__io, KaitaiStruct p__parent = null, Bcd p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _numDigits = p_numDigits;
            _bitsPerDigit = p_bitsPerDigit;
            _isLe = p_isLe;
            f_asInt = false;
            f_asIntLe = false;
            f_lastIdx = false;
            f_asIntBe = false;
            _read();
        }
        private void _read()
        {
            _digits = new List<int>((int) (NumDigits));
            for (var i = 0; i < NumDigits; i++)
            {
                switch (BitsPerDigit) {
                case 4: {
                    _digits.Add(m_io.ReadBitsIntBe(4));
                    break;
                }
                case 8: {
                    _digits.Add(m_io.ReadU1());
                    break;
                }
                }
            }
        }
        private bool f_asInt;
        private int _asInt;

        /// <summary>
        /// Value of this BCD number as integer. Endianness would be selected based on `is_le` parameter given.
        /// </summary>
        public int AsInt
        {
            get
            {
                if (f_asInt)
                    return _asInt;
                _asInt = (int) ((IsLe ? AsIntLe : AsIntBe));
                f_asInt = true;
                return _asInt;
            }
        }
        private bool f_asIntLe;
        private int _asIntLe;

        /// <summary>
        /// Value of this BCD number as integer (treating digit order as little-endian).
        /// </summary>
        public int AsIntLe
        {
            get
            {
                if (f_asIntLe)
                    return _asIntLe;
                _asIntLe = (int) ((Digits[0] + (NumDigits < 2 ? 0 : ((Digits[1] * 10) + (NumDigits < 3 ? 0 : ((Digits[2] * 100) + (NumDigits < 4 ? 0 : ((Digits[3] * 1000) + (NumDigits < 5 ? 0 : ((Digits[4] * 10000) + (NumDigits < 6 ? 0 : ((Digits[5] * 100000) + (NumDigits < 7 ? 0 : ((Digits[6] * 1000000) + (NumDigits < 8 ? 0 : (Digits[7] * 10000000))))))))))))))));
                f_asIntLe = true;
                return _asIntLe;
            }
        }
        private bool f_lastIdx;
        private int _lastIdx;

        /// <summary>
        /// Index of last digit (0-based).
        /// </summary>
        public int LastIdx
        {
            get
            {
                if (f_lastIdx)
                    return _lastIdx;
                _lastIdx = (int) ((NumDigits - 1));
                f_lastIdx = true;
                return _lastIdx;
            }
        }
        private bool f_asIntBe;
        private int _asIntBe;

        /// <summary>
        /// Value of this BCD number as integer (treating digit order as big-endian).
        /// </summary>
        public int AsIntBe
        {
            get
            {
                if (f_asIntBe)
                    return _asIntBe;
                _asIntBe = (int) ((Digits[LastIdx] + (NumDigits < 2 ? 0 : ((Digits[(LastIdx - 1)] * 10) + (NumDigits < 3 ? 0 : ((Digits[(LastIdx - 2)] * 100) + (NumDigits < 4 ? 0 : ((Digits[(LastIdx - 3)] * 1000) + (NumDigits < 5 ? 0 : ((Digits[(LastIdx - 4)] * 10000) + (NumDigits < 6 ? 0 : ((Digits[(LastIdx - 5)] * 100000) + (NumDigits < 7 ? 0 : ((Digits[(LastIdx - 6)] * 1000000) + (NumDigits < 8 ? 0 : (Digits[(LastIdx - 7)] * 10000000))))))))))))))));
                f_asIntBe = true;
                return _asIntBe;
            }
        }
        private List<int> _digits;
        private byte _numDigits;
        private byte _bitsPerDigit;
        private bool _isLe;
        private Bcd m_root;
        private KaitaiStruct m_parent;
        public List<int> Digits { get { return _digits; } }

        /// <summary>
        /// Number of digits in this BCD representation. Only values from 1 to 8 inclusive are supported.
        /// </summary>
        public byte NumDigits { get { return _numDigits; } }

        /// <summary>
        /// Number of bits per digit. Only values of 4 and 8 are supported.
        /// </summary>
        public byte BitsPerDigit { get { return _bitsPerDigit; } }

        /// <summary>
        /// Endianness used by this BCD representation. True means little-endian, false is for big-endian.
        /// </summary>
        public bool IsLe { get { return _isLe; } }
        public Bcd M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}

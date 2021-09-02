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
    public class SupportClass
    {
        public static int StringIndexOf(String s, Char c, int startIndex)
        {
            int result = -1;

            try
            {
                result = s.IndexOf(c, startIndex);
            }
            catch (Exception)
            {
                result = -1;
            }

            return result;
        }

        /*******************************/
        /// <summary>This method is used as a dummy method to simulate VJ++ behavior</summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static long Identity(long literal)
        {
            return literal;
        }

        /// <summary>This method is used as a dummy method to simulate VJ++ behavior</summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static ulong Identity(ulong literal)
        {
            return literal;
        }

        /// <summary>This method is used as a dummy method to simulate VJ++ behavior</summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static float Identity(float literal)
        {
            return literal;
        }

        /// <summary>This method is used as a dummy method to simulate VJ++ behavior</summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static double Identity(double literal)
        {
            return literal;
        }

        /*******************************/
        /// <summary>Converts a string to an array of bytes</summary>
        /// <param name="sourceString">The string to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] ToByteArray(string sourceString)
        {
            byte[] byteArray = new byte[sourceString.Length];
            for (int index = 0; index < sourceString.Length; index++)
                byteArray[index] = (byte)sourceString[index];
            return byteArray;
        }

        /*******************************/
        public class Tokenizer
        {
            private System.Collections.ArrayList elements;
            private string source;
            //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
            private string delimiters = " \t\n\r";

            public Tokenizer(string source)
            {
                this.elements = new System.Collections.ArrayList();
                this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
                this.RemoveEmptyStrings();
                this.source = source;
            }

            public Tokenizer(string source, string delimiters)
            {
                this.elements = new System.Collections.ArrayList();
                this.delimiters = delimiters;
                this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
                this.RemoveEmptyStrings();
                this.source = source;
            }

            public int Count
            {
                get
                {
                    return (this.elements.Count);
                }
            }

            public bool HasMoreTokens()
            {
                return (this.elements.Count > 0);
            }

            public string NextToken()
            {
                string result;
                if (source == "") throw new System.Exception();
                else
                {
                    this.elements = new System.Collections.ArrayList();
                    this.elements.AddRange(this.source.Split(delimiters.ToCharArray()));
                    RemoveEmptyStrings();
                    result = (string)this.elements[0];
                    this.elements.RemoveAt(0);
                    this.source = this.source.Remove(this.source.IndexOf(result), result.Length);
                    this.source = this.source.TrimStart(this.delimiters.ToCharArray());
                    return result;
                }
            }

            public string NextToken(string delimiters)
            {
                this.delimiters = delimiters;
                return NextToken();
            }

            private void RemoveEmptyStrings()
            {
                //VJ++ does not treat empty strings as tokens
                for (int index = 0; index < this.elements.Count; index++)
                    if ((string)this.elements[index] == "")
                    {
                        this.elements.RemoveAt(index);
                        index--;
                    }
            }
        }

        /*******************************/
        public static bool FileCanWrite(System.IO.FileInfo file)
        {
            return (System.IO.File.GetAttributes(file.FullName) & System.IO.FileAttributes.ReadOnly) !=
                    System.IO.FileAttributes.ReadOnly;
        }

        /*******************************/
        public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
        {
            stream.Write(throwable.StackTrace);
            stream.Flush();
        }

        /*******************************/
        public class TextNumberFormat
        {
            // Declaration of fields
            private System.Globalization.NumberFormatInfo numberFormat;
            private enum formatTypes { General, Number, Currency, Percent };
            private int numberFormatType;
            private bool groupingActivated;
            private string separator;
            private int maxIntDigits;
            private int minIntDigits;
            private int maxFractionDigits;
            private int minFractionDigits;

            // CONSTRUCTORS
            public TextNumberFormat()
            {
                this.numberFormat = new System.Globalization.NumberFormatInfo();
                this.numberFormatType = (int)TextNumberFormat.formatTypes.General;
                this.groupingActivated = true;
                this.separator = this.GetSeparator((int)TextNumberFormat.formatTypes.General);
                this.maxIntDigits = 127;
                this.minIntDigits = 1;
                this.maxFractionDigits = 3;
                this.minFractionDigits = 0;
            }

            private TextNumberFormat(TextNumberFormat.formatTypes theType, int digits)
            {
                this.numberFormat = System.Globalization.NumberFormatInfo.CurrentInfo;
                this.numberFormatType = (int)theType;
                this.groupingActivated = true;
                this.separator = this.GetSeparator((int)theType);
                this.maxIntDigits = 127;
                this.minIntDigits = 1;
                this.maxFractionDigits = 3;
                this.minFractionDigits = 0;
            }

            private TextNumberFormat(TextNumberFormat.formatTypes theType, System.Globalization.CultureInfo cultureNumberFormat, int digits)
            {
                this.numberFormat = cultureNumberFormat.NumberFormat;
                this.numberFormatType = (int)theType;
                this.groupingActivated = true;
                this.separator = this.GetSeparator((int)theType);
                this.maxIntDigits = 127;
                this.minIntDigits = 1;
                this.maxFractionDigits = 3;
                this.minFractionDigits = 0;
            }

            public static TextNumberFormat getTextNumberInstance()
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, 3);
                return instance;
            }

            public static TextNumberFormat getTextNumberCurrencyInstance()
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, 3);
                return instance;
            }

            public static TextNumberFormat getTextNumberPercentInstance()
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, 3);
                return instance;
            }

            public static TextNumberFormat getTextNumberInstance(System.Globalization.CultureInfo culture)
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, culture, 3);
                return instance;
            }

            public static TextNumberFormat getTextNumberCurrencyInstance(System.Globalization.CultureInfo culture)
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, culture, 3);
                return instance;
            }

            public static TextNumberFormat getTextNumberPercentInstance(System.Globalization.CultureInfo culture)
            {
                TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, culture, 3);
                return instance;
            }

            public System.Object Clone()
            {
                return (System.Object)this;
            }

            public override bool Equals(System.Object textNumberObject)
            {
                return System.Object.Equals((System.Object)this, textNumberObject);
            }

            public string FormatDouble(double number)
            {
                if (this.groupingActivated)
                {
                    return number.ToString(this.GetCurrentFormatString() + this.maxFractionDigits, this.numberFormat);
                }
                else
                {
                    return (number.ToString(this.GetCurrentFormatString() + this.maxFractionDigits, this.numberFormat)).Replace(this.separator, "");
                }
            }

            public string FormatLong(long number)
            {
                if (this.groupingActivated)
                {
                    return number.ToString(this.GetCurrentFormatString() + this.maxFractionDigits, this.numberFormat);
                }
                else
                {
                    return (number.ToString(this.GetCurrentFormatString() + this.maxFractionDigits, this.numberFormat)).Replace(this.separator, "");
                }
            }

            public static System.Globalization.CultureInfo[] GetAvailableCultures()
            {
                return System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
            }

            public override int GetHashCode()
            {
                return this.GetHashCode();
            }

            private string GetCurrentFormatString()
            {
                string currentFormatString = "n";  //Default value
                switch (this.numberFormatType)
                {
                    case (int)TextNumberFormat.formatTypes.Currency:
                        currentFormatString = "c";
                        break;

                    case (int)TextNumberFormat.formatTypes.General:
                        currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
                        break;

                    case (int)TextNumberFormat.formatTypes.Number:
                        currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
                        break;

                    case (int)TextNumberFormat.formatTypes.Percent:
                        currentFormatString = "p";
                        break;
                }
                return currentFormatString;
            }

            private string GetSeparator(int numberFormatType)
            {
                string separatorItem = " ";  //Default Separator

                switch (numberFormatType)
                {
                    case (int)TextNumberFormat.formatTypes.Currency:
                        separatorItem = this.numberFormat.CurrencyGroupSeparator;
                        break;

                    case (int)TextNumberFormat.formatTypes.General:
                        separatorItem = this.numberFormat.NumberGroupSeparator;
                        break;

                    case (int)TextNumberFormat.formatTypes.Number:
                        separatorItem = this.numberFormat.NumberGroupSeparator;
                        break;

                    case (int)TextNumberFormat.formatTypes.Percent:
                        separatorItem = this.numberFormat.PercentGroupSeparator;
                        break;
                }
                return separatorItem;
            }

            public bool GroupingUsed
            {
                get
                {
                    return (this.groupingActivated);
                }
                set
                {
                    this.groupingActivated = value;
                }
            }

            public int MinIntDigits
            {
                get
                {
                    return this.minIntDigits;
                }
                set
                {
                    this.minIntDigits = value;
                }
            }

            public int MaxIntDigits
            {
                get
                {
                    return this.maxIntDigits;
                }
                set
                {
                    this.maxIntDigits = value;
                }
            }

            public int MinFractionDigits
            {
                get
                {
                    return this.minFractionDigits;
                }
                set
                {
                    this.minFractionDigits = value;
                }
            }

            public int MaxFractionDigits
            {
                get
                {
                    return this.maxFractionDigits;
                }
                set
                {
                    this.maxFractionDigits = value;
                }
            }
        }
        /*******************************/
        /// <summary>
        /// Converts an array of bytes to an array of chars
        /// </summary>
        /// <param name="byteArray">The array of bytes to convert</param>
        /// <returns>The new array of chars</returns>
        public static char[] ToCharArray(byte[] byteArray)
        {
            char[] charArray = new char[byteArray.Length];
            byteArray.CopyTo(charArray, 0);
            return charArray;
        }

        /*******************************/
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2 << ~bits);
        }

        public static int URShift(int number, long bits)
        {
            return URShift(number, (int)bits);
        }

        public static long URShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2L << ~bits);
        }

        public static long URShift(long number, long bits)
        {
            return URShift(number, (int)bits);
        }

        public static char NextChar()
        {
            byte[] buf = new byte[2];
            Random.NextBytes(buf);
            char c = BitConverter.ToChar(buf, 0);
            while (!Char.IsLetterOrDigit(c) && !Char.IsPunctuation(c) && !Char.IsWhiteSpace(c))
            {
                Random.NextBytes(buf);
                c = BitConverter.ToChar(buf, 0);
            }

            return c;
        }

        static public System.Random Random = new System.Random();
    }
}

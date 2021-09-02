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
	
	public class FitsDate
	{
        /// <summary>Return the current date in FITS date format</summary>
		public static String FitsDateString
		{
			get
			{
				return GetFitsDateString(DateTime.Now, true);
			}
			
		}
		private int year = - 1;
		private int month = - 1;
		private int mday = - 1;
		private int hour = - 1;
		private int minute = - 1;
		private int second = - 1;
		private int millisecond = - 1;
		
		private DateTime date;
		
		/// <summary> Convert a FITS date string to a Java <CODE>Date</CODE> object.</summary>
		/// <param name="dStr">the FITS date</param>
		/// <returns>either <CODE>null</CODE> or a Date object</returns>
        /// <exception cref="FitsException">if <CODE>dStr</CODE> does not contain a valid FITS date.</exception>
		public FitsDate(String dStr)
		{
			// if the date string is null, we are done
			if (dStr == null)
			{
				return ;
			}
			
			// if the date string is empty, we are done
			dStr = dStr.Trim();
			if (dStr.Length == 0)
			{
				return ;
			}
			
			// if string contains at least 8 characters...
			int len = dStr.Length;
			if (len >= 8)
			{
				int first;
				
				// ... and there is a "/" in the string...
				first = dStr.IndexOf('-');
				if (first == 4 && first < len)
				{
					
					// ... this must be an new-style date
					BuildNewDate(dStr, first, len);
					
					// no "/" found; maybe it is an old-style date...
				}
				else
				{
					
					first = dStr.IndexOf('/');
					if (first > 1 && first < len)
					{
						
						// ... this must be an old-style date
						BuildOldDate(dStr, first, len);
					}
				}
			}
			
			if (year == - 1)
			{
				throw new FitsException("Bad FITS date string \"" + dStr + '"');
			}
		}
		
		private void BuildOldDate(String dStr, int first, int len)
		{
			int middle = dStr.IndexOf((System.Char) '/', first + 1);
            //int middle = SupportClass.StringIndexOf(dStr, '/', first + 1);
			if (middle > first + 2 && middle < len)
			{
				try
				{
					year = Int32.Parse(dStr.Substring(middle + 1)) + 1900;
					month = Int32.Parse(dStr.Substring(first + 1, (middle) - (first + 1)));
					mday = Int32.Parse(dStr.Substring(0, (first) - (0)));
				}
				catch(FormatException)
				{
					year = month = mday = - 1;
				}
			}
		}
		
		private void ParseTime(String tStr)
		{
			int first = tStr.IndexOf(':');
			if (first < 0)
			{
				throw new FitsException("Bad time");
			}
			
			int len = tStr.Length;
			
            int middle = tStr.IndexOf((System.Char) ':', first + 1);
            //int middle = SupportClass.StringIndexOf(tStr, ':', first + 1);
			if (middle > first + 2 && middle < len)
			{
				if (middle + 3 < len && tStr[middle + 3] == '.')
				{
					double d = Double.Parse(tStr.Substring(middle + 3));
					millisecond = (int)(d * 1000);
					
					len = middle + 3;
				}
				
				try
				{
					hour = Int32.Parse(tStr.Substring(0, (first) - (0)));
					minute = Int32.Parse(tStr.Substring(first + 1, (middle) - (first + 1)));
					second = Int32.Parse(tStr.Substring(middle + 1, (len) - (middle + 1)));
				}
				catch(FormatException)
				{
					hour = minute = second = millisecond = - 1;
				}
			}
		}
		
		private void BuildNewDate(String dStr, int first, int len)
		{
			// find the middle separator
			int middle = dStr.IndexOf((System.Char) '-', first + 1);
            //int middle = SupportClass.StringIndexOf(dStr, '-', first + 1);
			if (middle > first + 2 && middle < len)
			{
				try
				{
					// if this date string includes a time...
					if (middle + 3 < len && dStr[middle + 3] == 'T')
					{
						
						// ... try to parse the time
						try
						{
							ParseTime(dStr.Substring(middle + 4));
						}
						catch (FitsException)
						{
							throw new FitsException("Bad time in FITS date string \"" + dStr + "\"");
						}
						
						// we got the time; mark the end of the date string
						len = middle + 3;
					}
					
					// parse date string
					year = Int32.Parse(dStr.Substring(0, (first) - (0)));
					month = Int32.Parse(dStr.Substring(first + 1, (middle) - (first + 1)));
					mday = Int32.Parse(dStr.Substring(middle + 1, (len) - (middle + 1)));
				}
				catch (FormatException)
				{
					// yikes, something failed; reset everything
					year = month = mday = hour = minute = second = millisecond = - 1;
				}
			}
		}
		
		/// <summary>Get a Date object corresponding to this FITS date.</summary>
		/// <returns> The Date object.</returns>
		public virtual DateTime ToDate()
		{
          if(((Object)date) == null && year != -1)
          {
            date = hour == -1 ?
              new DateTime(year, month, mday, 0, 0, 0, 0) :
              new DateTime(year, month, mday, hour, minute, second,
                           millisecond == -1 ? 0 : millisecond);
          }

          return date;
		}


        /// <summary> Return the current date in FITS date format.</summary>
        public static String GetFitsDateString()
        {
            return GetFitsDateString(new DateTime(), true);
        }
    
		/// <summary>Create FITS format date string Date object.</summary>
		/// <param name="epoch">The epoch to be converted to FITS format.</param>
		public static String GetFitsDateString(DateTime epoch)
		{
			return GetFitsDateString(epoch, true);
		}
		
		/// <summary>Create FITS format date string. Note that the date is not rounded.</summary>
		/// <param name="epoch">The epoch to be converted to FITS format.</param>
		/// <param name="timeOfDay">Should time of day information be included?</param>
		public static String GetFitsDateString(DateTime epoch, bool timeOfDay)
		{
			try
			{
				System.Text.StringBuilder fitsDate = new System.Text.StringBuilder();
                if(timeOfDay)
                {
                  fitsDate.AppendFormat("{0:s}", epoch);
                  fitsDate.Append("." + epoch.Millisecond);
                }
                else
                {
                  fitsDate.AppendFormat("{0:D4}", epoch.Year);
                  fitsDate.Append("-");
                  fitsDate.AppendFormat("{0:D2}", epoch.Month);
                  fitsDate.Append("-");
                  fitsDate.AppendFormat("{0:D2}", epoch.Day);
                }

                return new String(fitsDate.ToString().ToCharArray());
			}
			catch(Exception)
			{
				return new String("".ToCharArray());
			}
		}
        /// <summary>
        /// Converts the buffer contents into String
        /// </summary>
        /// <returns></returns>
		public override String ToString()
		{
			if (year == - 1)
			{
				return "";
			}
			
			System.Text.StringBuilder buf = new System.Text.StringBuilder(23);
			buf.Append(year);
			buf.Append('-');
			if (month < 10)
			{
				buf.Append('0');
			}
			buf.Append(month);
			buf.Append('-');
			if (mday < 10)
			{
				buf.Append('0');
			}
			buf.Append(mday);
			
			if (hour != - 1)
			{
				
				buf.Append('T');
				if (hour < 10)
				{
					buf.Append('0');
				}
				
				buf.Append(hour);
				buf.Append(':');
				
				if (minute < 10)
				{
					buf.Append('0');
				}
				
				buf.Append(minute);
				buf.Append(':');
				
				if (second < 10)
				{
					buf.Append('0');
				}
				buf.Append(second);
				
				if (millisecond != - 1)
				{
					buf.Append('.');
					
					if (millisecond < 100)
					{
						if (millisecond < 10)
						{
							buf.Append("00");
						}
						else
						{
							buf.Append('0');
						}
					}
					buf.Append(millisecond);
				}
			}
			
			return buf.ToString();
		}
            
    }
}

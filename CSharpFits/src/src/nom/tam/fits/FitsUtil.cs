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
    using System.IO;
    using System.Security.Policy;
    using System.Net;
    using System.Collections;
    using nom.tam.util;

    /// <summary>This class comprises static
	/// utility functions used throughout
	/// the FITS classes.
    /// </summary>
	public class FitsUtil
	{
		/// <summary>Reposition a random access stream to a requested offset</summary>
		public static void Reposition(Object o, long offset)
		{
			if (o == null)
			{
				throw new FitsException("Attempt to reposition null stream");
			}
			if (!(o is RandomAccess) || offset < 0)
			{
				throw new FitsException("Invalid attempt to reposition stream " + o + " of type " + o.GetType().FullName + " to " + offset);
			}
			
			try
			{
				((RandomAccess)o).Seek(offset, SeekOrigin.Begin);
			}
			catch(IOException e)
			{
				throw new FitsException("Unable to repostion stream " + o + " of type " + o.GetType().FullName + " to " + offset + "   Exception:" + e);
			}
		}
		
		/// <summary>Find out where we are in a random access file</summary>
		public static long FindOffset(Object o)
		{
			if(o is ArrayDataIO && ((ArrayDataIO)o).CanSeek)//if(o is RandomAccess)
			{
				//return ((RandomAccess) o).FilePointer;
                return ((ArrayDataIO)o).Position;
			}
			else
			{
				return - 1;
			}
		}
		
		/// <summary>How many bytes are needed to fill the last 2880 block?</summary>
		public static int Padding(int size)
		{
			int mod = size % 2880;
			if (mod > 0)
			{
				mod = 2880 - mod;
			}
			return mod;
		}
		
        /// <summary>How many bytes are needed to fill the last 2880 block?</summary>
        public static int Padding(long size)
        {
            int mod = (int)(size % (long)2880);
            if (mod > 0)
            {
                mod = 2880 - mod;
            }
            return mod;
        }
    		
        /// <summary>Total size of blocked FITS element</summary>
		public static int AddPadding(int size)
		{
			return size + Padding(size);
		}

        // change suggested in .99 version: 
        /// <summary>Check if a file seems to be compressed.</summary>
		public static bool IsCompressed(String filename)
		{
            FileStream fis = null;
            // check if filename is an url. 
            // Required because FileInfo dosent take uri as parameter,
            // and throws argument exception.
            String lc = filename.ToLower();
            for (int i = 0; i < Fits.UrlProtocols.Length; i++) //check if url
            {
                if (lc.StartsWith(Fits.UrlProtocols[i]))
                {
                    // This seems to be a URL.hence get filename of the url to be passed to FileInfo constructor
                    int filelen = filename.Length;
                    return (filelen > 2 && (filename.Substring(filelen - 3, 3).ToLower().Equals(".gz")));
                    
                }
            }
            try
            {
                FileInfo test = new FileInfo(filename);
                if (File.Exists(test.FullName))
                {
                    fis = new FileStream(test.FullName, FileMode.Open);
                    if (fis.ReadByte() == 0x1f && fis.ReadByte() == 0x8b)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                // Ignore for the moment, but we'll probably fail when we
                // read this file.  However we don't know for sure
                // here that we are going to read the file right away,
                // so maybe just falling down to the filename test is best.
            }
            finally
            {
                if (fis != null)
                {
                    try
                    {
                        fis.Close();
                    }
                    catch (IOException e)
                    {
                        System.Console.Out.WriteLine("Exception: " + e);
                    }
                }
            }

            int len = filename.Length;
            return (len > 2 && (filename.Substring(len - 3, 3).ToLower().Equals(".gz")));
        }
		
		/// <summary>Get the maximum length of a String in a String array.</summary>
		public static int MaxLength(String[] o)
		{
			int max = 0;
			for (int i = 0; i < o.Length; i += 1)
			{
                // change suggested in .99.2 version: 
                // looked for nulls in the array pointer rather than the individual strings.
                if (o[i] != null && o[i].Length > max)
				{
					max = o[i].Length;
				}
			}
			return max;
		}
		
		/// <summary>Copy an array of Strings to bytes.</summary>
		public static byte[] StringsToByteArray(String[] o, int maxLen)
		{
			byte[] res = new byte[o.Length * maxLen];
			for (int i = 0; i < o.Length; i += 1)
			{
                // change suggested in .99.2 version: Added check for nulls.
                byte[] bstr;
                if (o[i] == null)
                {
                    bstr = new byte[0];
                }
                else
                {
                    bstr = SupportClass.ToByteArray(o[i]);
                }

				int cnt = bstr.Length;
				if (cnt > maxLen)
				{
					cnt = maxLen;
				}
				Array.Copy(bstr, 0, res, i * maxLen, cnt);
				for (int j = cnt; j < maxLen; j += 1)
				{
					res[i * maxLen + j] = (byte)SupportClass.Identity(' ');
				}
			}
			return res;
		}
		
		/// <summary>Convert bytes to Strings</summary>
		public static String[] ByteArrayToStrings(byte[] o, int maxLen)
		{
			String[] res = new String[o.Length / maxLen];
			for (int i = 0; i < res.Length; i += 1)
			{
				char[] tmpChar;
				tmpChar = new char[o.Length];
				o.CopyTo(tmpChar, 0);
				res[i] = new String(tmpChar, i * maxLen, maxLen).Trim();
			}
			return res;
		}
		
		
		/// <summary>Convert an array of booleans to bytes</summary>
		internal static byte[] BooleanToByte(bool[] bool_Renamed)
		{
			byte[] byt = new byte[bool_Renamed.Length];
			for (int i = 0; i < bool_Renamed.Length; i += 1)
			{
				byt[i] = bool_Renamed[i]?(byte)SupportClass.Identity('T'):(byte)SupportClass.Identity('F');
			}
			return byt;
		}

		/// <summary>Convert an array of bytes to booleans</summary>
		internal static bool[] ByteToBoolean(byte[] byt)
		{
			bool[] bool_Renamed = new bool[byt.Length];
			
			for(int i = 0; i < byt.Length; i += 1)
			{
				bool_Renamed[i] = (byt[i] == (byte)'T');
			}
			return bool_Renamed;
		}

        // change suggested in .99.5 version: Added this method.
        /// <summary>
        /// Get a stream to a URL accommodating possible redirections.
        /// Note that if a redirection request points to a different
        /// protocol than the original request, then the redirection
        /// is not handled automatically.
        /// </summary>
        public static Stream GetURLStream(String url, int level)
        {

            // Hard coded....sigh
            if (level > 5)
            {
                throw new IOException("Two many levels of redirection in URL");
            }
            WebRequest request = WebRequest.Create(url);

            /* To remove error "The remote server returned an error: (404) Not Found."
             * Reads the Internet Explorer nondynamic proxy settings
             * NOTE: This method is now obsolete. 
             * TO_DO :need to find the substitute method for this. */

            request.Proxy = WebProxy.GetDefaultProxy();
            //sets the credentials to submit to the proxy server for authentication

            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            WebResponse myResponse = request.GetResponse();

            WebHeaderCollection myWebHeaderCollection = request.Headers;

            // Read through the headers and see if there is a redirection header.
            // We loop (rather than just do a get on hdrs)
            // since we want to match without regard to case.
            String val = null;
            for (int i = 0; i < myWebHeaderCollection.Count; i++)
            {
                String key = myWebHeaderCollection.GetKey(i);
                if (key != null && key.ToLower().Equals("location"))
                    val = (String)myWebHeaderCollection.GetValues(key).GetValue(0);
                if (val != null)
                {
                    val = val.Trim();
                    if (val.Length > 0)
                    {
                        return GetURLStream(val, level + 1);
                    }
                }
            }

            // No redirection
            return myResponse.GetResponseStream();
        }
	}
}

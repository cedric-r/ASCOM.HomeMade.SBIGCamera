using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using nom.tam.util;

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
    /// <summary>This class provides access to routines to allow users
    /// to read and write FITS files.
    /// <p>
    /// *
    /// <p>
    /// <b> Description of the Package </b>
    /// <p>
    /// This FITS package attempts to make using FITS files easy,
    /// but does not do exhaustive error checking.  Users should
    /// not assume that just because a FITS file can be read
    /// and written that it is necessarily legal FITS.
    /// *
    /// *
    /// <ul>
    /// <li> The Fits class provides capabilities to
    /// read and write data at the HDU level, and to
    /// add and delete HDU's from the current Fits object.
    /// A large number of constructors are provided which
    /// allow users to associate the Fits object with
    /// some form of external data.  This external
    /// data may be in a compressed format.
    /// <li> The HDU class is a factory class which is used to
    /// create HDUs.  HDU's can be of a number of types
    /// derived from the abstract class BasicHDU.
    /// The hierarchy of HDUs is:
    /// <ul>
    /// <li>BasicHDU
    /// <ul>
    /// <li> ImageHDU
    /// <li> RandomGroupsHDU
    /// <li> TableHDU
    /// <ul>
    /// <li> BinaryTableHDU
    /// <li> AsciiTableHDU
    /// </ul>
    /// </ul>
    /// </ul>
    /// *
    /// <li> The Header class provides many functions to
    /// add, delete and read header keywords in a variety
    /// of formats.
    /// <li> The HeaderCard class provides access to the structure
    /// of a FITS header card.
    /// <li> The Data class is an abstract class which provides
    /// the basic methods for reading and writing FITS data.
    /// Users will likely only be interested in the getData
    /// method which returns that actual FITS data.
    /// <li> The TableHDU class provides a large number of
    /// methods to access and modify information in
    /// tables.
    /// <li> The Column class
    /// combines the Header information and Data corresponding to
    /// a given column.
    /// </ul>
    /// *
    /// Copyright: Thomas McGlynn 1997-1999.
    /// This code may be used for any purpose, non-commercial
    /// or commercial so long as this copyright notice is retained
    /// in the source code or included in or referred to in any
    /// derived software.
    /// *
    /// </summary>
    /// <version>  0.99.4  March 2, 2007
    /// 
    /// </version>
    public class Fits
    {
        /// <summary>
        /// default directory path to store files created temporarily during execution of library
        /// </summary>
        public static readonly String DEFAULT_TEMP_DIR = System.IO.Path.GetTempPath();

        // change suggested in .99.4 version: Added Support HTTPS, FTP and FILE URLs
        /// <summary> What might URLs begin with?</summary>
        public static String[] UrlProtocols = {"http:", "ftp:", "https:", "file:"};

        /// <summary>
        /// directory path to store temporary fits files
        /// </summary>
        protected static String _tempDir = DEFAULT_TEMP_DIR;

        /// <summary>Has the input stream reached the EOF?</summary>
        private bool atEOF;

        /// <summary>The input stream associated with this Fits object.</summary>
        private ArrayDataIO dataStr;

        /// <summary>A vector of HDUs that have been added to this Fits object.</summary>
        private System.Collections.ArrayList hduList = new System.Collections.ArrayList(100);

        /// <summary>Create an empty Fits object which is not associated with an input stream.</summary>
        public Fits()
        {
            InitBlock();
        }

        /// <summary>Create a Fits object associated with
        /// the given uncompressed data stream.
        /// </summary>
        /// <param name="str">The data stream.</param>
        public Fits(Stream str) : this(str, false)
        {
            InitBlock();
        }

        /// <summary>Create a Fits object associated with a possibly
        /// compressed data stream.
        /// </summary>
        /// <param name="str">The data stream.</param>
        /// <param name="compressed">Is the stream compressed?</param>
        public Fits(Stream str, bool compressed)
        {
            InitBlock();
            StreamInit(str, compressed, false);
        }

        /// <summary>Associate FITS object with an uncompressed File</summary>
        /// <param name="myFile">The File object.</param>
        public Fits(FileInfo myFile) : this(myFile, false)
        {
            InitBlock();
        }

        /// <summary>Associate the Fits object with a File</summary>
        /// <param name="myFile">The File object.</param>
        /// <param name="compressed">Is the data compressed?</param>
        public Fits(FileInfo myFile, bool compressed)
        {
            InitBlock();
            FileInit(myFile, compressed);
        }

        /// <summary>Associate the FITS object with a file or URL.
        /// *
        /// The string is assumed to be a URL if it begins one of the
        /// protocol strings.
        /// If the string ends in .gz it is assumed that
        /// the data is in a compressed format.
        /// All string comparisons are case insensitive.
        /// *
        /// </summary>
        /// <param name="filename">The name of the file or URL to be processed.</param>
        /// <exception cref=""> FitsException Thrown if unable to find or open
        /// a file or URL from the string given.</exception>
        public Fits(String filename) : this(filename, FitsUtil.IsCompressed(filename), FileAccess.ReadWrite)
        {
            InitBlock();
        }

        /// <summary>
        /// Asscoiates a fits object with a file specified by the Fileaccess parameter
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="access"></param>
        public Fits(String filename, FileAccess access)
            : this(filename, FitsUtil.IsCompressed(filename), access)
        {
            InitBlock();
        }

        // change suggested in .99.4 version: Added bool compressed field.
        /// <summary>Associate the FITS object with a file or URL. 
        /// 
        /// The string is assumed to be a URL if it begins one of the
        /// protocol strings.
        /// If the string ends in .gz it is assumed that
        /// the data is in a compressed format.
        /// All string comparisons are case insensitive.
        /// 
        /// <param name="filename"> The name of the file or URL to be processed.</summary>
        /// <exception cref="FitsException"> Thrown if unable to find or open 
        /// a file or URL from the string given.
        public Fits(String filename, bool compressed, FileAccess access)
        {
            InitBlock();

            //Stream inp;

            if (filename == null)
            {
                throw new FitsException("Null FITS Identifier String");
            }

            int len = filename.Length;
            String lc = filename.ToLower();

            // for (String protocol: urlProtocols) 
            for (int i = 0; i < UrlProtocols.Length; i++)
            {
                if (lc.StartsWith(UrlProtocols[i]))
                {
                    // This seems to be a URL.
                    try
                    {
                        // change suggested in .99.5 version: Added to retrieve GZIP stream.
                        Stream s = FitsUtil.GetURLStream(filename, 0);
                        StreamInit(s, compressed, false);
                    }
                    catch (IOException e)
                    {
                        throw new FitsException("Unable to open stream from URL:" + filename + " Exception=" + e);
                    }
                    return;
                }
            }
            if (compressed)
            {
                FileInit(new FileInfo(filename), true);
            }
            else
            {
                RandomInit(new FileInfo(filename), access);
            }
        }

        /// <summary>Associate the FITS object with a given uncompressed URL</summary>
        /// <param name="myURL">The URL to be associated with the FITS file.</param>
        /// <exception cref="FitsException">Thrown if unable to use the specified URL.</exception>
        public Fits(Uri myURL) : this(myURL, FitsUtil.IsCompressed(myURL.AbsoluteUri)) { }

        /// <summary>Associate the FITS object with a given URL</summary>
        /// <param name="myURL">The URL to be associated with the FITS file.</param>
        /// <param name="compressed">Is the data compressed?</param>
        /// <exception cref="FitsException">Thrown if unable to find or open
        /// a file or URL from the string given.</exception>
        public Fits(Uri myURL, bool compressed)
        {
            InitBlock();
            try
            {
                Stream s = FitsUtil.GetURLStream(myURL.OriginalString, 0);
                StreamInit(s, compressed, false);
            }
            catch (IOException)
            {
                throw new FitsException("Unable to open input from URL:" + myURL);
            }
        }

        /// <summary>
        /// Gets the temporary directory path
        /// </summary>
        public static String TempDirectory
        {
            get { return _tempDir; }

            set { _tempDir = value; }
        }

        /// <summary>Get the current number of HDUs in the Fits object.</summary>
        /// <returns>The number of HDU's in the object.</returns>
        virtual public int NumberOfHDUs
        {
            get { return hduList.Count; }
        }

        /// <summary>Get the data stream used for the Fits Data.</summary>
        /// <returns> The associated data stream.  Users may wish to
        /// call this function after opening a Fits object when
        /// they wish detailed control for writing some part of the FITS file.
        /// </returns>
        /// <summary>Set the data stream to be used for future input.</summary>
        /// <param name="stream">The data stream to be used.</param>
        virtual public ArrayDataIO Stream
        {
            get { return dataStr; }

            set
            {
                dataStr = value;
                atEOF = false;
            }
        }

        /// <summary>
        /// writes the header to fits file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="filename"></param>
        public static void Write(IDataReader reader, String filename)
        {
            Write(reader, filename, StreamedBinaryTableHDU.StringWriteMode.PAD, 128, true, ' ');
        }

        /// <summary>
        /// writes the header to fits file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="filename"></param>
        /// <param name="writeMode"></param>
        /// <param name="stringTruncationLength"></param>
        /// <param name="padStringsLeft"></param>
        /// <param name="stringPadChar"></param>
        public static void Write(IDataReader reader, String filename,
            StreamedBinaryTableHDU.StringWriteMode writeMode, int stringTruncationLength,
            bool padStringsLeft, char stringPadChar)
        {
            Header header = new Header();
            header.Simple = true;
            header.Bitpix = 8;
            header.Naxes = 0;

            Cursor c = header.GetCursor();
            // move to the end of the header cards
            for (c.MoveNext(); c.MoveNext();) ;
            // we know EXTEND isn't there yet
            c.Add("EXTEND", new HeaderCard("EXTEND", true, null));

            ImageHDU hdu1 = new ImageHDU(header, null);

            StreamedBinaryTableHDU hdu2 =
                new StreamedBinaryTableHDU(new DataReaderAdapter(reader), 4096,
                    writeMode, stringTruncationLength, padStringsLeft, stringPadChar);

            Fits fits = new Fits();
            fits.AddHDU(hdu1);
            fits.AddHDU(hdu2);

            Stream s = null;
            try
            {
                s = new FileStream(filename, FileMode.Create);
                fits.Write(s);
                s.Close();
            }
            catch (Exception e)
            {
                s.Close();
                throw (e);
            }
        }

        /// <summary>
        /// Initializes the HDU array list
        /// </summary>
        private void InitBlock()
        {
            hduList = new System.Collections.ArrayList();
        }

        /// <summary>Indicate the version of these classes</summary>
        public static System.String Version()
        {
            // Version 0.1: Original test FITS classes -- 9/96
            // Version 0.2: Pre-alpha release 10/97
            //              Complete rewrite using BufferedData*** and
            //              ArrayFuncs utilities.
            // Version 0.3: Pre-alpha release  1/98
            //              Incorporation of HDU hierarchy developed
            //              by Dave Glowacki and various bug fixes.
            // Version 0.4: Alpha-release 2/98
            //              BinaryTable classes revised to use
            //              ColumnTable classes.
            // Version 0.5: Random Groups Data 3/98
            // Version 0.6: Handling of bad/skipped FITS, FitsDate (D. Glowacki) 3/98
            // Version 0.9: ASCII tables, Tiled images, Faux, Bad and SkippedHDU class
            //              deleted. 12/99
            // Version 0.91: Changed visibility of some methods.
            //               Minor fixes.
            // Version 0.92: Fix bug in BinaryTable when reading from stream.
            // Version 0.93: Supports HIERARCH header cards.  Added FitsElement interface.
            //               Several bug fixes especially for null HDUs.
            // Version 0.96: Address issues with mandatory keywords.
            //               Fix problem where some keywords were not properly keyed.
            // Version 0.96a: Update version in FITS
            // Version 0.99: Added support for Checksums (thanks to RJ Mathar).
            //               Fixed bug with COMMENT and HISTORY keywords (Rose Early)
            //               Changed checking for compression and fixed bug with TFORM
            //               handling in binary tables (Laurent Michel)
            //               Distinguished arrays of length 1 from scalars in
            //               binary tables (Jorgo Bakker)
            //               Fixed bug in handling of length 0 values in headers (Fred Romerfanger, Jorgo Bakker)
            //               Truncated BufferedFiles when finishing write (but only
            //               for FITS file as a whole.)
            //               Fixed bug writing binary tables with deferred reads.
            //               Made addLine methods in Header public.
            //               Changed ArrayFuncs.newInstance to handle inputs with dimensionality of 0.
            // Version 0.99.1:
            //               Added deleteRows and deleteColumns functionality to all tables.  
            //               This includes changes
            //               to TableData, TableHDU, AsciiTable, BinaryTable and util/ColumnTable.
            //               Row deletions were suggested by code of R. Mathar but this works
            //               on all types of tables and implements the deletions at a lower level.
            //		  Completely revised util.HashedList to use more standard features from
            //               Collections.  The HashedList now melds a HashMap and ArrayList
            //               Added sort to HashedList function to enable sorting of the list.
            //               The logic now uses a simple index for the iterators rather than
            //               traversing a linked list.
            //               Added sort before write in Header to ensure keywords are in correct order.
            //               This uses a new HeaderOrder class which implements java.util.Comparator to
            //               indicate the required order for FITS keywords.  Users should now
            //               be able to write required keywords anywhere without getting errors
            //               later when they try to write out the FITS file.
            //               Fixed bug in setColumn in util.Column table where the new column
            //               was not being pointed to.  Any new column resets the table.
            //               Several fixes to BinaryTable to address errors in variable length
            //               array handling.
            //               Several fixes to the handling of variable length array in binary tables.
            //               (noted by Guillame Belanger).
            //               Several fixes and changes suggested by Richard Mathar mostly
            //               in BinaryTable.
            //  Version 0.99.2
            //               Revised test routines to use Junit.  Note that Junit tests
            //               use annotations and require Java 1.5.
            //               Added ArrayFuncs.arrayEquals() methods to compare
            //               arbitrary arrays.
            //               Fixed bugs in handling of 0 length columns and table update.
            //  Version 0.99.3
            //               Additional fixes for 0 length strings.
            //  Version 0.99.4
            //               Changed handling of constructor for File objects
            //          0.99.5
            //               Add ability to handle FILE, HTTPS and FTP URLs and to
            //               handle redirects amongst different protocols.
            //          0.99.5
            //               Fixes to String handling (A. Kovacs)
            //               Truncating long doubles to fit in
            //               standard header.
            //               Made some methods public in FitsFactory
            //               Added Set
            // Version 1.1
            //               This is the updated CSharp port of nom.tam.fits v0.99.5 (java library)
            return "1.1";
        }

        /// <summary>Do the stream initialization.</summary>
        /// <param name="str">The input stream.</param>
        /// <param name="compressed">Is this data compressed?  If so,
        /// then the GZIPInputStream class will be
        /// used to inflate it.</param>
        protected internal virtual void StreamInit(Stream str, bool compressed, bool seekable)
        {
            if (str == null)
            {
                throw new FitsException("Null input stream");
            }

            if (compressed)
            {
                //gzip functionality added
                try
                {
                    str = new GZipStream(str, CompressionMode.Decompress);
                }
                catch (System.IO.IOException e)
                {
                    throw new FitsException("Cannot inflate input stream" + e);
                }
            }

            if (str is ArrayDataIO)
            {
                dataStr = (ArrayDataIO) str;
            }
            else
            {
                // Use efficient blocking for input.
                dataStr = new BufferedDataStream(str);
            }
        }

        /// <summary>Initialize using buffered random access</summary>
        protected internal virtual void RandomInit(FileInfo f, FileAccess access)
        {
            // FileAccess access =
            // SupportClass.FileCanWrite(f) ? FileAccess.ReadWrite : FileAccess.Read;

            if (!f.Exists)
            {
                throw new FitsException("File '" + f + "' does not exist.");
            }
            try
            {
                // change suggested in .99.4 version: FileInfo passed instead of Filename (string)
                if (access.Equals(FileAccess.Read))
                    dataStr = new BufferedFile(f, access, FileShare.Read);
                else
                    dataStr = new BufferedFile(f, access, FileShare.ReadWrite);
                ((BufferedFile) dataStr).Seek(0);
            }
            catch (IOException)
            {
                throw new FitsException("Unable to open file " + f.FullName);
            }
        }

        /// <summary>Get a stream from the file and then use the stream initialization.</summary>
        /// <param name="myFile">The File to be associated.</param>
        /// <param name="compressed">Is the data compressed?</param>
        protected internal virtual void FileInit(FileInfo myFile, bool compressed)
        {
            try
            {
                // change suggested in .99.4 version
                if (compressed)
                {
                    FileStream str = new FileStream(myFile.FullName, FileMode.Open);
                    StreamInit(str, compressed, true);
                }
                else
                {
                    RandomInit(myFile, FileAccess.ReadWrite);
                }
            }
            catch (IOException)
            {
                throw new FitsException("Unable to create Input Stream from File: " + myFile);
            }
        }

        /// <summary>Return all HDUs for the Fits object.   If the
        /// FITS file is associated with an external stream make
        /// sure that we have exhausted the stream.</summary>
        /// <returns> an array of all HDUs in the Fits object.  Returns
        /// null if there are no HDUs associated with this object.
        /// </returns>
        public virtual BasicHDU[] Read()
        {
            ReadToEnd();

            int size = NumberOfHDUs;

            if (size == 0)
            {
                return null;
            }

            BasicHDU[] hdus = new BasicHDU[size];
            hduList.CopyTo(hdus);
            return hdus;
        }

        /// <summary>Read the next HDU on the default input stream.</summary>
        /// <returns>The HDU read, or null if an EOF was detected.
        /// Note that null is only returned when the EOF is detected immediately
        /// at the beginning of reading the HDU.</returns>
        public virtual BasicHDU ReadHDU()
        {
            if (dataStr == null || atEOF)
            {
                return null;
            }

            Header hdr = Header.ReadHeader(dataStr);
            if (hdr == null)
            {
                atEOF = true;
                return null;
            }

            Data datum = hdr.MakeData();
            datum.Read(dataStr);
            BasicHDU nextHDU = FitsFactory.HDUFactory(hdr, datum);

            hduList.Add(nextHDU);
            return nextHDU;
        }

        /// <summary>Skip HDUs on the associate input stream.</summary>
        /// <param name="n">The number of HDUs to be skipped.</param>
        public virtual void SkipHDU(int n)
        {
            for (int i = 0; i < n; i += 1)
            {
                SkipHDU();
            }
        }

        /// <summary>Skip the next HDU on the default input stream.</summary>
        public virtual void SkipHDU()
        {
            if (atEOF)
            {
                return;
            }
            else
            {
                Header hdr = new Header(dataStr);
                if (hdr == null)
                {
                    atEOF = true;
                    return;
                }
                int dataSize = (int) hdr.DataSize;
                // dataStr.Skip(dataSize);
                dataStr.Seek(dataSize);
            }
        }

        /// <summary>Return the n'th HDU.
        /// If the HDU is already read simply return a pointer to the
        /// cached data.  Otherwise read the associated stream
        /// until the n'th HDU is read.
        /// </summary>
        /// <param name="n">The index of the HDU to be read.  The primary HDU is index 0.</param>
        /// <returns> The n'th HDU or null if it could not be found.</returns>
        public virtual BasicHDU GetHDU(int n)
        {
            int size = NumberOfHDUs;

            for (int i = size; i <= n; i += 1)
            {
                BasicHDU hdu;
                hdu = ReadHDU();
                if (hdu == null)
                {
                    return null;
                }
            }

            try
            {
                return (BasicHDU) hduList[n];
            }
            catch (System.Exception)
            {
                throw new FitsException("Internal Error: hduList build failed");
            }
        }

        /// <summary>Read to the end of the associated input stream</summary>
        private void ReadToEnd()
        {
            while (dataStr != null && !atEOF)
            {
                try
                {
                    if (ReadHDU() == null)
                    {
                        break;
                    }
                }
                catch (IOException e)
                {
                    throw new FitsException("IO error: " + e);
                }
            }
        }

        /// <summary>Return the number of HDUs in the Fits object.   If the
        /// FITS file is associated with an external stream make
        /// sure that we have exhausted the stream.
        /// </summary>
        /// <returns>number of HDUs.</returns>
        /// <deprecated>The meaning of size of ambiguous.  Use</deprecated>
        public virtual int Size()
        {
            ReadToEnd();
            return NumberOfHDUs;
        }

        /// <summary>Add an HDU to the Fits object.  Users may intermix
        /// calls to functions which read HDUs from an associated
        /// input stream with the addHDU and insertHDU calls,
        /// but should be careful to understand the consequences.
        /// </summary>
        /// <param name="myHDU"> The HDU to be added to the end of the FITS object.</param>
        public virtual void AddHDU(BasicHDU myHDU)
        {
            InsertHDU(myHDU, NumberOfHDUs);
        }

        /// <summary>Insert a FITS object into the list of HDUs.</summary>
        /// <param name="myHDU">The HDU to be inserted into the list of HDUs.</param>
        /// <param name="n">The location at which the HDU is to be inserted.</param>
        public virtual void InsertHDU(BasicHDU myHDU, int n)
        {
            if (myHDU == null)
            {
                return;
            }

            if (n < 0 || n > NumberOfHDUs)
            {
                throw new FitsException("Attempt to insert HDU at invalid location: " + n);
            }

            try
            {
                if (n == 0)
                {
                    // Note that the previous initial HDU is no longer the first.
                    // If we were to insert tables backwards from last to first,
                    // we could get a lot of extraneous DummyHDUs but we currently
                    // do not worry about that.

                    if (NumberOfHDUs > 0)
                    {
                        ((BasicHDU) hduList[0]).PrimaryHDU = false;
                    }

                    if (myHDU.CanBePrimary)
                    {
                        myHDU.PrimaryHDU = true;
                        hduList.Insert(0, myHDU);
                    }
                    else
                    {
                        InsertHDU(BasicHDU.DummyHDU, 0);
                        myHDU.PrimaryHDU = false;
                        hduList.Insert(1, (Object) myHDU);
                    }
                }
                else
                {
                    myHDU.PrimaryHDU = false;
                    hduList.Insert(n, myHDU);
                }
            }
            catch (Exception e)
            {
                throw new FitsException("hduList inconsistency in insertHDU: " + e);
            }
        }

        /// <summary>Delete an HDU from the HDU list.</summary>
        /// <param name="n"> The index of the HDU to be deleted.
        /// If n is 0 and there is more than one HDU present, then
        /// the next HDU will be converted from an image to
        /// primary HDU if possible.  If not a dummy header HDU
        /// will then be inserted.</param>
        public virtual void DeleteHDU(int n)
        {
            int size = NumberOfHDUs;
            if (n < 0 || n >= size)
            {
                throw new FitsException("Attempt to delete non-existent HDU:" + n);
            }
            try
            {
                hduList.RemoveAt(n);
                if (n == 0 && size > 1)
                {
                    BasicHDU newFirst = (BasicHDU) hduList[0];
                    if (newFirst.CanBePrimary)
                    {
                        newFirst.PrimaryHDU = true;
                    }
                    else
                    {
                        InsertHDU(BasicHDU.DummyHDU, 0);
                    }
                }
            }
            catch (Exception)
            {
                throw new FitsException("Internal Error: hduList Vector Inconsitency");
            }
        }

        /// <summary>Write a Fits Object to an external Stream.  The stream is left open.</summary>
        /// <param name="dos">A DataOutput stream</param>
        public virtual void Write(Stream os)
        {
            ArrayDataIO obs;
            bool newOS = false;

            if (os is ArrayDataIO)
            {
                obs = (ArrayDataIO) os;
            }
            else
            {
                newOS = true;
                obs = new BufferedDataStream(os);
            }

            BasicHDU hh;
            for (int i = 0; i < NumberOfHDUs; i += 1)
            {
                try
                {
                    hh = (BasicHDU) hduList[i];
                    hh.Write(obs);
                }
                catch (IndexOutOfRangeException e)
                {
                    SupportClass.WriteStackTrace(e, Console.Error);
                    throw new FitsException("Internal Error: Vector Inconsistency" + e);
                }
            }

            if (newOS)
            {
                try
                {
                    obs.Flush();
                    obs.Close();
                }
                catch (IOException)
                {
                    Console.Error.WriteLine("Warning: error closing FITS output stream");
                }
            }

            // change suggested in .99 version
            try
            {
                if (obs is BufferedFile)
                {
                    ((BufferedFile) obs).SetLength(((BufferedFile) obs).FilePointer);
                }
            }
            catch (IOException e)
            {
                System.Console.Out.WriteLine("Exception occured while Writing BufferedFile: \n\t" + e.Message);
                // Ignore problems...  
            }
        }

        /// <summary>Read a FITS file from an InputStream object.</summary>
        /// <param name="is">The InputStream stream whence the FITS information is found.</param>
        public virtual void Read(Stream is_Renamed)
        {
            bool newIS = false;

            if (is_Renamed is ArrayDataIO)
            {
                dataStr = (ArrayDataIO) is_Renamed;
            }
            else
            {
                dataStr = new BufferedDataStream(is_Renamed);
            }

            Read();

            if (newIS)
            {
                dataStr.Close();
                dataStr = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Close()
        {
            if (dataStr != null)
            {
                dataStr.Close();
            }
            dataStr = null;
        }

        /// <summary>Get the current number of HDUs in the Fits object.</summary>
        /// <returns>The number of HDU's in the object.</returns>
        /// <deprecated>See getNumberOfHDUs()</deprecated>
        public virtual int CurrentSize()
        {
            return hduList.Count;
        }

        /// <summary>Create an HDU from the given header.</summary>
        /// <param name="h"> The header which describes the FITS extension</param>
        public static BasicHDU MakeHDU(Header h)
        {
            Data d = FitsFactory.DataFactory(h);
            return FitsFactory.HDUFactory(h, d);
        }

        /// <summary>Create an HDU from the given data kernel.</summary>
        /// <param name="o">The data to be described in this HDU.</param>
        public static BasicHDU MakeHDU(System.Object o)
        {
            return FitsFactory.HDUFactory(o);
        }

        /// <summary>Create an HDU from the given Data.</summary>
        /// <param name="datum">The data to be described in this HDU.</param>
        public static BasicHDU MakeHDU(Data datum)
        {
            Header hdr = new Header();
            datum.FillHeader(hdr);
            return FitsFactory.HDUFactory(hdr, datum);
        }

        // change suggested in .99 version: all the methods added in .99 version.

        #region CHECKSUM Methods

        /// <summary>Add or update the CHECKSUM keyword.</summary>
        /// <param name="hdu">The primary or other header to get the current DATE.</param>
        public static void SetChecksum(BasicHDU hdu)
        {
            /* the next line with the delete is needed to avoid some unexpected
	         *  problems with non.tam.fits.Header.checkCard() which otherwise says
	         *  it expected PCOUNT and found DATE.
	         */
            Header hdr = hdu.Header;
            hdr.DeleteKey("CHECKSUM");

            /* This would need org.nevec.utils.DateUtils compiled before org.nevec.prima.fits ....
	        * final String doneAt = DateUtils.dateToISOstring(0) ;
	        * We need to save the value of the comment string because this is becoming part
	        * of the checksum calculated and needs to be re-inserted again - with the same string -
	        * when the second/final call to addVallue() is made below.
	        */
            String doneAt = "as of " + FitsDate.FitsDateString;
            hdr.AddValue("CHECKSUM", "0000000000000000", doneAt);

            /* Convert the entire sequence of 2880 byte header cards into a byte array.
	         * The main benefit compared to the C implementations is that we do not need to worry
	         * about the particular byte order on machines (Linux/VAX/MIPS vs Hp-UX, Sparc...) supposed that
	         * the correct implementation is in the write() interface.
	         */
            MemoryStream hduByteImage = new MemoryStream();
            //System.err.flush();
            hdu.Write(new BufferedDataStream(hduByteImage));
            byte[] data = hduByteImage.ToArray();
            long csu = Checksum(data);

            /* This time we do not use a deleteKey() to ensure that the keyword is replaced "in place".
	        * Note that the value of the checksum is actually independent to a permutation of the
	        * 80-byte records within the header.
	        */
            hdr.AddValue("CHECKSUM", ChecksumEnc(csu, true), doneAt);
        }

        /// <summary>Add or Modify the CHECKSUM keyword in all headers.</summary>
        public void SetChecksum()
        {
            for (int i = 0; i < NumberOfHDUs; i += 1)
            {
                SetChecksum(GetHDU(i));
            }
        }

        /// <summary>
        /// Calculate the Seaman-Pence 32-bit 1's complement checksum over the byte stream. The option
        /// to start from an intermediate checksum accumulated over another previous
        /// byte stream is not implemented.
        /// The implementation accumulates in two 64-bit integer values the two low-order and the two
        /// high-order bytes of adjacent 4-byte groups. A carry-over of bits is never done within the main
        /// loop (only once at the end at reduction to a 32-bit positive integer) since an overflow
        /// of a 64-bit value (signed, with maximum at 2^63-1) by summation of 16-bit values could only
        /// occur after adding approximately 140G short values (=2^47) (280GBytes) or more. We assume
        /// for now that this routine here is never called to swallow FITS files of that size or larger.
        /// </summary>
        /// <param name="data">The byte sequence.</param>
        /// <return>The 32bit checksum in the range from 0 to 2^32-1. </return>
        private static long Checksum(byte[] data)
        {
            long hi = 0;
            long lo = 0;
            int len = 2 * (data.Length / 4);
            int remain = data.Length % 4;

            /* A write(2) on Sparc/PA-RISC would write the MSB first, on Linux the LSB; by some kind
	         * of coincidence, we can stay with the byte order known from the original C version of
	         * the algorithm.
	         */
            for (int i = 0; i < len; i += 2)
            {
                /* The four bytes in this block handled by a single 'i' are each signed (-128 to 127)
	             * in Java and need to be masked indivdually to avoid sign extension /propagation.
	             */
                hi += (data[2 * i] << 8) & 0xff00L | data[2 * i + 1] & 0xffL;
                lo += (data[2 * i + 2] << 8) & 0xff00L | data[2 * i + 3] & 0xffL;
            }

            /* The following three cases actually cannot happen 
             * since FITS records are multiples of 2880 bytes.
	         */
            if (remain >= 1)
                hi += (data[2 * len] << 8) & 0xff00L;

            if (remain >= 2)
                hi += data[2 * len + 1] & 0xffL;

            if (remain >= 3)
                lo += (data[2 * len + 2] << 8) & 0xff00L;

            long hicarry = hi >> 16;
            long locarry = lo >> 16;

            while (hicarry != 0 || locarry != 0)
            {
                hi = (hi & 0xffffL) + locarry;
                lo = (lo & 0xffffL) + hicarry;
                hicarry = hi >> 16;
                locarry = lo >> 16;
            }
            return (hi << 16) + lo;
        }

        /// <summary>Encode a 32bit integer according to the Seaman-Pence proposal.</summary>
        /// <param name="c">The checksum previously calculated.</param>
        /// <return>The encoded string of 16 bytes.</param>
        private static String ChecksumEnc(long c, bool compl)
        {
            byte[] asc = new byte[16];
            int[] exclude = {0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f, 0x60};
            long[] mask = {0xff000000L, 0xff0000L, 0xff00L, 0xffL};
            int offset = 0x30; /* ASCII 0 (zero */
            long value = compl ? ~c : c;
            for (int i = 0; i < 4; i++)
            {
                int byt = (int) ((value & mask[i]) >> (24 - 8 * i)); // each byte becomes four
                int quotient = byt / 4 + offset;
                int remainder = byt % 4;
                int[] ch = new int[4];
                for (int j = 0; j < 4; j++)
                    ch[j] = quotient;

                ch[0] += remainder;
                bool check = true;
                for (; check;) // avoid ASCII punctuation
                {
                    check = false;

                    for (int k = 0; k < exclude.Length; k++)
                        for (int j = 0; j < 4; j += 2)
                            if (ch[j] == exclude[k] || ch[j + 1] == exclude[k])
                            {
                                ch[j]++;
                                ch[j + 1]--;
                                check = true;
                            }
                }

                for (int j = 0; j < 4; j++) // assign the bytes
                    asc[4 * j + i] = (byte) (ch[j]);
            }

            // shift the bytes 1 to the right circularly.
            String resul = new String(SupportClass.ToCharArray(asc), 15, 1);
            return resul.Insert(resul.Length, new String(SupportClass.ToCharArray(asc), 0, 15));
        }

        #endregion
    }
}
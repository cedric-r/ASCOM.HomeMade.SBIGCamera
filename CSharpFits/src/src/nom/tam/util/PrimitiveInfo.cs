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

    /// <Remarks>This interface collects some information about C# primitives.</Remarks>
    public class PrimitiveInfo
	{
        /// <summary>Sizes.</summary>
        public readonly static Hashtable sizes;

        static PrimitiveInfo()
        {
            sizes = new Hashtable();
            sizes[typeof(byte)] = 1; // BitConverter.GetBytes((byte)0).Length;
            sizes[typeof(sbyte)] = BitConverter.GetBytes((sbyte)0).Length;
            sizes[typeof(bool)] = BitConverter.GetBytes(true).Length;
            sizes[typeof(char)] = 1;//BitConverter.GetBytes('a').Length;
            sizes[typeof(short)] = BitConverter.GetBytes((short)0).Length;
            sizes[typeof(ushort)] = BitConverter.GetBytes((ushort)0).Length;
            sizes[typeof(int)] = BitConverter.GetBytes((int)0).Length;
            sizes[typeof(uint)] = BitConverter.GetBytes((uint)0).Length;
            sizes[typeof(long)] = BitConverter.GetBytes((long)0).Length;
            sizes[typeof(ulong)] = BitConverter.GetBytes((ulong)0).Length;
            sizes[typeof(float)] = BitConverter.GetBytes((float)0.0).Length;
            sizes[typeof(double)] = BitConverter.GetBytes((double)0.0).Length;
        }

    }
}
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
    /// <summary>This class allows FITS binary and ASCII tables to be accessed via a common interface.</summary>
	
	public interface TableData
	{
		int NCols
		{
			get;
			
		}
		int NRows
		{
			get;
			
		}
		Array GetRow(int row);
		Object GetColumn(int col);
		Object GetElement(int row, int col);

		void SetRow(int row, Array newRow);
		void SetColumn(int col, Object newCol);
		void SetElement(int row, int col, Object element);
        
		int AddRow(Array newRow);
		int AddColumn(Object newCol);

        // suggested in .99.1 version:
        void DeleteRows(int row, int len);
        void DeleteColumns(int row, int len);

        // suggested in .99.1 version:
        void UpdateAfterDelete(int oldNcol, Header hdr);
    
	}
}

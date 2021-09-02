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
	
    /// <summary>This interface defines the properties that
	/// a generic table should have.
    /// </summary>
	
	public interface DataTable
	{
		int NRows
		{
			get;
		}
		int NCols
		{
			get;
		}
    Object GetRow(int row);
    void SetRow(int row, Object newRow);
    Object GetColumn(int column);
    void SetColumn(int column, Object newColumn);
    Object GetElement(int row, int col);
    void SetElement(int row, int col, Object newElement);
	}
}
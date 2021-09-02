using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
 namespace nom.tam.fits
{
    /*
    * Copyright: Thomas McGlynn 1997-2007.
    * Many thanks to David Glowacki (U. Wisconsin) for substantial
    * improvements, enhancements and bug fixes.
    * The CSharpFITS package is a C# port of Tom McGlynn's
    * nom.tam.fits Java package, initially ported by  Samuel Carliles
    *
    * Copyright: 2007 Virtual Observatory - India.    
    *
    * Use is subject to license terms
    */

    // suggested in .99.1 version: 
    ///<summary>Sorts keywords before a header is written to ensure that required keywords come where they need to be.
    ///</summary> 
    public class HeaderOrder : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// Which order should the cards indexed by these keys
        /// be written out?  This method assumes that the
        /// arguments are either the FITS Header keywords as
        /// strings, and some other type (or null) for comment
        /// style keywords.
        /// </summary>
        /// <returns>
        /// -1: if the first argument should be written first.
        /// 1:  if the second argument should be written first.
        /// 0:  if either is legal.
        /// </returns>
        public int Compare(object a, object b)
        {

            String c1, c2;
        
            if ( (a != null) && (a is String) )
            {
        	    c1 = (String) a;
            }
            else
            {
		        c1 = " ";
		    }
    		
            if ( (b != null) && (b is String) )
            {
		        c2 = (String) b;
		    }
            else
            {
		        c2 = " ";
		    }
    	    
    		
		    // Equals are equal
		    if (c1.Equals(c2))
		    {
		        return 0;
		    }
    		

		    // Now search in the order in which cards must appear
		    // in the header.
    		
		    if (c1.Equals("SIMPLE") || c1.Equals("XTENSION"))
		    {
		        return -1;
		    }
		    if (c2.Equals("SIMPLE") || c2.Equals("XTENSION"))
		    {
			    return 1;
		    }
    		
		    if (c1.Equals("BITPIX"))
		    {
			    return -1;
		    }
		    if (c2.Equals("BITPIX"))
		    {
			    return 1;
		    }
    		
		    if (c1.Equals("NAXIS"))
		    {
			    return -1;
		    } 
		    if (c2.Equals("NAXIS"))
		    {
			    return 1;
		    }
    		

		    // Check the NAXISn cards.  These must
		    // be in axis order.
    		
		    if (NAXISn(c1) > 0)
		    {
		        if (NAXISn(c2) > 0)
		        {
				    if (NAXISn(c1) < NAXISn(c2))
				    {
				        return -1;
				    }
				    else
				    {
				        return  1;
				    }
		        }
		        return -1;
		    }
    		
		    if (NAXISn(c2) > 0)
		    {
		        return 1;
		    }
    		
		    if (c1.Equals("PCOUNT"))
		    {
		        return -1;
		    }
		    if (c2.Equals("PCOUNT"))
		    {
		        return 1;
		    }
    		
		    if (c1.Equals("GCOUNT"))
		    {
		        return -1;
		    }
		    if (c2.Equals("GCOUNT"))
		    {
		        return 1;
		    }
    		
		    if (c1.Equals("TFIELDS"))
		    {
		        return -1;
		    }
		    if (c2.Equals("TFIELDS"))
		    {
		        return 1;
		    }
    		

		    // In principal this only needs to be in the first 36 cards,
		    // but we put it here since it's convenient.  BLOCKED is
		    // deprecated currently.
		    if (c1.Equals("BLOCKED"))
		    {
		        return -1;
		    }
		    if (c2.Equals("BLOCKED"))
		    {
		        return 1;
		    }
    		
		    // Note that this must be at the end, so the
		    // values returned are inverted.
		    if (c1.Equals("END"))
		    {
		        return 1;
		    }
		    if (c2.Equals("END"))
		    {
		        return -1;
		    }
    		

		    // All other cards can be in any order.
		    return 0;
        }

        #endregion

        #region Helper Methods

        /// <summary>Can two cards be exchanged when being written out?</summary>
        public bool EqualsTo(Object a, Object b)
        {
            return Compare(a,b) == 0;
        }

        /// <summary>Find the index for NAXISn keywords.</summary>
        private int NAXISn(String key)
        {

            if (key.Length > 5 && key.Substring(0,5).Equals("NAXIS"))
            {
                for (int i = 5; i < key.Length; i += 1)
                {
                    bool number = true;
                    char c = key[i];
                    if ('0' > c || c > '9')
                    {
                        number = false;
                        break;
                    }
                    if (number)
                    {
                        return Int32.Parse(key.Substring(5));
                    }
                }
            }
            return -1;
        }
        
        #endregion

    }
}

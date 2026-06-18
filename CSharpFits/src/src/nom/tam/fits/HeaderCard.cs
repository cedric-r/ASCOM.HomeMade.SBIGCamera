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

    using System;
    using nom.tam.util;
    /// <summary>This class describes methods to access and manipulate the individual
    /// cards for a FITS Header.</summary>
	public class HeaderCard
	{
    #region Properties
        /// <summary>Does this card contain a string value?</summary>
		virtual public bool IsStringValue
		{
			get
			{
				return isString;
			}
		}

        /// <summary>Is this a key/value card?</summary>
		virtual public bool KeyValuePair
		{
			get
			{
				return (key != null && val != null);
			}
		}

        /// <summary>Return the keyword from this card</summary>
	    virtual public String Key
	    {
          get
          {
            return key;
          }
          // should be internal
          set
          {
            key = value;
          }
        }

        /// <summary>Return the value from this card</summary>
		virtual public String Value
		{
			get
			{
				return val;
			}
            // suggested in .99.1 version
            set
            {
                val = value;
            }
		}

        /// <summary>Return the comment from this card</summary>
		virtual public String Comment
		{
			get
			{
				return comment;
			}
		}
    #endregion

    #region Variables
        /// <summary>The keyword part of the card (set to null if there's no keyword)</summary>
		private String key;
		
		/// <summary>The value part of the card (set to null if there's no value)</summary>
		private String val;
		
		/// <summary>The comment part of the card (set to null if there's no comment)</summary>
		private String comment;

        // suggested in .97 version:
        /// <summary>Does this card represent a nullable field. ?</summary>
		/// KILL THIS FIELD
		private bool nullable;
		
		/// <summary>A flag indicating whether or not this is a string value</summary>
		private bool isString;
		
		/// <summary>Maximum length of a FITS keyword field</summary>
		public const int MAX_KEYWORD_LENGTH = 8;
		
		/// <summary>Maximum length of a FITS value field</summary>
		public const int MAX_VALUE_LENGTH = 70;
		
		/// <summary>padding for building card images</summary>
		private static String space80 = "                                                                                ";
    #endregion

    #region Constructors

        // suggested in .97 version:
        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">Keyword (null for a COMMENT)</param>
        /// <param name="value">Value</param>
        /// <param name="comment">Comment</param>
        /// <param name="nullable">Is this a nullable value card?</param>
        /// <exception cref=""> HeaderCardException for any invalid keyword or value</exception>
        public HeaderCard(String key, String val, String comment, bool nullable)
        {
          if(key == null && val != null)
          {
            throw new HeaderCardException("Null keyword with non-null value");
          }
    			
          if(key != null && key.Length > MAX_KEYWORD_LENGTH)
          {
            if (!FitsFactory.UseHierarch || !key.Substring(0, (9) - (0)).Equals("HIERARCH."))
            {
              throw new HeaderCardException("Keyword too long");
            }
          }
    			
          if(val != null)
          {
            val = val.Trim();
    				
            if (val.Length > MAX_VALUE_LENGTH)
            {
              throw new HeaderCardException("Value too long");
            }
    		
            if(val[0] == '\'')
            {
              if(val[val.Length - 1] != '\'')
              {
                throw new HeaderCardException("Missing end quote in string value");
              }
    					
              val = val.Substring(1, (val.Length - 1) - (1)).Trim();
            }
          }
    			
          this.key = key;
          this.val = val;
          this.comment = comment;
          this.nullable = nullable;
          isString = true;
        }

        // suggested in .97 version:
        /// <summary>Create a comment style card.
        /// This constructor builds a card which has no value.
        /// This may be either a comment style card in which case the
        /// nullable field should be false, or a value field which 
        /// has a null value, in which case the nullable field should be
        /// true.</summary>
        /// <param name="key">The key for the comment or nullable field.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="nullable">Is this a nullable field or a comment-style card?</param>
        public HeaderCard(String key, String comment, bool nullable)
                : this(key, null, comment, nullable)
	    {
        }

        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">keyword (null for a comment)</param>
        /// <param name="value">value (null for a comment or keyword without an '=')</param>
        /// <param name="comment">comment</param>
        /// <exception cref=""> HeaderCardException for any invalid keyword or value</exception>
        public HeaderCard(String key, String val, String comment):this(key, val, comment, false)
        {
        }
    		
        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">keyword (null for a comment)</param>
        /// <param name="value">value (null for a comment or keyword without an '=')</param>
        /// <param name="comment">comment</param>
        /// <exception cref=""> HeaderCardException for any invalid keyword</exception>
        public HeaderCard(String key, bool val, String comment):this(key, val ? "T" : "F", comment)
        {
          isString = false;
        }
    		
        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">keyword (null for a comment)</param>
        /// <param name="value">value (null for a comment or keyword without an '=')</param>
        /// <param name="comment">comment</param>
        /// <exception cref="">HeaderCardException for any invalid keyword</exception>
        public HeaderCard(String key, int val, String comment):this(key, val.ToString(), comment)
        {
          isString = false;
        }

        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">keyword (null for a comment)</param>
        /// <param name="value">value (null for a comment or keyword without an '=')</param>
        /// <param name="comment">comment</param>
        /// <exception cref="">HeaderCardException for any invalid keyword</exception>
        public HeaderCard(String key, float val, String comment):this(key, val.ToString(), comment)
        {
          isString = false;
        }

        /// <summary>Create a HeaderCard from its component parts</summary>
        /// <param name="key">keyword (null for a comment)</param>
        /// <param name="value">value (null for a comment or keyword without an '=')</param>
        /// <param name="comment">comment</param>
        /// <exception cref=""> HeaderCardException for any invalid keyword</exception>
        public HeaderCard(String key, long val, String comment):this(key, val.ToString(), comment)
        {
          isString = false;
        }
    		
        /// <summary>Create a HeaderCard from its component parts</summary>
	    /// <param name="key">keyword (null for a comment)</param>
	    /// <param name="value">value (null for a comment or keyword without an '=')</param>
	    /// <param name="comment">comment</param>
	    /// <exception cref="">HeaderCardException for any invalid keyword</exception>
	    public HeaderCard(String key, double val, String comment):this(key, DblString(val), comment)
	    {
		    isString = false;
	    }

        // suggested in .99.5 version:
        /// <summary>
        /// Create a string from a double making sure that it's 
        /// not more than 20 characters long.
        /// Probably would be better if we had a way to override this
        /// since we can loose precision for some doubles.
        /// </summary>
        private static String DblString(double input)
        {
	        String value = input.ToString();
            
	        if (value.Length > 20)
            {
                String v = value;
                if (value.IndexOf("E") != -1)
                    v = value.Substring(0, value.IndexOf("E"));

                v = Double.Parse(v).ToString("N12");
                if (!v.Equals(value))
                    value = v + value.Substring(value.IndexOf("E"));
                else
                    value = v;
	        }
	        return value;
        }

        /// <summary>Create a HeaderCard from a FITS card image</summary>
		/// <param name="card">the 80 character card image</param>
		public HeaderCard(String card)
		{
			key = null;
			val = null;
			comment = null;
			isString = false;


            // suggested in .99.2 version
            // Truncated String in one argument constructor to
            // a maximum of 80 characters.            
            if (card.Length > 80)
                card = card.Substring(0, 80);

			if(FitsFactory.UseHierarch && card.Length > 9 && card.Substring(0, (9) - (0)).Equals("HIERARCH "))
			{
				HierarchCard(card);
				return ;
			}
			
			
			// We are going to assume that the value has no blanks in
			// it unless it is enclosed in quotes.  Also, we assume that
			// a / terminates the string (except inside quotes)
			
			// treat short lines as special keywords
			if (card.Length < 9)
			{
				key = card;
				return ;
			}
			
			// extract the key
			key = card.Substring(0, (8) - (0)).Trim();
			
			// if it is an empty key, assume the remainder of the card is a comment
			if (key.Length == 0)
			{
				key = "";
				comment = card.Substring(8);
				return ;
			}

            // change suggested in .97 version:
            // Non-key/value pair lines are treated as keyed comments
            if (key.Equals("COMMENT") || key.Equals("HISTORY") ||
                !card.Substring(8, (10) - (8)).Equals("= "))
			{
				comment = card.Substring(8).Trim();
				return ;
			}
			
			// extract the value/comment part of the string
			String valueAndComment = card.Substring(10).Trim();
			
			// If there is no value/comment part, we are done.
			if (valueAndComment.Length == 0)
			{
				val = "";
				return ;
			}
			
			int vend = - 1;
			//bool quote = false;
			
			// If we have a ' then find the matching  '.
			if (valueAndComment[0] == '\'')
			{
				
				int offset = 1;
				while (offset < valueAndComment.Length)
				{
					
					// look for next single-quote character
					vend = SupportClass.StringIndexOf(valueAndComment, '\'', offset);
					
					// if the quote character is the last character on the line...
					if (vend == valueAndComment.Length - 1)
					{
						break;
					}
					
					// if we did not find a matching single-quote...
					if (vend == - 1)
					{
						// pretend this is a comment card
						key = null;
						comment = card;
						return ;
					}
					
					// if this is not an escaped single-quote, we are done
					if (valueAndComment[vend + 1] != '\'')
					{
						break;
					}
					
					// skip past escaped single-quote
					offset = vend + 2;
				}
				
				// break apart character string
				val = valueAndComment.Substring(1, (vend) - (1)).Trim();
				
				if (vend + 1 >= valueAndComment.Length)
				{
					comment = null;
				}
				else
				{
					comment = valueAndComment.Substring(vend + 1).Trim();
					if (comment[0] == '/')
					{
						if (comment.Length > 1)
						{
							comment = comment.Substring(1);
						}
						else
						{
							comment = "";
						}
					}
					if (comment.Length == 0)
					{
						comment = null;
					}
				}
				isString = true;
			}
			else
			{
				
				// look for a / to terminate the field.
				int slashLoc = valueAndComment.IndexOf((System.Char) '/');
				if (slashLoc != - 1)
				{
					comment = valueAndComment.Substring(slashLoc + 1).Trim();
					val = valueAndComment.Substring(0, (slashLoc) - (0)).Trim();
				}
				else
				{
					val = valueAndComment;
				}
			}
		}
    #endregion

		/// <summary>Process HIERARCH style cards...
		/// HIERARCH LEV1 LEV2 ...  value / comment
		/// The keyword for the card will be "HIERARCH.LEV1.LEV2..."
		/// The value will be the first token which starts with a non-alphabetic
		/// character (i.e., not A-Z or _).
		/// A '/' is assumed to start a comment.
		/// </summary>
		private void HierarchCard(String card)
		{
			String name = "";
			String token = null;
			String separator = "";
			int[] tokLimits;
			int posit = 0;
			int commStart = - 1;
			
			// First get the hierarchy levels
			while((tokLimits = GetToken(card, posit)) != null)
			{
				token = card.Substring(tokLimits[0], (tokLimits[1]) - (tokLimits[0]));
				if (!token.Equals("="))
				{
					name += separator + token;
					separator = ".";
				}
				else
				{
					tokLimits = GetToken(card, tokLimits[1]);
					if (tokLimits != null)
					{
						token = card.Substring(tokLimits[0], (tokLimits[1]) - (tokLimits[0]));
					}
					else
					{
						key = name;
						val = null;
						comment = null;
						return ;
					}
					break;
				}
				posit = tokLimits[1];
			}
			key = name;
			
			
			// At the end?
			if (tokLimits == null)
			{
				val = null;
				comment = null;
				isString = false;
				return ;
			}
			
			
			if (token[0] == '\'')
			{
				// Find the next undoubled quote...
				isString = true;
				if (token.Length > 1 && token[1] == '\'' && (token.Length == 2 || token[2] != '\''))
				{
					val = "";
					commStart = tokLimits[0] + 2;
				}
				else if (card.Length < tokLimits[0] + 2)
				{
					val = null;
					comment = null;
					isString = false;
					return ;
				}
				else
				{
					int i;
					for (i = tokLimits[0] + 1; i < card.Length; i += 1)
					{
						if (card[i] == '\'')
						{
							if (i == card.Length - 1)
							{
								val = card.Substring(tokLimits[0] + 1, (i) - (tokLimits[0] + 1));
								commStart = i + 1;
								break;
							}
							else if (card[i + 1] == '\'')
							{
								// Doubled quotes.
								i += 1;
								continue;
							}
							else
							{
								val = card.Substring(tokLimits[0] + 1, (i) - (tokLimits[0] + 1));
								commStart = i + 1;
								break;
							}
						}
					}
				}
				if (commStart < 0)
				{
					val = null;
					comment = null;
					isString = false;
					return ;
				}
				for (int i = commStart; i < card.Length; i += 1)
				{
					if (card[i] == '/')
					{
						comment = card.Substring(i + 1).Trim();
						break;
					}
					else if (card[i] != ' ')
					{
						comment = null;
						break;
					}
				}
			}
			else
			{
				isString = false;
				int sl = token.IndexOf('/');
				if (sl == 0)
				{
					val = null;
					comment = card.Substring(tokLimits[0] + 1);
				}
				else if (sl > 0)
				{
					val = token.Substring(0, (sl) - (0));
					comment = card.Substring(tokLimits[0] + sl + 1);
				}
				else
				{
					val = token;
					
					for (int i = tokLimits[1]; i < card.Length; i += 1)
					{
						if (card[i] == '/')
						{
							comment = card.Substring(i + 1).Trim();
							break;
						}
						else if (card[i] != ' ')
						{
							comment = null;
							break;
						}
					}
				}
			}
		}
		
		/// <summary>Get the next token.  Can't use StringTokenizer
		/// since we sometimes need to know the position within
		/// the string.</summary>
		private int[] GetToken(String card, int posit)
		{
			int i;
			for (i = posit; i < card.Length; i += 1)
			{
				if (card[i] != ' ')
				{
					break;
				}
			}
			
			if (i >= card.Length)
			{
				return null;
			}
			
			if (card[i] == '=')
			{
				return new int[]{i, i + 1};
			}
			
			int j;
			for (j = i + 1; j < card.Length; j += 1)
			{
				if (card[j] == ' ' || card[j] == '=')
				{
					break;
				}
			}
			return new int[]{i, j};
		}

        /// <summary>Return the 80 character card image</summary>
		public override String ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder(80);
			
			// start with the keyword, if there is one
			if(key != null)
			{
				if(key.Length > 9 && key.Substring(0, 9).Equals("HIERARCH."))
				{
					return HierarchToString();
				}
				buf.Append(key);

                if(key.Length < 8)
				{
					buf.Append(space80.Substring(0, 8 - buf.Length));
				}
			}
			
          if(val != null || nullable)
          {
            buf.Append("= ");
    				
            if(val != null)
            {
              if (isString)
              {
                // left justify the string inside the quotes
                buf.Append('\'');
                buf.Append(val);
                if(buf.Length < 19)
                {
                  buf.Append(space80.Substring(0, 19 - buf.Length));
                }
                buf.Append('\'');
                // Now add space to the comment area starting at column 40
                if(buf.Length < 30)
                {
                  buf.Append(space80.Substring(0, 30 - buf.Length));
                }
              }
              else
              {
                int offset = buf.Length;
                if(val.Length < 20)
                {
                  buf.Append(space80.Substring(0, 20 - val.Length));
                }
                buf.Append(val);
              }
            }
            else
            {
              // Pad out a null value.
              buf.Append(space80.Substring(0, 20));
            }
    				
            // if there is a comment, add a comment delimiter
            if(comment != null)
            {
              buf.Append(" /" + (comment.StartsWith(" ") ? comment : (" " + comment)));
            }
          }
          else if(comment != null && comment.StartsWith("= "))
          {
            buf.Append("  " + comment);
          }
          else if(comment != null)
          {
            buf.Append(comment);
          }
			// finally, add any comment
            // if(comment != null)
            // {
            //     buf.Append(comment);
            // }
			
			// make sure the final string is exactly 80 characters long
			if(buf.Length > 80)
			{
				buf.Length = 80;
			}
			else if(buf.Length < 80)
			{
				buf.Append(space80.Substring(0, 80 - buf.Length));
			}
			
			return buf.ToString();
		}

        private String HierarchToString()
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder(80);
			int p = 0;
			String space = "";
			while(p < key.Length)
			{
				int q = SupportClass.StringIndexOf(key, '.', p);
				if (q < 0)
				{
					b.Append(space + key.Substring(p));
					break;
				}
				else
				{
					b.Append(space + key.Substring(p, (q) - (p)));
				}
				space = " ";
				p = q + 1;
			}
			
			if (val != null || nullable)
			{
				b.Append("= ");
				
				if (val != null)
				{
					// Try to align values
					int avail = 80 - (b.Length + val.Length);
					
					if (isString)
					{
						avail -= 2;
					}
					if (comment != null)
					{
						avail -= 3 + comment.Length;
					}
					
					if (avail > 0 && b.Length < 29)
					{
						b.Append(space80.Substring(0, (Math.Min(avail, 29 - b.Length)) - (0)));
					}
					
					if (isString)
					{
						b.Append('\'');
					}
					else if (avail > 0 && val.Length < 10)
					{
						b.Append(space80.Substring(0, (Math.Min(avail, 10 - val.Length)) - (0)));
					}
					b.Append(val);
					if (isString)
					{
						b.Append('\'');
					}
				}
				else if (b.Length < 30)
				{
					
					// Pad out a null value
					b.Append(space80.Substring(0, (30 - b.Length) - (0)));
				}
			}
			
			
			if (comment != null)
			{
				b.Append(" / " + comment);
			}
			if (b.Length < 80)
			{
				b.Append(space80.Substring(0, (80 - b.Length) - (0)));
			}
			String card = new String(b.ToString().ToCharArray());
			if (card.Length > 80)
			{
				card = card.Substring(0, (80) - (0));
			}
			return card;
		}
	}
}

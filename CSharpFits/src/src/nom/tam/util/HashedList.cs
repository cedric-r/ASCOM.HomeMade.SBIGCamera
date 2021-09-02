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
    using nom.tam.fits;
    // suggested in .99.1 version: 
    // Changes made as the data structure storing keys in ArrayList and values in Hashtable.
      
    /// <summary>
    /// This class implements a structure which can
    ///   be accessed either through a hash or
    ///   as linear list.  Only some elements may have
    ///   a hash key.

    ///   This class is motivated by the FITS header
    ///    structure where a user may wish to go through
    ///   the header element by element, or jump directly
    ///    to a given keyword.  It assumes that all
    ///   keys are unique.  However, all elements in the
    ///   structure need not have a key.

    ///   This class does only the search structure
    ///   and knows nothing of the semantics of the
    ///   referenced objects.

    ///   Users may wish to access the HashedList using
    ///   HashedListCursor which extends the Cursor
    ///   interface to allow adding and deleting entries.
    /// </summary>
     public class HashedList : IDictionary
    {
        #region Instance Variables

        /// <summary>The HashTable of keyed indices.</summary>
        private Hashtable keyed = new Hashtable();

        /// <summary>An ordered List of the keys.</summary>
        private ArrayList ordered = new ArrayList();

        /// <summary>This is used to generate unique keys for
        /// elements entered without an key.</summary>
        private int unkeyedIndex = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor creating new hast table and arraylist structures
        /// </summary>
        public HashedList()
        {
            InitBlock();
        }
        /// <summary>
        /// Initializes a new HashTable and Arraylist
        /// </summary>
        private void InitBlock()
        {
            keyed = new Hashtable();
            ordered = new ArrayList();
            unkeyedIndex = 0;
        }
        #endregion

        #region Properties

        /// <summary>Returns Value corresponding to \"Key\".</summary>
        /// <param name="key">Key pointing Value</param>
        public object this[object key]
        {
            get
            {
                if (!keyed.ContainsKey(key))
                {
                    return null;
                }

                return keyed[key];
            }
            set
            {
                Add(key, value);
            }
        }

        // suggested in .99.1 version: 
        //  Added to retrieve key value, needed as the internal data structure changes to ArrayList.
        /// <summary>Key at specified \"index\".</summary>
        /// <param name="index">Index of Key</param>
        public object this[int index]
        {
            get
            {
                if ((ordered.Count < index) || (index < 0))
                {
                    return null;
                }

                return ordered[index];
            }
        }


        /// <summary>Is the HashedList empty?</summary>
        public bool Empty
        {
            get
            {
                return keyed.Count == 0;
            }
        }
        #endregion

        /*
    /// <summary>This inner class defines a Cursor over the hashed list.
		/// A Cursor need not start at the beginning of the list.
		/// <p>
		/// This class can be used by external users to both add
		/// and delete elements from the list.  It implements the
		/// standard IDictionaryEnumerator interface but also provides methods
		/// to add keyed or unkeyed elements at the current location.
		/// <p>
		/// Users may move either direction in the list using the
		/// next and prev calls.  Note that a call to prev followed
		/// by a call to next (or vice versa) will return the same
		/// element twice.
		/// <p>
		/// The class is implemented as an inner class so that it can
		/// easily access the state of the associated HashedList.
		/// </summary>
		*/
        private class HashedListCursor : Cursor, System.Collections.IDictionaryEnumerator
        {

            protected class NullKey
            {
            }

            #region IDictionaryEnumerator Members

            public DictionaryEntry Entry
            {
                get
                {
                    int c = current;
                    if (current == -1)
                        c++;

                    if ((c < 0) || (c > Enclosing_Instance.Count))
                    {
                        throw new InvalidOperationException("Attempt to retrieve from IEnumerator in wrong state.");
                    }

                    return new DictionaryEntry(Enclosing_Instance[c] == null ?
                                  new NullKey() : Enclosing_Instance[c],
                                                  Enclosing_Instance[Enclosing_Instance[c]]);
                }
            }
            /// <summary>
            /// Point the Cursor to a particular keyed entry.  This
            /// method is not in the IEnumerator interface.
            /// </summary>
            public Object Key
            {
                get
                {
                    int c = current;
                    if (current == -1)
                        c++;

                    if ((c < 0) || (c > Enclosing_Instance.Count))
                    {
                        //  throw new InvalidOperationException("Attempt to retrieve from IEnumerator in wrong state.");
                        return null;
                    }

                    Object key = Enclosing_Instance.ordered[c];
                    HeaderCard hc = (HeaderCard)Enclosing_Instance.keyed[key];
                     //   return Enclosing_Instance[c];

                    return hc.Key;
                }

               set
                {
                    if (Enclosing_Instance.ContainsKey(value))
                    {
                        current = Enclosing_Instance.ordered.IndexOf(value);
                    }
                    else
                    {
                        current = Enclosing_Instance.ordered.Count;
                    }
                }
            }

            public Object Value
            {
                get
                {
                    if ((current < 0) || (current > Enclosing_Instance.Count))
                    {
                        throw new InvalidOperationException("Attempt to retrieve from IEnumerator in wrong state.");
                    }

                    return Enclosing_Instance[Enclosing_Instance[current]];
                }
            }
            #endregion

            #region IEnumerator Methods

            public Object Current
            {
                get
                {
                    int c = current;
                    if (current == -1 && Enclosing_Instance.ordered.Count > 0)
                        c++;

                    return ((c < 0) || (c >= Enclosing_Instance.Count)) ?
                                      null : (Object)this.Entry;
                }
            }

            /// <summary>Is there another element?</summary>
            public bool MoveNext()
            {
                if (current < -1 || current > Enclosing_Instance.Count)
                {
                    // throw new Exception("Outside List");
                    // System.Console.Out.WriteLine("Outside List");
                    return false;
                }
                else
                {
                    current += 1;

                    // Check whether the cursor is at last position or not.
                    return (current < Enclosing_Instance.keyed.Count);
                }

            }
            /// <summary>
            /// Resetting the cursor's current position to -1
            /// </summary>
            public void Reset()
            {
                current = -1;
            }
            #endregion

            #region Cursor Methods

            /// <summary>Move to the previous entry.</summary>
            public bool MovePrevious()
            {
                if (current < 0)
                {
                    throw new Exception("Before beginning of list");
                }

                current -= 1;

                return (current > -1);
            }

            /// <summary>Remove the current entry. Note that remove can
            /// be called only after a call to next, and only once per such
            /// call.  Remove cannot be called after a call to prev.</summary>
            public void Remove()
            {
                if (current >= 0 && current <= Enclosing_Instance.Count)
                {
                    Enclosing_Instance.RemoveElement(current);

                    // If we just removed the last entry, then we need
                    // to go back one.
                    if (current > -1)
                    {
                        current -= 1;
                    }
                }
            }

            /// <summary>Add an entry at the current location. The new entry goes before
            /// the entry that would be returned in the next 'next' call, and
            /// that call will not be affected by the insertion. 
            /// Note: this method is not in the IEnumerator interface.</summary>
            public void Add(System.Object val)
            {
                if (current == -1)
                    MoveNext();
                int nKey = Enclosing_Instance.unkeyedIndex;
                Enclosing_Instance.unkeyedIndex += 1;
                Enclosing_Instance.Add(current, nKey, val);
                //System.Console.Out.WriteLine("Added " + nKey + "-" + val + ", Cursor Position: #" + current);
                MoveNext();
            }
            /// <summary>
            /// Insert an entry ar the current location.Cursor positionis not incremented.So new entry is retived using next call
            /// </summary>
            /// <param name="key"></param>
            /// <param name="val"></param>
            public void Insert(Object key, Object val)
            {
                if (current == -1)
                    MoveNext();
                Enclosing_Instance.Add(current, key, val);
                //System.Console.Out.WriteLine("Added " + key + "-" + val + ", Cursor Position: #" + current);
            }

            /// <summary>
            /// Add a keyed entry at the current location. The new entry is inserted
            /// before the entry that would be returned in the next invocation of
            /// 'next'.  The return value for that call is unaffected.
            /// Note: this method is not in the IEnumerator interface.
            /// </summary>
            public void Add(System.Object key, System.Object val)
            {
                if (current == -1)
                    MoveNext();
                Enclosing_Instance.Add(current, key, val);
                //System.Console.Out.WriteLine("Inserted " + key + "-" + val + ", Cursor Position: #" + current);
                MoveNext();
            }
            #endregion

            #region Book-keeping Crap
            internal HashedListCursor(HashedList enclosingInstance, int start)
            {
                this.enclosingInstance = enclosingInstance;
                current = start;
                System.Console.Out.WriteLine("------------------------------------------");
                System.Console.Out.WriteLine("Cursor Started from position: #" + current);
            }

            protected HashedList Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }

            private HashedList enclosingInstance;
            /// <summary>The element that will be returned by next.</summary>
            private int current;
            #endregion
        }

        #region Methods

        #region Add Methods

        /// <summary>Add an element to the end of the list.</summary>
        public bool Add(System.Object val)
        {
            int nKey = unkeyedIndex;
            unkeyedIndex += 1;
            Add(ordered.Count, nKey, val);
            return true;
        }

        /// <summary>Add an element to the list.</summary>
        /// <param name="pos">The element after which the current element 
        /// be placed.  If pos is null put the element at the end of the list.</param>
        /// <param name="key">The hash key for the new object.  This may be null
        /// for an unkeyed entry.</param>
        /// <param name="reference">The actual object being stored.</param>
        public bool Add(int pos, System.Object key, System.Object val)
        {
            if (keyed.ContainsKey(key))
            {
                int oldPos = ordered.IndexOf(key);
                RemoveKey(key);
                if (oldPos < pos)
                {
                    pos -= 1;
                }
            }

            keyed.Add(key, val);
            if (pos >= ordered.Count)
            {
                ordered.Add(key);
            }
            else
            {
                ordered.Insert(pos, key);
            }

            return true;

        }
        #endregion

        #region Remove Methods

        /// <summary>Remove an element from the list.
        /// This method is also called by the HashedListCursor.
        /// </summary>
        /// <param name="index">The element to be removed.</param>
        private bool RemoveElement(int index)
        {
            if (index >= 0 && index < ordered.Count)
            {
                Object key = ordered[index];
                return RemoveKey(key);
            }
            return false;
        }
        /// <summary>
        /// Removes the key from hash and list
        /// </summary>
        /// <param name="key"></param>
        /// <returns>returns false if key not found in has or true on success</returns>
        public bool RemoveKey(System.Object key)
        {
            if (keyed.ContainsKey(key))
            {
                System.Console.Out.WriteLine("Removing " + key + "-" + keyed[key]);
                keyed.Remove(key);
                ordered.Remove(key);
                return true;
            }
            return false;
        }

        #endregion

        #region Cursor Methods
        /// <summary>Return a Cursor over the entire list.
        /// The Cursor may be used to delete
        /// entries as well as to retrieve existing
        /// entries.  A knowledgeable user can
        /// cast this to a HashedListCursor and
        /// use it to add as well as delete entries.
        /// NOTE: Cursor is initialized with -1 in case the start index is not specified.
        /// </summary>
        public Cursor GetCursor()
        {
            return new HashedListCursor(this, -1);
        }
        /// <summary>
        /// Returns a cursor pointing to key "key"
        /// </summary>
        /// <param name="key"></param>
        public Cursor GetCursor(Object key)
        {
            if (keyed.ContainsKey(key))
            {
                return new HashedListCursor(this, ordered.IndexOf(key));
            }
            else
            {
                throw new Exception("Unknown key for iterator:" + key);
            }
        }

        /// <summary>Return a Cursor starting with the n'th entry.</summary>
        public Cursor GetCursor(int n)
        {
            if (n >= -1 && n <= ordered.Count - 1)
            {
                return new HashedListCursor(this, n);
            }
            else
            {
                throw new Exception("Invalid index for iterator: #" + n);
            }
        }
        #endregion

        #region Element Methods


        /// <summary>Replace the key of a given element.</summary>
        /// <param name="oldKey"> The previous key.  This key must
        /// be present in the hash.</param>
        /// <param name="newKey"> The new key.  This key
        /// must not be present in the hash.</param>
        /// <returns>if the replacement was successful.</returns>
        public bool ReplaceKey(Object oldKey, Object newKey)
        {
            if (!keyed.ContainsKey(oldKey) || keyed.ContainsKey(newKey))
            {
                return false;
            }

            Object oldVal = keyed[oldKey];
            int index = ordered.IndexOf(oldKey);
            Remove(oldKey);
            return Add(index, newKey, oldVal);
        }
        /// <summary>
        /// Returns true if val present in hashtable,else false
        /// </summary>
        /// <param name="val"></param>
        public bool ContainsValue(Object val)
        {
            if (val == null)
            {
                return false;
            }
            return keyed.ContainsValue(val);
        }

        /// <summary>Check if the key is included in the list.</summary>
        public bool ContainsKey(System.Object key)
        {
            return keyed.ContainsKey(key);
        }

        /// <summary>Clear the collection 
        /// </summary>
        public void Clear()
        {
            keyed.Clear();
            ordered.Clear();
        }
        #endregion
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetCursor();
        }
        #endregion

        #region ICollection Members

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets the count of hashtable
        /// </summary>
        public int Count
        {
            get
            {
                return keyed.Count;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new InvalidCastException("Sorry, elements of this list can't be copied into an array.");
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>Convert to an array of objects</summary>
        public Object[] toArray()
        {
            Object[] o = new Object[ordered.Count];
            return toArray(o);
        }

        /// <summary>Convert to an array of objects of a specified type.</summary>
        public Object[] toArray(Object[] o)
        {
            int i = 0;
            foreach (Object a in keyed.Values)
            {
                o[i] = a;
                i++;
            }
            return o;
        }

        // suggested in .99.1 version: Added new method to facilitate extra features.
        /// <summary>Sort the keys into some desired order.</summary>
        public void Sort(IComparer comp)
        {
            ordered.Sort(comp);
        }

        #endregion

        #region IDictionary Members

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// gets the current cursor
        /// </summary>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)GetCursor();
        }

        /// <summary>Add a keyed element to the end of the list.</summary>
        public void Add(System.Object key, System.Object val)
        {
            Add(ordered.Count, key, val);
        }

        // suggested in .99.1 version: Added new method for testing purpose.
        /// <summary>Add another collection to this one list.
        /// All entries are added as unkeyed entries to the end of the list.</summary>
        public bool AddAll(IDictionary c)
        {
            ICollection array = c.Values;
            foreach (Object o in array)
            {
                Add(o);
            }
            return true;
        }

        /// <summary>Remove a keyed object from the list.  Unkeyed
        /// objects can be removed from the list using a
        /// HashedListCursor.</summary>
        public void Remove(System.Object key)
        {
            if (keyed.ContainsKey(key))
            {
                RemoveKey(key);
            }
        }
        /// <summary>
        /// Removes the value from the list
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool RemoveValue(Object val)
        {
            if (keyed.ContainsValue(val))
            {
                for (int i = 0; i < ordered.Count; i += 1)
                {
                    if (keyed[ordered[i]].Equals(val))
                    {
                        return RemoveKey(ordered[i]);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes Unkeyed value that matches 'val'.
        /// NOTE: if the value that is passed to be removed has to keys,
        /// one which was inserted with a key, and another without it, then
        /// it may be possible that the keyed value is removed if it is first in the order.
        /// </summary>
        public bool RemoveUnkeyedObject(Object val)
        {
            System.Console.Out.WriteLine("In RemoveUnkeyedObject method.");
            if (keyed.ContainsValue(val))
            {
                for (int i = 0; i < ordered.Count; i += 1)
                {
                    if (keyed[ordered[i]].Equals(val))
                    {
                        return RemoveKey(ordered[i]);
                    }
                }
            }
            return false;
        }

        // suggested in .99.1 version: Added new method for testing purpose.
        /// <summary>Remove all the elements that are found in another collection.</summary>
        public bool RemoveAll(IDictionary c)
        {
            ICollection o = c.Values;
            bool result = false;
            foreach (Object i in o)
            {
                result = result | RemoveValue(i);
            }
            return result;
        }

        // suggested in .99.1 version: Added new method for testing purpose.
        /// <summary>Retain only elements contained in another collection.</summary>
        public bool RetainAll(IDictionary c)
        {
            ICollection v = c.Values;
            bool result = false;
            bool rec = true;

            Cursor cursor = (HashedListCursor)GetCursor();
            while (cursor.MoveNext())
            {
                Object o = ((DictionaryEntry)cursor.Current).Value;

                // Check whether the Current element in Cursor is present in passed object 'c'
                foreach (Object a in v)
                {
                    rec = rec & !a.Equals(o);
                }

                // If the element is not present in 'c'
                if (rec)
                {
                    // Remove the element.
                    cursor.Remove();
                    result = true;
                }
                else
                    rec = true;
            }

            return result;
        }
        /// <summary>Check if the key is included in the list.</summary>
        public bool Contains(object key)
        {
            return ContainsKey(key);
        }
        /// <summary>Check if the collection is included in the list.</summary>
        public bool ContainsAll(IDictionary c)
        {
            ICollection v = c.Values;
            bool contains = true;
            foreach (Object o in v)
            {
                if (!keyed.ContainsValue(o))
                {
                    contains = false;
                    break;
                }
            }

            return contains;
        }
        /// <summary>
        /// gets the values in hashtable
        /// </summary>
        public ICollection Values
        {
            get
            {
                return keyed.Values;
            }
        }
        /// <summary>
        /// Gets the keys in hashtable
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return keyed.Keys;
            }
        }
        
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }


        #endregion

    }
}

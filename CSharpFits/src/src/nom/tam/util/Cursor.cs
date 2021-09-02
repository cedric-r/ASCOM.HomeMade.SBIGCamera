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

    /// <summary>This interface extends the IEnumerator interface
    /// to allow insertion of data and move to previous entries
    /// in a collection.
    /// </summary>
    public interface Cursor : IEnumerator
    {
        /// <summary>Point the list at a particular element.
        /// Point to the end of the list if the key is not found.
        /// </summary>
        Object Key
        {
            get;
            set;
        }

        /// <summary>Move to the previous element</summary>
        bool MovePrevious();

        /// <summary>Add an unkeyed element to the collection.
        /// The new element is placed such that it will be called
        /// by a prev() call, but not a next() call.
        /// </summary>
        void Add(Object val);

        /// <summary>Add a keyed element to the collection.
        /// The new element is placed such that a next() call. 
        /// The cursor position is not incremented in this method.</summary>
        void Insert(Object key, Object val);

        /// <summary>Add a keyed element to the collection.
        /// The new element is placed such that it will be called
        /// by a prev() call, but not a next() call.
        /// The cursor position is changed after adding a value.
        /// </summary>
        void Add(Object key, Object val);

        /// <summary>Remove the current object from the collection</summary>
        void Remove();

    }
}

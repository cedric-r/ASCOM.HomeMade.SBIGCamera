namespace nom.tam.util
{
    /* Copyright: Thomas McGlynn 1999.
    * This code may be used for any purpose, non-commercial
    * or commercial so long as this copyright notice is retained
    * in the source code or included in or referred to in any
    * derived software.
    */
    using System;
    using nom.tam.util;
    using System.Collections;
    using NUnit.Framework;

    /// <summary>This class tests and illustrates the use
    /// of the HashedList class.  Tests are in three
    /// parts.  
    /// <p>
    /// The first section tests the methods
    /// that are present in the Collection interface.
    /// All of the optional methods of that interface
    /// are supported.  This involves tests of the
    /// HashedClass interface directly.
    /// <p>
    /// The second set of tests uses the Cursor
    /// returned by the GetCursor() method and tests
    /// the standard Cursor methods to display
    /// and remove rows from the HashedList.
    /// <p>
    /// The third set of tests tests the extended
    /// capabilities of the HashedListCursor
    /// to add rows to the table, and to work
    /// as a cursor to move in a non-linear fashion
    /// through the list.
    /// <p>
    /// There is as yet no testing that the HashedList
    /// fails appropriately and gracefully.
    /// </p></p></p></p>
    /// </summary>
    [TestFixture]
    public class HashedListTester
    {
        [STAThread]
        [Test]
        public void Test()
        {
            HashedList h = new HashedList {{"key1", 20}, {"key2", 21}};

            Cursor c = h.GetCursor();
            Console.WriteLine("current: " + ((DictionaryEntry) c.Current).Value);
        }

        [Test]
        public void TestHashedList()
        {
            HashedList h1 = new HashedList();
            HashedList h2 = new HashedList();

            Cursor i = h1.GetCursor(-1);
            Cursor j;

            // Add a few unkeyed rows.
            h1.Add("Row 1");
            h1.Add("Row 2");
            h1.Add("Row 3");

            System.Console.Out.WriteLine("***** Collection methods *****\n");
            show("Three unkeyed elements", h1);
            h1.RemoveUnkeyedObject("Row 2");
            show("Did we remove Row 2?", h1);

            h1.Clear();
            show("Cleared", h1);

            // Insert Rows with Keys.
            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("key 3", "Row 3");

            show("Three keyed elements", h1);
            h1.Remove("key 2");
            show("Did we remove Row 2 using a key?", h1);
            h1.Clear();
            show("Cleared", h1);

            // Again insert Rows with Keys.
            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("key 3", "Row 3");
            show("Three elements again!", h1);
            Console.Out.WriteLine("Check contains (true):" + h1.ContainsValue("Row 2"));

            // Inserting Rows in h2.
            h2.Add("key 4", "Row 4");
            h2.Add("key 5", "Row 5");
            Console.Out.WriteLine("Check containsAll (false):" + h1.ContainsAll(h2));

            h1.AddAll(h2);
            show("Should have 5 elements now", h1);
            Console.Out.WriteLine("Check containsAll (true):" + h1.ContainsAll(h2));
            Console.Out.WriteLine("Check contains (true):" + h1.ContainsKey("key 4"));

            h1.RemoveValue("Row 4");
            show("Dropped Row 4:", h1);
            Console.Out.WriteLine("Check containsAll (false):" + h1.ContainsAll(h2));
            Console.Out.WriteLine("Check contains (false):" + h1.ContainsKey("Row 4"));

            Console.Out.WriteLine("Check isEmpty (false):" + h1.Empty);
            h1.RemoveValue("Row 1");
            h1.RemoveValue("Row 2");
            h1.RemoveValue("Row 3");
            h1.RemoveValue("Row 5");
            show("Removed all elements", h1);
            Console.Out.WriteLine("Check isEmpty (true):" + h1.Empty);

            h1.Add("Row 1");
            h1.Add("Row 2");
            h1.Add("Row 3");
            h1.AddAll(h2);
            show("Back to 5", h1);
            h1.RemoveAll(h2);
            show("Testing removeAll back to 3?", h1);
            h1.AddAll(h2);
            h1.RetainAll(h2);
            show("Testing retainAll now just 2?", h1);

            Console.Out.WriteLine("\n\n**** Test Cursor **** \n");

            j = h1.GetCursor();
            while (j.MoveNext())
            {
                Console.Out.WriteLine("Cursor got: [" + ((DictionaryEntry) j.Current).Key + "] \"" +
                                             ((DictionaryEntry) j.Current).Value + "\"");
            }

            h1.Clear();
            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("Row 3");
            h1.Add("key 4", "Row 4");
            h1.Add("Row 5");
            j = h1.GetCursor();
            j.MoveNext();
            j.MoveNext();
            j.Remove(); // Should get rid of second row
            show("Removed second row with cursor", h1);
            Console.Out.WriteLine("Cursor should still be OK:" + j.MoveNext() + " [" +
                                         ((DictionaryEntry) j.Current).Key + "] \"" +
                                         ((DictionaryEntry) j.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should still be OK:" + j.MoveNext() + " [" +
                                         ((DictionaryEntry) j.Current).Key + "] \"" +
                                         ((DictionaryEntry) j.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should be done:" + j.MoveNext());

            Console.Out.WriteLine("\n\n**** HashedListCursor ****\n");
            i = h1.GetCursor(-1);
            Console.Out.WriteLine("Cursor should still be OK:" + i.MoveNext() + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should still be OK:" + i.MoveNext() + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should still be OK:" + i.MoveNext() + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should still be OK:" + i.MoveNext() + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should be done:" + i.MoveNext());

            i.Key = "key 1";
            i.MoveNext();
            i.Add("key 2", "Row 2");
            Console.Out.WriteLine("Cursor should still be OK:" + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            i.MoveNext();
            Console.Out.WriteLine("Cursor should still be OK:" + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            i.MoveNext();
            Console.Out.WriteLine("Cursor should still be OK:" + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should be done:" + i.MoveNext());

            i.Key = "key 4";
            Console.Out.WriteLine("Cursor should still be OK:" + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            i.MoveNext();
            Console.Out.WriteLine("Cursor should still be OK:" + " [" +
                                         ((DictionaryEntry) i.Current).Key + "] \"" +
                                         ((DictionaryEntry) i.Current).Value + "\"");
            Console.Out.WriteLine("Cursor should be done:" + i.MoveNext());

            i.Key = "key 2";
            i.MoveNext();
            i.MoveNext();
            i.Add("Row 3.5");
            i.Add("Row 3.6");
            show("Added some rows... should be 7", h1);

            i = h1.GetCursor("key 2");
            i.Add("Row 1.5");
            i.Add("key 1.7", "Row 1.7");
            i.Add("Row 1.9");
            Console.Out.WriteLine("Cursor should point to 2:" +
                                         ((DictionaryEntry) i.Current).Key);
            i.Key = "key 1.7";
            Console.Out.WriteLine("Cursor should point to 1.7:" +
                                         ((DictionaryEntry) i.Current).Key);
        }

        public static void show(String descrip, HashedList h)
        {
            Console.Out.WriteLine(descrip + " : [" + h.Count + "]");
            Object[] o = h.toArray();
            for (int i = 0; i < o.Length; i += 1)
            {
                Console.Out.WriteLine("  " + o[i]);
            }
        }

        // Test methods with Asserts

        [Test]
        public void TestCollection()
        {

            HashedList h1 = new HashedList();
            HashedList h2 = new HashedList();

            Cursor i = h1.GetCursor(-1);

            // Add a few unkeyed rows.
            h1.Add("Row 1");
            h1.Add("Row 2");
            h1.Add("Row 3");

            Assert.AreEqual(3, h1.Count);

            Assert.AreEqual(true, h1.ContainsValue("Row 1"));
            Assert.AreEqual(true, h1.ContainsValue("Row 2"));
            h1.RemoveValue("Row 2");
            Assert.AreEqual(true, h1.ContainsValue("Row 1"));
            Assert.AreEqual(false, h1.ContainsValue("Row 2"));

            Assert.AreEqual(2, h1.Count);
            h1.Clear();
            Assert.AreEqual(0, h1.Count);


            // Add few Keyed rows.
            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("key 3", "Row 3");

            Assert.AreEqual(3, h1.Count);

            Assert.AreEqual(true, h1.ContainsValue("Row 1"));
            Assert.AreEqual(true, h1.ContainsKey("key 1"));
            Assert.AreEqual(true, h1.ContainsValue("Row 2"));
            Assert.AreEqual(true, h1.ContainsKey("key 2"));
            Assert.AreEqual(true, h1.ContainsValue("Row 3"));
            Assert.AreEqual(true, h1.ContainsKey("key 3"));

            h1.RemoveKey("key 2");
            Assert.AreEqual(2, h1.Count);
            Assert.AreEqual(true, h1.ContainsValue("Row 1"));
            Assert.AreEqual(true, h1.ContainsKey("key 1"));
            Assert.AreEqual(false, h1.ContainsValue("Row 2"));
            Assert.AreEqual(false, h1.ContainsKey("key 2"));
            Assert.AreEqual(true, h1.ContainsValue("Row 3"));
            Assert.AreEqual(true, h1.ContainsKey("key 3"));

            h1.Clear();
            Assert.AreEqual(0, h1.Count);

            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("key 3", "Row 3");
            Assert.AreEqual(3, h1.Count);
            Assert.AreEqual(true, h1.ContainsValue("Row 2"));
            Assert.AreEqual(true, h1.ContainsKey("key 2"));

            h2.Add("key 4", "Row 4");
            h2.Add("key 5", "Row 5");

            Assert.AreEqual(false, h1.ContainsAll(h2));

            h1.AddAll(h2);

            Assert.AreEqual(5, h1.Count);
            Assert.AreEqual(true, h1.ContainsAll(h2));
            Assert.AreEqual(true, h1.ContainsValue("Row 4"));
            h1.RemoveValue("Row 4");
            Assert.AreEqual(false, h1.ContainsValue("Row 4"));
            Assert.AreEqual(false, h1.ContainsAll(h2));

            Assert.AreEqual(false, h1.Empty);
            h1.RemoveValue("Row 1");
            h1.RemoveValue("Row 2");
            h1.RemoveValue("Row 3");
            h1.RemoveValue("Row 5");
            Assert.AreEqual(true, h1.Empty);
            h1.Add("Row 1");
            h1.Add("Row 2");
            h1.Add("Row 3");
            h1.AddAll(h2);
            Assert.AreEqual(5, h1.Count);
            h1.RemoveAll(h2);

            Assert.AreEqual(3, h1.Count);
            h1.AddAll(h2);

            Assert.AreEqual(5, h1.Count);
            h1.RetainAll(h2);
            Assert.AreEqual(2, h1.Count);
        }

        [Test]
        public void TestIterator()
        {

            HashedList h1 = new HashedList();

            h1.Add("key 4", "Row 4");
            h1.Add("key 5", "Row 5");


            Cursor j = h1.GetCursor();
            Assert.AreEqual(true, j.MoveNext());
            Assert.AreEqual("Row 4", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(true, j.MoveNext());
            Assert.AreEqual("Row 5", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(false, j.MoveNext());

            h1.Clear();

            h1.Add("key 1", "Row 1");
            h1.Add("key 2", "Row 2");
            h1.Add("Row 3");
            h1.Add("key 4", "Row 4");
            h1.Add("Row 5");

            Assert.AreEqual(true, h1.ContainsValue("Row 2"));
            j = h1.GetCursor();
            j.MoveNext();
            j.MoveNext();
            j.Remove(); // Should get rid of second row
            Assert.AreEqual(false, h1.ContainsValue("Row 2"));
            Assert.AreEqual(true, j.MoveNext());
            Assert.AreEqual("Row 3", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(true, j.MoveNext());
            Assert.AreEqual("Row 4", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(true, j.MoveNext());
            Assert.AreEqual("Row 5", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(false, j.MoveNext());
        }

        [Test]
        public void TestCursor()
        {

            HashedList h1 = new HashedList();

            h1.Add("key 1", "Row 1");
            h1.Add("Row 3");
            h1.Add("key 4", "Row 4");
            h1.Add("Row 5");

            Cursor j = (Cursor) h1.GetCursor(0);
            Assert.AreEqual("Row 1", (String) ((DictionaryEntry) j.Current).Value);
            j.MoveNext();
            Assert.AreEqual("Row 3", (String) ((DictionaryEntry) j.Current).Value);

            Assert.AreEqual(false, h1.ContainsKey("key 2"));
            Assert.AreEqual(false, h1.ContainsValue("Row 2"));
            j.Key = "key 1";
            Assert.AreEqual("Row 1", (String) ((DictionaryEntry) j.Current).Value);
            j.MoveNext();
            j.Add("key 2", "Row 2");
            Assert.AreEqual(true, h1.ContainsValue("Row 2"));
            Assert.AreEqual("Row 3", (String) ((DictionaryEntry) j.Current).Value);

            j.Key = "key 4";
            Assert.AreEqual("Row 4", (String) ((DictionaryEntry) j.Current).Value);
            j.MoveNext();
            Assert.AreEqual("Row 5", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(false, j.MoveNext());

            j.Key = "key 2";
            Assert.AreEqual("Row 2", (String) ((DictionaryEntry) j.Current).Value);
            j.MoveNext();
            Assert.AreEqual("Row 3", (String) ((DictionaryEntry) j.Current).Value);
            j.Add("Row 3.5");
            j.Add("Row 3.6");
            Assert.AreEqual(7, h1.Count);

            j = h1.GetCursor("key 2");
            j.Add("Row 1.5");
            j.Add("key 1.7", "Row 1.7");
            j.Add("Row 1.9");
            Assert.AreEqual("Row 2", (String) ((DictionaryEntry) j.Current).Value);
            j.Key = "key 1.7";
            Assert.AreEqual("Row 1.7", (String) ((DictionaryEntry) j.Current).Value);
            j.MoveNext();
            j.MovePrevious();
            Assert.AreEqual("Row 1.7", (String) ((DictionaryEntry) j.Current).Value);
            j.MovePrevious();
            Assert.AreEqual("Row 1.5", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(true, j.MovePrevious());
            Assert.AreEqual("Row 1", (String) ((DictionaryEntry) j.Current).Value);
            Assert.AreEqual(false, j.MovePrevious());
        }
    }
}
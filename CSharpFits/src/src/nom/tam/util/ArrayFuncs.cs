// Member of the utility package.
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

    /// <summary>This is a package of static functions which perform
    /// computations on arrays.  Generally these routines attempt
    /// to complete without throwing errors by ignoring data
    /// they cannot understand.
    /// </summary>
    public class ArrayFuncs : PrimitiveInfo
    {
        /// <summary>
        /// Method to count the dimensions of array
        /// </summary>
        /// <param name="o"></param>
        /// <returns>returns dimension of array</returns>
        public static int CountDimensions(Object o)
        {
            if (o == null)
            {
                return 0;
            }

            // Object is of type Array
            if (o.GetType().IsArray)
            {
                // Multi-dimension Arrays
                if (((Array)o).Rank > 1)
                {
                    return ((Array)o).Rank;
                }
                // Jagged and One Dimensional Arrays
                else
                {
                    int r = 0;
                    for (int i = 0; i < ((Array)o).GetLength(0) && r == 0; i++)
                        r = CountDimensions(((Array)o).GetValue(i));
                    return 1 + r;
                }
            }
            // Object is Premitive type
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// Checks whether the object is a type of Array of Arrays
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Returns boolean depending upon the Arra type</returns>
        public static bool IsArrayOfArrays(Object o)
        {
            if (o == null || !o.GetType().IsArray)
            {
                return false;
            }

            // If the object passed is of type Multi-dimension Array
            if (((Array)o).Rank > 1)
            {
                IEnumerator e = ((Array)o).GetEnumerator();

                int i = 0;

                // If the argument passed is a multi-dimension array,
                // like int[2,2], below loop will give you back 'false';
                // If the input is Array[2,2]. below loop would return 'true'
                // as each element is an array.
                for (; e.MoveNext(); i++)
                {
                    if (e.Current != null && e.Current.GetType().IsArray)
                    {
                        return true;
                    }
                    else if (e.Current != null && !e.Current.GetType().IsArray)
                    {
                        return false;
                    }
                    else if (e.Current == null)
                    {
                        continue;
                    }
                }
                if (i == ((Array)o).Length)
                {
                    return false;
                }
            }
            // If the object passed is of type Jagged Array
            else
            {
                int i = 0;
                for (; i < ((Array)o).Length; i++)
                {
                    if (((Array)o).GetValue(i) != null && ((Array)o).GetValue(i).GetType().IsArray)
                    {
                        return true;
                    }
                    else if (((Array)o).GetValue(i) != null && !(((Array)o).GetValue(i).GetType().IsArray))
                    {
                        return false;
                    }
                    else if (((Array)o).GetValue(i) == null)
                    {
                        continue;
                    }
                }
                if (i == ((Array)o).Length)
                {
                    return false;
                }
            }

            return false;
            // return !(!o.GetType().IsArray && ((Array)o).Rank > 1);
        }

        /// <summary>Compute the size of an object.  Note that this only handles
        /// arrays or scalars of the primitive objects and Strings.  It
        /// returns 0 for any object array element it does not understand.</summary>
        /// <param name="o">The object whose size is desired.</param>
        public static int ComputeSize(Object o)
        {
            int result = 0;

            if (o != null)
            {
                Type t = o.GetType();
                if (t.IsArray)
                {
                    int size = 0;

                    for (IEnumerator i = ((Array)o).GetEnumerator(); i.MoveNext(); )
                    {
                        size += ComputeSize(i.Current);
                    }

                    //return size;
                    result = size;
                }
                else if (t == typeof(String))
                {
                    //return ((String)o).Length * (int)sizes[typeof(char)];
                    result = ((String)o).Length * (int)sizes[typeof(char)];
                }
                else if (t == typeof(Troolean))
                {
                    result = 1;
                }
                else if (t.IsPrimitive)
                {
                    //return (int)sizes[t];
                    result = (int)sizes[t];
                }
                else
                {
                    //return 0;
                    result = 0;
                }
            }

            return result;
        }

        /// <summary>Count the number of elements in an array</summary>
        public static int CountElements(Object o)
        {
            int result = 0;

            if (o.GetType().IsArray)
            {
                if (IsArrayOfArrays(o))
                {
                    for (IEnumerator i = ((Array)o).GetEnumerator(); i.MoveNext(); )
                    {
                        result += CountElements(i.Current);
                    }
                }
                else
                {
                    result = ((Array)o).Length;
                }
            }
            else
            {
                result = 1;
            }

            return result;
        }


        /// <summary>Try to create a deep clone of an Array or a standard clone of a scalar.
        /// The object may comprise arrays of any primitive type or any Object type which
        /// implements Cloneable.  However, if the Object is some kind of collection,
        /// e.g., a Vector then only a shallow copy of that object is made.  I.e., deep
        /// refers only to arrays.
        /// </summary>
        /// <param name="o">The object to be copied.</param>
        /*TODO: If multidimension array is passed as an input, it is getting flattened out
        TODO: For normal multidimension, we get an error because NewInstance always returns Jagged array.*/
        public static System.Object DeepClone(System.Object o)
        {
            if (o == null)
            {
                return null;
            }
            if (!o.GetType().IsArray)
            {
                return GenericClone(o);
            }

            Array a = (Array)o;
            if (ArrayFuncs.IsArrayOfArrays(o))
            {
                Array result = NewInstance(o.GetType().GetElementType(), a.Length);
                for (int i = 0; i < result.Length; ++i)
                {
                    result.SetValue(DeepClone(a.GetValue(i)), i);
                }

                return result;
            }
            else
            {
                int[] lengths = new int[a.Rank];
                for (int i = 0; i < lengths.Length; ++i)
                {
                    lengths[i] = a.GetLength(i);
                }
                Array result = ArrayFuncs.NewInstance(o.GetType().GetElementType(), lengths);
                Array.Copy(a, result, a.Length);

                return result;
            }
            return null;
        }

        /// <summary>Clone an Object if possible.
        /// *
        /// This method returns an Object which is a clone of the
        /// input object.  It checks if the method implements the
        /// Cloneable interface and then uses reflection to invoke
        /// the clone method.  This can't be done directly since
        /// as far as the compiler is concerned the clone method for
        /// Object is protected and someone could implement Cloneable but
        /// leave the clone method protected.  The cloning can fail in a
        /// variety of ways which are trapped so that it returns null instead.
        /// This method will generally create a shallow clone.  If you
        /// wish a deep copy of an array the method DeepClone should be used.
        /// *
        /// </summary>
        /// <param name="o">The object to be cloned.
        /// 
        /// </param>
        public static Object GenericClone(Object o)
        {
            if (!(o is ICloneable))
            {
                return null;
            }

            return ((ICloneable)o).Clone();
        }


        /// <summary>Find the dimensions of an object.
        /// *
        /// This method returns an integer array with the dimensions
        /// of the object o which should usually be an array.
        /// *
        /// It returns an array of dimension 0 for scalar objects
        /// and it returns -1 for dimension which have not been allocated,
        /// e.g., int[][][] x = new int[100][][]; should return [100,-1,-1].
        /// *
        /// </summary>
        /// <param name="o">The object to get the dimensions of.</param>
        public static int[] GetDimensions(Object o)
        {

            if (o == null)
            {
                return new int[0];
            }

            // object is of type Array
            if (o.GetType().IsArray)
            {
                // if the object is a one-dimensional Array, the dimension can be retrieve by GetLength() directly.
                if (o.GetType().GetArrayRank() > 1)
                {
                    Array a = (Array)o;
                    int ndim = a.Rank;
                    int[] dimens = new int[ndim];
                    for (int i = 0; i < ndim; i += 1)
                    {
                        dimens[i] = a.GetLength(i);
                    }

                    return dimens;
                }
                // if its Jagged array
                else
                {
                    int ndim = 1;

                    Object temp = o;
                    bool done = false;
                    while (!done)
                    {
                        if (temp is Array)
                        {
                            // if the array is 1D with no elements defined in it.
                            if (((Array)temp).Length == 0)
                            {
                                done = true;
                            }
                            // else check for the element type whether it is an Array or not.
                            else if (((Array)temp).GetValue(0) is Array)
                            {
                                temp = ((Array)temp).GetValue(0);
                                ndim++;
                                continue;
                            }

                            // if there is only one element at a particular dimension with data of type primitive.
                            if (((Array)temp).Length == 1)
                            {
                                done = true;
                            }

                            for (int i = 1; i < ((Array)temp).Length; i++)
                            {
                                if (((Array)temp).GetValue(i) is Array)
                                {
                                    temp = ((Array)o).GetValue(i);
                                    ndim++;
                                    break;
                                }
                                else if (i != ((Array)temp).Length - 1)
                                {
                                    continue;
                                }
                                else
                                {
                                    done = true;
                                    break;
                                }
                            }
                        }
                        else
                            done = true;
                    }


                    int[] dimens = new int[ndim];
                    for (int i = 0; i < ndim; i += 1)
                    {
                        dimens[i] = -1; // So that we can distinguish a null from a 0 length.
                    }

                    for (int i = 0; i < ndim; i += 1)
                    {
                        dimens[i] = ((Array)o).Length;
                        if (dimens[i] == 0)
                        {
                            return dimens;
                        }
                        if (i != ndim - 1)
                        {
                            Object x;
                            x = ((Array)o).GetValue(0);

                            // If first element of array is null, then check for other elements.
                            if (x == null)
                            {
                                int j = 1;
                                for (; j < dimens[i]; j++)
                                {
                                    if (((Array)o).GetValue(j) == null)
                                        continue;
                                    else
                                        o = ((Array)o).GetValue(j);
                                }

                                // if all elements are null
                                if (j == dimens[i] - 1)
                                    return dimens;
                            }
                            else
                            {
                                o = x;
                            }
                        }
                    }

                    return dimens;
                }
            }
            // if object is of primitive type
            else
            {
                return new int[] { 1 };
            }

        }


        /// <summary>This routine returns the base class of an object. This is just
        /// the class of the object for non-arrays.</summary>
        public static Type GetBaseClass(Object o)
        {
            if (o == null)
                return Type.GetType("System.Void");

            if (o.GetType().IsArray)
            {
                // if 'o' is a jagged array of 2 or more than 2 dimensions
                // with all dimensions of type premitive, 
                // check for the type of last dimension.
                if (o.GetType().ToString().StartsWith("System.Array"))
                {
                    return GetBaseClass(((Array)o).GetValue(0));
                }
                // if 'o' is a multidimension array of any dimension 
                // or a jagged array of 2 or more than 2 dimension having
                // first dimension of non-primitive type,
                // return the type of 'o' directly without dimension brackets.
                else
                {
                    return Type.GetType(o.ToString().Substring(0, o.ToString().IndexOf("[")));
                }
            }
            else
            {
                return o.GetType();
            }

        }

        /// <summary>This routine returns the base class of an object with the dimension indication
        /// in form of brackets and/or commas. This is just
        /// the type of the object for non-arrays.</summary>
        /// TODO: This routine is implemented to get type of the object including all the dimensions with it.
        /// If object 'x' is 2D Array type with elements filled in of type byte,
        /// then x.GetType will return only System.Array[], but this routine will give System.Byte[][].
        /// One can remove this method in future, if similar functionality provided by any library routine itself.
        /// "This routine is only used in ArrayEquals."
        public static Type GetBaseType(Object x)
        {
            Type baseClass = GetBaseClass(x);
            int dim = CountDimensions(x);

            if (dim == 0)
                return baseClass;

            String baseCl = baseClass.ToString();
            Array a = (Array)x;
            if (a.Rank > 1)
            {
                baseCl += "[";
                for (int i = 0; i < dim - 1; i++)
                    baseCl += ",";
                baseCl += "]";
            }
            else
            {
                for (int i = 0; i < dim; i++)
                    baseCl += "[]";
            }

            return Type.GetType(baseCl);
        }

        /// <summary>This routine returns the size of the base element of an array.</summary>
        /// <param name="o">The array object whose base length is desired.</param>
        /// <returns>the size of the object in bytes, 0 if null, or -1 if not a primitive array.
        /// </returns>
        public static int GetBaseLength(Object o)
        {
            if (o == null)
            {
                return 0;
            }

            Type t = GetBaseClass(o);

            if (t.IsPrimitive)
            {
                return (int)sizes[t];
            }
            else
            {
                return -1;
            }
        }


        /// <summary>Generate a description of an array (presumed rectangular).</summary>
        /// <param name="o">The array to be described.</param>
        public static String ArrayDescription(Object o)
        {
            if (o == null)
            {
                return "null";
            }
            else if (o.GetType().IsArray)
            {
                return CountDimensions(o) + "D array of " + GetBaseClass(o).FullName;
            }
            else
            {
                return o.GetType().FullName;
            }
        }


        /// <summary>Given an array of arbitrary dimensionality return
        /// the array flattened into a single dimension.</summary>
        /// <param name="input">The input array.</param>
        public static Object Flatten(Object input)
        {
            int[] dimens = GetDimensions(input);
            if (dimens.Length <= 1)
            {
                return input;
            }
            int size = 1;
            for (int i = 0; i < dimens.Length; i += 1)
            {
                size *= dimens[i];
            }

            Array flat = NewInstance(GetBaseClass(input), size);

            if (size == 0)
            {
                return flat;
            }

            int offset = 0;
            DoFlatten((Array)input, flat, offset);

            return flat;
        }


        /// <summary>This routine does the actually flattening of multi-dimensional arrays.</summary>
        /// <param name="input"> The input array to be flattened.</param>
        /// <param name="output">The flattened array.</param>
        /// <param name="offset">The current offset within the output array.</param>
        /// <returns>The number of elements within the array.</returns>
        protected internal static int DoFlatten(Object input, Array output, int offset)
        {
            int result = 0;

            if (IsArrayOfArrays(input))
            {
                int i = offset;
                for (IEnumerator e = ((Array)input).GetEnumerator(); e.MoveNext(); )
                {
                    Object a = (Object)e.Current;
                    result += DoFlatten(a, output, offset + result);
                }
            }
            else if (input.GetType().IsArray)
            {
                IEnumerator e = ((Array)input).GetEnumerator();
                for (int i = offset; e.MoveNext(); ++i)
                {
                    output.SetValue(e.Current, i);
                    ++result;
                }
            }
            else
            {
                output.SetValue(input, offset);
                result = 1;
            }

            return result;
        }

        /// <summary>Curl an input array up into a multi-dimensional array.</summary>
        /// <param name="input">The one dimensional array to be curled.</param>
        /// <param name="dimens">The desired dimensions</param>
        /// <returns>The curled array.</returns>
        public static Array Curl(Array input, int[] dimens)
        {
            if (CountDimensions(input) != 1)
            {
                throw new SystemException("Attempt to curl non-1D array");
            }

            int test = 1;
            for (int i = 0; i < dimens.Length; i += 1)
            {
                test *= dimens[i];
            }

            if (test != input.Length)
            {
                throw new SystemException("Curled array does not fit desired dimensions");
            }

            Array newArray = NewInstance(GetBaseClass(input), dimens);

            int[] index = new int[dimens.Length];
            index[index.Length - 1] = -1;
            for (int i = 0; i < index.Length - 1; ++i)
            {
                index[i] = 0;
            }

            for (IEnumerator i = input.GetEnumerator(); i.MoveNext() && NextIndex(index, dimens); )
            {
                //NewInstance call creates a jagged array. So we cannot set the value using Array.SetValue
                //as it works for multi-dimensional arrays and not for jagged
                //newArray.SetValue(i.Current, index);

                Array tarr = newArray;
                for (int i2 = 0; i2 < index.Length - 1; i2++)
                {
                    tarr = (Array)tarr.GetValue(index[i2]);
                }
                tarr.SetValue(i.Current, index[index.Length - 1]);
            }
            //int offset = 0;
            //DoCurl(input, newArray, dimens, offset);
            return newArray;
        }
        /// <summary>
        /// Check whether next index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dims"></param>
        /// <returns>returns boolean value depending upon the values in array</returns>
        public static bool NextIndex(int[] index, int[] dims)
        {
            bool ok = false;

            for (int i = index.Length - 1; i >= 0 && !ok; --i)
            {
                index[i] += 1;
                if (index[i] < dims[i])
                {
                    ok = true;
                }
                else
                {
                    index[i] = 0;
                }
            }

            return ok;
        }
        /// <summary>
        /// Checks whether next index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="starts"></param>
        /// <param name="dims"></param>
        /// <returns></returns>
        public static bool NextIndex(int[] index, int[] starts, int[] dims)
        {
            bool ok = false;

            for (int i = index.Length - 1; i >= 0 && !ok; --i)
            {
                index[i] += 1;
                if (index[i] < dims[i])
                {
                    ok = true;
                }
                else
                {
                    index[i] = starts[i];
                }
            }

            return ok;
        }

        /*
            /// <summary>Do the curling of the 1-d to multi-d array.</summary>
            /// <param name="input"> The 1-d array to be curled.</param>
            /// <param name="output">The multi-dimensional array to be filled.</param>
            /// <param name="dimens">The desired output dimensions.</param>
            /// <param name="offset">The current offset in the input array.</param>
            /// <returns>The number of elements curled.</returns>
            protected internal static int DoCurl(Array input, Array output, int[] dimens, int offset)
            {			
                if(dimens.Length == 1)
                {
                    Array.Copy(input, offset, output, 0, dimens[0]);
                    return dimens[0];
                }
			
                int total = 0;
                int[] xdimens = new int[dimens.Length - 1];
          Array.Copy(dimens, 1, xdimens, 0, xdimens.Length);
          for(int i = 0; i < dimens[0]; i += 1)
          {
            total += DoCurl(input, (Array)output.GetValue(i), xdimens, offset + total);
          }
                return total;
            }
        */
        /// <summary>Allocate an array dynamically. The Array.NewInstance method
        /// does not throw an error.</summary>
        /// <param name="cl"> The class of the array.</param>
        /// <param name="dim"> The dimension of the array.</param>
        /// <returns> The allocated array.</returns>
        public static Array NewInstance(Type cl, int dim)
        {
            Array o = Array.CreateInstance(cl, dim);
            if (o == null)
            {
                String desc = cl + "[" + dim + "]";
                throw new System.OutOfMemoryException("Unable to allocate array: " + desc);
            }
            return o;
        }

        /// <summary>Allocate an array dynamically. The Array.NewInstance method
        /// does not throw an error.</summary>
        /// <param name="cl"> The class of the array.</param>
        /// <param name="dims">The dimensions of the array.</param>
        /// <returns>The allocated array.</returns>
        /// <throws>An OutOfMemoryError if insufficient space is available.</throws>
        public static Array NewInstance(Type cl, int[] dims)
        {
            return NewInstance(cl, dims, 0);
        }
        /// <summary>allocate an arrayof passed dimensions dynamically. The Array.NewInstance method
        /// does not throw an error
        /// </summary>
        /// <param name="cl">The class of the array.</param>
        /// <param name="dims">The dimensions of the array.</param>
        /// <param name="dim">The index in the array</param>
        /// <returns>The allocated array.</returns>
        public static Array NewInstance(Type cl, int[] dims, int dim)
        {
            // suggested in .99 version:
            // Treat a scalar as a 1-d array of length 1
            if (dims.Length == 0)
            {
                dims = new int[] { 1 };
            }

            if (dim == dims.Length - 1)
            {
                return NewInstance(cl, dims[dim]);
            }
            else
            {
                Array a = new Array[dims[dim]];
                for (int i = 0; i < a.Length; ++i)
                {
                    a.SetValue(NewInstance(cl, dims, dim + 1), i);
                }

                return a;
            }
            /*
            Array o = Array.CreateInstance(cl, dims);
            if(o == null)
            {
              System.String desc = cl + "[";
              System.String comma = "";
              for(int i = 0; i < dims.Length; i += 1)
              {
                desc += comma + dims[i];
                comma = ",";
              }
              desc += "]";
              throw new System.OutOfMemoryException("Unable to allocate array: " + desc);
            }
            return o;
            */
        }


        // suggested in .99.2 version:
        /// <summary>
        /// Are two objects equal?  Arrays have the standard object equals
        /// method which only returns true if the two object are the same.
        /// This method returns true if every element of the arrays match.
        /// The inputs may be of any dimensionality.  The dimensionality
        /// and dimensions of the arrays must match as well as any elements.
        /// If the elements are non-primitive. non-array objects, then the 
        /// equals method is called for each element.
        /// If both elements are multi-dimensional arrays, then
        /// the method recurses.
        /// </summary>
        public static bool ArrayEquals(Object x, Object y)
        {
            return ArrayEquals(x, y, 0, 0);
        }

        // suggested in .99.2 version:
        /// <summary>
        /// Are two objects equal?  Arrays have the standard object equals
        /// method which only returns true if the two object are the same.
        /// This method returns true if every element of the arrays match.
        /// The inputs may be of any dimensionality.  The dimensionality
        /// and dimensions of the arrays must match as well as any elements.
        /// If the elements are non-primitive. non-array objects, then the 
        /// equals method is called for each element.
        /// If both elements are multi-dimensional arrays, then
        /// the method recurses.
        ///   </summary>
        /*TODO: This method handles only int, float, long, double, byte, Objects 
         does not handle signed byte, unsigned (int/float/long/double/byte)
         TODO: If the passed arguments are multidimension Arrays, a[3,2] and b[2,3]
        and if they have the same elements then the return value is true. 
        (Should we return false?)*/
      
        public static bool ArrayEquals(Object x, Object y, double tolf, double told)
        {

            // Handle the special cases first.
            // We treat null == null so that two object arrays
            // can match if they have matching null elements.
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            //Type xClass = x.GetType();
            //Type yClass = y.GetType();

            Type xClass = GetBaseType(x);
            Type yClass = GetBaseType(y);

            if (xClass != yClass)
            {
                return false;
            }

            // if type of x is not an array
            if (!xClass.IsArray)
            {
                return x.Equals(y);

            }
            else
            {
                bool comparison = true;
                Array tx = (Array)x;
                Array ty = (Array)y;

                if (xClass.Equals(typeof(int[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;
                }
                else if (xClass.Equals(typeof(double[])))
                {
                    if (told == 0)
                    {
                        for (int i = 0; i < ((Array)x).Length && comparison; i++)
                            comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                        return comparison;
                    }
                    else
                    {
                        return DoubleArrayEquals((double[])x, (double[])y, told);
                    }
                }
                else if (xClass.Equals(typeof(long[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;
                }
                else if (xClass.Equals(typeof(float[])))
                {
                    if (tolf == 0)
                    {
                        for (int i = 0; i < ((Array)x).Length && comparison; i++)
                            comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                        return comparison;
                    }
                    else
                    {
                        return FloatArrayEquals((float[])x, (float[])y, (float)tolf);
                    }
                }
                else if (xClass.Equals(typeof(byte[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;
                }
                else if (xClass.Equals(typeof(short[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;
                }
                else if (xClass.Equals(typeof(char[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;
                }
                else if (xClass.Equals(typeof(bool[])))
                {
                    for (int i = 0; i < ((Array)x).Length && comparison; i++)
                        comparison = comparison && ArrayEquals(tx.GetValue(i), ty.GetValue(i));
                    return comparison;

                }
                // Array having more than 1 dimension
                else
                {
                    // Multi-dimension Arrays
                    if (((Array)x).Rank > 1)
                    {
                        Array xo = (Array)x;
                        Array yo = (Array)y;

                        if (xo.Rank != yo.Rank)
                        {
                            return false;
                        }
                        else if (xo.Length != yo.Length)
                        {
                            return false;
                        }

                        // Check for values at each dimension.
                        for (int i = 0; i < xo.Length; i++)
                        {
                            IEnumerator xe = xo.GetEnumerator();
                            xe.MoveNext();

                            IEnumerator ye = yo.GetEnumerator();
                            ye.MoveNext();

                            if (!ArrayEquals(xe.Current, ye.Current, tolf, told))
                            {
                                return false;
                            }
                        }
                    }
                    // Non-primitive and Jagged Arrays
                    else
                    {
                        // type casting to Object[]
                        Object[] xo = (Object[])x;
                        Object[] yo = (Object[])y;

                        if (xo.Length != yo.Length)
                        {
                            return false;
                        }

                        for (int i = 0; i < xo.Length; i += 1)
                        {
                            if (!ArrayEquals(xo[i], yo[i], tolf, told))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    return true;
                }
            }
        }

        // suggested in .99.2 version:
        /// <summary> Compare two double arrays using a given tolerance.</summary>
        static bool DoubleArrayEquals(double[] x, double[] y, double tol)
        {

            for (int i = 0; i < x.Length; i += 1)
            {
                if (x[i] == 0)
                {
                    return y[i] == 0;
                }
                if (Math.Abs((y[i] - x[i]) / x[i]) > tol)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> Compare two float arrays using a given tolerance.</summary>
        static bool FloatArrayEquals(float[] x, float[] y, float tol)
        {
            for (int i = 0; i < x.Length; i += 1)
            {
                if (x[i] == 0)
                {
                    return y[i] == 0;
                }
                if (Math.Abs((y[i] - x[i]) / x[i]) > tol)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Convert an array to a specified type. This method supports conversions
        /// only among the primitive numeric types.</summary>
        /// <param name="array">A possibly multidimensional array to be converted.</param>
        /// <param name="newType">The desired output type.  This should be one of the
        ///                class descriptors for primitive numeric data, e.g., double.type.</param>
        public static Object ConvertArray(Object array, Type newType)
        {
            // We break this up into two steps so that users
            // can reuse an array many times and only allocate a
            // new array when needed.

            // First create the full new array.
            Object mimic = MimicArray(array, newType);
            if (mimic != null)
            {
                // Now copy the info into the new array.
                CopyArray(array, mimic);
            }

            return mimic;
        }


        /// <summary>Create an array of a type given by new type with
        /// the dimensionality given in array.</summary>
        /// <param name="array">A possibly multidimensional array to be converted.</param>
        /// <param name="newType">The desired output type.  This should be one of the
        /// class descriptors for primitive numeric data, e.g., double.type.</param>		
        public static System.Object MimicArray(System.Object array, System.Type newType)
        {
            if (array == null || !array.GetType().IsArray)
            {
                return null;
            }

            int dims = array.GetType().GetArrayRank();

            Object mimic;

            if (dims > 1)
            {
                Object[] xarray = (Object[])array;
                int[] dimens = new int[dims];
                dimens[0] = xarray.Length; // Leave other dimensions at 0.

                mimic = ArrayFuncs.NewInstance(newType, dimens);

                for (int i = 0; i < xarray.Length; i += 1)
                {
                    System.Object temp = MimicArray(xarray[i], newType);
                    ((System.Object[])mimic)[i] = temp;
                }
            }
            else
            {
                mimic = ArrayFuncs.NewInstance(newType, GetDimensions(array));
            }

            return mimic;
        }

        /// <summary>Copy one array into another.
        /// This function copies the contents of one array
        /// into a previously allocated array.
        /// The arrays must agree in type and size.
        /// </summary>
        /// <param name="original">The array to be copied.
        /// </param>
        /// <param name="copy">    The array to be copied into.  This
        /// array must already be fully allocated.
        /// 
        /// </param>
        public static void CopyArray(System.Object original, System.Object copy)
        {
            System.String oname = original.GetType().FullName;
            System.String cname = copy.GetType().FullName;

            if (!original.GetType().IsArray || !copy.GetType().IsArray ||
               !original.GetType().GetElementType().Equals(copy.GetType().GetElementType()) ||
               !original.GetType().GetArrayRank().Equals(copy.GetType().GetArrayRank()))
            {
                return;
            }

            if (original.GetType().GetArrayRank() >= 2)
            {
                System.Object[] x = (System.Object[])original;
                System.Object[] y = (System.Object[])copy;
                for (int i = 0; i < x.Length; i += 1)
                {
                    CopyArray(x, y);
                }
            }
            else
            {
                Array.Copy((Array)original, 0, (Array)copy, 0, ((Array)original).Length);
            }
        }

        #region Useless Crap
        /*
        /// <summary> Count the number of elements in an array.</summary>
        public static int NElements(Object o)
        {
            String classname = o.GetType().Name;
            if (classname[1] == '[')
            {
                int count = 0;
                for (int i = 0; i < ((Object[])o).Length; i += 1)
                {
                    count += NElements(((Object[])o)[i]);
                }
                return count;

            }
            else if (classname[0] == '[')
            {
                return ((Array)o).Length;

            }
            else
            {
                return 1;
            }
        }
        */
        /*
        /// <summary> Convert an array to a specified type. This method supports conversions
        /// only among the primitive numeric types.</summary>
        /// <param name="array">A possibly multidimensional array to be converted.</param>
        /// <param name="newType">The desired output type.  This should be one of the
        ///                       class descriptors for primitive numeric data, e.g., double.type.</param>
        /// <param name="reuse"> If set, and the requested type is the same as the
        ///                         original, then the original is returned.</param>
        public static Object ConvertArray(Object array, Type newType, bool reuse)
        {

            if (GetBaseClass(array) == newType && reuse)
            {
                return array;
            }
            else
            {
                return ConvertArray(array, newType);
            }
        }
        */
        /*
          /// <summary>This routine returns the base array of a multi-dimensional
          /// array.  I.e., a one-d array of whatever the array is composed
          /// of.  Note that arrays are not guaranteed to be rectangular,
          /// so this returns o[0][0]....
          /// </summary>
          public static Object GetBaseArray(Object o)
          {
            if(o.GetType().IsArray)
            {
              IEnumerator i = ((Array)o).GetEnumerator();
              i.MoveNext();
              return i.Current;
            }
          }
      */
        /*
        /// <summary>Create an array and populate it with a test pattern.</summary>
        /// <param name="baseType"> The base type of the array. This is expected to
        /// be a numeric type, but this is not checked.</param>
        /// <param name="dims">The desired dimensions.</param>
        /// <returns> An array object populated with a simple test pattern.</returns>
        public static System.Object GenerateArray(System.Type baseType, int[] dims)
        {
			
          // Generate an array and populate it with a test pattern of
          // data.
			
          System.Object x = ArrayFuncs.NewInstance(baseType, dims);
          testPattern(x, (sbyte) 0);
          return x;
        }
		
        /// <summary>Just create a simple pattern cycling through valid byte values.
        /// We use bytes because they can be cast to any other numeric type.
        /// </summary>
        /// <param name="o">     The array in which the test pattern is to be set.
        /// </param>
        /// <param name="start"> The value for the first element.
        /// 
        /// </param>
        public static sbyte TestPattern(System.Object o, sbyte start)
        {
			
          int[] dims = getDimensions(o);
          if (dims.Length > 1)
          {
            for (int i = 0; i < ((System.Object[]) o).Length; i += 1)
            {
              start = testPattern(((System.Object[]) o)[i], start);
            }
          }
          else if (dims.Length == 1)
          {
            for (int i = 0; i < dims[0]; i += 1)
            {
              ((System.Array) o).SetValue(start, i);
              start = (sbyte) (start + 1);
            }
          }
          return start;
        }
        */
        /*
        /// <summary>Copy one array into another.
        /// This function copies the contents of one array
        /// into a previously allocated array.
        /// The arrays must agree in type and size.
        /// </summary>
        /// <param name="original">The array to be copied.
        /// </param>
        /// <param name="copy">    The array to be copied into.  This
        /// array must already be fully allocated.
        /// 
        /// </param>
        public static void CopyArray(System.Object original, System.Object copy)
        {
            System.String oname = original.GetType().FullName;
            System.String cname = copy.GetType().FullName;

            if (!original.GetType().IsArray || !copy.GetType().IsArray ||
               !original.GetType().GetElementType().Equals(copy.GetType().GetElementType()) ||
               !original.GetType().GetArrayRank().Equals(copy.GetType().GetArrayRank()))
            {
                return;
            }

            if (original.GetType().GetArrayRank() >= 2)
            {
                System.Object[] x = (System.Object[])original;
                System.Object[] y = (System.Object[])copy;
                for (int i = 0; i < x.Length; i += 1)
                {
                    CopyArray(x, y);
                }
            }
            else
            {
                Array.Copy((Array)original, 0, (Array)copy, 0, ((Array)original).Length);
            }
        }
         */
        /*
          private static Type GetBaseClass(Object o, Type parent)
          {
            
              if (o == null && !parent.IsArray)
              {
                  return Type.GetType("System.Void");
              }
              else if (o == null && parent.IsArray)
              {
                  // return Type.GetType( parent.ToString().Substring(0, parent.ToString().IndexOf("[")) );
                  return parent.GetElementType();
              }

              if (o.GetType().IsArray)
              {
                  IEnumerator i = ((Array)o).GetEnumerator();
                  i.MoveNext();

                  return GetBaseClass(i.Current, o.GetType());
              }
              else
              {
                  return o.GetType();
              }
          }
        */
        #endregion
    }
}

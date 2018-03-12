using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapToPalette
{
    /// <summary>
    /// Utility functions
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Return a pre-allocated jagged array
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="arrayCount">Number of arrays to allocate</param>
        /// <param name="arraySize">Nuber of elements per array</param>
        /// <returns>Array of arrays of type T</returns>
        public static T[][] Allocate2d<T>(int arrayCount, int arraySize)
        {
            if (arrayCount < 0 || arraySize < 0)
            {
                throw new ArgumentException("Array count and -size must be positive");
            }

            var array = new T[arrayCount][];

            for (var n = 0; n < arrayCount; ++n)
            {
                array[n] = new T[arraySize];
            }

            return array;
        }

        /// <summary>
        /// Return a custom attribute of the given type
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="obj">Instance of the type to get the attribute from</param>
        /// <returns>Custom attribute if present in the assembly</returns>
        public static T GetCustomAttribute<T>(this Object obj) where T : Attribute
            => ((T)obj.GetType().Assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault());

        /// <summary>
        /// Return the minimum element of an enumeration by a custom evaluation function
        /// </summary>
        /// <typeparam name="TElement">Type of the sequence elements</typeparam>
        /// <typeparam name="TCompare">Result type of the evaluation function</typeparam>
        /// <param name="sequence">Sequence of elements to return the minimum element of</param>
        /// <param name="evaluate">The element with the smallest value returned by this function is returned</param>
        /// <returns>First element that returns the smallest value for <paramref name="evaluate"/></returns>
        public static TElement MinBy<TElement, TCompare>(
            this IEnumerable<TElement> sequence,
            Func<TElement, TCompare> evaluate)
            where TCompare : IComparable<TCompare>
        {
            var val = sequence.First();
            var min = evaluate(val);
            var best = (min, val);

            var (_, result) = sequence.Skip(1).Aggregate(best, (res, cur) =>
            {
                var cmp = evaluate(cur);
                return cmp.CompareTo(res.min) < 0 ? (cmp, cur) : res;
            });

            return result;
        }

        /// <summary>
        /// Return the squared Euclidean distance between to another colour
        /// </summary>
        /// <param name="colour">Colour value</param>
        /// <param name="other">Colour toget the distance to</param>
        /// <returns>Squared Euclidean distance of the R, G, and B components</returns>
        public static int SquaredDistance(this ColorBgra colour, ColorBgra other)
        {
            return (colour.B - other.B).Squared() + (colour.G - other.G).Squared() + (colour.R - other.R).Squared();
        }

        /// <summary>
        /// Return the square of an integer
        /// </summary>
        /// <param name="n">Integer to square</param>
        /// <returns>Squared value of <paramref name="n"/></returns>
        public static int Squared(this int n) => n * n;

        /// <summary>
        /// Return a memoised version of a pure function 
        /// </summary>
        /// <typeparam name="T">Type of the function parameter</typeparam>
        /// <typeparam name="R">Type of the function's return value</typeparam>
        /// <param name="fn">Function to memoise</param>
        /// <returns>A function that caches the results of previous calls to the original function</returns>
        /// <remarks>The provided function <b>must</b> be pure!</remarks>
        public static Func<T, R> Memoise<T, R>(this Func<T, R> fn)
        {
            var cache = new Dictionary<T, R>();

            R Lookup(T value)
            {
                if (!cache.TryGetValue(value, out R result))
                {
                    result = fn(value);
                    cache.Add(value, result);
                }
                return result;
            }

            return Lookup;
        }
    }
}

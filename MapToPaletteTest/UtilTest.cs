using MapToPalette;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace MapToPaletteTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void Allocate2d_Accepts_Empty_Array_Count()
        {
            var arrayCount = 0;
            var arraySize = 0;
            var empty = Util.Allocate2d<int>(arrayCount, arraySize);
            Assert.IsNotNull(empty);
            Assert.AreEqual(arrayCount, empty.Length);
        }

        [TestMethod]
        public void Allocate2d_Accepts_Empty_Array_Length()
        {
            var arrayCount = 5;
            var arraySize = 0;
            var empty = Util.Allocate2d<int>(arrayCount, arraySize);
            Assert.IsNotNull(empty);
            Assert.AreEqual(arrayCount, empty.Length);
            Array.ForEach(empty, array =>
            {
                Assert.IsNotNull(array);
                Assert.AreEqual(arraySize, array.Length);
            });
        }

        [TestMethod]
        public void Allocate2d_Initialises_Arrays()
        {
            var arrayCount = 3;
            var arraySize = 5;
            var nonEmpty = Util.Allocate2d<int>(arrayCount, arraySize);
            Assert.IsNotNull(nonEmpty);
            Assert.AreEqual(arrayCount, nonEmpty.Length);
            Array.ForEach(nonEmpty, array =>
            {
                Assert.IsNotNull(array);
                Assert.AreEqual(arraySize, array.Length);
            });
        }

        [TestMethod]
        public void Allocate2d_Throws_Given_Negative_Array_Count()
        {
            Assert.ThrowsException<ArgumentException>(() => Util.Allocate2d<int>(-5, 2));
        }

        [TestMethod]
        public void Allocate2d_Throws_Given_Negative_Array_Length()
        {
            Assert.ThrowsException<ArgumentException>(() => Util.Allocate2d<int>(5, -1));
        }

        [TestMethod]
        public void GetCustomAttribute_Returns_Existing_Attribute()
        {
            var attr = this.GetCustomAttribute<AssemblyTitleAttribute>();
            Assert.IsNotNull(attr);
        }

        [TestMethod]
        public void GetCustomAttribute_Returns_Null_For_Missing_Attribute()
        {
            var attr = this.GetCustomAttribute<AssemblyTrademarkAttribute>();
            Assert.IsNull(attr);
        }

        [TestMethod]
        public void MinBy_Throws_Given_Empty_Sequence()
        {
            var n = new int[0];
            int Negated(int x) => -x;
            Assert.ThrowsException<InvalidOperationException>(() => n.MinBy(Negated));
        }

        [TestMethod]
        public void MinBy_Returns_Single_Element()
        {
            var expected = 42;
            var n = new int[] { expected };
            int Negated(int x) => -x;
            var actual = n.MinBy(Negated);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MinBy_Returns_Minimum_Element()
        {
            var r = new Random();
            var n = Enumerable.Range(0, 17).Select(i => r.Next(100)).ToArray();
            var expected = n.Max();
            int Negated(int x) => -x;
            var actual = n.MinBy(Negated);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Memoise_Returns_Cached_Values()
        {
            var callCount = 0;
            int Sq(int n)
            {
                ++callCount;
                return n * n;
            }

            Func<int, int> fn = Sq;
            var memoised = fn.Memoise();

            for (var i = 0; i < 3; ++i)
            {
                Assert.AreEqual(Sq(2), memoised(2));
                Assert.AreEqual(Sq(3), memoised(3));
                Assert.AreEqual(Sq(4), memoised(4));
            }

            var expected = 12;
            Assert.AreEqual(expected, callCount);
        }
    }
}

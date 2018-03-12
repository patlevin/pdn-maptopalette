using System;
using System.Linq;
using MapToPalette;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapToPaletteTest
{
    [TestClass]
    public class DitherKernelTest
    {
        private static readonly int[] SierraLite = { 0, 0, 2, 1, 1, 0 };

        private static readonly float[] SierraLiteCoeff = { 0.0f, 0.0f, 0.5f, 0.25f, 0.25f, 0.0f };

        [TestMethod]
        public void Ctor_Assigns_Factor_To_Sum()
        {
            var dither = new DitherKernel(SierraLite, 3);
            var actual = dither.Coefficients.ToArray();
            for (int i = 0; i < SierraLiteCoeff.Length; ++i)
            {
                Assert.AreEqual(SierraLiteCoeff[i], actual[i]);
            }
        }

        [TestMethod]
        public void Ctor_Calculates_Coefficients()
        {
            var factor = 6.0f;
            var dither = new DitherKernel(SierraLite, 3, (int)factor);
            var actual = dither.Coefficients.ToArray();
            for (int i = 0; i < SierraLiteCoeff.Length; ++i)
            {
                Assert.AreEqual(SierraLite[i] / factor, actual[i]);
            }
        }

        [TestMethod]
        public void Ctor_Accepts_Coeffecients()
        {
            var dither = new DitherKernel(SierraLiteCoeff, 3);
            var actual = dither.Coefficients.ToArray();
            for (int i = 0; i < SierraLiteCoeff.Length; ++i)
            {
                Assert.AreEqual(SierraLiteCoeff[i], actual[i]);
            }
        }

        [TestMethod]
        public void Ctor_Throws_Given_Invalid_Arguments()
        {
            // negative factor
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, 3, -1));
            // nought factor
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, 3, 0));
            // negative width
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, -1));
            // nought width
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, 0));
            // incompatible width
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, 4));
            // width too big
            Assert.ThrowsException<ArgumentException>(() => new DitherKernel(SierraLite, 7));
        }

        [TestMethod]
        public void Columns_Holds_Kernel_Width()
        {
            var width = 3;
            var dither = new DitherKernel(SierraLite, width);
            Assert.AreEqual(width, dither.Columns);
        }

        [TestMethod]
        public void Rows_Holds_Kernel_Rows()
        {
            var width = 3;
            var dither = new DitherKernel(SierraLite, width);
            var expected = SierraLite.Length / width;
            Assert.AreEqual(expected, dither.Rows);
        }

        [TestMethod]
        public void Centre_Holds_Kernel_Centre_Point()
        {
            var width = 3;
            var dither = new DitherKernel(SierraLite, width);
            var expected = 1;
            Assert.AreEqual(expected, dither.Centre);
        }

        [TestMethod]
        public void Index_Operator_Returns_Coefficient()
        {
            var width = 3;
            var dither = new DitherKernel(SierraLite, width);

            for (int y = 0, n = 0; y < dither.Rows; ++y)
            {
                for (int x = 0; x < dither.Columns; ++x, ++n)
                {
                    var actual = dither[x, y];
                    var expected = SierraLiteCoeff[n];
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }
}

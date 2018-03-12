using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MapToPalette;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet;

namespace MapToPaletteTest
{
    [TestClass]
    public class ErrorDiffusionTest
    {
        // gets a greyscale test image
        private static (ColorBgra[] data, Size size) Image
            => LoadResource(Resources.gray);
        // gets a black-and-white, Floyd-Steinberg-dithered version of the test image
        private static (ColorBgra[] data, Size size) DitheredImage
            => LoadResource(Resources.bwfs);

        [TestMethod]
        public void Ctor_Throws_Given_Null_Kernel()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ErrorDiffusion(null, 1, 100));
        }

        [TestMethod]
        public void Ctor_Throws_Given_Invalid_Amount()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ErrorDiffusion(DitherKernel.Sierra, -1.0f, 100));
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ErrorDiffusion(DitherKernel.Sierra, 1.01f, 100));
        }

        [TestMethod]
        public void Ctor_Throws_Given_Invalid_Pixel_Count()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new ErrorDiffusion(DitherKernel.Sierra, 1.0f, -1));
            Assert.ThrowsException<ArgumentException>(
                () => new ErrorDiffusion(DitherKernel.Sierra, 1.0f, 0));
        }

        [TestMethod]
        public void Ctor_Initialises_Amount_To_Given_Argument()
        {
            var expected = 0.4f;
            var errorDiffusion = new ErrorDiffusion(DitherKernel.Sierra, expected, 16);
            Assert.AreEqual(expected, errorDiffusion.Amount);
        }

        [TestMethod]
        public void GetFinalColour_Respects_And_Spreads_Error()
        {
            var (image, size) = Image;
            var (dithered, dsize) = DitheredImage;
            var kernel = DitherKernel.FloydSteinberg;
            var dither = new ErrorDiffusion(kernel, 1, size.Width);
            var expected = GetMeanSquaredError(image, dithered);
            var total = 0L;

            ColorBgra Threshold(ColorBgra bgra) =>
                bgra.GetIntensityByte() > 128 ? ColorBgra.White : ColorBgra.Black;

            for (int y = 0, n = 0; y < size.Height; ++y)
            {
                for (int x = 0; x < size.Width; ++x, ++n)
                {
                    var original = image[n];
                    var actual = dither.GetFinalColour(x, original, Threshold);
                    total += GetSquaredError(original, actual);
                }
                dither.MoveToNextLine();
            }

            var mse = (int)(total / image.Length);
            Assert.IsTrue(mse <= expected, $"Unexpected mean error: {mse} > {expected}");
        }

        // manually load a bitmap to a flat array of ColorBgra values
        private static (ColorBgra[] data, Size size) LoadResource(Bitmap bmp)
        {
            using (bmp)
            {
                var size = bmp.Size;
                var area = new Rectangle(Point.Empty, bmp.Size);
                var bits = bmp.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var data = new ColorBgra[size.Width * size.Height];

                Parallel.For(0, size.Height, y =>
                {
                    var row = new int[size.Width];
                    var ofs = y * bits.Stride;

                    Marshal.Copy(bits.Scan0 + ofs, row, 0, size.Width);

                    ofs = y * size.Width;
                    for (int x = 0; x < size.Width; ++x, ++ofs)
                    {
                        data[ofs] = ColorBgra.FromUInt32((uint)row[x]);
                    }
                });

                bmp.UnlockBits(bits);

                return (data, size);
            }
        }

        // calculate the mean squared error between two images
        private static int GetMeanSquaredError(ColorBgra[] original, ColorBgra[] dithered)
        {
            var total = original.Zip(dithered, GetSquaredError).Sum();
            return (int)(total / original.Length);
        }

        // calculate the squared error between two greyscale colours
        private static long GetSquaredError(ColorBgra lhs, ColorBgra rhs) => Square(lhs.B - rhs.B);

        // calculate the square of a value
        private static long Square(int n) => ((long)n) * n;
    }
}

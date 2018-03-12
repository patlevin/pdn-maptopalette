using PaintDotNet;
using System;

namespace MapToPalette
{
    /// <summary>
    /// Error diffusion dithering algorithm implementation.
    /// </summary>
    public class ErrorDiffusion
    {
        /// <summary>
        /// Initialise from a dithering kernel, an amount and a pixel count
        /// </summary>
        /// <param name="kernel">Dithering kernel parameters</param>
        /// <param name="amount">Amount of dithering (0..1]</param>
        /// <param name="pixelsPerLine">Number of pixels to cache (columns per row)</param>
        public ErrorDiffusion(DitherKernel kernel, float amount, int pixelsPerLine)
        {
            if (amount < 0 || amount > 1) throw new ArgumentOutOfRangeException("amount");
            if (pixelsPerLine < 1) throw new ArgumentException("Invalid pixels per line");

            Amount = amount;
            kern = kernel ?? throw new ArgumentNullException("kernel");
            var row = kern.Rows;
            // allocate some extra space so we can skip bounds checking during
            // error diffusion
            var col = pixelsPerLine + kern.Centre;
            errorRed = Util.Allocate2d<int>(row, col);
            errorGreen = Util.Allocate2d<int>(row, col);
            errorBlue = Util.Allocate2d<int>(row, col);
        }

        /// <summary>
        /// Amount of dithering to be applied (0..1]
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Return the final colour after error diffusion has been applied
        /// </summary>
        /// <param name="x">Current pixel position in the row</param>
        /// <param name="original">Original colour value</param>
        /// <param name="map">Colour value mapper</param>
        /// <returns></returns>
        public ColorBgra GetFinalColour(int x, ColorBgra original, Func<ColorBgra, ColorBgra> map)
        {
            var b = (int)(original.B + errorRed[0][x] + 0.5f);
            var g = (int)(original.G + errorGreen[0][x] + 0.5f);
            var r = (int)(original.R + errorRed[0][x] + 0.5f);

            var mapped = map(ColorBgra.FromBgraClamped(b, g, r, original.A));

            var errB = original.B - mapped.B;
            var errG = original.G - mapped.G;
            var errR = original.R - mapped.R;

            SpreadErrorRight(x, errB, errG, errR);
            SpreadErrorDown(x, errB, errG, errR);

            return mapped;
        }

        /// <summary>
        /// Notify start of the next pixel row - call after a row has been processed
        /// </summary>
        public void MoveToNextLine()
        {
            // move all cached errors one row up
            var row = 0;
            var pixelsPerRow = errorBlue[0].Length;
            while (++row < kern.Rows)
            {
                Array.Copy(errorBlue[row], errorBlue[row - 1], pixelsPerRow);
                Array.Copy(errorGreen[row], errorGreen[row - 1], pixelsPerRow);
                Array.Copy(errorRed[row], errorRed[row - 1], pixelsPerRow);
            }
            // clear the last cached error row
            row = errorBlue.Length - 1;
            Array.Clear(errorBlue[row], 0, pixelsPerRow);
            Array.Clear(errorGreen[row], 0, pixelsPerRow);
            Array.Clear(errorRed[row], 0, pixelsPerRow);
        }

        // Spread the error on the current row
        private void SpreadErrorDown(int x, int blue, int green, int red)
        {
            var row = 0;
            var i = 0;
            var start = x - kern.Centre;
            if (start < 0)
            {
                i = kern.Centre;
                start = kern.Centre;
            }
            var rows = kern.Rows;
            var cols = kern.Columns;
            var factor = Amount;
            while (++row < rows)
            {
                for (int n = i, col = start; n < cols; ++n, ++col)
                {
                    var coeff = kern[n, row] * factor;
                    errorBlue[0][col] = Madd(errorBlue[0][col], blue, coeff);
                    errorGreen[0][col] = Madd(errorGreen[0][col], green, coeff);
                    errorRed[0][col] = Madd(errorRed[0][col], red, coeff);
                }
            }
        }

        // Spread the error to the next row(s)
        private void SpreadErrorRight(int x, int blue, int green, int red)
        {
            var factor = Amount;
            var cols = kern.Columns;
            for (int i = kern.Centre + 1, col = x + 1; i < cols; ++i, ++col)
            {
                var coeff = kern[i, 0] * factor;
                errorBlue[0][col] = Madd(errorBlue[0][col], blue, coeff);
                errorGreen[0][col] = Madd(errorGreen[0][col], green, coeff);
                errorRed[0][col] = Madd(errorRed[0][col], red, coeff);
            }
        }

        private static int Madd(int n, int x, float c)
            => Math.Min(255, Math.Max(0, (int)(n + x * c)));

        // Red error
        private readonly int[][] errorRed;
        // Green error
        private readonly int[][] errorGreen;
        // Blue error
        private readonly int[][] errorBlue;
        // dithering kernel
        private readonly DitherKernel kern;
    }
}

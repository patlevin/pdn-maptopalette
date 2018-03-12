using System;
using System.Collections.Generic;
using System.Linq;

namespace MapToPalette
{
    /// <summary>
    /// Error-diffusion dithering kernel.
    /// </summary>
    /// <remarks>
    /// A dithering kernel is a MxN matrix of coefficents.
    /// Error-diffusion dithering is a feedback process that diffuses
    /// the quantization error to neighboring pixels. The coefficients
    /// denote the influence of neighouring pixels on the current pixel.
    /// </remarks>
    public class DitherKernel
    {
        /// <summary>
        /// Initialise a dithering kernel from integer coefficients and width
        /// </summary>
        /// <param name="kernel">Kernel coefficients</param>
        /// <param name="width">Width of the kernel (e.g. columns)</param>
        public DitherKernel(IEnumerable<int> kernel, int width)
            : this(kernel, width, kernel.Sum())
        { }

        /// <summary>
        /// Initialise a dithering kernel from integer coefficients and width
        /// </summary>
        /// <param name="kernel">Kernel coefficients</param>
        /// <param name="width">Width of the kernel (e.g. columns)</param>
        /// <param name="factor">Normalisation factor if different from sum of coefficients</param>
        public DitherKernel(IEnumerable<int> kernel, int width, int factor)
        {
            if (factor < 1)
            {
                throw new ArgumentException("Factor must be greater than zero");
            }

            Columns = width;
            var i = 1.0f / Math.Max(1, factor);
            c = kernel.Select(x => x * i).ToArray();
            ValidateKernelWidth();
        }

        /// <summary>
        /// Initialise a dithering kernel from weighted coefficients and width
        /// </summary>
        /// <param name="kernel">Weighted (read: normalised) kernel coefficients</param>
        /// <param name="width">Width of the kernel (e.g. columns)</param>
        public DitherKernel(IEnumerable<float> kernel, int width)
        {
            Columns = width;
            c = kernel.ToArray();
            ValidateKernelWidth();
        }

        /// <summary>
        /// Return the coefficient at a kernel position
        /// </summary>
        /// <param name="col">Column in the kernel matrix</param>
        /// <param name="row">Row in the kernal matrix</param>
        /// <returns>Coefficient at the requested row and column</returns>
        public float this[int col, int row] => c[row * Columns + col];

        /// <summary>
        /// Get the kernel coefficients
        /// </summary>
        public IEnumerable<float> Coefficients => c;

        /// <summary>
        /// Get the kernel centre column.
        /// </summary>
        public int Centre => (Columns - 1) / 2;

        /// <summary>
        /// Get the number of columns in the kernel matrix
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// Get the number of rows in the kernel matrix
        /// </summary>
        public int Rows => c.Length / Columns;

        /// <summary>
        /// Get the Floyd-Steinberg dithering kernel
        /// </summary>
        public static readonly DitherKernel FloydSteinberg = new DitherKernel(new int[]
        {
            0, 0, 7,
            3, 5, 1
        }, width: 3);

        /// <summary>
        /// Get the Jarvis-Judice-Ninke dithering kernel
        /// </summary>
        public static readonly DitherKernel JarvisJudiceNinke = new DitherKernel(new int[]
        {
            0, 0, 0, 7, 5,
            3, 5, 7, 5, 3,
            1, 3, 5, 3, 1
        }, width: 5);

        /// <summary>
        /// Get the Stucki dithering kernel
        /// </summary>
        public static readonly DitherKernel Stucki = new DitherKernel(new int[]
        {
            0, 0, 0, 8, 4,
            2, 4, 8, 4, 2,
            1, 2, 4, 2, 1
        }, width: 5);

        /// <summary>
        /// Get the Burkes dithering kernel
        /// </summary>
        public static readonly DitherKernel Burkes = new DitherKernel(new int[]
        {
            0, 0, 0, 8, 4,
            2, 4, 8, 4, 2
        }, width: 5);

        /// <summary>
        /// Get the Sierra dithering kernel
        /// </summary>
        public static readonly DitherKernel Sierra = new DitherKernel(new[]
        {
            0, 0, 0, 5, 3,
            2, 4, 5, 4, 2,
            0, 2, 3, 2, 0
        }, width: 5);

        private void ValidateKernelWidth()
        {
            if (Columns < 1)
            {
                throw new ArgumentException("Invalid kernel width");
            }

            if (c.Length < Columns)
            {
                throw new ArgumentException("Not enough coefficients");
            }

            if ((c.Length % Columns) != 0)
            {
                throw new ArgumentException("Incompatible kernel width");
            }
        }

        // normalised coefficient matrix (flattened)
        private readonly float[] c;
    }
}

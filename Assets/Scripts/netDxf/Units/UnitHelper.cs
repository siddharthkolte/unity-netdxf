#region netDxf library licensed under the MIT License
// 
//                       netDxf library
// Copyright (c) 2019-2021 Daniel Carvajal (haplokuon@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
#endregion

namespace netDxf.Units
{
    /// <summary>
    /// Helper methods for unit conversion.
    /// </summary>
    public static class UnitHelper
    {
        #region constants

        // conversion factors from the DrawingUnits to meters
        private static readonly float[] UnitFactors =
        {
            1.0f,
            0.0254f,
            0.3048f,
            1609.344f,
            0.001f,
            0.01f,
            1.0f,
            1000f,
            0.0000000254f,
            0.0000254f,
            0.9144f,
            0.0000000001f,
            0.000000001f,
            0.000001f,
            0.1f,
            10.0f,
            100.0f,
            1000000000.0f,
            149597900000.0f,
            9460732325559000.0f,
            30856781858520240.0f,
            0.3048006096f,
            0.0254000508f,
            0.9144018288f,
            1609.3472187f
        };

        #endregion

        #region public methods

        /// <summary>
        /// Converts a value from one drawing unit to another.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <param name="from">Original drawing units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The converted value to the new drawing units.</returns>
        public static float ConvertUnit(float value, DrawingUnits from, DrawingUnits to)
        {
            return value * ConversionFactor(from, to);
        }

        /// <summary>
        /// Gets the conversion factor between drawing units.
        /// </summary>
        /// <param name="from">Original drawing units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The conversion factor between the drawing units.</returns>
        public static float ConversionFactor(DrawingUnits from, DrawingUnits to)
        {
            if (from == DrawingUnits.Unitless || to == DrawingUnits.Unitless)
            {
                return 1.0f;
            }

            decimal factor1 = (decimal) UnitFactors[(int) from];
            decimal factor2 = (decimal) UnitFactors[(int) to];
            return (float) (factor1 / factor2);
        }

        /// <summary>
        /// Gets the conversion factor between image and drawing units.
        /// </summary>
        /// <param name="from">Original image units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The conversion factor between the units.</returns>
        public static float ConversionFactor(ImageUnits from, DrawingUnits to)
        {
            return ConversionFactor(ImageToDrawingUnits(from), to);
        }

        /// <summary>
        /// Gets the conversion factor between units.
        /// </summary>
        /// <param name="from">Original value units.</param>
        /// <param name="to">Destination value units.</param>
        /// <returns>The conversion factor between the passed units.</returns>
        public static float ConversionFactor(DrawingUnits from, ImageUnits to)
        {
            return ConversionFactor(from, ImageToDrawingUnits(to));
        }

        #endregion

        #region private methods

        private static DrawingUnits ImageToDrawingUnits(ImageUnits units)
        {
            // more on the DXF format none sense, they don't even use the same integers for the drawing and the image units
            DrawingUnits rasterUnits;
            switch (units)
            {
                case ImageUnits.Unitless:
                    rasterUnits = DrawingUnits.Unitless;
                    break;
                case ImageUnits.Millimeters:
                    rasterUnits = DrawingUnits.Millimeters;
                    break;
                case ImageUnits.Centimeters:
                    rasterUnits = DrawingUnits.Centimeters;
                    break;
                case ImageUnits.Meters:
                    rasterUnits = DrawingUnits.Meters;
                    break;
                case ImageUnits.Kilometers:
                    rasterUnits = DrawingUnits.Kilometers;
                    break;
                case ImageUnits.Inches:
                    rasterUnits = DrawingUnits.Inches;
                    break;
                case ImageUnits.Feet:
                    rasterUnits = DrawingUnits.Feet;
                    break;
                case ImageUnits.Yards:
                    rasterUnits = DrawingUnits.Yards;
                    break;
                case ImageUnits.Miles:
                    rasterUnits = DrawingUnits.Miles;
                    break;
                default:
                    rasterUnits = DrawingUnits.Unitless;
                    break;
            }

            return rasterUnits;
        }

        #endregion
    }
}
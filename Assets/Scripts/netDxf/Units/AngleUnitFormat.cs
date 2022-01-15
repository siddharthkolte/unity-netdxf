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

using netDxf.Math;
using System;
using System.Globalization;
using UnityEngine;

namespace netDxf.Units
{
    /// <summary>
    /// Utility methods to format a decimal angle in degrees to its different string representations.
    /// </summary>
    public static class AngleUnitFormat
    {
        #region public methods

        /// <summary>
        /// Converts an angle value in degrees into its decimal string representation.
        /// </summary>
        /// <param name="angle">The angle value in degrees.</param>
        /// <param name="format">The unit style format.</param>
        /// <returns>A string that represents the angle in decimal units.</returns>
        public static string ToDecimal(float angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            NumberFormatInfo numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = format.DecimalSeparator
            };

            return angle.ToString(DecimalNumberFormat(format), numberFormat) + format.DegreesSymbol;
        }

        /// <summary>
        /// Converts an angle value in degrees into its degrees, minutes and seconds string representation.
        /// </summary>
        /// <param name="angle">The angle value in degrees.</param>
        /// <param name="format">The unit style format.</param>
        /// <returns>A string that represents the angle in degrees, minutes and seconds.</returns>
        public static string ToDegreesMinutesSeconds(float angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            float degrees = angle;
            float minutes = (degrees - (int) degrees) * 60;
            float seconds = (minutes - (int) minutes) * 60;

            NumberFormatInfo numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = format.DecimalSeparator
            };

            if (format.AngularDecimalPlaces == 0)
            {
                return string.Format(numberFormat, "{0}" + format.DegreesSymbol, (int) Mathf.Round(degrees));
            }

            if (format.AngularDecimalPlaces == 1 || format.AngularDecimalPlaces == 2)
            {
                return string.Format(numberFormat, "{0}" + format.DegreesSymbol + "{1}" + format.MinutesSymbol, (int) degrees, (int) Mathf.Round(minutes));
            }

            if (format.AngularDecimalPlaces == 3 || format.AngularDecimalPlaces == 4)
            {
                return string.Format(numberFormat, "{0}" + format.DegreesSymbol + "{1}" + format.MinutesSymbol + "{2}" + format.SecondsSymbol, (int) degrees, (int) minutes, (int) Mathf.Round(seconds));
            }

            // the suppression of leading or trailing zeros is not applicable to DegreesMinutesSeconds angles format
            string f = "0." + new string('0', format.AngularDecimalPlaces - 4);
            return string.Format(numberFormat, "{0}" + format.DegreesSymbol + "{1}" + format.MinutesSymbol + "{2}" + format.SecondsSymbol, (int) degrees, (int) minutes, seconds.ToString(f, numberFormat));
        }

        /// <summary>
        /// Converts an angle value in degrees into its gradians string representation.
        /// </summary>
        /// <param name="angle">The angle value in degrees.</param>
        /// <param name="format">The unit style format.</param>
        /// <returns>A string that represents the angle in gradians.</returns>
        public static string ToGradians(float angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            NumberFormatInfo numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = format.DecimalSeparator
            };

            return (angle*Mathd.DegToGrad).ToString(DecimalNumberFormat(format), numberFormat) + format.GradiansSymbol;
        }

        /// <summary>
        /// Converts an angle value in degrees into its radians string representation.
        /// </summary>
        /// <param name="angle">The angle value in degrees.</param>
        /// <param name="format">The unit style format.</param>
        /// <returns>A string that represents the angle in radians.</returns>
        public static string ToRadians(float angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            NumberFormatInfo numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            return (angle*Mathf.Deg2Rad).ToString(DecimalNumberFormat(format), numberFormat) + format.RadiansSymbol;
        }

        #endregion

        #region private methods

        private static string DecimalNumberFormat(UnitStyleFormat format)
        {
            char[] zeroes = new char[format.AngularDecimalPlaces + 2];
            if (format.SuppressAngularLeadingZeros)
            {
                zeroes[0] = '#';
            }
            else
            {
                zeroes[0] = '0';
            }

            zeroes[1] = '.';

            for (int i = 2; i < zeroes.Length; i++)
            {
                if (format.SuppressAngularTrailingZeros)
                {
                    zeroes[i] = '#';
                }
                else
                {
                    zeroes[i] = '0';
                }
            }
            return new string(zeroes);
        }

        #endregion
    }
}
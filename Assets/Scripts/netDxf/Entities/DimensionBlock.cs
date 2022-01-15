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

using System;
using System.Collections.Generic;
using System.Threading;
using netDxf.Blocks;
using netDxf.Math;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Holds methods to build the dimension blocks.
    /// </summary>
    public static class DimensionBlock
    {      
        #region private methods

        private static List<string> FormatDimensionText(float measure, DimensionType dimType, string userText, DimensionStyle style, Block owner)
        {
            List<string> texts = new List<string>();
            if (userText == " ")
            {
                texts.Add(string.Empty);
                return texts;
            }

            string dimText = string.Empty;

            UnitStyleFormat unitFormat = new UnitStyleFormat
            {
                LinearDecimalPlaces = style.LengthPrecision,
                AngularDecimalPlaces = style.AngularPrecision == -1 ? style.LengthPrecision : style.AngularPrecision,
                DecimalSeparator = style.DecimalSeparator.ToString(),
                FractionHeightScale = style.TextFractionHeightScale,
                FractionType = style.FractionType,
                SuppressLinearLeadingZeros = style.SuppressLinearLeadingZeros,
                SuppressLinearTrailingZeros = style.SuppressLinearTrailingZeros,
                SuppressAngularLeadingZeros = style.SuppressAngularLeadingZeros,
                SuppressAngularTrailingZeros = style.SuppressAngularTrailingZeros,
                SuppressZeroFeet = style.SuppressZeroFeet,
                SuppressZeroInches = style.SuppressZeroInches
            };

            if (dimType== DimensionType.Angular || dimType == DimensionType.Angular3Point)
            {
                switch (style.DimAngularUnits)
                {
                    case AngleUnitType.DecimalDegrees:
                        dimText = AngleUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                    case AngleUnitType.DegreesMinutesSeconds:
                        dimText = AngleUnitFormat.ToDegreesMinutesSeconds(measure, unitFormat);
                        break;
                    case AngleUnitType.Gradians:
                        dimText = AngleUnitFormat.ToGradians(measure, unitFormat);
                        break;
                    case AngleUnitType.Radians:
                        dimText = AngleUnitFormat.ToRadians(measure, unitFormat);
                        break;
                    case AngleUnitType.SurveyorUnits:
                        dimText = AngleUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                }
            }
            else
            {
                float scale = Mathf.Abs(style.DimScaleLinear);
                if (owner != null)
                {
                    Layout layout = owner.Record.Layout;
                    if (layout != null)
                    {
                        // if DIMLFAC is negative the scale value is only applied to dimensions in PaperSpace
                        if (style.DimScaleLinear < 0 && !layout.IsPaperSpace)
                        {
                            scale = 1.0f;
                        }
                    }
                }

                if (style.DimRoundoff > 0.0)
                {
                    measure = Mathd.RoundToNearest(measure*scale, style.DimRoundoff);
                }
                else
                {
                    measure *= scale;
                }

                switch (style.DimLengthUnits)
                {
                    case LinearUnitType.Architectural:
                        dimText = LinearUnitFormat.ToArchitectural(measure, unitFormat);
                        break;
                    case LinearUnitType.Decimal:
                        dimText = LinearUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                    case LinearUnitType.Engineering:
                        dimText = LinearUnitFormat.ToEngineering(measure, unitFormat);
                        break;
                    case LinearUnitType.Fractional:
                        dimText = LinearUnitFormat.ToFractional(measure, unitFormat);
                        break;
                    case LinearUnitType.Scientific:
                        dimText = LinearUnitFormat.ToScientific(measure, unitFormat);
                        break;
                    case LinearUnitType.WindowsDesktop:
                        unitFormat.LinearDecimalPlaces = (short) Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalDigits;
                        unitFormat.DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        dimText = LinearUnitFormat.ToDecimal(measure*style.DimScaleLinear, unitFormat);
                        break;
                }
            }

            string prefix = string.Empty;
            if (dimType == DimensionType.Diameter)
            {
                prefix = string.IsNullOrEmpty(style.DimPrefix) ? "Ø" : style.DimPrefix;
            }

            if (dimType == DimensionType.Radius)
            {
                prefix = string.IsNullOrEmpty(style.DimPrefix) ? "R" : style.DimPrefix;
            }

            dimText = string.Format("{0}{1}{2}", prefix, dimText, style.DimSuffix);

            if (!string.IsNullOrEmpty(userText))
            {
                int splitPos = 0;

                // check for the first appearance of \X
                // it will break the dimension text into two one over the dimension line and the other under
                for (int i = 0; i < userText.Length; i++)
                {
                    if (userText[i].Equals('\\'))
                    {
                        int j = i + 1;
                        if (j >= userText.Length)
                        {
                            break;
                        }

                        if (userText[j].Equals('X'))
                        {
                            splitPos = i;
                            break;
                        }
                    }
                }

                if (splitPos > 0)
                {
                    texts.Add(userText.Substring(0, splitPos).Replace("<>", dimText));
                    texts.Add(userText.Substring(splitPos + 2, userText.Length - (splitPos + 2)).Replace("<>", dimText));
                }
                else
                {
                    texts.Add(userText.Replace("<>", dimText));
                }
            }
            else
            {
                texts.Add(dimText);
            }

            return texts;
        }

        private static Line DimensionLine(Vector2 start, Vector2 end, float rotation, DimensionStyle style)
        {
            float ext1 = style.ArrowSize*style.DimScaleOverall;
            float ext2 = -style.ArrowSize*style.DimScaleOverall;

            Block block;

            //start arrow
            block = style.DimArrow1;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext1 = -style.DimLineExtend*style.DimScaleOverall;
                }
            }

            //end arrow
            block = style.DimArrow2;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext2 = style.DimLineExtend*style.DimScaleOverall;
                }
            }

            start = Mathd.Polar(start, ext1, rotation);
            end = Mathd.Polar(end, ext2, rotation);

            return new Line(start, end)
            {
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static Arc DimensionArc(Vector2 center, Vector2 start, Vector2 end, float startAngle, float endAngle, float radius, DimensionStyle style, out float e1, out float e2)
        {
            float ext1 = style.ArrowSize*style.DimScaleOverall;
            float ext2 = -style.ArrowSize*style.DimScaleOverall;
            e1 = ext1;
            e2 = ext2;

            Block block;

            block = style.DimArrow1;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext1 = 0.0f;
                    e1 = 0.0f;
                }
            }

            block = style.DimArrow2;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext2 = 0.0f;
                    e2 = 0.0f;
                }
            }

            start = Mathd.Polar(start, ext1, startAngle + (Mathf.PI / 2));
            end = Mathd.Polar(end, ext2, endAngle + (Mathf.PI / 2));
            return new Arc(center, radius, Mathd.Angle(center, start)*Mathf.Rad2Deg, Mathd.Angle(center, end)*Mathf.Rad2Deg)
            {
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static Line DimensionRadialLine(Vector2 start, Vector2 end, float rotation, short reversed, DimensionStyle style)
        {
            float ext = -style.ArrowSize*style.DimScaleOverall;
            Block block;

            // the radial dimension only has an arrowhead at its end
            block = style.DimArrow2;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                    ext = style.DimLineExtend*style.DimScaleOverall;
            }

            end = Mathd.Polar(end, reversed*ext, rotation);

            return new Line(start, end)
            {
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static Line ExtensionLine(Vector2 start, Vector2 end, DimensionStyle style, Linetype linetype)
        {
            return new Line(start, end)
            {
                Color = style.ExtLineColor,
                Linetype = linetype,
                Lineweight = style.ExtLineLineweight
            };
        }

        private static EntityObject StartArrowHead(Vector2 position, float rotation, DimensionStyle style)
        {
            Block block = style.DimArrow1;

            if (block == null)
            {
                Vector2 arrowRef = Mathd.Polar(position, -style.ArrowSize*style.DimScaleOverall, rotation);
                Solid arrow = new Solid(position,
                    Mathd.Polar(arrowRef, -(style.ArrowSize/6)*style.DimScaleOverall, rotation + (Mathf.PI / 2)),
                    Mathd.Polar(arrowRef, (style.ArrowSize/6)*style.DimScaleOverall, rotation + (Mathf.PI / 2)))
                {
                    Color = style.DimLineColor
                };
                return arrow;
            }
            else
            {
                Insert arrow = new Insert(block, position)
                {
                    Color = style.DimLineColor,
                    Scale = new Vector3(style.ArrowSize*style.DimScaleOverall, style.ArrowSize * style.DimScaleOverall),
                    Rotation = rotation*Mathf.Rad2Deg,
                    Lineweight = style.DimLineLineweight
                };
                return arrow;
            }
        }

        private static EntityObject EndArrowHead(Vector2 position, float rotation, DimensionStyle style)
        {
            Block block = style.DimArrow2;

            if (block == null)
            {
                Vector2 arrowRef = Mathd.Polar(position, -style.ArrowSize*style.DimScaleOverall, rotation);
                Solid arrow = new Solid(position,
                    Mathd.Polar(arrowRef, -(style.ArrowSize/6)*style.DimScaleOverall, rotation + (Mathf.PI / 2)),
                    Mathd.Polar(arrowRef, (style.ArrowSize/6)*style.DimScaleOverall, rotation + (Mathf.PI / 2)))
                {
                    Color = style.DimLineColor
                };
                return arrow;
            }
            else
            {
                Insert arrow = new Insert(block, position)
                {
                    Color = style.DimLineColor,
                    Scale = new Vector3(style.ArrowSize*style.DimScaleOverall, style.ArrowSize * style.DimScaleOverall),
                    Rotation = rotation*Mathf.Rad2Deg,
                    Lineweight = style.DimLineLineweight
                };
                return arrow;
            }
        }

        private static MText DimensionText(Vector2 position, MTextAttachmentPoint attachmentPoint, float rotation, string text, DimensionStyle style)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            MText mText = new MText(text, position, style.TextHeight*style.DimScaleOverall, 0.0f, style.TextStyle)
            {
                Color = style.TextColor,
                AttachmentPoint = attachmentPoint,
                Rotation = rotation*Mathf.Rad2Deg
            };

            return mText;
        }

        private static List<EntityObject> CenterCross(Vector2 center, float radius, DimensionStyle style)
        {
            List<EntityObject> lines = new List<EntityObject>();
            if (Mathd.IsZero(style.CenterMarkSize))
            {
                return lines;
            }

            Vector2 c1;
            Vector2 c2;
            float dist = Mathf.Abs(style.CenterMarkSize*style.DimScaleOverall);

            // center mark
            c1 = new Vector2(0.0f, -dist) + center;
            c2 = new Vector2(0.0f, dist) + center;
            lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});
            c1 = new Vector2(-dist, 0.0f) + center;
            c2 = new Vector2(dist, 0.0f) + center;
            lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});

            // center lines
            if (style.CenterMarkSize < 0)
            {
                c1 = new Vector2(2*dist, 0.0f) + center;
                c2 = new Vector2(radius + dist, 0.0f) + center;
                lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});

                c1 = new Vector2(-2*dist, 0.0f) + center;
                c2 = new Vector2(-radius - dist, 0.0f) + center;
                lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});

                c1 = new Vector2(0.0f, 2*dist) + center;
                c2 = new Vector2(0.0f, radius + dist) + center;
                lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});

                c1 = new Vector2(0.0f, -2*dist) + center;
                c2 = new Vector2(0.0f, -radius - dist) + center;
                lines.Add(new Line(c1, c2) {Color = style.ExtLineColor, Lineweight = style.ExtLineLineweight});
            }
            return lines;
        }

        private static DimensionStyle BuildDimensionStyleOverride(Dimension dim)
        {
            // to avoid the cloning, return the actual dimension style if there are no overrides
            if (dim.StyleOverrides.Count == 0)
            {
                return dim.Style;
            }

            // make a shallow copy of the actual dimension style, there is no need of the full copy that the Clone method does
            DimensionStyle copy = new DimensionStyle(dim.Style.Name)
            {
                // dimension lines
                DimLineColor = dim.Style.DimLineColor,
                DimLineLinetype = dim.Style.DimLineLinetype,
                DimLineLineweight = dim.Style.DimLineLineweight,
                DimLine1Off = dim.Style.DimLine1Off,
                DimLine2Off = dim.Style.DimLine2Off,
                DimBaselineSpacing = dim.Style.DimBaselineSpacing,
                DimLineExtend = dim.Style.DimLineExtend,

                // extension lines
                ExtLineColor = dim.Style.ExtLineColor,
                ExtLine1Linetype = dim.Style.ExtLine1Linetype,
                ExtLine2Linetype = dim.Style.ExtLine2Linetype,
                ExtLineLineweight = dim.Style.ExtLineLineweight,
                ExtLine1Off = dim.Style.ExtLine1Off,
                ExtLine2Off = dim.Style.ExtLine2Off,
                ExtLineOffset = dim.Style.ExtLineOffset,
                ExtLineExtend = dim.Style.ExtLineExtend,

                // symbols and arrows
                ArrowSize = dim.Style.ArrowSize,
                CenterMarkSize = dim.Style.CenterMarkSize,

                // text appearance
                TextStyle = dim.Style.TextStyle,
                TextColor = dim.Style.TextColor,
                TextHeight = dim.Style.TextHeight,
                TextOffset = dim.Style.TextOffset,
                TextFractionHeightScale = dim.Style.TextFractionHeightScale,

                // primary units
                AngularPrecision = dim.Style.AngularPrecision,
                LengthPrecision = dim.Style.LengthPrecision,
                DimPrefix = dim.Style.DimPrefix,
                DimSuffix = dim.Style.DimSuffix,
                DecimalSeparator = dim.Style.DecimalSeparator,
                DimScaleLinear = dim.Style.DimScaleLinear,
                DimLengthUnits = dim.Style.DimLengthUnits,
                DimAngularUnits = dim.Style.DimAngularUnits,
                FractionType = dim.Style.FractionType,
                SuppressLinearLeadingZeros = dim.Style.SuppressLinearLeadingZeros,
                SuppressLinearTrailingZeros = dim.Style.SuppressLinearTrailingZeros,
                SuppressAngularLeadingZeros = dim.Style.SuppressAngularLeadingZeros,
                SuppressAngularTrailingZeros = dim.Style.SuppressAngularTrailingZeros,
                SuppressZeroFeet = dim.Style.SuppressZeroFeet,
                SuppressZeroInches = dim.Style.SuppressZeroInches,
                DimRoundoff = dim.Style.DimRoundoff,

                LeaderArrow = dim.Style.LeaderArrow,
                DimArrow1 = dim.Style.DimArrow1,
                DimArrow2 = dim.Style.DimArrow2
            };

            foreach (DimensionStyleOverride styleOverride in dim.StyleOverrides.Values)
            {
                switch (styleOverride.Type)
                {
                    case DimensionStyleOverrideType.DimLineColor:
                        copy.DimLineColor = (AciColor) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLineLinetype:
                        copy.DimLineLinetype = (Linetype) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLineLineweight:
                        copy.DimLineLineweight = (Lineweight) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLine1Off:
                        copy.DimLine1Off = (bool)styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLine2Off:
                        copy.DimLine2Off = (bool)styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLineExtend:
                        copy.DimLineExtend = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLineColor:
                        copy.ExtLineColor = (AciColor) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLine1Linetype:
                        copy.ExtLine1Linetype = (Linetype) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLine2Linetype:
                        copy.ExtLine2Linetype = (Linetype) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLineLineweight:
                        copy.ExtLineLineweight = (Lineweight) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLine1Off:
                        copy.ExtLine1Off = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLine2Off:
                        copy.ExtLine2Off = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLineOffset:
                        copy.ExtLineOffset = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ExtLineExtend:
                        copy.ExtLineExtend = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.ArrowSize:
                        copy.ArrowSize = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.CenterMarkSize:
                        copy.CenterMarkSize = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.LeaderArrow:
                        copy.LeaderArrow = (Block) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimArrow1:
                        copy.DimArrow1 = (Block) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimArrow2:
                        copy.DimArrow2 = (Block) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.TextStyle:
                        copy.TextStyle = (TextStyle) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.TextColor:
                        copy.TextColor = (AciColor) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.TextHeight:
                        copy.TextHeight = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.TextOffset:
                        copy.TextOffset = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.TextFractionHeightScale:
                        copy.TextFractionHeightScale = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimScaleOverall:
                        copy.DimScaleOverall = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.AngularPrecision:
                        copy.AngularPrecision = (short) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.LengthPrecision:
                        copy.LengthPrecision = (short) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimPrefix:
                        copy.DimPrefix = (string) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimSuffix:
                        copy.DimSuffix = (string) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DecimalSeparator:
                        copy.DecimalSeparator = (char) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimScaleLinear:
                        copy.DimScaleLinear = (float) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimLengthUnits:
                        copy.DimLengthUnits = (LinearUnitType) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimAngularUnits:
                        copy.DimAngularUnits = (AngleUnitType) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.FractionalType:
                        copy.FractionType = (FractionFormatType) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressLinearLeadingZeros:
                        copy.SuppressLinearLeadingZeros = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressLinearTrailingZeros:
                        copy.SuppressLinearTrailingZeros = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressAngularLeadingZeros:
                        copy.SuppressAngularLeadingZeros = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressAngularTrailingZeros:
                        copy.SuppressAngularTrailingZeros = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressZeroFeet:
                        copy.SuppressZeroFeet = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.SuppressZeroInches:
                        copy.SuppressZeroInches = (bool) styleOverride.Value;
                        break;
                    case DimensionStyleOverrideType.DimRoundoff:
                        copy.DimRoundoff = (float) styleOverride.Value;
                        break;
                }
            }

            return copy;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="Dimension">Dimension</see> from which the block will be created.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// By the fault the block will have the name "DimBlock". The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(Dimension dim)
        {
            return Build(dim, "DimBlock");
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="Dimension">Dimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(Dimension dim, string name)
        {
            Block block;
            switch (dim.DimensionType)
            {
                case DimensionType.Linear:
                    block = Build((LinearDimension)dim, name);
                    break;
                case DimensionType.Aligned:
                    block = Build((AlignedDimension)dim, name);
                    break;
                case DimensionType.Angular:
                    block = Build((Angular2LineDimension)dim, name);
                    break;
                case DimensionType.Angular3Point:
                    block = Build((Angular3PointDimension)dim, name);
                    break;
                case DimensionType.Diameter:
                    block = Build((DiametricDimension)dim, name);
                    break;
                case DimensionType.Radius:
                    block = Build((RadialDimension)dim, name);
                    break;
                case DimensionType.Ordinate:
                    block = Build((OrdinateDimension)dim, name);
                    break;
                default:
                    block = null;
                    break;
            }

            return block;
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="AlignedDimension">AlignedDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(AlignedDimension dim, string name)
        {
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            float measure = dim.Measurement;

            Vector2 ref1 = dim.FirstReferencePoint;
            Vector2 ref2 = dim.SecondReferencePoint;
            Vector2 vec = (dim.DimLinePosition - ref2).normalized;
            Vector2 dimRef1 = ref1 + dim.Offset*vec;
            Vector2 dimRef2 = dim.DimLinePosition;

            float refAngle = Mathd.Angle(ref1, ref2);

            // reference points
            Layer defPointLayer = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1) {Layer = defPointLayer});
            entities.Add(new Point(ref2) {Layer = defPointLayer});
            entities.Add(new Point(dimRef2) {Layer = defPointLayer});

            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                entities.Add(DimensionLine(dimRef1, dimRef2, refAngle, style));
                entities.Add(StartArrowHead(dimRef1, refAngle + Mathf.PI, style));
                entities.Add(EndArrowHead(dimRef2, refAngle, style));
            }

            // extension lines
            float dimexo = style.ExtLineOffset * style.DimScaleOverall;
            float dimexe = style.ExtLineExtend * style.DimScaleOverall;
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(ref1 + dimexo * vec, dimRef1 + dimexe * vec, style, style.ExtLine1Linetype));
            }

            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(ref2 + dimexo * vec, dimRef2 + dimexe * vec, style, style.ExtLine2Linetype));
            }

            // dimension text
            Vector2 textRef = Mathd.MidPoint(dimRef1, dimRef2);
            float gap = style.TextOffset*style.DimScaleOverall;
            float textRot = refAngle;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                gap = -gap;
                textRot += Mathf.PI;
            }

            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);

            MText mText = DimensionText(textRef + gap*vec, MTextAttachmentPoint.BottomCenter, textRot, texts[0], style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            // there might be an additional text if the code \X has been used in the dimension UserText 
            // this additional text appears under the dimension line
            if (texts.Count > 1)
            {
                MText mText2 = DimensionText(textRef - gap * vec, MTextAttachmentPoint.TopCenter, textRot, texts[1], style);
                if (mText2 != null)
                {
                    entities.Add(mText2);
                }
            }

            dim.TextReferencePoint = textRef + gap*vec;
            dim.TextPositionManuallySet = false;

            // drawing block
            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="LinearDimension">LinearDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(LinearDimension dim, string name)
        {
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            float measure = dim.Measurement;
            float dimRotation = dim.Rotation*Mathf.Deg2Rad;

            Vector2 ref1 = dim.FirstReferencePoint;
            Vector2 ref2 = dim.SecondReferencePoint;

            Vector2 vec = (Mathd.Rotate(Mathd.V2UnitY, dimRotation)).normalized;
            Vector2 dimRef1 = dim.DimLinePosition + measure * Vector2.Perpendicular(vec);
            Vector2 dimRef2 = dim.DimLinePosition;

            // reference points
            Layer defPointLayer = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1) {Layer = defPointLayer});
            entities.Add(new Point(ref2) {Layer = defPointLayer});
            entities.Add(new Point(dimRef1) { Layer = defPointLayer });

            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                // dimension line
                entities.Add(DimensionLine(dimRef1, dimRef2, dimRotation, style));
                entities.Add(StartArrowHead(dimRef1, dimRotation + Mathf.PI, style));
                entities.Add(EndArrowHead(dimRef2, dimRotation, style));
            }

            // extension lines
            Vector2 dirRef1 = (dimRef1 - ref1).normalized;
            Vector2 dirRef2 = (dimRef2 - ref2).normalized;
            float dimexo = style.ExtLineOffset*style.DimScaleOverall;
            float dimexe = style.ExtLineExtend*style.DimScaleOverall;
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(ref1 + dimexo*dirRef1, dimRef1 + dimexe*dirRef1, style, style.ExtLine1Linetype));
            }

            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(ref2 + dimexo*dirRef2, dimRef2 + dimexe*dirRef2, style, style.ExtLine2Linetype));
            }

            // dimension text
            Vector2 textRef = Mathd.MidPoint(dimRef1, dimRef2);
            float gap = style.TextOffset * style.DimScaleOverall;
            float textRot = dimRotation;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                gap = -gap;
                textRot += Mathf.PI;
            }

            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);
            MText mText = DimensionText(textRef + gap * vec, MTextAttachmentPoint.BottomCenter, textRot, texts[0], style);
            //MText mText = DimensionText(Mathd.Polar(textRef, (style.TextOffset + style.TextHeight*0.5) * style.DimScaleOverall, textRot + (Mathf.PI / 2)), MTextAttachmentPoint.MiddleCenter, textRot, texts[0], style);

            if (mText != null)
            {
                entities.Add(mText);
            }

            // there might be an additional text if the code \X has been used in the dimension UserText 
            // this additional text appears under the dimension line
            if (texts.Count > 1)
            {
                MText mText2 = DimensionText(textRef - gap * vec, MTextAttachmentPoint.TopCenter, textRot, texts[1], style);
                if (mText2 != null)
                {
                    entities.Add(mText2);
                }
            }

            dim.TextReferencePoint = textRef + gap * vec;
            dim.TextPositionManuallySet = false;

            // drawing block
            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="Angular2LineDimension">Angular2LineDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(Angular2LineDimension dim, string name)
        {
            float offset = dim.Offset;
            float measure = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 ref1Start = dim.StartFirstLine;
            Vector2 ref1End = dim.EndFirstLine;
            Vector2 ref2Start = dim.StartSecondLine;
            Vector2 ref2End = dim.EndSecondLine;
            Vector2 center = dim.CenterPoint;

            float startAngle = Mathd.Angle(ref1Start, ref1End);
            float endAngle = Mathd.Angle(ref2Start, ref2End);

            float midRot = startAngle + measure*Mathf.Deg2Rad*0.5f;

            Vector2 dimRef1 = Mathd.Polar(center, offset, startAngle);
            Vector2 dimRef2 = Mathd.Polar(center, offset, endAngle);
            Vector2 midDim = Mathd.Polar(center, offset, midRot);

            // reference points
            Layer defPoints = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1Start) {Layer = defPoints});
            entities.Add(new Point(ref1End) {Layer = defPoints});
            entities.Add(new Point(ref2Start) {Layer = defPoints});
            entities.Add(new Point(ref2End) {Layer = defPoints});

            // dimension lines
            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                entities.Add(DimensionArc(center, dimRef1, dimRef2, startAngle, endAngle, offset, style, out float ext1, out float ext2));
                float angle1 = Mathf.Asin(ext1 * 0.5f / offset);
                float angle2 = Mathf.Asin(ext2 * 0.5f / offset);
                entities.Add(StartArrowHead(dimRef1, angle1 + startAngle - (Mathf.PI / 2), style));
                entities.Add(EndArrowHead(dimRef2, angle2 + endAngle + (Mathf.PI / 2), style));
            }

            // dimension lines         
            float dimexo = style.ExtLineOffset*style.DimScaleOverall;
            float dimexe = style.ExtLineExtend*style.DimScaleOverall;

            // the dimension line is only drawn if the end of the extension line is outside the line segment
            int t;
            t = Mathd.PointInSegment(dimRef1, ref1Start, ref1End);
            if (!style.ExtLine1Off && t != 0)
            {
                Vector2 s = Mathd.Polar(t < 0 ? ref1Start : ref1End, t*dimexo, startAngle);
                entities.Add(ExtensionLine(s, Mathd.Polar(dimRef1, t*dimexe, startAngle), style, style.ExtLine1Linetype));
            }

            t = Mathd.PointInSegment(dimRef2, ref2Start, ref2End);
            if (!style.ExtLine2Off && t != 0)
            {
                Vector2 s = Mathd.Polar(t < 0 ? ref2Start : ref2End, t*dimexo, endAngle);
                entities.Add(ExtensionLine(s, Mathd.Polar(dimRef2, t*dimexe, endAngle), style, style.ExtLine1Linetype));
            }

            float textRot = midRot - (Mathf.PI / 2);
            float gap = style.TextOffset*style.DimScaleOverall;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                textRot += Mathf.PI;
                gap *= -1;
            }

            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);
            string dimText;
            Vector2 position;
            MTextAttachmentPoint attachmentPoint;
            if (texts.Count > 1)
            {
                position = midDim;
                dimText = texts[0] + "\\P" + texts[1];
                attachmentPoint = MTextAttachmentPoint.MiddleCenter;
            }
            else
            {
                position = Mathd.Polar(midDim, gap, midRot);
                dimText = texts[0];
                attachmentPoint = MTextAttachmentPoint.BottomCenter;
            }
            MText mText = DimensionText(position, attachmentPoint, textRot, dimText, style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            dim.TextReferencePoint = position;
            dim.TextPositionManuallySet = false;

            // drawing block
            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="Angular3PointDimension">Angular3PointDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(Angular3PointDimension dim, string name)
        {
            float offset = dim.Offset;
            float measure = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 refCenter = dim.CenterPoint;
            Vector2 ref1 = dim.StartPoint;
            Vector2 ref2 = dim.EndPoint;

            float startAngle = Mathd.Angle(refCenter, ref1);
            float endAngle = Mathd.Angle(refCenter, ref2);
            float midRot = startAngle + measure*Mathf.Deg2Rad*0.5f;
            Vector2 dimRef1 = Mathd.Polar(refCenter, offset, startAngle);
            Vector2 dimRef2 = Mathd.Polar(refCenter, offset, endAngle);
            Vector2 midDim = Mathd.Polar(refCenter, offset, midRot);

            // reference points
            Layer defPoints = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1) {Layer = defPoints});
            entities.Add(new Point(ref2) {Layer = defPoints});
            entities.Add(new Point(refCenter) {Layer = defPoints});

            // dimension lines
            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                entities.Add(DimensionArc(refCenter, dimRef1, dimRef2, startAngle, endAngle, offset, style, out float ext1, out float ext2));

                float angle1 = Mathf.Asin(ext1*0.5f/offset);
                float angle2 = Mathf.Asin(ext2*0.5f/offset);
                entities.Add(StartArrowHead(dimRef1, angle1 + startAngle - (Mathf.PI / 2), style));
                entities.Add(EndArrowHead(dimRef2, angle2 + endAngle + (Mathf.PI / 2), style));
            }
                
            // extension lines
            float refAngle = 0.0f;
            if (Vector2.Distance(refCenter, ref1) > Vector2.Distance(refCenter, dimRef1))
            {
                refAngle = Mathf.PI;
            }
            float dimexo = style.ExtLineOffset*style.DimScaleOverall;
            float dimexe = style.ExtLineExtend*style.DimScaleOverall;
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(Mathd.Polar(ref1, dimexo, startAngle + refAngle), Mathd.Polar(dimRef1, dimexe, startAngle + refAngle), style, style.ExtLine1Linetype));
            }

            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(Mathd.Polar(ref2, dimexo, endAngle + refAngle), Mathd.Polar(dimRef2, dimexe, endAngle + refAngle), style, style.ExtLine1Linetype));
            }

            // dimension text
            float textRot = midRot - (Mathf.PI / 2);
            float gap = style.TextOffset*style.DimScaleOverall;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                textRot += Mathf.PI;
                gap *= -1;
            }

            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);
            string dimText;
            Vector2 position;
            MTextAttachmentPoint attachmentPoint;
            if (texts.Count > 1)
            {
                position = midDim;
                dimText = texts[0] + "\\P" + texts[1];
                attachmentPoint = MTextAttachmentPoint.MiddleCenter;
            }
            else
            {
                position = Mathd.Polar(midDim, gap, midRot);
                dimText = texts[0];
                attachmentPoint = MTextAttachmentPoint.BottomCenter;
            }
            MText mText = DimensionText(position, attachmentPoint, textRot, dimText, style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            dim.TextReferencePoint = position;
            dim.TextPositionManuallySet = false;

            // drawing block
            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="DiametricDimension">DiametricDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(DiametricDimension dim, string name)
        {
            float measure = dim.Measurement;
            float offset = Vector2.Distance(dim.CenterPoint, dim.TextReferencePoint);
            float radius = measure*0.5f;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 centerRef = dim.CenterPoint;
            Vector2 ref1 = dim.ReferencePoint;
            Vector2 defPoint = dim.DefinitionPoint;

            float angleRef = Mathd.Angle(centerRef, ref1);

            short inside; // 1 if the dimension line is inside the circumference, -1 otherwise
            float minOffset = (2*style.ArrowSize + style.TextOffset)*style.DimScaleOverall;
            if (offset >= radius && offset <= radius + minOffset)
            {
                offset = radius + minOffset;
                inside = -1;
            }
            else if (offset >= radius - minOffset && offset <= radius)
            {
                offset = radius - minOffset;
                inside = 1;
            }
            else if (offset > radius)
            {
                inside = -1;
            }
            else
            {
                inside = 1;
            }

            Vector2 dimRef = Mathd.Polar(centerRef, offset - style.TextOffset * style.DimScaleOverall, angleRef);

            // reference points
            Layer defPoints = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1) {Layer = defPoints});

            // dimension lines
            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                if (inside > 0)
                {
                    entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, inside, style));
                    entities.Add(EndArrowHead(ref1, angleRef, style));
                }
                else
                {
                    entities.Add(new Line(defPoint, ref1)
                    {
                        Color = style.DimLineColor,
                        Linetype = style.DimLineLinetype,
                        Lineweight = style.DimLineLineweight
                    });
                    entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, inside, style));
                    entities.Add(EndArrowHead(ref1, Mathf.PI + angleRef, style));

                    Vector2 dimRef2 = Mathd.Polar(centerRef, radius + minOffset - style.TextOffset * style.DimScaleOverall, Mathf.PI + angleRef);
                    entities.Add(DimensionRadialLine(dimRef2, defPoint, Mathf.PI + angleRef, inside, style));
                    entities.Add(EndArrowHead(defPoint,  angleRef, style));
                }
            }

            // center cross
            if (!Mathd.IsZero(style.CenterMarkSize))
            {
                entities.AddRange(CenterCross(centerRef, radius, style));
            }

            // dimension text
            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);
            string dimText ;
            if (texts.Count > 1)
            {
                dimText = texts[0] + "\\P" + texts[1];
            }
            else
            {
                dimText = texts[0];
            }

            float textRot = angleRef;
            short reverse = 1;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                textRot += Mathf.PI;
                reverse = -1;
            }

            Vector2 textPos = Mathd.Polar(dimRef, -reverse*inside*style.TextOffset*style.DimScaleOverall, textRot);
            MTextAttachmentPoint attachmentPoint = reverse*inside < 0 ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
            MText mText = DimensionText(textPos, attachmentPoint, textRot, dimText, style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            dim.TextReferencePoint = textPos;
            dim.TextPositionManuallySet = false;

            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="RadialDimension">RadialDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(RadialDimension dim, string name)
        {
            float offset = Vector2.Distance(dim.CenterPoint, dim.TextReferencePoint);
            float radius = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 centerRef = dim.CenterPoint;
            Vector2 ref1 = dim.ReferencePoint;

            float angleRef = Mathd.Angle(centerRef, ref1);

            short side; // 1 if the dimension line is inside the circumference, -1 otherwise
            float minOffset = 2 * style.ArrowSize * style.DimScaleOverall;

            if (offset >= radius && offset <= radius + minOffset)
            {
                offset = radius + minOffset;
                side = -1;
            }
            else if (offset >= radius - minOffset && offset <= radius)
            {
                offset = radius - minOffset;
                side = 1;
            }
            else if (offset > radius)
            {
                side = -1;
            }
            else
            {
                side = 1;
            }

            Vector2 dimRef = Mathd.Polar(centerRef, offset - style.TextOffset * style.DimScaleOverall, angleRef);

            // reference points
            Layer defPoints = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPoints });

            // dimension lines
            if (!style.DimLine1Off && !style.DimLine2Off)
            {
                if (side > 0)
                {
                    entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, side, style));
                    entities.Add(EndArrowHead(ref1, angleRef, style));
                }
                else
                {
                    entities.Add(new Line(centerRef, ref1)
                                {
                                    Color = style.DimLineColor,
                                    Linetype = style.DimLineLinetype,
                                    Lineweight = style.DimLineLineweight
                                });
                    entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, side, style));
                    entities.Add(EndArrowHead(ref1, Mathf.PI + angleRef, style));
                }
            }

            // center cross
            if(!Mathd.IsZero(dim.Style.CenterMarkSize))
                entities.AddRange(CenterCross(centerRef, radius, style));

            // dimension text
            List<string> texts = FormatDimensionText(radius, dim.DimensionType, dim.UserText, style, dim.Owner);
            string dimText;
            if (texts.Count > 1)
            {
                dimText = texts[0] + "\\P" + texts[1];
            }
            else
            {
                dimText = texts[0];
            }

            float textRot = angleRef;
            short reverse = 1;
            if (textRot > (Mathf.PI / 2) && textRot <= (3*Mathf.PI*0.5f))
            {
                textRot += Mathf.PI;
                reverse = -1;
            }

            Vector2 textPos = Mathd.Polar(dimRef, -reverse*side*style.TextOffset*style.DimScaleOverall, textRot);
            MTextAttachmentPoint attachmentPoint = reverse * side < 0 ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
            MText mText = DimensionText(textPos, attachmentPoint, textRot, dimText, style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            dim.TextReferencePoint = textPos;
            dim.TextPositionManuallySet = false;

            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};

        }

        /// <summary>
        /// Creates a block that represents the drawing of the specified dimension.
        /// </summary>
        /// <param name="dim"><see cref="OrdinateDimension">OrdinateDimension</see> from which the block will be created.</param>
        /// <param name="name">The blocks name.</param>
        /// <returns>A block that represents the specified dimension.</returns>
        /// <remarks>
        /// The block's name is irrelevant when the dimension belongs to a document,
        /// it will be automatically renamed to accommodate to the nomenclature of the DXF.<br />
        /// The dimension block creation only supports a limited number of <see cref="DimensionStyle">dimension style</see> properties.
        /// Also the list of <see cref="DimensionStyleOverride">dimension style overrides</see> associated with the specified dimension will be applied where necessary.
        /// </remarks>
        public static Block Build(OrdinateDimension dim, string name)
        {
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();

            float measure = dim.Measurement;
            float minOffset = 2*dim.Style.ArrowSize;
            Vector2 ref1 = dim.FeaturePoint;
            Vector2 ref2 = dim.LeaderEndPoint;
            Vector2 refDim = ref2 - ref1;
            Vector2 pto1;
            Vector2 pto2;
            float rotation = dim.Rotation * Mathf.Deg2Rad;
            int side = 1;

            if (dim.Axis == OrdinateDimensionAxis.X)
            {
                rotation += (Mathf.PI / 2);
            }

            Vector2 ocsDimRef = Mathd.Rotate(refDim, -rotation);

            if (ocsDimRef.x >= 0)
            {
                if (ocsDimRef.x >= 2*minOffset)
                {
                    pto1 = new Vector2(ocsDimRef.x - minOffset, 0);
                    pto2 = new Vector2(ocsDimRef.x - minOffset, ocsDimRef.y);
                }
                else
                {
                    pto1 = new Vector2(minOffset, 0);
                    pto2 = new Vector2(ocsDimRef.x - minOffset, ocsDimRef.y);
                }
            }
            else
            {
                if (ocsDimRef.x <= -2*minOffset)
                {
                    pto1 = new Vector2(ocsDimRef.x + minOffset, 0);
                    pto2 = new Vector2(ocsDimRef.x + minOffset, ocsDimRef.y);
                }
                else
                {
                    pto1 = new Vector2(-minOffset, 0);
                    pto2 = new Vector2(ocsDimRef.x + minOffset, ocsDimRef.y);
                }
                side = -1;
            }
            pto1 = ref1 + Mathd.Rotate(pto1, rotation);
            pto2 = ref1 + Mathd.Rotate(pto2, rotation);

            
            // reference points
            Layer defPoints = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(dim.Origin) {Layer = defPoints});
            entities.Add(new Point(dim.FeaturePoint) {Layer = defPoints});

            // dimension lines
            entities.Add(new Line(Mathd.Polar(ref1, style.ExtLineOffset * style.DimScaleOverall, rotation), pto1));
            entities.Add(new Line(pto1, pto2));
            entities.Add(new Line(pto2, ref2));

            // dimension text

            Vector2 midText = Mathd.Polar(ref2, side*style.TextOffset*style.DimScaleOverall, rotation);

            List<string> texts = FormatDimensionText(measure, dim.DimensionType, dim.UserText, style, dim.Owner);
            string dimText;
            if (texts.Count > 1)
            {
                dimText = texts[0] + "\\P" + texts[1];
            }
            else
            {
                dimText = texts[0];
            }

            MTextAttachmentPoint attachmentPoint = side < 0 ? MTextAttachmentPoint.MiddleRight : MTextAttachmentPoint.MiddleLeft;
            MText mText = DimensionText(midText, attachmentPoint, rotation, dimText, style);
            if (mText != null)
            {
                entities.Add(mText);
            }

            dim.TextReferencePoint = midText;
            dim.TextPositionManuallySet = false;

            // drawing block
            return new Block(name, entities, null, false) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        #endregion
    }
}
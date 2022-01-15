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
using System.Diagnostics;
using netDxf.Blocks;
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Angular2LineDimension :
        Dimension
    {
        #region private fields

        private float offset;
        private Vector2 startFirstLine;
        private Vector2 endFirstLine;
        private Vector2 startSecondLine;
        private Vector2 endSecondLine;
        private Vector2 arcDefinitionPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        public Angular2LineDimension()
            : this(Vector2.zero, Mathd.V2UnitX, Vector2.zero, Mathd.V2UnitY, 0.1f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, float offset)
            : this(firstLine, secondLine, offset, Mathd.V3UnitZ, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, float offset, Vector3 normal)
            : this(firstLine, secondLine, offset, normal, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, float offset, DimensionStyle style)
            : this(firstLine, secondLine, offset, Mathd.V3UnitZ, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, float offset, Vector3 normal, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            if (firstLine == null)
            {
                throw new ArgumentNullException(nameof(firstLine));
            }

            if (secondLine == null)
            {
                throw new ArgumentNullException(nameof(secondLine));
            }

            if (Mathd.AreParallel(firstLine.Direction, secondLine.Direction))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            List<Vector3> ocsPoints = new List<Vector3>
                    {
                        firstLine.StartPoint,
                        firstLine.EndPoint,
                        secondLine.StartPoint,
                        secondLine.EndPoint
                    };

            this.startFirstLine = new Vector2(ocsPoints[0].x, ocsPoints[0].y);
            this.endFirstLine = new Vector2(ocsPoints[1].x, ocsPoints[1].y);
            this.startSecondLine = new Vector2(ocsPoints[2].x, ocsPoints[2].y);
            this.endSecondLine = new Vector2(ocsPoints[3].x, ocsPoints[3].y);
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            }
            this.offset = offset;
            this.Style = style ?? throw new ArgumentNullException(nameof(style));
            this.Normal = normal;
            this.Elevation = ocsPoints[0].z;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, float offset)
            : this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, float offset, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            Vector2 dir1 = endFirstLine - startFirstLine;
            Vector2 dir2 = endSecondLine - startSecondLine;
            if (Mathd.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            this.startFirstLine = startFirstLine;
            this.endFirstLine = endFirstLine;
            this.startSecondLine = startSecondLine;
            this.endSecondLine = endSecondLine;

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            }
            this.offset = offset;

            this.Style = style ?? throw new ArgumentNullException(nameof(style));
            this.Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the center <see cref="Vector2">point</see> of the measured arc in local coordinates.
        /// </summary>
        public Vector2 CenterPoint
        {
            get
            {
                return Mathd.FindIntersection(
                    this.startFirstLine, this.endFirstLine - this.startFirstLine,
                    this.startSecondLine, this.endSecondLine - this.startSecondLine);
            }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 StartFirstLine
        {
            get { return this.startFirstLine; }
            set { this.startFirstLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 EndFirstLine
        {
            get { return this.endFirstLine; }
            set { this.endFirstLine = value; }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 StartSecondLine
        {
            get { return this.startSecondLine; }
            set { this.startSecondLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 EndSecondLine
        {
            get { return this.endSecondLine; }
            set { this.endSecondLine = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line arc.
        /// </summary>
        public Vector2 ArcDefinitionPoint
        {
            get { return this.arcDefinitionPoint; }
            internal set { this.arcDefinitionPoint = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension line.
        /// </summary>
        /// <remarks>
        /// Offset values cannot be negative and, even thought, zero values are allowed, they are not recommended.
        /// </remarks>
        public float Offset
        {
            get { return this.offset; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The offset value must be equal or greater than zero.");
                }
                this.offset = value;
            }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        /// <remarks>The dimension is always measured in the plane defined by the normal.</remarks>
        public override float Measurement
        {
            get
            {
                Vector2 dirRef1 = this.endFirstLine - this.startFirstLine;
                Vector2 dirRef2 = this.endSecondLine - this.startSecondLine;
                return Mathd.Angle(dirRef1, dirRef2)*Mathf.Rad2Deg;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        /// <remarks>
        /// The start and end points of the reference lines will be modified,
        /// the angle measurement is always made from the direction of the first line to the direction of the second line.
        /// </remarks>
        public void SetDimensionLinePosition(Vector2 point)
        {
            this.SetDimensionLinePosition(point, true);
        }

        #endregion

        #region private methods

        private void SetDimensionLinePosition(Vector2 point, bool updateRefs)
        {
            Vector2 dir1 = this.endFirstLine - this.startFirstLine;
            Vector2 dir2 = this.endSecondLine - this.startSecondLine;
            if (Mathd.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            Vector2 center = this.CenterPoint;

            if (updateRefs)
            {
                float cross = Mathd.Cross(this.EndFirstLine - this.StartFirstLine, this.EndSecondLine - this.StartSecondLine);
                if (cross < 0)
                {
                    Mathd.Swap(ref this.startFirstLine, ref this.startSecondLine);
                    Mathd.Swap(ref this.endFirstLine, ref this.endSecondLine);
                }

                Vector2 ref1Start = this.StartFirstLine;
                Vector2 ref1End = this.EndFirstLine;
                Vector2 ref2Start = this.StartSecondLine;
                Vector2 ref2End = this.EndSecondLine;
                Vector2 dirRef1 = ref1End - ref1Start;
                Vector2 dirRef2 = ref2End - ref2Start;

                Vector2 dirOffset = point - center;
                float crossStart = Mathd.Cross(dirRef1, dirOffset);
                float crossEnd = Mathd.Cross(dirRef2, dirOffset);

                if (crossStart >= 0 && crossEnd >= 0)
                {
                    this.StartFirstLine = ref2Start;
                    this.EndFirstLine = ref2End;
                    this.StartSecondLine = ref1End;
                    this.EndSecondLine = ref1Start;
                }
                else if (crossStart < 0 && crossEnd >= 0)
                {
                    this.StartFirstLine = ref1End;
                    this.EndFirstLine = ref1Start;
                    this.StartSecondLine = ref2End;
                    this.EndSecondLine = ref2Start;
                }
                else if (crossStart < 0 && crossEnd < 0)
                {
                    this.StartFirstLine = ref2End;
                    this.EndFirstLine = ref2Start;
                    this.StartSecondLine = ref1Start;
                    this.EndSecondLine = ref1End;
                }
            }

            float newOffset = Vector2.Distance(center, point);
            this.offset = Mathd.IsZero(newOffset) ? Mathf.Epsilon : newOffset;
            
            this.defPoint = this.endSecondLine;

            float measure = this.Measurement * Mathf.Deg2Rad;
            float startAngle = Mathd.Angle(center, this.endFirstLine);
            float midRot = startAngle + measure * 0.5f;
            Vector2 midDim = Mathd.Polar(center, this.offset, midRot);
            this.arcDefinitionPoint = midDim;

            if (!this.TextPositionManuallySet)
            {
                DimensionStyleOverride styleOverride;
                float textGap = this.Style.TextOffset;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (float) styleOverride.Value;
                }
                float scale = this.Style.DimScaleOverall;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (float) styleOverride.Value;
                }

                float gap = textGap * scale;
                this.textRefPoint = midDim + gap * (midDim - center).normalized;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            Vector2 dir1 = this.endFirstLine - this.startFirstLine;
            Vector2 dir2 = this.endSecondLine - this.startSecondLine;
            if (Mathd.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            DimensionStyleOverride styleOverride;

            float measure = this.Measurement * Mathf.Deg2Rad ;
            Vector2 center = this.CenterPoint;

            float startAngle = Mathd.Angle(center, this.endFirstLine);
            float midRot = startAngle + measure * 0.5f;
            Vector2 midDim = Mathd.Polar(center, this.offset, midRot);

            this.defPoint = this.endSecondLine;
            this.arcDefinitionPoint = midDim;

            if (this.TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    this.SetDimensionLinePosition(this.textRefPoint, false);
                }
            }
            else
            {
                float textGap = this.Style.TextOffset;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (float)styleOverride.Value;
                }
                float scale = this.Style.DimScaleOverall;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (float)styleOverride.Value;
                }

                float gap = textGap * scale;
                this.textRefPoint = midDim + gap * (midDim-center).normalized;
            }
        }

        /// <summary>
        /// Gets the block that contains the entities that make up the dimension picture.
        /// </summary>
        /// <param name="name">Name to be assigned to the generated block.</param>
        /// <returns>The block that represents the actual dimension.</returns>
        protected override Block BuildBlock(string name)
        {
            return DimensionBlock.Build(this, name);
        }

        /// <summary>
        /// Creates a new Angular2LineDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular2LineDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular2LineDimension entity = new Angular2LineDimension
            {
                //EntityObject properties
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Dimension properties
                Style = (DimensionStyle) this.Style.Clone(),
                DefinitionPoint = this.DefinitionPoint,
                TextReferencePoint = this.TextReferencePoint,
                TextPositionManuallySet = this.TextPositionManuallySet,
                TextRotation = this.TextRotation,
                AttachmentPoint = this.AttachmentPoint,
                LineSpacingStyle = this.LineSpacingStyle,
                LineSpacingFactor = this.LineSpacingFactor,
                UserText = this.UserText,
                Elevation = this.Elevation,
                //Angular2LineDimension properties
                StartFirstLine = this.startFirstLine,
                EndFirstLine = this.endFirstLine,
                StartSecondLine = this.startSecondLine,
                EndSecondLine = this.endSecondLine,
                Offset = this.offset,
                arcDefinitionPoint = this.arcDefinitionPoint
            };

            foreach (DimensionStyleOverride styleOverride in this.StyleOverrides.Values)
            {
                object copy = styleOverride.Value is ICloneable value ? value.Clone() : styleOverride.Value;
                entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
            }

            foreach (XData data in this.XData.Values)
            {
                entity.XData.Add((XData) data.Clone());
            }

            return entity;
        }

        #endregion
    }
}
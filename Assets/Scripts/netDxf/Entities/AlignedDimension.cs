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
using netDxf.Blocks;
using netDxf.Tables;
using netDxf.Math;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a dimension <see cref="EntityObject">entity</see> that is aligned the reference line.
    /// </summary>
    public class AlignedDimension :
        Dimension
    {
        #region private fields

        private Vector2 firstRefPoint;
        private Vector2 secondRefPoint;
        private float offset;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        public AlignedDimension()
            : this(Vector2.zero, Mathd.V2UnitX, 0.1f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public AlignedDimension(Line referenceLine, float offset)
            : this(referenceLine, offset, Mathd.V3UnitZ, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public AlignedDimension(Line referenceLine, float offset, DimensionStyle style)
            : this(referenceLine, offset, Mathd.V3UnitZ, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public AlignedDimension(Line referenceLine, float offset, Vector3 normal)
            : this(referenceLine, offset, normal, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference line define the distance to be measure.</remarks>
        public AlignedDimension(Line referenceLine, float offset, Vector3 normal, DimensionStyle style)
            : base(DimensionType.Aligned)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            List<Vector3> ocsPoints = new List<Vector3> { referenceLine.StartPoint, referenceLine.EndPoint };
            this.firstRefPoint = new Vector2(ocsPoints[0].x, ocsPoints[0].y);
            this.secondRefPoint = new Vector2(ocsPoints[1].x, ocsPoints[1].y);

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
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, float offset)
            : this(firstPoint, secondPoint, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>AlignedDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, float offset, DimensionStyle style)
            : base(DimensionType.Aligned)
        {
            this.firstRefPoint = firstPoint;
            this.secondRefPoint = secondPoint;
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
        /// Gets or sets the first definition point of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 FirstReferencePoint
        {
            get { return this.firstRefPoint; }
            set { this.firstRefPoint = value; }
        }

        /// <summary>
        /// Gets or sets the second definition point of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 SecondReferencePoint
        {
            get { return this.secondRefPoint; }
            set { this.secondRefPoint = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line.
        /// </summary>
        public Vector2 DimLinePosition
        {
            get { return this.defPoint; }
        }

        /// <summary>
        /// Gets or sets the distance between the reference line and the dimension line.
        /// </summary>
        /// <remarks>
        /// The offset value must be equal or greater than zero.<br />
        /// The side at which the dimension line is drawn depends of the direction of its reference line.
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
            get { return Vector2.Distance(this.firstRefPoint, this.secondRefPoint); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        public void SetDimensionLinePosition(Vector2 point)
        {
            Vector2 refDir = this.secondRefPoint - this.firstRefPoint;
            Vector2 offsetDir = point - this.firstRefPoint;

            float cross = Mathd.Cross(refDir, offsetDir);
            if (cross < 0)
            {
                Mathd.Swap(ref this.firstRefPoint, ref this.secondRefPoint);
                refDir *= -1;
            }
            refDir.Normalize();

            Vector2 vec = Vector2.Perpendicular(refDir);
            this.offset = Mathd.PointLineDistance(point, this.firstRefPoint, refDir);
            this.defPoint = this.secondRefPoint + this.offset * vec;

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

                float gap = this.offset + textGap * scale;
                this.textRefPoint = Mathd.MidPoint(this.firstRefPoint, this.secondRefPoint) + gap * vec;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            DimensionStyleOverride styleOverride;

            Vector2 ref1 = this.FirstReferencePoint;
            Vector2 ref2 = this.SecondReferencePoint;
            Vector2 dirRef = ref2 - ref1;
            Vector2 dirDesp = Vector2.Perpendicular(dirRef).normalized;
            Vector2 vec = this.offset* dirDesp;
            Vector2 dimRef1 = ref1 + vec;
            Vector2 dimRef2 = ref2 + vec;

            this.defPoint = dimRef2;

            if (this.TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove) styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    this.SetDimensionLinePosition(this.textRefPoint);
                }
            }
            else
            {
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

                float gap = textGap*scale;
                this.textRefPoint = Mathd.MidPoint(dimRef1, dimRef2) + gap*dirDesp;
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
        /// Creates a new AlignedDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new AlignedDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            AlignedDimension entity = new AlignedDimension
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
                //AlignedDimension properties
                FirstReferencePoint = this.firstRefPoint,
                SecondReferencePoint = this.secondRefPoint,
                Offset = this.offset
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
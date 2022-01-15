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
using System.Diagnostics;
using netDxf.Blocks;
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a radial dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class RadialDimension :
        Dimension
    {
        #region private fields

        private Vector2 center;
        private Vector2 refPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        public RadialDimension()
            : this(Vector2.zero, Mathd.V2UnitX, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Arc arc, float rotation)
            : this(arc, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Arc arc, float rotation, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (arc == null)
            {
                throw new ArgumentNullException(nameof(arc));
            }

            Vector3 ocsCenter = arc.Center;
            this.center = new Vector2(ocsCenter.x, ocsCenter.y);
            this.refPoint = Mathd.Polar(this.center, arc.Radius, rotation*Mathf.Deg2Rad);

            this.Style = style ?? throw new ArgumentNullException(nameof(style));
            this.Normal = arc.Normal;
            this.Elevation = ocsCenter.z;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Circle circle, float rotation)
            : this(circle, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Circle circle, float rotation, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (circle == null)
            {
                throw new ArgumentNullException(nameof(circle));
            }

            Vector3 ocsCenter = circle.Center;
            this.center = new Vector2(ocsCenter.x, ocsCenter.y);
            this.refPoint = Mathd.Polar(this.center, circle.Radius, rotation*Mathf.Deg2Rad);

            this.Style = style ?? throw new ArgumentNullException(nameof(style));
            this.Normal = circle.Normal;
            this.Elevation = ocsCenter.z;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint)
            : this(centerPoint, referencePoint, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (Vector2.Equals(centerPoint, referencePoint))
            {
                throw new ArgumentException("The center and the reference point cannot be the same");
            }
            this.center = centerPoint;
            this.refPoint = referencePoint;

            this.Style = style ?? throw new ArgumentNullException(nameof(style));

            this.Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center <see cref="Vector2">point</see> of the circumference in OCS (object coordinate system).
        /// </summary>
        public Vector2 CenterPoint
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector2">point</see> on circumference or arc in OCS (object coordinate system).
        /// </summary>
        public Vector2 ReferencePoint
        {
            get { return this.refPoint; }
            set { this.refPoint = value; }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override float Measurement
        {
            get { return Vector2.Distance(this.center, this.refPoint); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the reference point and dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        public void SetDimensionLinePosition(Vector2 point)
        {
            float radius = Vector2.Distance(this.center, this.refPoint);
            float rotation = Mathd.Angle(this.center, point);
            this.defPoint = this.center;
            this.refPoint = Mathd.Polar(this.center, radius, rotation);

            if (!this.TextPositionManuallySet)
            {
                DimensionStyleOverride styleOverride;
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
                float arrowSize = this.Style.ArrowSize;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
                {
                    arrowSize = (float)styleOverride.Value;
                }

                Vector2 vec = (this.refPoint - this.center).normalized;
                float minOffset = (2 * arrowSize + textGap) * scale;
                this.textRefPoint = this.refPoint + minOffset * vec;
            }
        }

        #endregion

        #region overrides

        protected override void CalculateReferencePoints()
        {
            if (Vector2.Equals(this.center, this.refPoint))
            {
                throw new ArgumentException("The center and the reference point cannot be the same");
            }

            this.defPoint = this.center;

            if (this.TextPositionManuallySet)
            {
                this.SetDimensionLinePosition(this.textRefPoint);
            }
            else
            {
                DimensionStyleOverride styleOverride;

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
                float arrowSize = this.Style.ArrowSize;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
                {
                    arrowSize = (float)styleOverride.Value;
                }

                Vector2 vec = (this.refPoint - this.center).normalized;
                float minOffset = (2 * arrowSize + textGap) * scale;
                this.textRefPoint = this.refPoint + minOffset * vec;
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
        /// Creates a new RadialDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new RadialDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            RadialDimension entity = new RadialDimension
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
                //RadialDimension properties
                CenterPoint = this.center,
                ReferencePoint = this.refPoint
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
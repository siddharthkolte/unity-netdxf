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
using netDxf.Blocks;
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an ordinate dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class OrdinateDimension :
        Dimension
    {
        #region private fields

        private float rotation;
        private OrdinateDimensionAxis axis;
        private Vector2 firstPoint;
        private Vector2 secondPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        public OrdinateDimension()
            : this(Vector2.zero, new Vector2(0.5f, 0), new Vector2(1.0f, 0), OrdinateDimensionAxis.Y, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
        /// <remarks>
        /// Uses the difference between the feature location and the leader endpoint to determine whether it is an X or a Y ordinate dimension.
        /// If the difference in the Y ordinate is greater, the dimension measures the X ordinate. Otherwise, it measures the Y ordinate.
        /// </remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint)
            : this(origin, featurePoint, leaderEndPoint, DimensionStyle.Default)
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>
        /// Uses the difference between the feature location and the leader endpoint to determine whether it is an X or a Y ordinate dimension.
        /// If the difference in the Y ordinate is greater, the dimension measures the X ordinate. Otherwise, it measures the Y ordinate.
        /// </remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint, DimensionStyle style)
            : base(DimensionType.Ordinate)
        {
            this.defPoint = origin;
            this.firstPoint = featurePoint;
            this.secondPoint = leaderEndPoint;
            this.textRefPoint = leaderEndPoint;
            Vector2 vec = leaderEndPoint - featurePoint;
            this.axis = vec.y > vec.x ? OrdinateDimensionAxis.X : OrdinateDimensionAxis.Y;
            this.rotation = 0.0f;
            this.Style = style ?? throw new ArgumentNullException(nameof(style));
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="leaderEndPoint">Leader end <see cref="Vector2">point</see> in local coordinates of the ordinate dimension</param>
        /// <param name="axis">Length of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, Vector2 leaderEndPoint, OrdinateDimensionAxis axis, DimensionStyle style)
            : base(DimensionType.Ordinate)
        {
            this.defPoint = origin;
            this.firstPoint = featurePoint;
            this.secondPoint = leaderEndPoint;
            this.textRefPoint = leaderEndPoint;
            this.axis = axis;
            this.rotation = 0.0f;
            this.Style = style ?? throw new ArgumentNullException(nameof(style));
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Length of the dimension line.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, float length, OrdinateDimensionAxis axis)
            : this(origin, featurePoint, length, axis, 0.0f, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Length of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, float length, OrdinateDimensionAxis axis, DimensionStyle style)
            : this(origin, featurePoint, length, axis, 0.0f, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector2">point</see> of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Length of the dimension line.</param>
        /// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, float length, OrdinateDimensionAxis axis, float rotation)
            : this(origin, featurePoint, length, axis, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector3">point</see> in world coordinates of the ordinate dimension.</param>
        /// <param name="featurePoint">Base location <see cref="Vector2">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Local axis that measures the ordinate dimension.</param>
        /// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector2 origin, Vector2 featurePoint, float length, OrdinateDimensionAxis axis, float rotation, DimensionStyle style)
            : base(DimensionType.Ordinate)
        {
            this.defPoint = origin;
            this.rotation = Mathd.NormalizeAngle(rotation);
            this.firstPoint = featurePoint;
            this.axis = axis;

            this.Style = style ?? throw new ArgumentNullException(nameof(style));

            float angle = rotation * Mathf.Deg2Rad;
            if (this.Axis == OrdinateDimensionAxis.X)
            {
                angle += (Mathf.PI / 2);
            }

            this.secondPoint = Mathd.Polar(featurePoint, length, angle);
            this.textRefPoint = this.secondPoint;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the origin <see cref="Vector2">point</see> in local coordinates.
        /// </summary>
        public Vector2 Origin
        {
            get { return this.defPoint; }
            set { this.defPoint = value; }
        }

        /// <summary>
        /// Gets or set the base <see cref="Vector2">point</see> in local coordinates, a point on a feature such as an endpoint, intersection, or center of an object.
        /// </summary>
        public Vector2 FeaturePoint
        {
            get { return this.firstPoint; }
            set { this.firstPoint = value; }
        }

        /// <summary>
        /// Gets or sets the leader end <see cref="Vector2">point</see> in local coordinates
        /// </summary>
        public Vector2 LeaderEndPoint
        {
            get { return this.secondPoint; }
            set { this.secondPoint = value; }
        }

        /// <summary>
        /// Gets or sets the angle of rotation in degrees of the ordinate dimension local coordinate system.
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { Mathd.NormalizeAngle(this.rotation = value); }
        }

        /// <summary>
        /// Gets or sets the local axis that measures the ordinate dimension.
        /// </summary>
        public OrdinateDimensionAxis Axis
        {
            get { return this.axis; }
            set { this.axis = value; }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override float Measurement
        {
            get
            {
                Vector2 dirRef = Mathd.Rotate(this.axis == OrdinateDimensionAxis.X ? Mathd.V2UnitY : Mathd.V2UnitX, this.rotation*Mathf.Deg2Rad);
                return Mathd.PointLineDistance(this.firstPoint, this.defPoint, dirRef);
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            if (this.TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
                DimensionStyleOverride styleOverride;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText != DimensionStyleFitTextMove.OverDimLineWithoutLeader)
                {
                    this.secondPoint = this.textRefPoint;
                }
            }
            else
            {
                this.textRefPoint = this.secondPoint;
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
        /// Creates a new OrdinateDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new OrdinateDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            OrdinateDimension entity = new OrdinateDimension
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
                DefinitionPoint = this.defPoint,
                TextReferencePoint = this.TextReferencePoint,
                TextPositionManuallySet = this.TextPositionManuallySet,
                TextRotation = this.TextRotation,
                AttachmentPoint = this.AttachmentPoint,
                LineSpacingStyle = this.LineSpacingStyle,
                LineSpacingFactor = this.LineSpacingFactor,
                UserText = this.UserText,
                Elevation = this.Elevation,
                //OrdinateDimension properties
                FeaturePoint = this.firstPoint,
                LeaderEndPoint = this.secondPoint,
                Rotation = this.rotation,
                Axis = this.axis
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
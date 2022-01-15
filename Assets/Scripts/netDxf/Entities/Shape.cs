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
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a shape entity.
    /// </summary>
    public class Shape :
        EntityObject
    {
        #region delegates and events

        public delegate void StyleChangedEventHandler(Shape sender, TableObjectChangedEventArgs<ShapeStyle> e);
        public event StyleChangedEventHandler StyleChanged;
        protected virtual ShapeStyle OnStyleChangedEvent(ShapeStyle oldStyle, ShapeStyle newStyle)
        {
            StyleChangedEventHandler ae = this.StyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<ShapeStyle> eventArgs = new TableObjectChangedEventArgs<ShapeStyle>(oldStyle, newStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newStyle;
        }

        #endregion

        #region private fields

        private string name;
        private ShapeStyle style;
        private Vector3 position;
        private float size;
        private float rotation;
        private float obliqueAngle;
        private float widthFactor;
        private float thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
        public Shape(string name, ShapeStyle style) 
            : this(name, style, Vector3.zero, 1.0f, 0.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="position">Shape insertion point.</param>
        /// <param name="size">Shape size.</param>
        /// <param name="rotation">Shape rotation.</param>
        public Shape(string name, ShapeStyle style, Vector3 position, float size, float rotation) 
            : base(EntityType.Shape, DxfObjectCode.Shape)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.name = name;
            this.style = style ?? throw new ArgumentNullException(nameof(style));
            this.position = position;
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The shape size must be greater than zero.");
            }
            this.size = size;
            this.rotation = rotation;
            this.obliqueAngle = 0.0f;
            this.widthFactor = 1.0f;
            this.thickness = 0.0f;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the shape name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.name = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ShapeStyle">shape style</see>.
        /// </summary>
        public ShapeStyle Style
        {
            get { return this.style; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.style = this.OnStyleChangedEvent(this.style, value);
            }
        }

        /// <summary>
        /// Gets or sets the shape <see cref="Vector3">insertion point</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the size of the shape.
        /// </summary>
        /// <remarks>
        /// The shape size is relative to the actual size of the shape definition.
        /// The size value works as an scale value applied to the dimensions of the shape definition.
        /// The DXF allows for negative values but that is the same as rotating the shape 180 degrees.<br />
        /// Size values must be greater than zero. Default: 1.0.
        /// </remarks>
        public float Size
        {
            get { return this.size; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape size must be greater than zero.");
                }
                this.size = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape rotation in degrees.
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape oblique angle in degrees.
        /// </summary>
        public float ObliqueAngle
        {
            get { return this.obliqueAngle; }
            set { this.obliqueAngle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape width factor.
        /// </summary>
        /// <remarks>Width factor values cannot be zero. Default: 1.0.</remarks>
        public float WidthFactor
        {
            get { return this.widthFactor; }
            set
            {
                if (Mathd.IsZero(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape width factor cannot be zero.");
                }
                this.widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or set the shape thickness.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region overrides

        public override object Clone()
        {
            Shape entity = new Shape(this.name, (ShapeStyle)this.style.Clone())
            {
                //EntityObject properties
                Layer = (Layer)this.Layer.Clone(),
                Linetype = (Linetype)this.Linetype.Clone(),
                Color = (AciColor)this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency)this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Shape properties
                Position = this.position,
                Size = this.size,
                Rotation = this.rotation,
                ObliqueAngle = this.obliqueAngle,
                Thickness = this.thickness
        };

            foreach (XData data in this.XData.Values)
            {
                entity.XData.Add((XData)data.Clone());
            }

            return entity;
        }

        #endregion
    }
}

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
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a point <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Point :
        EntityObject
    {
        #region private fields

        private Vector3 position;
        private float thickness;
        private float rotation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Point</c> class.
        /// </summary>
        /// <param name="position">Point <see cref="Vector3">position</see>.</param>
        public Point(Vector3 position)
            : base(EntityType.Point, DxfObjectCode.Point)
        {
            this.position = position;
            this.thickness = 0.0f;
            this.rotation = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Point</c> class.
        /// </summary>
        /// <param name="position">Point <see cref="Vector2">position</see>.</param>
        public Point(Vector2 position)
            : this(new Vector3(position.x, position.y, 0.0f))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Point</c> class.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public Point(float x, float y, float z)
            : this(new Vector3(x, y, z))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Point</c> class.
        /// </summary>
        public Point()
            : this(Vector3.zero)
        {
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the point <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the point thickness.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        /// <summary>
        /// Gets or sets the point local rotation in degrees along its normal.
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = Mathd.NormalizeAngle(value); }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Point that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Point that is a copy of this instance.</returns>
        public override object Clone()
        {
            Point entity = new Point
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
                //Point properties
                Position = this.position,
                Rotation = this.rotation,
                Thickness = this.thickness
            };

            foreach (XData data in this.XData.Values)
            {
                entity.XData.Add((XData) data.Clone());
            }

            return entity;
        }

        #endregion
    }
}
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
    /// Represents a circular arc <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Arc :
        EntityObject
    {
        #region private fields

        private Vector3 center;
        private float radius;
        private float startAngle;
        private float endAngle;
        private float thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        public Arc()
            : this(Vector3.zero, 1.0f, 0.0f, 180.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        /// <param name="center">Arc <see cref="Vector2">center</see> in world coordinates.</param>
        /// <param name="radius">Arc radius.</param>
        /// <param name="startAngle">Arc start angle in degrees.</param>
        /// <param name="endAngle">Arc end angle in degrees.</param>
        public Arc(Vector2 center, float radius, float startAngle, float endAngle)
            : this(new Vector3(center.x, center.y, 0.0f), radius, startAngle, endAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        /// <param name="center">Arc <see cref="Vector3">center</see> in world coordinates.</param>
        /// <param name="radius">Arc radius.</param>
        /// <param name="startAngle">Arc start angle in degrees.</param>
        /// <param name="endAngle">Arc end angle in degrees.</param>
        public Arc(Vector3 center, float radius, float startAngle, float endAngle)
            : base(EntityType.Arc, DxfObjectCode.Arc)
        {
            this.center = center;
            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "The circle radius must be greater than zero.");
            }
            this.radius = radius;
            this.startAngle = Mathd.NormalizeAngle(startAngle);
            this.endAngle = Mathd.NormalizeAngle(endAngle);
            this.thickness = 0.0f;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the arc <see cref="Vector3">center</see> in world coordinates.
        /// </summary>
        public Vector3 Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the arc radius.
        /// </summary>
        public float Radius
        {
            get { return this.radius; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The arc radius must be greater than zero.");
                this.radius = value;
            }
        }

        /// <summary>
        /// Gets or sets the arc start angle in degrees.
        /// </summary>
        public float StartAngle
        {
            get { return this.startAngle; }
            set { this.startAngle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the arc end angle in degrees.
        /// </summary>
        public float EndAngle
        {
            get { return this.endAngle; }
            set { this.endAngle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the arc thickness.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Converts the arc in a list of vertexes.
        /// </summary>
        /// <param name="precision">Number of vertexes generated.</param>
        /// <returns>A list vertexes that represents the arc expressed in object coordinate system.</returns>
        public List<Vector2> PolygonalVertexes(int precision)
        {
            if (precision < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), precision, "The arc precision must be equal or greater than two.");
            }

            List<Vector2> ocsVertexes = new List<Vector2>();
            float start = this.startAngle * Mathf.Deg2Rad;
            float end = this.endAngle * Mathf.Deg2Rad;
            if (end < start)
            {
                end += (2 * Mathf.PI);
            }

            float delta = (end - start) / (precision - 1);
            for (int i = 0; i < precision; i++)
            {
                float angle = start + delta*i;
                float sine = this.radius*Mathf.Sin(angle);
                float cosine = this.radius*Mathf.Cos(angle);
                ocsVertexes.Add(new Vector2(cosine, sine));
            }

            return ocsVertexes;
        }

        /// <summary>
        /// Converts the arc in a Polyline2D.
        /// </summary>
        /// <param name="precision">Number of divisions.</param>
        /// <returns>A new instance of <see cref="Polyline2D">Polyline2D</see> that represents the arc.</returns>
        public Polyline2D ToPolyline2D(int precision)
        {
            IEnumerable<Vector2> vertexes = this.PolygonalVertexes(precision);
            Vector3 ocsCenter = this.center;

            Polyline2D poly = new Polyline2D
            {
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                Elevation = ocsCenter.z,
                Thickness = this.Thickness,
                IsClosed = false
            };
            foreach (Vector2 v in vertexes)
            {
                poly.Vertexes.Add(new Polyline2DVertex(v.x + ocsCenter.x, v.y + ocsCenter.y));
            }
            return poly;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Arc that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Arc that is a copy of this instance.</returns>
        public override object Clone()
        {
            Arc entity = new Arc
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
                //Arc properties
                Center = this.center,
                Radius = this.radius,
                StartAngle = this.startAngle,
                EndAngle = this.endAngle,
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
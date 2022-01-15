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
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a <see cref="Polyline2D">Polyline2D</see> vertex.
    /// </summary>
    public class Polyline2DVertex :
        ICloneable
    {
        #region private fields

        private Vector2 position;
        private float startWidth;
        private float endWidth;
        private float bulge;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2DVertex</c> class.
        /// </summary>
        public Polyline2DVertex()
            : this(Vector2.zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2DVertex</c> class.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public Polyline2DVertex(float x, float y)
            : this(new Vector2(x, y), 0.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2DVertex</c> class.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="bulge">Vertex bulge  (default: 0.0).</param>
        public Polyline2DVertex(float x, float y, float bulge)
            : this(new Vector2(x, y), bulge)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2DVertex</c> class.
        /// </summary>
        /// <param name="position">Lightweight polyline <see cref="Vector2">vertex</see> coordinates.</param>
        public Polyline2DVertex(Vector2 position)
            : this(position, 0.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2DVertex</c> class.
        /// </summary>
        /// <param name="position">Lightweight polyline <see cref="Vector2">vertex</see> coordinates.</param>
        /// <param name="bulge">Vertex bulge  (default: 0.0).</param>
        public Polyline2DVertex(Vector2 position, float bulge)
        {
            this.position = position;
            this.bulge = bulge;
            this.startWidth = 0.0f;
            this.endWidth = 0.0f;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the light weight polyline vertex <see cref="Vector2">position</see>.
        /// </summary>
        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the light weight polyline start segment width.
        /// </summary>
        /// <remarks>Widths greater than zero produce wide lines.</remarks>
        public float StartWidth
        {
            get { return this.startWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex start width must be equals or greater than zero.");
                }
                this.startWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the light weight polyline end segment width.
        /// </summary>
        /// <remarks>Widths greater than zero produce wide lines.</remarks>
        public float EndWidth
        {
            get { return this.endWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex end width must be equals or greater than zero.");
                this.endWidth = value;
            }
        }

        /// <summary>
        /// Gets or set the light weight polyline bulge.
        /// </summary>
        /// <remarks>
        /// The bulge is the tangent of one fourth the included angle for an arc segment, 
        /// made negative if the arc goes clockwise from the start point to the endpoint. 
        /// A bulge of 0 indicates a straight segment, and a bulge of 1 is a semicircle.
        /// </remarks>
        public float Bulge
        {
            get { return this.bulge; }
            set { this.bulge = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}: ({1})", "Polyline2DVertex", this.position);
        }

        /// <summary>
        /// Creates a new Polyline2DVertex that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Polyline2DVertex that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Polyline2DVertex
            {
                Position = this.position,
                Bulge = this.bulge,
                StartWidth = this.startWidth,
                EndWidth = this.endWidth
            };
        }

        #endregion
    }
}
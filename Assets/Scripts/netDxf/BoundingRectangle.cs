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
using UnityEngine;

namespace netDxf
{
    /// <summary>
    /// Represents an axis aligned bounding rectangle.
    /// </summary>
    public class BoundingRectangle
    {
        #region private fields

        private Vector2 min;
        private Vector2 max;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle from a rotated ellipse.
        /// </summary>
        /// <param name="center">Center of the ellipse.</param>
        /// <param name="majorAxis">Major axis of the ellipse.</param>
        /// <param name="minorAxis">Minor axis of the ellipse.</param>
        /// <param name="rotation">Rotation in degrees of the ellipse.</param>
        public BoundingRectangle(Vector2 center, float majorAxis, float minorAxis, float rotation)
        {
            float rot = rotation * Mathf.Deg2Rad;
            float a = majorAxis * 0.5f * Mathf.Cos(rot);
            float b = minorAxis * 0.5f * Mathf.Sin(rot);
            float c = majorAxis * 0.5f * Mathf.Sin(rot);
            float d = minorAxis * 0.5f * Mathf.Cos(rot);

            float width = Mathf.Sqrt(a * a + b * b) * 2;
            float height = Mathf.Sqrt(c * c + d * d) * 2;
            this.min = new Vector2(center.x - width * 0.5f, center.y - height * 0.5f);
            this.max = new Vector2(center.x + width * 0.5f, center.y + height * 0.5f);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle from a circle.
        /// </summary>
        /// <param name="center">Center of the bounding rectangle.</param>
        /// <param name="radius">Radius of the circle.</param>
        public BoundingRectangle(Vector2 center, float radius)
        {
            this.min = new Vector2(center.x - radius, center.y - radius);
            this.max = new Vector2(center.x + radius, center.y + radius);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="center">Center of the bounding rectangle.</param>
        /// <param name="width">Width of the bounding rectangle.</param>
        /// <param name="height">Height of the bounding rectangle.</param>
        public BoundingRectangle(Vector2 center, float width, float height)
        {
            this.min = new Vector2(center.x - width * 0.5f, center.y - height * 0.5f);
            this.max = new Vector2(center.x + width * 0.5f, center.y + height * 0.5f);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="min">Lower-left corner.</param>
        /// <param name="max">Upper-right corner.</param>
        public BoundingRectangle(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="points">A list of Vector2.</param>
        public BoundingRectangle(IEnumerable<Vector2> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            bool any = false;
            foreach (Vector2 point in points)
            {
                any = true;
                if (minX > point.x)
                {
                    minX = point.x;
                }

                if (minY > point.y)
                {
                    minY = point.y;
                }

                if (maxX < point.x)
                {
                    maxX = point.x;
                }

                if (maxY < point.y)
                {
                    maxY = point.y;
                }
            }
            if (any)
            {
                this.min = new Vector2(minX, minY);
                this.max = new Vector2(maxX, maxY);
            }
            else
            {
                this.min = new Vector2(float.MinValue, float.MinValue);
                this.max = new Vector2(float.MaxValue, float.MaxValue);
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the bounding rectangle lower-left corner.
        /// </summary>
        public Vector2 Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        /// <summary>
        /// Gets or sets the bounding rectangle upper-right corner.
        /// </summary>
        public Vector2 Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        /// <summary>
        /// Gets the bounding rectangle center.
        /// </summary>
        public Vector2 Center
        {
            get { return (this.min + this.max) * 0.5f; }
        }

        /// <summary>
        /// Gets the radius of the circle that contains the bounding rectangle.
        /// </summary>
        public float Radius
        {
            get { return Vector2.Distance(this.min, this.max) * 0.5f; }
        }

        /// <summary>
        /// Gets the bounding rectangle width.
        /// </summary>
        public float Width
        {
            get { return this.max.x - this.min.x; }
        }

        /// <summary>
        /// Gets the bounding rectangle height.
        /// </summary>
        public float Height
        {
            get { return this.max.y - this.min.y; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Checks if a point is inside the bounding rectangle.
        /// </summary>
        /// <param name="point">Vector2 to check.</param>
        /// <returns>True if the point is inside the bounding rectangle, false otherwise.</returns>
        /// <remarks></remarks>
        public bool PointInside(Vector2 point)
        {
            return point.x >= this.min.x && point.x <= this.max.x && point.y >= this.min.y && point.y <= this.max.y;
        }

        /// <summary>
        /// Obtains the union between two bounding rectangles.
        /// </summary>
        /// <param name="aabr1">A bounding rectangle.</param>
        /// <param name="aabr2">A bounding rectangle.</param>
        /// <returns>The resulting bounding rectangle.</returns>
        public static BoundingRectangle Union(BoundingRectangle aabr1, BoundingRectangle aabr2)
        {
            if (aabr1 == null && aabr2 == null)
            {
                return null;
            }

            if (aabr1 == null)
            {
                return aabr2;
            }

            if (aabr2 == null)
            {
                return aabr1;
            }

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();
            for (int i = 0; i < 2; i++)
            {
                if (aabr1.Min[i] <= aabr2.Min[i])
                {
                    min[i] = aabr1.Min[i];
                }
                else
                {
                    min[i] = aabr2.Min[i];
                }

                if (aabr1.Max[i] >= aabr2.Max[i])
                {
                    max[i] = aabr1.Max[i];
                }
                else
                {
                    max[i] = aabr2.Max[i];
                }
            }
            return new BoundingRectangle(min, max);
        }

        /// <summary>
        /// Obtains the union of a bounding rectangles list.
        /// </summary>
        /// <param name="rectangles">A list of bounding rectangles.</param>
        /// <returns>The resulting bounding rectangle.</returns>
        public static BoundingRectangle Union(IEnumerable<BoundingRectangle> rectangles)
        {
            if (rectangles == null)
            {
                throw new ArgumentNullException(nameof(rectangles));
            }

            BoundingRectangle rtnAABR = null;
            foreach (BoundingRectangle aabr in rectangles)
            {
                rtnAABR = Union(rtnAABR, aabr);
            }

            return rtnAABR;
        }

        #endregion
    }
}
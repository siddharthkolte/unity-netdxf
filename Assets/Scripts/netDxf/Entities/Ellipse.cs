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
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an ellipse <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Ellipse :
        EntityObject
    {
        #region private classes

        private static class ConicThroughFivePoints
        {
            private static float[] CoefficientsLine(Vector2 p1, Vector2 p2)
            {
                // line: A*x + B*y + C = 0
                float[] coefficients = new float[3];
                coefficients[0] = p1.y - p2.y; //A
                coefficients[1] = p2.x - p1.x; //B
                coefficients[2] = p1.x * p2.y - p2.x * p1.y; //C
                return coefficients;
            }

            private static float[] CoefficientsProductLines(float[] l1, float[] l2)
            {
                // lines product: A*x2 + B*xy + C*y2 + D*x + E*y + F = 0
                float[] coefficients = new float[6];
                coefficients[0] = l1[0] * l2[0]; //A
                coefficients[1] = l1[0] * l2[1] + l1[1] * l2[0]; //B
                coefficients[2] = l1[1] * l2[1]; //C
                coefficients[3] = l1[0] * l2[2] + l1[2] * l2[0]; //D
                coefficients[4] = l1[1] * l2[2] + l1[2] * l2[1]; //E
                coefficients[5] = l1[2] * l2[2]; //F
                return coefficients;
            }

            private static float Lambda(float[] alpha_beta, float[] gamma_delta, Vector2 p)
            {
                // conic families: alpha_beta + lambda*gamma_delta = 0
                float a1 = alpha_beta[0] * p.x * p.x;
                float b1 = alpha_beta[1] * p.x * p.y;
                float c1 = alpha_beta[2] * p.y * p.y;
                float d1 = alpha_beta[3] * p.x;
                float e1 = alpha_beta[4] * p.y;
                float f1 = alpha_beta[5];

                float a2 = gamma_delta[0] * p.x * p.x;
                float b2 = gamma_delta[1] * p.x * p.y;
                float c2 = gamma_delta[2] * p.y * p.y;
                float d2 = gamma_delta[3] * p.x;
                float e2 = gamma_delta[4] * p.y;
                float f2 = gamma_delta[5];

                float sum1 = a1 + b1 + c1 + d1 + e1 + f1;
                float sum2 = a2 + b2 + c2 + d2 + e2 + f2;

                if (Mathd.IsZero(sum2))
                {
                    return float.NaN;
                }

                return -sum1 / sum2;
            }

            private static float[] ConicCoefficients(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, Vector2 point5)
            {
                float[] lineAlpha = CoefficientsLine(point1, point2);
                float[] lineBeta = CoefficientsLine(point3, point4);
                float[] lineGamma = CoefficientsLine(point1, point3);
                float[] lineDelta = CoefficientsLine(point2, point4);

                float[] alphaBeta = CoefficientsProductLines(lineAlpha, lineBeta);
                float[] gammaDelta = CoefficientsProductLines(lineGamma, lineDelta);

                float lambda = Lambda(alphaBeta, gammaDelta, point5);
                if (float.IsNaN(lambda))
                { 
                    // conic coefficients cannot be found, duplicate points
                    return null;
                }

                float[] coefficients = new float[6];
                coefficients[0] = alphaBeta[0] + lambda * gammaDelta[0];
                coefficients[1] = alphaBeta[1] + lambda * gammaDelta[1];
                coefficients[2] = alphaBeta[2] + lambda * gammaDelta[2];
                coefficients[3] = alphaBeta[3] + lambda * gammaDelta[3];
                coefficients[4] = alphaBeta[4] + lambda * gammaDelta[4];
                coefficients[5] = alphaBeta[5] + lambda * gammaDelta[5];

                return coefficients;
            }

            public static bool EllipseProperties(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, Vector2 point5, out Vector2 center, out float semiMajorAxis, out float semiMinorAxis, out float rotation)
            {         
                center = new Vector2(float.NaN, float.NaN);
                semiMajorAxis = float.NaN;
                semiMinorAxis = float.NaN;
                rotation = float.NaN;

                float[] coefficients = ConicCoefficients(point1, point2, point3, point4, point5);
                if (coefficients == null)
                {
                    return false;
                }

                float a = coefficients[0];
                float b = coefficients[1];
                float c = coefficients[2];
                float d = coefficients[3];
                float e = coefficients[4];
                float f = coefficients[5];

                float q = b * b - 4 * a * c;
                           
                if (q >= 0)
                {
                    // not an ellipse
                    return false;
                }

                center.x = (2 * c * d - b * e) / q;
                center.y = (2 * a * e - b * d) / q;

                float m = Mathf.Sqrt((a - c) * (a - c) + b * b);
                float n = 2 * (a * e * e + c * d * d - b * d * e + q * f);
                float axis1 = -Mathf.Sqrt(n * (a + c + m)) / q;
                float axis2 = -Mathf.Sqrt(n * (a + c - m)) / q;

                if (Mathd.IsZero(b))
                {
                    // ellipse parallel to world axis
                    if (Mathd.IsEqual(a, c))
                    {
                        rotation = 0.0f;
                    }
                    else
                    {
                        rotation = a < c ? 0.0f : (Mathf.PI / 2);
                    }
                }
                else
                {
                    rotation = Mathf.Atan((c - a - Mathf.Sqrt((a - c) * (a - c) + b * b)) / b);
                }

                if (axis1 >= axis2)
                {
                    semiMajorAxis = axis1;
                    semiMinorAxis = axis2;
                }
                else
                {
                    semiMajorAxis = axis2;
                    semiMinorAxis = axis1;
                    rotation += (Mathf.PI / 2);
                }
                return true;
            }
        }

        #endregion

        #region private fields

        private Vector3 center;
        private float majorAxis;
        private float minorAxis;
        private float rotation;
        private float startAngle;
        private float endAngle;
        private float thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Ellipse</c> class.
        /// </summary>
        public Ellipse()
            : this(Vector3.zero, 1.0f, 0.5f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Ellipse</c> class.
        /// </summary>
        /// <param name="center">Ellipse <see cref="Vector2">center</see> in object coordinates.</param>
        /// <param name="majorAxis">Ellipse major axis.</param>
        /// <param name="minorAxis">Ellipse minor axis.</param>
        /// <remarks>The center Z coordinate represents the elevation of the arc along the normal.</remarks>
        public Ellipse(Vector2 center, float majorAxis, float minorAxis)
            : this(new Vector3(center.x, center.y, 0.0f), majorAxis, minorAxis)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Ellipse</c> class.
        /// </summary>
        /// <param name="center">Ellipse <see cref="Vector3">center</see> in object coordinates.</param>
        /// <param name="majorAxis">Ellipse major axis.</param>
        /// <param name="minorAxis">Ellipse minor axis.</param>
        /// <remarks>The center Z coordinate represents the elevation of the arc along the normal.</remarks>
        public Ellipse(Vector3 center, float majorAxis, float minorAxis)
            : base(EntityType.Ellipse, DxfObjectCode.Ellipse)
        {
            this.center = center;

            if (majorAxis <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(majorAxis), majorAxis, "The major axis value must be greater than zero.");
            }

            if (minorAxis <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minorAxis), minorAxis, "The minor axis value must be greater than zero.");
            }

            if (minorAxis > majorAxis)
            {
                throw new ArgumentException("The major axis must be greater than the minor axis.");
            }

            this.majorAxis = majorAxis;
            this.minorAxis = minorAxis;
            this.startAngle = 0.0f;
            this.endAngle = 0.0f;
            this.rotation = 0.0f;
            this.thickness = 0.0f;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the ellipse <see cref="Vector3">center</see>.
        /// </summary>
        /// <remarks>The center Z coordinate represents the elevation of the arc along the normal.</remarks>
        public Vector3 Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the ellipse mayor axis.
        /// </summary>
        /// <remarks>The MajorAxis value must be positive and greater than the MinorAxis.</remarks>
        public float MajorAxis
        {
            get { return this.majorAxis; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The major axis value must be greater than zero.");
                }
                this.majorAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets the ellipse minor axis.
        /// </summary>
        /// <remarks>The MinorAxis value must be positive and smaller than the MajorAxis.</remarks>
        public float MinorAxis
        {
            get { return this.minorAxis; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The minor axis value must be greater than zero.");
                }
                this.minorAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets the ellipse local rotation in degrees along its normal.
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the ellipse start angle in degrees.
        /// </summary>
        /// <remarks>To get a full ellipse set the start angle equal to the end angle.</remarks>
        public float StartAngle
        {
            get { return this.startAngle; }
            set { this.startAngle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the ellipse end angle in degrees.
        /// </summary>
        /// <remarks>To get a full ellipse set the end angle equal to the start angle.</remarks>
        public float EndAngle
        {
            get { return this.endAngle; }
            set { this.endAngle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the ellipse thickness.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        /// <summary>
        /// Checks if the actual instance is a full ellipse.
        /// </summary>
        /// <remarks>An ellipse is considered full when its start and end angles are equal.</remarks>
        public bool IsFullEllipse
        {
            get { return Mathd.IsEqual(this.startAngle, this.endAngle); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculate the local point on the ellipse for a given angle relative to the center.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>A local point on the ellipse for the given angle relative to the center.</returns>
        public Vector2 PolarCoordinateRelativeToCenter(float angle)
        {
            float a = this.MajorAxis * 0.5f;
            float b = this.MinorAxis * 0.5f;
            float radians = angle * Mathf.Deg2Rad;

            float a1 = a * Mathf.Sin(radians);
            float b1 = b * Mathf.Cos(radians);

            float radius = (a * b) / Mathf.Sqrt(b1 * b1 + a1 * a1);

            // convert the radius back to Cartesian coordinates
            return new Vector2(radius * Mathf.Cos(radians), radius * Mathf.Sin(radians));
        }

        /// <summary>
        /// Converts the ellipse in a list of vertexes.
        /// </summary>
        /// <param name="precision">Number of vertexes generated.</param>
        /// <returns>A list vertexes that represents the ellipse expressed in object coordinate system.</returns>
        public List<Vector2> PolygonalVertexes(int precision)
        {
            if (precision < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), precision, "The arc precision must be equal or greater than two.");
            }

            List<Vector2> points = new List<Vector2>();
            float beta = this.rotation * Mathf.Deg2Rad;
            float sinBeta = Mathf.Sin(beta);
            float cosBeta = Mathf.Cos(beta);
            float start;
            float end;
            float steps;

            if (this.IsFullEllipse)
            {
                start = 0;
                end = (2 * Mathf.PI);
                steps = precision;
            }
            else
            {
                Vector2 startPoint = this.PolarCoordinateRelativeToCenter(this.startAngle);
                Vector2 endPoint = this.PolarCoordinateRelativeToCenter(this.endAngle);
                float a = 1 / (0.5f * this.majorAxis);
                float b = 1 / (0.5f * this.minorAxis);
                start = Mathf.Atan2(startPoint.y * b, startPoint.x * a);
                end = Mathf.Atan2(endPoint.y * b, endPoint.x * a);

                if (end < start)
                {
                    end += (2 * Mathf.PI);
                }
                steps = precision - 1;
            }
           
            float delta = (end - start) / steps;

            for (int i = 0; i < precision; i++)
            {
                float angle = start + delta * i;
                float sinAlpha = Mathf.Sin(angle);
                float cosAlpha = Mathf.Cos(angle);

                float pointX = 0.5f * (this.majorAxis * cosAlpha * cosBeta - this.minorAxis * sinAlpha * sinBeta);
                float pointY = 0.5f * (this.majorAxis * cosAlpha * sinBeta + this.minorAxis * sinAlpha * cosBeta);

                points.Add(new Vector2(pointX, pointY));
            }

            return points;
        }

        /// <summary>
        /// Converts the ellipse in a Polyline2D.
        /// </summary>
        /// <param name="precision">Number of vertexes generated.</param>
        /// <returns>A new instance of <see cref="Polyline2D">Polyline2D</see> that represents the ellipse.</returns>
        public Polyline2D ToPolyline2D(int precision)
        {
            List<Vector2> vertexes = this.PolygonalVertexes(precision);
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
                IsClosed = this.IsFullEllipse
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
        /// Creates a new Ellipse that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Ellipse that is a copy of this instance.</returns>
        public override object Clone()
        {
            Ellipse entity = new Ellipse
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
                //Ellipse properties
                Center = this.center,
                MajorAxis = this.majorAxis,
                MinorAxis = this.minorAxis,
                Rotation = this.rotation,
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
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
    /// Represents a two dimensional polyline <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>
    /// Two dimensional polylines can hold information about the width of the lines and arcs that compose them.
    /// </remarks>
    public class Polyline2D :
        EntityObject
    {
        #region private fields

        private readonly List<Polyline2DVertex> vertexes;
        private PolylineTypeFlags flags;
        private float elevation;
        private float thickness;
        private PolylineSmoothType smoothType;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2D</c> class.
        /// </summary>
        public Polyline2D()
            : this(new List<Polyline2DVertex>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2D</c> class.
        /// </summary>
        /// <param name="vertexes">Polyline2D <see cref="Vector2">vertex</see> list in object coordinates.</param>
        public Polyline2D(IEnumerable<Vector2> vertexes)
            : this(vertexes, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2D</c> class.
        /// </summary>
        /// <param name="vertexes">Polyline2D <see cref="Vector2">vertex</see> list in object coordinates.</param>
        /// <param name="isClosed">Sets if the polyline is closed, by default it will create an open polyline.</param>
        public Polyline2D(IEnumerable<Vector2> vertexes, bool isClosed)
            : base(EntityType.Polyline2D, DxfObjectCode.LwPolyline)
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException(nameof(vertexes));
            }

            this.vertexes = new List<Polyline2DVertex>();
            foreach (Vector2 vertex in vertexes)
            {
                this.vertexes.Add(new Polyline2DVertex(vertex));
            }

            this.elevation = 0.0f;
            this.thickness = 0.0f;
            this.flags = isClosed ? PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylineTypeFlags.OpenPolyline;
            this.smoothType = PolylineSmoothType.NoSmooth;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2D</c> class.
        /// </summary>
        /// <param name="vertexes">Polyline2D <see cref="Polyline2DVertex">vertex</see> list in object coordinates.</param>
        public Polyline2D(IEnumerable<Polyline2DVertex> vertexes)
            : this(vertexes, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polyline2D</c> class.
        /// </summary>
        /// <param name="vertexes">Polyline2D <see cref="Polyline2DVertex">vertex</see> list in object coordinates.</param>
        /// <param name="isClosed">Sets if the polyline is closed (default: false).</param>
        public Polyline2D(IEnumerable<Polyline2DVertex> vertexes, bool isClosed)
            : base(EntityType.Polyline2D, DxfObjectCode.LwPolyline)
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException(nameof(vertexes));
            }

            this.vertexes = new List<Polyline2DVertex>(vertexes);
            this.elevation = 0.0f;
            this.thickness = 0.0f;
            this.flags = isClosed ? PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylineTypeFlags.OpenPolyline;
            this.smoothType = PolylineSmoothType.NoSmooth;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the polyline <see cref="Polyline2DVertex">vertex</see> list.
        /// </summary>
        public List<Polyline2DVertex> Vertexes
        {
            get { return this.vertexes; }
        }

        /// <summary>
        /// Gets or sets if the polyline is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return this.flags.HasFlag(PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM); }
            set
            {
                if (value)
                {
                    this.flags |= PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
                else
                {
                    this.flags &= ~PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
            }
        }

        /// <summary>
        /// Gets or sets the polyline thickness.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        /// <summary>
        /// Gets or sets the polyline elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the light weight polyline.</remarks>
        public float Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        /// <summary>
        /// Enable or disable if the linetype pattern is generated continuously around the vertexes of the polyline.
        /// </summary>
        public bool LinetypeGeneration
        {
            get { return this.flags.HasFlag(PolylineTypeFlags.ContinuousLinetypePattern); }
            set
            {
                if (value)
                {
                    this.flags |= PolylineTypeFlags.ContinuousLinetypePattern;
                }
                else
                {
                    this.flags &= ~PolylineTypeFlags.ContinuousLinetypePattern;
                }
            }
        }

        /// <summary>
        /// Gets or sets the polyline smooth type.
        /// </summary>
        /// <remarks>
        /// The additional polyline vertexes corresponding to the SplineFit will be created when writing the DXF file.
        /// It is not recommended to use any kind of smoothness in polylines other than NoSmooth. Use a Spline entity instead.
        /// </remarks>
        public PolylineSmoothType SmoothType
        {
            get { return this.smoothType; }
            set
            {
                if (value == PolylineSmoothType.NoSmooth)
                {
                    this.CodeName = DxfObjectCode.LwPolyline;
                    this.flags &= ~PolylineTypeFlags.SplineFit;
                }
                else
                {
                    this.CodeName = DxfObjectCode.Polyline;
                    this.flags |= PolylineTypeFlags.SplineFit;
                }
                this.smoothType = value;
            }
        }

        #endregion

        #region internal properties

        /// <summary>
        /// Gets the polyline flags.
        /// </summary>
        internal PolylineTypeFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Switch the polyline direction.
        /// </summary>
        public void Reverse()
        {
            if (this.vertexes.Count < 2)
            {
                return;
            }

            this.vertexes.Reverse();

            float firstBulge = this.vertexes[0].Bulge;
       
            for (int i = 0; i < this.vertexes.Count - 1; i++)
            {
                this.vertexes[i].Bulge = -this.vertexes[i + 1].Bulge;
            }

            this.vertexes[this.vertexes.Count - 1].Bulge = -firstBulge;
        }

        /// <summary>
        /// Sets a constant width for all the polyline segments.
        /// </summary>
        /// <param name="width">Polyline width.</param>
        /// <remarks>
        /// Smoothed polylines can only have a constant width, the start width of the first vertex will be used.
        /// </remarks>
        public void SetConstantWidth(float width)
        {
            foreach (Polyline2DVertex v in this.vertexes)
            {
                v.StartWidth = width;
                v.EndWidth = width;
            }
        }

        /// <summary>
        /// Decompose the actual polyline in its internal entities, <see cref="Line">lines</see> and <see cref="Arc">arcs</see>.
        /// </summary>
        /// <returns>A list of <see cref="Line">lines</see> and <see cref="Arc">arcs</see> that made up the polyline.</returns>
        public List<EntityObject> Explode()
        {
            List<EntityObject> entities = new List<EntityObject>();
            int index = 0;
            foreach (Polyline2DVertex vertex in this.Vertexes)
            {
                float bulge = vertex.Bulge;
                Vector2 p1;
                Vector2 p2;

                if (index == this.Vertexes.Count - 1)
                {
                    if (!this.IsClosed)
                    {
                        break;
                    }
                    p1 = new Vector2(vertex.Position.x, vertex.Position.y);
                    p2 = new Vector2(this.vertexes[0].Position.x, this.vertexes[0].Position.y);
                }
                else
                {
                    p1 = new Vector2(vertex.Position.x, vertex.Position.y);
                    p2 = new Vector2(this.vertexes[index + 1].Position.x, this.vertexes[index + 1].Position.y);
                }

                if (Mathd.IsZero(bulge))
                {
                    // the polyline edge is a line
                    Vector3 start = new Vector3(p1.x, p1.y, this.elevation);
                    Vector3 end = new Vector3(p2.x, p2.y, this.elevation);

                    entities.Add(new Line
                    {
                        Layer = (Layer) this.Layer.Clone(),
                        Linetype = (Linetype) this.Linetype.Clone(),
                        Color = (AciColor) this.Color.Clone(),
                        Lineweight = this.Lineweight,
                        Transparency = (Transparency) this.Transparency.Clone(),
                        LinetypeScale = this.LinetypeScale,
                        Normal = this.Normal,
                        StartPoint = start,
                        EndPoint = end,
                        Thickness = this.Thickness
                    });
                }
                else
                {
                    // the polyline edge is an arc
                    float theta = 4 * Mathf.Atan(Mathf.Abs(bulge));
                    float c = Vector2.Distance(p1, p2) / 2.0f;
                    float r = c / Mathf.Sin(theta / 2.0f);

                    // avoid arcs with very small radius, draw a line instead
                    if (Mathd.IsZero(r))
                    {
                        // the polyline edge is a line
                        List<Vector3> points = new List<Vector3>
                            {
                                new Vector3(p1.x, p1.y, this.elevation),
                                new Vector3(p2.x, p2.y, this.elevation)
                            };

                        entities.Add(new Line
                        {
                            Layer = (Layer)this.Layer.Clone(),
                            Linetype = (Linetype)this.Linetype.Clone(),
                            Color = (AciColor)this.Color.Clone(),
                            Lineweight = this.Lineweight,
                            Transparency = (Transparency)this.Transparency.Clone(),
                            LinetypeScale = this.LinetypeScale,
                            Normal = this.Normal,
                            StartPoint = points[0],
                            EndPoint = points[1],
                            Thickness = this.Thickness,
                        });
                    }
                    else
                    {
                        float gamma = (Mathf.PI - theta) / 2;
                        float phi = Mathd.Angle(p1, p2) + Mathf.Sign(bulge) * gamma;
                        Vector2 center = new Vector2(p1.x + r * Mathf.Cos(phi), p1.y + r * Mathf.Sin(phi));
                        float startAngle;
                        float endAngle;
                        if (bulge > 0)
                        {
                            startAngle = Mathf.Rad2Deg * Mathd.Angle(p1 - center);
                            endAngle = startAngle + Mathf.Rad2Deg*theta;
                        }
                        else
                        {
                            endAngle = Mathf.Rad2Deg * Mathd.Angle(p1 - center);
                            startAngle = endAngle - Mathf.Rad2Deg * theta;
                        }
                        Vector3 point = new Vector3(center.x, center.y, this.elevation);

                        entities.Add(new Arc
                        {
                            Layer = (Layer) this.Layer.Clone(),
                            Linetype = (Linetype) this.Linetype.Clone(),
                            Color = (AciColor) this.Color.Clone(),
                            Lineweight = this.Lineweight,
                            Transparency = (Transparency) this.Transparency.Clone(),
                            LinetypeScale = this.LinetypeScale,
                            Normal = this.Normal,
                            Center = point,
                            Radius = r,
                            StartAngle = startAngle,
                            EndAngle = endAngle,
                            Thickness = this.Thickness,
                        });
                    }
                }
                index++;
            }

            return entities;
        }

        /// <summary>
        /// Obtains a list of vertexes that represent the polyline approximating the curve segments as necessary.
        /// </summary>
        /// <param name="precision">Curve segments precision. The number of vertexes created, a value of zero means that no approximation will be made.</param>
        /// <returns>A list of vertexes expressed in object coordinate system.</returns>
        public List<Vector2> PolygonalVertexes(int precision)
        {
            return this.PolygonalVertexes(precision, Mathf.Epsilon, Mathf.Epsilon);
        }

        /// <summary>
        /// Obtains a list of vertexes that represent the polyline approximating the curve segments as necessary.
        /// </summary>
        /// <param name="bulgePrecision">Curve segments precision. The number of vertexes created, a value of zero means that no approximation will be made.</param>
        /// <param name="weldThreshold">Tolerance to consider if two new generated vertexes are equal.</param>
        /// <param name="bulgeThreshold">Minimum distance from which approximate curved segments of the polyline.</param>
        /// <returns>A list of vertexes expressed in object coordinate system.</returns>
        public List<Vector2> PolygonalVertexes(int bulgePrecision, float weldThreshold, float bulgeThreshold)
        {
            if (bulgePrecision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bulgePrecision), bulgePrecision, "The bulgePrecision must be equal or greater than zero.");
            }

            List<Vector2> ocsVertexes = new List<Vector2>();
            int degree;
            if (this.smoothType == PolylineSmoothType.Quadratic)
            {
                degree = 2;
            }
            else if (this.smoothType == PolylineSmoothType.Cubic)
            {
                degree = 3;
            }
            else
            {
                int index = 0;

                foreach (Polyline2DVertex vertex in this.Vertexes)
                {
                    float bulge = vertex.Bulge;
                    Vector2 p1;
                    Vector2 p2;

                    if (index == this.Vertexes.Count - 1)
                    {
                        p1 = new Vector2(vertex.Position.x, vertex.Position.y);
                        if (!this.IsClosed)
                        {
                            ocsVertexes.Add(p1);
                            continue;
                        }
                        p2 = new Vector2(this.vertexes[0].Position.x, this.vertexes[0].Position.y);
                    }
                    else
                    {
                        p1 = new Vector2(vertex.Position.x, vertex.Position.y);
                        p2 = new Vector2(this.vertexes[index + 1].Position.x, this.vertexes[index + 1].Position.y);
                    }

                    if (!p1.IsEquals(p2, weldThreshold))
                    {
                        if (Mathd.IsZero(bulge) || bulgePrecision == 0)
                        {
                            ocsVertexes.Add(p1);
                        }
                        else
                        {
                            float c = Vector2.Distance(p1, p2) * 0.5f;
                            if (c >= bulgeThreshold)
                            {
                                float s = c * Mathf.Abs(bulge);
                                float r = (c * c + s * s) / (2.0f * s);
                                float theta = 4 * Mathf.Atan(Mathf.Abs(bulge));
                                float gamma = (Mathf.PI - theta) * 0.5f;
                                float phi = Mathd.Angle(p1, p2) + Mathf.Sign(bulge) * gamma;
                                Vector2 center = new Vector2(p1.x + r * Mathf.Cos(phi), p1.y + r * Mathf.Sin(phi));
                                Vector2 a1 = p1 - center;
                                float angle = Mathf.Sign(bulge) * theta / (bulgePrecision + 1);
                                ocsVertexes.Add(p1);
                                Vector2 prevCurvePoint = p1;
                                for (int i = 1; i <= bulgePrecision; i++)
                                {
                                    Vector2 curvePoint = new Vector2
                                    {
                                        x = center.x + Mathf.Cos(i * angle) * a1.x - Mathf.Sin(i * angle) * a1.y,
                                        y = center.y + Mathf.Sin(i * angle) * a1.x + Mathf.Cos(i * angle) * a1.y
                                    };

                                    if (!curvePoint.IsEquals(prevCurvePoint, weldThreshold) && !curvePoint.IsEquals(p2, weldThreshold))
                                    {
                                        ocsVertexes.Add(curvePoint);
                                        prevCurvePoint = curvePoint;
                                    }
                                }
                            }
                            else
                            {
                                ocsVertexes.Add(p1);
                            }
                        }
                    }
                    index++;
                }

                return ocsVertexes;
            }

            List<SplineVertex> ctrlPoints = new List<SplineVertex>();
            foreach (Polyline2DVertex vertex in this.vertexes)
            {
                ctrlPoints.Add(new SplineVertex(vertex.Position));
            }

            // closed polylines will be considered as closed and periodic
            List<Vector3> points = Spline.NurbsEvaluator(ctrlPoints, null, degree, false, this.IsClosed, bulgePrecision);
            foreach (Vector3 point in points)
            {
                ocsVertexes.Add(new Vector2(point.x, point.y));
            }

            return ocsVertexes;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Polyline2D that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Polyline2D that is a copy of this instance.</returns>
        public override object Clone()
        {
            Polyline2D entity = new Polyline2D
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
                //LwPolyline properties
                Elevation = this.elevation,
                Thickness = this.thickness,
                Flags = this.flags
            };

            foreach (Polyline2DVertex vertex in this.vertexes)
            {
                entity.Vertexes.Add((Polyline2DVertex) vertex.Clone());
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
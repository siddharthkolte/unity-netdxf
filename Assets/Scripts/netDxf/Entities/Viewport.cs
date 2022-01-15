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
using netDxf.Collections;
using netDxf.Math;
using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a view in paper space of the model.
    /// </summary>
    /// <remarks>
    /// The viewport with id equals 1 is the view of the paper space layout itself and it does not show the model.
    /// </remarks>
    public class Viewport :
        EntityObject
    {
        #region delegates and events

        public delegate void ClippingBoundaryAddedEventHandler(Viewport sender, EntityChangeEventArgs e);

        public event ClippingBoundaryAddedEventHandler ClippingBoundaryAdded;

        protected virtual void OnClippingBoundaryAddedEvent(EntityObject item)
        {
            ClippingBoundaryAddedEventHandler ae = this.ClippingBoundaryAdded;
            if (ae != null)
                ae(this, new EntityChangeEventArgs(item));
        }

        public delegate void ClippingBoundaryRemovedEventHandler(Viewport sender, EntityChangeEventArgs e);

        public event ClippingBoundaryRemovedEventHandler ClippingBoundaryRemoved;

        protected virtual void OnClippingBoundaryRemovedEvent(EntityObject item)
        {
            ClippingBoundaryRemovedEventHandler ae = this.ClippingBoundaryRemoved;
            if (ae != null)
                ae(this, new EntityChangeEventArgs(item));
        }

        #endregion

        #region private fields

        private Vector3 center;
        private float width;
        private float height;
        private short stacking;
        private short id;
        private Vector2 viewCenter;
        private Vector2 snapBase;
        private Vector2 snapSpacing;
        private Vector2 gridSpacing;
        private Vector3 viewDirection;
        private Vector3 viewTarget;
        private float lensLength;
        private float frontClipPlane;
        private float backClipPlane;
        private float viewHeight;
        private float snapAngle;
        private float twistAngle;
        private short circleZoomPercent;
        private ViewportStatusFlags status;
        private readonly ObservableCollection<Layer> frozenLayers;
        private Vector3 ucsOrigin;
        private Vector3 ucsXAxis;
        private Vector3 ucsYAxis;
        private float elevation;
        private EntityObject boundary;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new viewport object.
        /// </summary>
        public Viewport()
            : this(2)
        {
            this.status |= ViewportStatusFlags.GridMode;
        }

        public Viewport(Vector2 bottomLeftCorner, Vector2 topRightCorner)
            :this(2)
        {
            this.center = new Vector3((topRightCorner.x + bottomLeftCorner.x) * 0.5f, (topRightCorner.y + bottomLeftCorner.y) * 0.5f, 0);
            this.width = (topRightCorner.x - bottomLeftCorner.x) * 0.5f;
            this.height = (topRightCorner.y - bottomLeftCorner.y) * 0.5f;
        }

        public Viewport(Vector2 center, float width, float height)
            :this(2)
        {
            this.center = new Vector3(center.x, center.y, 0.0f);
            this.width = width;
            this.height = height;
        }

        public Viewport(EntityObject clippingBoundary)
            :this(2)
        {
            this.ClippingBoundary = clippingBoundary;
        }

        internal Viewport(short id)
            : base(EntityType.Viewport, DxfObjectCode.Viewport)
        {
            this.center = Vector3.zero;
            this.width = 297;
            this.height = 210;
            this.stacking = id;
            this.id = id;
            this.viewCenter = Vector2.zero;
            this.snapBase = Vector2.zero;
            this.snapSpacing = new Vector2(10.0f, 10.0f);
            this.gridSpacing = new Vector2(10.0f, 10.0f);
            this.viewDirection = Mathd.V3UnitZ;
            this.viewTarget = Vector3.zero;
            this.lensLength = 50.0f;
            this.frontClipPlane = 0.0f;
            this.backClipPlane = 0.0f;
            this.viewHeight = 250f;
            this.snapAngle = 0.0f;
            this.twistAngle = 0.0f;
            this.circleZoomPercent = 1000;
            this.status = ViewportStatusFlags.AdaptiveGridDisplay |
                          ViewportStatusFlags.DisplayGridBeyondDrawingLimits |
                          ViewportStatusFlags.CurrentlyAlwaysEnabled |
                          ViewportStatusFlags.UcsIconVisibility |
                          ViewportStatusFlags.GridMode;
            this.frozenLayers = new ObservableCollection<Layer>();
            this.frozenLayers.BeforeAddItem += this.FrozenLayers_BeforeAddItem;
            this.ucsOrigin = Vector3.zero;
            this.ucsXAxis = Mathd.V3UnitX;
            this.ucsYAxis = Mathd.V3UnitY;
            this.elevation = 0.0f;
            this.boundary = null;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center point in paper space units.
        /// </summary>
        public Vector3 Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the width in paper space units.
        /// </summary>
        public float Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the height in paper space units.
        /// </summary>
        public float Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Viewport status field:<br />
        /// -1 = On, but is fully off screen, or is one of the viewports that is not active because the $MAXACTVP count is currently being exceeded.<br />
        /// 0 = Off<br />
        /// 1 = Stacking value reserved for the layout view.
        /// positive value = On and active. The value indicates the order of stacking for the viewports, where 1 is the active viewport, 2 is the next, and so forth.
        /// </summary>
        public short Stacking
        {
            get { return this.stacking; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The stacking value must be greater than -1.");
                }
                this.stacking = value;
            }
        }

        /// <summary>
        /// Gets or sets the viewport ID.
        /// </summary>
        internal short Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets the view center point (in DCS).
        /// </summary>
        public Vector2 ViewCenter
        {
            get { return this.viewCenter; }
            set { this.viewCenter = value; }
        }

        /// <summary>
        /// Gets or sets the snap base point.
        /// </summary>
        public Vector2 SnapBase
        {
            get { return this.snapBase; }
            set { this.snapBase = value; }
        }

        /// <summary>
        /// Gets or sets the snap spacing.
        /// </summary>
        public Vector2 SnapSpacing
        {
            get { return this.snapSpacing; }
            set { this.snapSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the grid spacing.
        /// </summary>
        public Vector2 GridSpacing
        {
            get { return this.gridSpacing; }
            set { this.gridSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the view direction vector (in WCS).
        /// </summary>
        public Vector3 ViewDirection
        {
            get { return this.viewDirection; }
            set { this.viewDirection = value; }
        }

        /// <summary>
        /// Gets or sets the view target point (in WCS).
        /// </summary>
        public Vector3 ViewTarget
        {
            get { return this.viewTarget; }
            set { this.viewTarget = value; }
        }

        /// <summary>
        /// Gets or sets the perspective lens length.
        /// </summary>
        public float LensLength
        {
            get { return this.lensLength; }
            set { this.lensLength = value; }
        }

        /// <summary>
        /// Gets or sets the front clip plane Z value.
        /// </summary>
        public float FrontClipPlane
        {
            get { return this.frontClipPlane; }
            set { this.frontClipPlane = value; }
        }

        /// <summary>
        /// Gets or sets the back clip plane Z value.
        /// </summary>
        public float BackClipPlane
        {
            get { return this.backClipPlane; }
            set { this.backClipPlane = value; }
        }

        /// <summary>
        /// Gets or sets the view height (in model space units).
        /// </summary>
        public float ViewHeight
        {
            get { return this.viewHeight; }
            set { this.viewHeight = value; }
        }

        /// <summary>
        /// Gets or sets the snap angle.
        /// </summary>
        public float SnapAngle
        {
            get { return this.snapAngle; }
            set { this.snapAngle = value; }
        }

        /// <summary>
        /// Gets or sets the view twist angle.
        /// </summary>
        public float TwistAngle
        {
            get { return this.twistAngle; }
            set { this.twistAngle = value; }
        }

        /// <summary>
        /// Gets or sets the circle zoom percent.
        /// </summary>
        public short CircleZoomPercent
        {
            get { return this.circleZoomPercent; }
            set { this.circleZoomPercent = value; }
        }

        /// <summary>
        /// Gets the list of layers that are frozen in this viewport.
        /// </summary>
        /// <remarks>
        /// The FrozenLayers list cannot contain null items and layers that belong to different documents.
        /// Even if duplicate items should not cause any problems, it is not allowed to have two layers with the same name in the list.
        /// </remarks>
        public ObservableCollection<Layer> FrozenLayers
        {
            get { return this.frozenLayers; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ViewportStatusFlags">viewport status flags</see>:
        /// </summary>
        public ViewportStatusFlags Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// Gets or sets the UCS origin.
        /// </summary>
        public Vector3 UcsOrigin
        {
            get { return this.ucsOrigin; }
            set { this.ucsOrigin = value; }
        }

        /// <summary>
        /// Gets or sets the UCS X axis.
        /// </summary>
        public Vector3 UcsXAxis
        {
            get { return this.ucsXAxis; }
            set { this.ucsXAxis = value; }
        }

        /// <summary>
        /// Gets or sets the UCS Y axis.
        /// </summary>
        public Vector3 UcsYAxis
        {
            get { return this.ucsYAxis; }
            set { this.ucsYAxis = value; }
        }

        /// <summary>
        /// Gets or sets the elevation.
        /// </summary>
        public float Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        /// <summary>
        /// Entity that serves as the viewport clipping boundary (only present if viewport is non-rectangular).
        /// </summary>
        /// <remarks>
        /// AutoCad does not allow the creation of viewports from open shapes such as LwPolylines, Polylines, or ellipse arcs;
        /// but if they are edited afterward, making them open, it will not complain, and they will work without problems.
        /// So, it is possible to use open shapes as clipping boundaries, even if it is not recommended.
        /// It might not be supported by all programs that read DXF files and a redraw of the layout might be required to show them correctly inside AutoCad.<br />
        /// Only X and Y coordinates will be used the entity normal will be considered as UnitZ.<br />
        /// When the viewport is added to the document this entity will be added too.
        /// </remarks>
        public EntityObject ClippingBoundary
        {
            get { return this.boundary; }
            set
            {
                if (value != null)
                {
                    BoundingRectangle abbr;
                    switch (value.Type)
                    {
                        case EntityType.Circle:
                            Circle circle = (Circle) value;
                            abbr = new BoundingRectangle(new Vector2(circle.Center.x, circle.Center.y), circle.Radius);
                            break;
                        case EntityType.Ellipse:
                            Ellipse ellipse = (Ellipse) value;
                            abbr = new BoundingRectangle(new Vector2(ellipse.Center.x, ellipse.Center.y), ellipse.MajorAxis, ellipse.MinorAxis, ellipse.Rotation);
                            break;
                        case EntityType.Polyline2D:
                            Polyline2D poly2D = (Polyline2D) value;
                            abbr = new BoundingRectangle(poly2D.PolygonalVertexes(6, Mathf.Epsilon, Mathf.Epsilon));
                            break;
                        case EntityType.Polyline3D:
                            Polyline3D poly3D = (Polyline3D) value;
                            List<Vector2> pPoints = new List<Vector2>();
                            foreach (Vector3 point in poly3D.Vertexes)
                            {
                                pPoints.Add(new Vector2(point.x, point.y));
                            }
                            abbr = new BoundingRectangle(pPoints);
                            break;
                        case EntityType.Spline:
                            Spline spline = (Spline) value;
                            List<Vector2> sPoints = new List<Vector2>();
                            foreach (SplineVertex point in spline.ControlPoints)
                            {
                                sPoints.Add(new Vector2(point.Position.x, point.Position.y));
                            }
                            abbr = new BoundingRectangle(sPoints);
                            break;
                        default:
                            throw new ArgumentException("Only lightweight polylines, polylines, circles, ellipses and splines are allowed as a viewport clipping boundary.");
                    }

                    this.width = abbr.Width;
                    this.height = abbr.Height;
                    this.center = new Vector3(abbr.Center.x, abbr.Center.y, 0.0f);
                    this.status |= ViewportStatusFlags.NonRectangularClipping;
                }
                else
                {
                    this.status &= ~ViewportStatusFlags.NonRectangularClipping;
                }

                // nothing else to do if it is the same
                if (ReferenceEquals(this.boundary, value))
                    return;

                // remove the previous clipping boundary
                if (this.boundary != null)
                {
                    this.boundary.RemoveReactor(this);
                    this.OnClippingBoundaryRemovedEvent(this.boundary);
                }

                // add the new clipping boundary
                if (value != null)
                {
                    value.AddReactor(this);
                    this.OnClippingBoundaryAddedEvent(value);
                }

                this.boundary = value;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new viewport that is a copy of the current instance.
        /// </summary>
        /// <returns>A new viewport that is a copy of this instance.</returns>
        public override object Clone()
        {
            Viewport viewport = new Viewport
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
                //viewport properties
                ClippingBoundary = (EntityObject) this.boundary?.Clone(),
                Center = this.center,
                Width = this.width,
                Height = this.height,
                Stacking = this.stacking,
                Id = this.id,
                ViewCenter = this.viewCenter,
                SnapBase = this.snapBase,
                SnapSpacing = this.snapSpacing,
                GridSpacing = this.gridSpacing,
                ViewDirection = this.viewDirection,
                ViewTarget = this.viewTarget,
                LensLength = this.lensLength,
                FrontClipPlane = this.frontClipPlane,
                BackClipPlane = this.backClipPlane,
                ViewHeight = this.viewHeight,
                SnapAngle = this.snapAngle,
                TwistAngle = this.twistAngle,
                CircleZoomPercent = this.circleZoomPercent,
                Status = this.status,
                UcsOrigin = this.ucsOrigin,
                UcsXAxis = this.ucsXAxis,
                UcsYAxis = this.ucsYAxis,
                Elevation = this.elevation
            };

            foreach (Layer layer in this.frozenLayers)
                viewport.frozenLayers.Add((Layer) layer.Clone());

            foreach (XData data in this.XData.Values)
                viewport.XData.Add((XData) data.Clone());

            return viewport;
        }

        #endregion

        #region FrozenLayers events

        private void FrozenLayers_BeforeAddItem(ObservableCollection<Layer> sender, ObservableCollectionEventArgs<Layer> e)
        {
            if (e.Item == null)
            {
                // the frozen layer list cannot contain null items
                e.Cancel = true; 
            }
            else if (this.Owner != null && e.Item.Owner == null)
            {
                // the frozen layer and the viewport must belong to the same document
                e.Cancel = true;
            }
            else if (this.Owner == null && e.Item.Owner != null)
            {
                // the frozen layer and the viewport must belong to the same document
                e.Cancel = true;
            }
            else if (this.Owner != null && e.Item.Owner != null)
            {
               // the frozen layer and the viewport must belong to the same document
               if (!ReferenceEquals(this.Owner.Owner.Owner.Owner, e.Item.Owner.Owner))
               {
                   e.Cancel = true;
               }
            }
            else if (this.frozenLayers.Contains(e.Item))
            {
                // the frozen layer list cannot contain duplicates
                e.Cancel = true;
            }
        }

        #endregion
    }
}
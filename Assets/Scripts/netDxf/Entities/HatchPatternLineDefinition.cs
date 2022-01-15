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
    /// Defines a single line thats is part of a <see cref="HatchPattern">hatch pattern</see>.
    /// </summary>
    public class HatchPatternLineDefinition :
        ICloneable
    {
        #region private fields

        private float angle;
        private Vector2 origin;
        private Vector2 delta;
        private readonly List<float> dashPattern;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>HatchPatternLineDefinition</c> class.
        /// </summary>
        public HatchPatternLineDefinition()
        {
            this.angle = 0.0f;
            this.origin = Vector2.zero;
            this.delta = Vector2.zero;
            this.dashPattern = new List<float>();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the angle of the line.
        /// </summary>
        public float Angle
        {
            get { return this.angle; }
            set { this.angle = Mathd.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the origin of the line.
        /// </summary>
        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the local displacements between lines of the same family.
        /// </summary>
        /// <remarks>
        /// The Delta.x value indicates the displacement between members of the family in the direction of the line. It is used only for dashed lines.
        /// The Delta.y value indicates the spacing between members of the family; that is, it is measured perpendicular to the lines. 
        /// </remarks>
        public Vector2 Delta
        {
            get { return this.delta; }
            set { this.delta = value; }
        }

        /// <summary>
        /// Gets he dash pattern of the line it is equivalent as the segments of a <see cref="Linetype">Linetype</see>.
        /// </summary>
        /// <remarks>
        /// Positive values means solid segments and negative values means spaces (one entry per element).
        /// </remarks>
        public List<float> DashPattern
        {
            get { return this.dashPattern; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new HatchPatternLineDefinition that is a copy of the current instance.
        /// </summary>
        /// <returns>A new HatchPatternLineDefinition that is a copy of this instance.</returns>
        public object Clone()
        {
            HatchPatternLineDefinition copy = new HatchPatternLineDefinition
            {
                Angle = this.angle,
                Origin = this.origin,
                Delta = this.delta,
            };

            foreach (float dash in this.dashPattern)
                copy.DashPattern.Add(dash);

            return copy;
        }

        #endregion
    }
}
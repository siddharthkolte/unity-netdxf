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

using netDxf.Tables;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a solid <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Solid :
        EntityObject
    {
        #region private fields

        private Vector2 firstVertex;
        private Vector2 secondVertex;
        private Vector2 thirdVertex;
        private Vector2 fourthVertex;
        private float elevation;
        private float thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        public Solid()
            : this(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex)
            : this(new Vector2(firstVertex.x, firstVertex.y),
                new Vector2(secondVertex.x, secondVertex.y),
                new Vector2(thirdVertex.x, thirdVertex.y),
                new Vector2(thirdVertex.x, thirdVertex.y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        /// <param name="fourthVertex">Solid <see cref="Vector2">fourth vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex)
            : base(EntityType.Solid, DxfObjectCode.Solid)
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            this.elevation = 0.0f;
            this.thickness = 0.0f;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FirstVertex
        {
            get { return this.firstVertex; }
            set { this.firstVertex = value; }
        }

        /// <summary>
        /// Gets or sets the second solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 SecondVertex
        {
            get { return this.secondVertex; }
            set { this.secondVertex = value; }
        }

        /// <summary>
        /// Gets or sets the third solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 ThirdVertex
        {
            get { return this.thirdVertex; }
            set { this.thirdVertex = value; }
        }

        /// <summary>
        /// Gets or sets the fourth solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FourthVertex
        {
            get { return this.fourthVertex; }
            set { this.fourthVertex = value; }
        }

        /// <summary>
        /// Gets or sets the solid elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the solid.</remarks>
        public float Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        /// <summary>
        /// Gets or sets the thickness of the solid.
        /// </summary>
        public float Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Solid that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Solid that is a copy of this instance.</returns>
        public override object Clone()
        {
            Solid entity = new Solid
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
                //Solid properties
                FirstVertex = this.firstVertex,
                SecondVertex = this.secondVertex,
                ThirdVertex = this.thirdVertex,
                FourthVertex = this.fourthVertex,
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
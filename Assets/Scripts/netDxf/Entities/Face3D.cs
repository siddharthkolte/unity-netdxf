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
    /// Represents a 3d Face <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Face3D :
        EntityObject
    {
        #region private fields

        private Vector3 firstVertex;
        private Vector3 secondVertex;
        private Vector3 thirdVertex;
        private Vector3 fourthVertex;
        private Face3DEdgeFlags edgeFlags;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Face3D</c> class.
        /// </summary>
        public Face3D()
            : this(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Face3D</c> class.
        /// </summary>
        /// <param name="firstVertex">Face3D <see cref="Vector2">first vertex</see>.</param>
        /// <param name="secondVertex">Face3D <see cref="Vector2">second vertex</see>.</param>
        /// <param name="thirdVertex">Face3D <see cref="Vector2">third vertex</see>.</param>
        public Face3D(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex)
            : this(new Vector3(firstVertex.x, firstVertex.y, 0.0f),
                new Vector3(secondVertex.x, secondVertex.y, 0.0f),
                new Vector3(thirdVertex.x, thirdVertex.y, 0.0f),
                new Vector3(thirdVertex.x, thirdVertex.y, 0.0f))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Face3D</c> class.
        /// </summary>
        /// <param name="firstVertex">Face3D <see cref="Vector2">first vertex</see>.</param>
        /// <param name="secondVertex">Face3D <see cref="Vector2">second vertex</see>.</param>
        /// <param name="thirdVertex">Face3D <see cref="Vector2">third vertex</see>.</param>
        /// <param name="fourthVertex">Face3D <see cref="Vector2">fourth vertex</see>.</param>
        public Face3D(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex)
            : this(new Vector3(firstVertex.x, firstVertex.y, 0.0f),
                new Vector3(secondVertex.x, secondVertex.y, 0.0f),
                new Vector3(thirdVertex.x, thirdVertex.y, 0.0f),
                new Vector3(fourthVertex.x, fourthVertex.y, 0.0f))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Face3D</c> class.
        /// </summary>
        /// <param name="firstVertex">Face3D <see cref="Vector3">first vertex</see>.</param>
        /// <param name="secondVertex">Face3D <see cref="Vector3">second vertex</see>.</param>
        /// <param name="thirdVertex">Face3D <see cref="Vector3">third vertex</see>.</param>
        public Face3D(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex)
            : this(firstVertex, secondVertex, thirdVertex, thirdVertex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Face3D</c> class.
        /// </summary>
        /// <param name="firstVertex">Face3D <see cref="Vector3">first vertex</see>.</param>
        /// <param name="secondVertex">Face3D <see cref="Vector3">second vertex</see>.</param>
        /// <param name="thirdVertex">Face3D <see cref="Vector3">third vertex</see>.</param>
        /// <param name="fourthVertex">Face3D <see cref="Vector3">fourth vertex</see>.</param>
        public Face3D(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex, Vector3 fourthVertex)
            : base(EntityType.Face3D, DxfObjectCode.Face3d)
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            this.edgeFlags = Face3DEdgeFlags.None;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first Face3D <see cref="Vector3">vertex</see>.
        /// </summary>
        public Vector3 FirstVertex
        {
            get { return this.firstVertex; }
            set { this.firstVertex = value; }
        }

        /// <summary>
        /// Gets or sets the second Face3D <see cref="Vector3">vertex</see>.
        /// </summary>
        public Vector3 SecondVertex
        {
            get { return this.secondVertex; }
            set { this.secondVertex = value; }
        }

        /// <summary>
        /// Gets or sets the third Face3D <see cref="Vector3">vertex</see>.
        /// </summary>
        public Vector3 ThirdVertex
        {
            get { return this.thirdVertex; }
            set { this.thirdVertex = value; }
        }

        /// <summary>
        /// Gets or sets the fourth Face3D <see cref="Vector3">vertex</see>.
        /// </summary>
        public Vector3 FourthVertex
        {
            get { return this.fourthVertex; }
            set { this.fourthVertex = value; }
        }

        /// <summary>
        /// Gets or sets the Face3D edge visibility.
        /// </summary>
        public Face3DEdgeFlags EdgeFlags
        {
            get { return this.edgeFlags; }
            set { this.edgeFlags = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Face3D that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Face3D that is a copy of this instance.</returns>
        public override object Clone()
        {
            Face3D entity = new Face3D
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
                //Face3d properties
                FirstVertex = this.firstVertex,
                SecondVertex = this.secondVertex,
                ThirdVertex = this.thirdVertex,
                FourthVertex = this.fourthVertex,
                EdgeFlags = this.edgeFlags
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
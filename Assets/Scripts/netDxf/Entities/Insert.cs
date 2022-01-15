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
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Math;
using netDxf.Tables;
using netDxf.Units;
using UnityEngine;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a block insertion <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Insert :
        EntityObject
    {
        #region delegates and events

        //public delegate void BlockChangedEventHandler(Insert sender, TableObjectChangedEventArgs<Block> e);
        //public event BlockChangedEventHandler BlockChanged;
        //protected virtual Block OnBlockChangedEvent(Block oldBlock, Block newBlock)
        //{
        //    BlockChangedEventHandler ae = this.BlockChanged;
        //    if (ae != null)
        //    {
        //        TableObjectChangedEventArgs<Block> eventArgs = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
        //        ae(this, eventArgs);
        //        return eventArgs.NewValue;
        //    }
        //    return newBlock;
        //}

        public delegate void AttributeAddedEventHandler(Insert sender, AttributeChangeEventArgs e);
        public event AttributeAddedEventHandler AttributeAdded;
        protected virtual void OnAttributeAddedEvent(Attribute item)
        {
            AttributeAddedEventHandler ae = this.AttributeAdded;
            if (ae != null)
            {
                ae(this, new AttributeChangeEventArgs(item));
            }
        }

        public delegate void AttributeRemovedEventHandler(Insert sender, AttributeChangeEventArgs e);
        public event AttributeRemovedEventHandler AttributeRemoved;
        protected virtual void OnAttributeRemovedEvent(Attribute item)
        {
            AttributeRemovedEventHandler ae = this.AttributeRemoved;
            if (ae != null)
            {
                ae(this, new AttributeChangeEventArgs(item));
            }
        }

        #endregion

        #region private fields

        private readonly EndSequence endSequence;
        private Block block;
        private Vector3 position;
        private Vector3 scale;
        private float rotation;
        private AttributeCollection attributes;

        #endregion

        #region constructors

        internal Insert(List<Attribute> attributes)
            : base(EntityType.Insert, DxfObjectCode.Insert)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            this.attributes = new AttributeCollection(attributes);
            foreach (Attribute att in this.attributes)
            {
                if (att.Owner != null)
                {
                    throw new ArgumentException("The attributes list contains at least an attribute that already has an owner.", nameof(attributes));
                }
                att.Owner = this;
            }

            this.block = null;
            this.position = Vector3.zero;
            this.scale = Vector3.one;
            this.rotation = 0.0f;
            this.endSequence = new EndSequence
            {
                Owner = this
            };
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert <see cref="Block">block definition</see>.</param>
        public Insert(Block block)
            : this(block, Vector3.zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert block definition.</param>
        /// <param name="position">Insert <see cref="Vector2">position</see> in world coordinates.</param>
        public Insert(Block block, Vector2 position)
            : this(block, new Vector3(position.x, position.y, 0.0f))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert block definition.</param>
        /// <param name="position">Insert <see cref="Vector3">point</see> in world coordinates.</param>
        public Insert(Block block, Vector3 position)
            : base(EntityType.Insert, DxfObjectCode.Insert)
        {  
            this.block = block ?? throw new ArgumentNullException(nameof(block));
            this.position = position;
            this.scale = Vector3.one;
            this.rotation = 0.0f;
            this.endSequence = new EndSequence
            {
                Owner = this
            };

            List<Attribute> atts = new List<Attribute>(block.AttributeDefinitions.Count);
            foreach (AttributeDefinition attdef in block.AttributeDefinitions.Values)
            {
                Attribute att = new Attribute(attdef)
                {
                    Position = attdef.Position + this.position - this.block.Origin,
                    Owner = this
                };
                atts.Add(att);
            }

            this.attributes = new AttributeCollection(atts);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the default drawing units to obtain the Insert transformation matrix, when the current Insert entity does not belong to a DXF document.
        /// </summary>
        public static DrawingUnits DefaultInsUnits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the insert list of <see cref="Attribute">attributes</see>.
        /// </summary>
        public AttributeCollection Attributes
        {
            get { return this.attributes; }
        }

        /// <summary>
        /// Gets the insert <see cref="Block">block definition</see>.
        /// </summary>
        /// <remarks>
        /// When changing the block associated to 
        /// </remarks>
        public Block Block
        {
            get { return this.block; }
            internal set
            {
                //if (value == null)
                //{
                //    throw new ArgumentNullException(nameof(value));
                //}

                //if (value.IsForInternalUseOnly)
                //{
                //    throw new ArgumentException("The block is for internal use only.");
                //}

                //this.block = this.OnBlockChangedEvent(this.block, value);

                //// remove all attributes in the actual insert
                //foreach (Attribute att in this.attributes)
                //{
                //    this.OnAttributeRemovedEvent(att);
                //    att.Handle = null;
                //    att.Owner = null;
                //}
                //this.attributes = new AttributeCollection();

                this.block = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the insert <see cref="Vector3">scale</see>.
        /// </summary>
        /// <remarks>None of the vector scale components can be zero.</remarks>
        public Vector3 Scale
        {
            get { return this.scale; }
            set
            {
                if (Mathd.IsZero(value.x) || Mathd.IsZero(value.y) || Mathd.IsZero(value.z))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "None of the vector scale components can be zero.");
                }

                this.scale = value;
            }
        }

        /// <summary>
        /// Gets or sets the insert rotation along the normal vector in degrees.
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = Mathd.NormalizeAngle(value); }
        }

        #endregion

        #region internal properties

        internal EndSequence EndSequence
        {
            get { return this.endSequence; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates the attribute list of the actual insert with the attribute definition list of the referenced block. This does not affect any value assigned to the Value property.
        /// </summary>
        /// <remarks>
        /// This method will automatically call the TransformAttributes method, to keep all attributes position and orientation up to date.<br />
        /// This method will does not change the values assigned to attributes in the actual insert, besides the ones modified by the TransformAttributes() method;
        /// position, normal, rotation, text height, width factor, oblique angle, is backwards, is upside down, and alignment values.
        /// </remarks>
        public void Sync()
        {
            List<Attribute> atts = new List<Attribute>();

            // remove all attributes that have no attribute definition in the block
            foreach (Attribute att in this.attributes)
            {
                string tag = att.Tag;
                if (this.block.AttributeDefinitions.ContainsTag(tag))
                {
                    atts.Add(att);
                }
                else
                {
                    this.OnAttributeRemovedEvent(att);
                    att.Handle = null;
                    att.Owner = null;
                }
            }

            // add any new attributes from the attribute definitions of the block
            foreach (AttributeDefinition attdef in this.block.AttributeDefinitions.Values)
            {
                if (this.attributes.AttributeWithTag(attdef.Tag) == null)
                {
                    Attribute att = new Attribute(attdef)
                    {
                        Owner = this
                    };

                    atts.Add(att);
                    this.OnAttributeAddedEvent(att);
                }
            }
            this.attributes = new AttributeCollection(atts);
        }

        /// <summary>
        /// Explodes the current insert.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<EntityObject> Explode()
        {
            List<EntityObject> entities = new List<EntityObject>();
            Vector3 translation = this.Position - this.block.Origin;

            foreach (EntityObject entity in this.block.Entities)
            {
                Vector3 localScale = this.Scale;
                bool isUniformScale = Mathd.IsEqual(localScale.x, localScale.y);

                // entities with reactors are associated with other entities they will handle the transformation
                if (entity.Reactors.Count > 0)
                {
                    continue;
                }

                if(!isUniformScale)
                {
                    switch (entity.Type)
                    {
                        case EntityType.Circle:
                        {
                            Circle circle = (Circle) entity;
                            Ellipse ellipse = new Ellipse
                            {
                                //EntityObject properties
                                Layer = (Layer) entity.Layer.Clone(),
                                Linetype = (Linetype) entity.Linetype.Clone(),
                                Color = (AciColor) entity.Color.Clone(),
                                Lineweight = entity.Lineweight,
                                Transparency = (Transparency) entity.Transparency.Clone(),
                                LinetypeScale = entity.LinetypeScale,
                                Normal = entity.Normal,
                                IsVisible = entity.IsVisible,
                                //Ellipse properties
                                Center = circle.Center,
                                MajorAxis = 2 * circle.Radius,
                                MinorAxis = 2 * circle.Radius,
                                Thickness = circle.Thickness
                            };

                            entities.Add(ellipse);
                            break;
                        }
                        case EntityType.Arc:
                        {
                            Arc arc = (Arc) entity;
                            Ellipse ellipse = new Ellipse
                            {
                                //EntityObject properties
                                Layer = (Layer) entity.Layer.Clone(),
                                Linetype = (Linetype) entity.Linetype.Clone(),
                                Color = (AciColor) entity.Color.Clone(),
                                Lineweight = entity.Lineweight,
                                Transparency = (Transparency) entity.Transparency.Clone(),
                                LinetypeScale = entity.LinetypeScale,
                                Normal = entity.Normal,
                                IsVisible = entity.IsVisible,
                                //Ellipse properties
                                Center = arc.Center,
                                MajorAxis = 2 * arc.Radius,
                                MinorAxis = 2 * arc.Radius,
                                StartAngle = arc.StartAngle,
                                EndAngle = arc.EndAngle,
                                Thickness = arc.Thickness
                            };

                            entities.Add(ellipse);
                            break;
                        }
                        case EntityType.Polyline2D:
                        {
                            List<EntityObject> newEntities = ((Polyline2D) entity).Explode();
                            foreach (EntityObject newEntity in newEntities)
                            {
                                if (newEntity.Type == EntityType.Arc)
                                {
                                    Arc arc = (Arc) newEntity;
                                    Ellipse ellipse = new Ellipse
                                    {
                                        //EntityObject properties
                                        Layer = (Layer) entity.Layer.Clone(),
                                        Linetype = (Linetype) entity.Linetype.Clone(),
                                        Color = (AciColor) entity.Color.Clone(),
                                        Lineweight = entity.Lineweight,
                                        Transparency = (Transparency) entity.Transparency.Clone(),
                                        LinetypeScale = entity.LinetypeScale,
                                        Normal = entity.Normal,
                                        IsVisible = entity.IsVisible,
                                        //Ellipse properties
                                        Center = arc.Center,
                                        MajorAxis = 2 * arc.Radius,
                                        MinorAxis = 2 * arc.Radius,
                                        StartAngle = arc.StartAngle,
                                        EndAngle = arc.EndAngle,
                                        Thickness = arc.Thickness
                                    };

                                    entities.Add(ellipse);
                                }
                                else
                                {
                                    entities.Add(newEntity);
                                }                                
                            }
                            break;
                        }
                        case EntityType.MLine:
                        {
                            List<EntityObject> newEntities = ((MLine)entity).Explode();
                            foreach (EntityObject newEntity in newEntities)
                            {
                                if (newEntity.Type == EntityType.Arc)
                                {
                                    Arc arc = (Arc) newEntity;
                                    Ellipse ellipse = new Ellipse
                                    {
                                        //EntityObject properties
                                        Layer = (Layer) entity.Layer.Clone(),
                                        Linetype = (Linetype) entity.Linetype.Clone(),
                                        Color = (AciColor) entity.Color.Clone(),
                                        Lineweight = entity.Lineweight,
                                        Transparency = (Transparency) entity.Transparency.Clone(),
                                        LinetypeScale = entity.LinetypeScale,
                                        Normal = entity.Normal,
                                        IsVisible = entity.IsVisible,
                                        //Ellipse properties
                                        Center = arc.Center,
                                        MajorAxis = 2 * arc.Radius,
                                        MinorAxis = 2 * arc.Radius,
                                        StartAngle = arc.StartAngle,
                                        EndAngle = arc.EndAngle,
                                        Thickness = arc.Thickness
                                    };

                                    entities.Add(ellipse);
                                }
                                else
                                {
                                    entities.Add(newEntity);
                                }                             
                            }
                            break;
                        }
                        default:
                        {
                            EntityObject newEntity = (EntityObject) entity.Clone();
                            entities.Add(newEntity);
                            break;
                        }
                    }
                }
                else
                {
                    EntityObject newEntity = (EntityObject) entity.Clone();
                    entities.Add(newEntity);
                }
            }

            foreach (Attribute attribute in this.attributes)
            {
                // the attributes will be exploded as a Text entity
                Text text = new Text
                {
                    //Attribute properties
                    Layer = (Layer) attribute.Layer.Clone(),
                    Linetype = (Linetype) attribute.Linetype.Clone(),
                    Color = (AciColor) attribute.Color.Clone(),
                    Lineweight = attribute.Lineweight,
                    Transparency = (Transparency) attribute.Transparency.Clone(),
                    LinetypeScale = attribute.LinetypeScale,
                    Normal = attribute.Normal,
                    IsVisible = attribute.IsVisible,
                    Height = attribute.Height,
                    WidthFactor = attribute.WidthFactor,
                    ObliqueAngle = attribute.ObliqueAngle,
                    Value = attribute.Value,
                    Style = (TextStyle) attribute.Style.Clone(),
                    Position = attribute.Position,
                    Rotation = attribute.Rotation,
                    Alignment = attribute.Alignment,
                    IsBackward = attribute.IsBackward,
                    IsUpsideDown = attribute.IsUpsideDown
                };
                entities.Add(text);
            }

            return entities;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Assigns a handle to the object based in a integer counter.
        /// </summary>
        /// <param name="entityNumber">Number to assign.</param>
        /// <returns>Next available entity number.</returns>
        /// <remarks>
        /// Some objects might consume more than one, is, for example, the case of polylines that will assign
        /// automatically a handle to its vertexes. The entity number will be converted to an hexadecimal number.
        /// </remarks>
        internal override long AssignHandle(long entityNumber)
        {
            entityNumber = this.endSequence.AssignHandle(entityNumber);
            foreach (Attribute attrib in this.attributes)
            {
                entityNumber = attrib.AssignHandle(entityNumber);
            }
            return base.AssignHandle(entityNumber);
        }


        /// <summary>
        /// Creates a new Insert that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Insert that is a copy of this instance.</returns>
        public override object Clone()
        {
            // copy attributes
            List<Attribute> copyAttributes = new List<Attribute>();
            foreach (Attribute att in this.attributes)
                copyAttributes.Add((Attribute)att.Clone());

            Insert entity = new Insert(copyAttributes)
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
                //Insert properties
                Position = this.position,
                Block = (Block) this.block.Clone(),
                Scale = this.scale,
                Rotation = this.rotation,
            };

            // copy extended data
            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}
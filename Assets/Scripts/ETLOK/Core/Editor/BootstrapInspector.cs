using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using netDxf;
using netDxf.Entities;

namespace ETLOK.Core.Config
{
    
    [CustomEditor(typeof(Bootstrap))]
    public class BootstrapInspector : Editor
    {
        private SerializedProperty _doc;

        private bool HeaderVariablesFoldout = false;
        private bool EntitiesFoldout = true;
        private bool LayersFoldout = true;
        private bool linesFoldout = false;

        private void OnEnable()
        {
            _doc = serializedObject.FindProperty("dxfDoc");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Bootstrap bootstrap = target as Bootstrap;
            
            if (_doc.managedReferenceValue == null)
            {
                EditorGUILayout.LabelField("DXF File is null");
                return;
            }

            DxfDocument doc = (DxfDocument)_doc.managedReferenceValue;

            SerializeHeaderVariables(doc);
            SerializeLayers(doc);
            SerializeEntities(doc);
        }

        private void SerializeLayers(DxfDocument doc)
        {
            LayersFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(LayersFoldout, "Layers");

            if (LayersFoldout)
            {
                EditorGUI.indentLevel++;

                var layers = doc.Layers;
                foreach (var layer in layers)
                {
                    ESU.Label("Name", layer.Name);
                    EditorGUI.indentLevel++;
                    ESU.Color("Color", layer.Color.ToColor());
                    ESU.Check("Visibility", layer.IsVisible);
                    ESU.Check("Frozen", layer.IsFrozen);
                    ESU.Check("Locked", layer.IsLocked);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void SerializeHeaderVariables(DxfDocument doc)
        {
            HeaderVariablesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(HeaderVariablesFoldout, "Header Variables");

            if (HeaderVariablesFoldout)
            {
                EditorGUI.indentLevel++;
                var drawingVars = doc.DrawingVariables;
                
                ESU.Label("Autocad Version", drawingVars.AcadVer);
                ESU.Label("Last Modified", drawingVars.TduUpdate);
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void SerializeEntities(DxfDocument doc)
        {
            EntitiesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EntitiesFoldout, "Entities");

            if (EntitiesFoldout)
            {
                EditorGUI.indentLevel++;
                var entities = doc.Entities;

                ESU.Label("Active Layout", entities.ActiveLayout);

                SerializeLineEntities(doc);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void SerializeLineEntities(DxfDocument doc)
        {
            linesFoldout = EditorGUILayout.Foldout(linesFoldout, "Lines");
            if (linesFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (Line obj in doc.Entities.Lines)
                {
                    ESU.Label("Linetype", obj.Linetype.Name);

                    SerializeColor(obj);

                    EditorGUI.indentLevel++;
                    ESU.V3("Start Point", obj.StartPoint);
                    ESU.V3("End Point", obj.EndPoint);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        private void SerializeColor(EntityObject obj)
        {
            if (obj.Color.IsByLayer)
            {
                ESU.Color("Color", obj.Layer.Color.ToColor());
            }
            else if (obj.Color.IsByBlock)
            {
                ESU.Label("Color", obj.Color);
            }
            else
            {
                ESU.Color("Color", obj.Color.ToColor());
            }
        }
    }
    
}


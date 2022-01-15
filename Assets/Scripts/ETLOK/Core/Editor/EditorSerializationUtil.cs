using UnityEngine;
using UnityEditor;

namespace ETLOK.Core.Config
{
    public static class ESU
    {
        public static void Label(string t, object s)
        {
            EditorGUILayout.LabelField(t + ": " + s.ToString());
        }

        public static bool Button(string s)
        {
            return GUILayout.Button(s);
        }

        public static Vector3 V3(string s, Vector3 v)
        {
            return EditorGUILayout.Vector3Field(s, v);
        }

        public static Color Color(string s, Color c)
        {
            return EditorGUILayout.ColorField(s, c);
        }

        public static bool Check(string s, bool t)
        {
            return EditorGUILayout.Toggle(s, t);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace DotsClone
{
    [CustomEditor(typeof(Dot))]
    public class DotInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var dot = target as Dot;

            EditorGUILayout.LabelField("Type", dot.dotType.ToString());
            EditorGUILayout.LabelField("Column", dot.coordinates.column.ToString());
            EditorGUILayout.LabelField("Row", dot.coordinates.row.ToString());
        }

    }
}
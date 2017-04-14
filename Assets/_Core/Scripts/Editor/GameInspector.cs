using UnityEditor;
using UnityEngine;

namespace DotsClone
{
    [CustomEditor(typeof(Game))]
    public class GameInspector : Editor
    {
        SerializedProperty useRandomSeed;
        SerializedProperty seed;
        SerializedProperty selectedTheme;

        private void OnEnable()
        {
            useRandomSeed = serializedObject.FindProperty("useRandomSeed");
            seed = serializedObject.FindProperty("seed");
            selectedTheme = serializedObject.FindProperty("selectedTheme");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(useRandomSeed);
            GUI.enabled = !useRandomSeed.boolValue;
            EditorGUILayout.PropertyField(seed);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(selectedTheme);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
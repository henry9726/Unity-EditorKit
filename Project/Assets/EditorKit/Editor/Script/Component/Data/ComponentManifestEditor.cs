using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.Component
{
    [CustomEditor(typeof(ComponentManifest))]
    public class ComponentManifestEditor : Editor
    {
        SerializedProperty scriptProp;
        SerializedProperty configProp;
        SerializedProperty nameProp;
        SerializedProperty descriptionProp;
        SerializedProperty authorProp;
        SerializedProperty versionProp;
        SerializedProperty readmePathProp;

        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("script");
            configProp = serializedObject.FindProperty("config");

            if (configProp != null)
            {
                nameProp = configProp.FindPropertyRelative("<Name>k__BackingField");
                descriptionProp = configProp.FindPropertyRelative("<Description>k__BackingField");
                authorProp = configProp.FindPropertyRelative("<Author>k__BackingField");
                versionProp = configProp.FindPropertyRelative("<Version>k__BackingField");
                readmePathProp = configProp.FindPropertyRelative("<ReadmePath>k__BackingField");

                // Fallback for non-auto-properties if backing fields are not found (though currently they are auto-props)
                if (nameProp == null) nameProp = configProp.FindPropertyRelative("Name");
                if (descriptionProp == null) descriptionProp = configProp.FindPropertyRelative("Description");
                if (authorProp == null) authorProp = configProp.FindPropertyRelative("Author");
                if (versionProp == null) versionProp = configProp.FindPropertyRelative("Version");
                if (readmePathProp == null) readmePathProp = configProp.FindPropertyRelative("ReadmePath");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("# Target Script", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(scriptProp);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("# Configuration", EditorStyles.boldLabel);

            if (nameProp != null) EditorGUILayout.PropertyField(nameProp, new GUIContent("DisplayName"));
            if (authorProp != null) EditorGUILayout.PropertyField(authorProp);
            if (versionProp != null) EditorGUILayout.PropertyField(versionProp);
            if (descriptionProp != null) EditorGUILayout.PropertyField(descriptionProp);
            if (readmePathProp != null) EditorGUILayout.PropertyField(readmePathProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

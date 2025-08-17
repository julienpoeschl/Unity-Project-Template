using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PackageListSO))]
public class PackageListEditor : Editor
{
    private PackageListSO editorTarget;
    private SerializedProperty packagesProp;

    void OnEnable()
    {
        editorTarget = (PackageListSO)target;
        packagesProp = serializedObject.FindProperty("packages");
    }

    void OnDisable()
    {
        serializedObject.Update();

        Debug.Log($"Disable {editorTarget.name}", this);
        for (int i = 0; i < packagesProp.arraySize; i++)
        {
            SerializedProperty element = packagesProp.GetArrayElementAtIndex(i);

            if (element == null)
            {
                Debug.LogError($"Serialized Property {element} is null.");
                continue;
            }

            SerializedProperty nameProp = element.FindPropertyRelative("name");

            if (nameProp == null)
            {
                Debug.LogError($"Serialized Property {nameProp} is null.");
                continue;
            }

            PackageChecker.CheckIfAvailable(nameProp.stringValue);
            PackageChecker.CheckIfInstalled(nameProp.stringValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Objects", EditorStyles.boldLabel);

        for (int i = 0; i < packagesProp.arraySize; i++)
        {
            SerializedProperty element = packagesProp.GetArrayElementAtIndex(i);

            SerializedProperty nameProp = element.FindPropertyRelative("name");
            SerializedProperty valueProp = element.FindPropertyRelative("version");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(nameProp, GUIContent.none, GUILayout.MinWidth(100));
            EditorGUILayout.PropertyField(valueProp, GUIContent.none, GUILayout.Width(80));
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                packagesProp.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Object"))
        {
            packagesProp.InsertArrayElementAtIndex(packagesProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

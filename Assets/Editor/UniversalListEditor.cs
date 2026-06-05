using BigBalls.Attributes;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObject), true)]
public class UniversalListEditor : Editor
{
    private List<FieldInfo> listsToDraw;

    private void OnEnable()
    {
        listsToDraw = new List<FieldInfo>();

        var type = target.GetType();

        var hasAttribute = type.GetCustomAttribute<CustomScriptableObjectListEditorAttribute>() != null;

        if (hasAttribute == false)
            return;

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.IsDefined(typeof(SerializeField), true) == false && field.IsPublic == false)
                continue; 

            var fieldType = field.FieldType;

            if (fieldType.IsGenericType &&
                fieldType.GetGenericTypeDefinition() == typeof(List<>) &&
                typeof(ScriptableObject).IsAssignableFrom(fieldType.GetGenericArguments()[0]))
            {
                listsToDraw.Add(field);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (listsToDraw == null || listsToDraw.Count == 0)
        {
            DrawDefaultInspector();
        }
        else
        {
            var property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                bool skip = false;

                foreach (var listField in listsToDraw)
                {
                    if (property.name == listField.Name)
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                    EditorGUILayout.PropertyField(property, true);
            }

            foreach (var listField in listsToDraw)
            {
                var listProp = serializedObject.FindProperty(listField.Name);
                if (listProp == null)
                    continue;

                EditorGUILayout.PropertyField(listProp, new GUIContent(listField.Name), true);

                for (int i = 0; i < listProp.arraySize; i++)
                {
                    var element = listProp.GetArrayElementAtIndex(i);
                    ScriptableObject obj = element.objectReferenceValue as ScriptableObject;

                    if (obj == null)
                    {
                        EditorGUILayout.HelpBox($"Ёлемент с индексом {i} равен null", MessageType.Warning);
                        continue;
                    }

                    Editor editor = Editor.CreateEditor(obj);

                    if (editor != null)
                    {
                        EditorGUILayout.BeginVertical("box");
                        editor.OnInspectorGUI();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

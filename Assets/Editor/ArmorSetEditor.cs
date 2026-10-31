using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArmorSet))]
public class ArmorSetEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();

        ArmorSet armorSet = (ArmorSet) target;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Включить всю броню"))
            SetArmorActive(armorSet, true);

        if (GUILayout.Button("Выключить всю броню"))
            SetArmorActive(armorSet, false);

        EditorGUILayout.EndHorizontal();

        // Опционально: переключатель текущего состояния
        EditorGUILayout.Space();
        if (GUILayout.Button("Переключить (Toggle)"))
        {
            bool anyActive = false;

            foreach (GameObject element in armorSet.ArmorElements)
            {
                if (element != null && element.activeSelf)
                {
                    anyActive = true;
                    break;
                }
            }
            SetArmorActive(armorSet, !anyActive);
        }
    }

    private void SetArmorActive (ArmorSet armorSet, bool state)
    {
        foreach (GameObject element in armorSet.ArmorElements)
        {
            if (element == null) continue;

            Undo.RecordObject(element, state ? "Enable Armor" : "Disable Armor");
            element.SetActive(state);
            EditorUtility.SetDirty(element);
        }

        // Помечаем сцену как изменённую
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(armorSet.gameObject.scene);
    }
}
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class ApplyMaterialChangesOnExitPlayMode3
{
    static ApplyMaterialChangesOnExitPlayMode3()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged1;
    }

    private static void OnPlayModeStateChanged1(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Применить изменения материалов
            MaterialReplace[] materialReplaces = Object.FindObjectsOfType<MaterialReplace>();
            foreach (MaterialReplace materialReplace in materialReplaces)
            {
                materialReplace.ApplyMaterialChanges();
                PrefabUtility.RecordPrefabInstancePropertyModifications(materialReplace); // Сохранить изменения в префабе
            }
        }
    }
}
#endif

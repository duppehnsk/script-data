using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ColliderMaster : MonoBehaviour
{
#if UNITY_EDITOR
    // Метод, который будет вызываться в редакторе
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // Находим все объекты с ColliderMaster и вызываем для них метод сохранения
        ColliderMaster[] colliderMasters = Object.FindObjectsOfType<ColliderMaster>();
        foreach (ColliderMaster colliderMaster in colliderMasters)
        {
            colliderMaster.SaveChanges();
        }
    }
#endif

    void Start()
    {
        if (!Application.isPlaying)
        {
            // Выходим, если мы находимся в режиме редактора
            return;
        }

        AddMeshCollidersToChildren(transform);
    }

    void AddMeshCollidersToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                Material material = meshRenderer.material;

                // Проверяем условия для материалов "trees" и "_bark"
                if (material.name.StartsWith("trees") && material.name.Substring("trees".Length).StartsWith("_bark"))
                {
                    // Добавляем Mesh Collider
                    AddMeshCollider(child.gameObject);
                }
                // Проверяем условия для материалов "water" и "decal"
                else if (material.name.StartsWith("water") && material.name.StartsWith("decal"))
                {
                    // Пропускаем объект, не добавляя Mesh Collider
                    Debug.Log("Skipped adding Mesh Collider to: " + child.name);
                }
                else if (!material.name.StartsWith("trees") && !material.name.StartsWith("water") && !material.name.StartsWith("decal") && !material.name.StartsWith("level"))
                {
                    // Если материал не начинается на "trees", "water", и "decal", также добавляем Mesh Collider
                    AddMeshCollider(child.gameObject);
                }
            }

            // Рекурсивно вызываем функцию для дочерних объектов
            AddMeshCollidersToChildren(child);
        }
    }

    void AddMeshCollider(GameObject obj)
    {
        // Проверяем, не существует ли уже Mesh Collider на объекте
        MeshCollider existingCollider = obj.GetComponent<MeshCollider>();
        if (existingCollider == null)
        {
            // Если нет Mesh Collider, добавляем его
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            // Дополнительные настройки Mesh Collider могут быть добавлены здесь

            Debug.Log("Added Mesh Collider to: " + obj.name);
        }
        else
        {
            // Если Mesh Collider уже есть, выводим предупреждение
            Debug.LogWarning("Mesh Collider already exists on: " + obj.name);
        }

#if UNITY_EDITOR
        // Помечаем объект как измененный в редакторе
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

#if UNITY_EDITOR
    // Метод для сохранения изменений в редакторе
    public void SaveChanges()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif
}
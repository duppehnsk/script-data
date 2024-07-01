using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceMaterial : MonoBehaviour
{
    public Material[] replacementMaterials; // Массив заменяемых материалов

    void Start()
    {
        ReplaceMaterialsRecursively(transform);
    }

    void ReplaceMaterialsRecursively(Transform parent)
    {
        // Обход всех дочерних объектов
        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();

            // Проверка на наличие компонента Renderer
            if (renderer != null)
            {
                // Получение текущего материала объекта
                Material currentMaterial = renderer.material;

                // Поиск совпадения в массиве replacementMaterials
                int replacementIndex = System.Array.IndexOf(replacementMaterials, currentMaterial);

                // Если совпадение найдено, заменить материал
                if (replacementIndex != -1)
                {
                    renderer.material = replacementMaterials[replacementIndex];
                }
            }

            // Рекурсивный вызов для дочерних объектов
            ReplaceMaterialsRecursively(child);
        }
    }
}

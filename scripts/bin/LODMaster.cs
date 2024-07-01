using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LODMaster : MonoBehaviour
{
    void Start()
    {
        // Проверяем, находимся ли мы в режиме редактирования
        if (!Application.isPlaying)
        {
            // Добавляем компонент LOD Group к текущему объекту или получаем существующий
            LODGroup lodGroup = GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                lodGroup = gameObject.AddComponent<LODGroup>();
            }

            // Настраиваем параметры LOD Group
            lodGroup.fadeMode = LODFadeMode.CrossFade;
            lodGroup.animateCrossFading = true;

            // Создаем массив для хранения LOD значения для каждого уровня
            LOD[] lods = new LOD[2]; // Два уровня LOD

            // Создаем LOD для LOD 0 (дочерние объекты)
            float lod0ScreenRelativeTransitionHeight = 0.43f; // Пример значения
            Renderer[] childRenderers = GetChildRenderers();
            lods[0] = new LOD(lod0ScreenRelativeTransitionHeight, childRenderers);

            // Создаем LOD для LOD 1 (родительский объект)
            float lod1ScreenRelativeTransitionHeight = 0.16f; // Указанное значение
            Renderer[] parentRenderer = { GetComponent<Renderer>() }; // Получаем Renderer для родительского объекта
            lods[1] = new LOD(lod1ScreenRelativeTransitionHeight, parentRenderer);

            // Присваиваем массив LOD объекту LOD Group
            lodGroup.SetLODs(lods);
        }
    }

    // Получить все Renderer для дочерних объектов
    Renderer[] GetChildRenderers()
    {
        Renderer[] childRenderers = new Renderer[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childRenderers[i] = transform.GetChild(i).GetComponent<Renderer>();
        }

        return childRenderers;
    }
}
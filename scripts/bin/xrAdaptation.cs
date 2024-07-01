using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine.Audio;

//для тех кто будет рыскать код, будьте добры, не меняйте имя на своё, уважайте труд разработчика
public class xrAdaptation : EditorWindow
{
    [MenuItem("X-Ray/Adaptation Level")]
    private static void OpenWindow()
    {
        xrAdaptation window = GetWindow<xrAdaptation>();
        window.titleContent = new GUIContent("Adaptation Level v0.2 by whonax");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("xrAdaptation v0.2 by whonax");
        GUILayout.Label("=================================");
        GUILayout.Label("Добавляет MeshCollider на объекты");
        if (GUILayout.Button("ColliderMaster"))
        {
            OpenColliderMaster();
        }
        GUILayout.Label("Заменяет материалы");
        if (GUILayout.Button("MaterialMaster"))
        {
            OpenMaterialMaster();
        }
        GUILayout.Label("Добавляет LOD систему");
        if (GUILayout.Button("LODMaster"))
        {
            OpenLODMaster();
        }
        GUILayout.Label("Добавление soundSRC объектов");
        if (GUILayout.Button("soundSRC Parse"))
        {
            OpenSoundSRCParse();
        }
        GUILayout.Label("=============================");
        if (GUILayout.Button("About"))
        {
            OpenAbout();
        }
    }
    private void OpenAbout()
    {
        AboutWindow newWindow = GetWindow<AboutWindow>();
        newWindow.titleContent = new GUIContent("About");
        newWindow.Show();
    }
    private void OpenSoundSRCParse()
    {
        SoundSRCParseWindow newWindow = GetWindow<SoundSRCParseWindow>();
        newWindow.titleContent = new GUIContent("soundSRCParse");
        newWindow.Show();
    }
    private void OpenColliderMaster()
    {
        ColliderMasterWindow newWindow = GetWindow<ColliderMasterWindow>();
        newWindow.titleContent = new GUIContent("ColliderMaster");
        newWindow.Show();
    }
    private static void OpenMaterialMaster()
    {
        MaterialMasterWindow window = GetWindow<MaterialMasterWindow>();
        window.titleContent = new GUIContent("Material Replacer");
        window.Show();
    }
    private static void OpenLODMaster()
    {
        LODMasterWindow window = GetWindow<LODMasterWindow>();
        window.titleContent = new GUIContent("LODMaster");
        window.Show();
    }

    
}
public class AboutWindow : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.Label("Данное деяние ускоряет процесс адаптации локаций сталкера в юнити");
        GUILayout.Label("Не стоит менять код и менять имя разработчика, цените его труд.");
        GUILayout.Label("Откуда это все пришло можно узнать по ссылке:");
        if (GUILayout.Button("Перейти в группу разработчика"))
        {
            Application.OpenURL("https://vk.com/stalkercsmobile");
        }
        GUILayout.Label("======================================================");
        GUILayout.Label("ChangeLog:");
        GUILayout.Label("--------------------");
        GUILayout.Label("v0.2");
        GUILayout.Label("+Парсинг soundSRC");
        GUILayout.Label("+Оптимизация");
        GUILayout.Label("--------------------");
        GUILayout.Label("v0.1");
        GUILayout.Label("+Замена материалов");
        GUILayout.Label("+Добавление LODMaster");
        GUILayout.Label("+Добавление ColliderMaster");
        GUILayout.Label("+Создание");
        GUILayout.Label("--------------------");
        GUILayout.Label("======================================================");
        GUILayout.Label("ver 0.2 build 1181 by whonax");
    }
}
public class SoundSRCParseWindow : EditorWindow
{
    private TextAsset documentTextAsset;
    private AudioMixerGroup _mixer;
    private string soundPath = "Assets/gamedata/sounds/";

    private void OnGUI()
    {
        GUILayout.Label("Добавляет soundSRC объекты на карту");
        soundPath = EditorGUILayout.TextField("Путь до папки к звукам", soundPath);
        _mixer = EditorGUILayout.ObjectField("Audio Mixer Group", _mixer, typeof(AudioMixerGroup), false) as AudioMixerGroup;

        documentTextAsset = EditorGUILayout.ObjectField("Document Text Asset", documentTextAsset, typeof(TextAsset), false) as TextAsset;

        if (GUILayout.Button("Parse"))
        {
            if (documentTextAsset != null)
            {
                ParseAndCreateObjects();
            }
            else
            {
                Debug.LogError("Please assign Document Text Asset.");
            }
        }
    }
    private List<Dictionary<string, string>> ParseDocument(string documentText)
    {
        List<Dictionary<string, string>> objects = new List<Dictionary<string, string>>();
        Dictionary<string, string> currentObject = null;

        string[] lines = documentText.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith("[object_"))
            {
                if (currentObject != null)
                {
                    objects.Add(currentObject);
                }
                currentObject = new Dictionary<string, string>();
            }
            else if (currentObject != null && line.Contains("="))
            {
                string[] keyValue = line.Trim().Split('=');
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();
                currentObject[key] = value;
            }
        }

        if (currentObject != null)
        {
            objects.Add(currentObject);
        }

        return objects;
    }

    private void ParseAndCreateObjects()
    {
        string documentText = documentTextAsset.text;
        List<Dictionary<string, string>> objects = ParseDocument(documentText);

        GameObject container = new GameObject("sound_src");
    
        foreach (Dictionary<string, string> objData in objects)
        {
            if (objData.ContainsKey("snd_name") && objData.ContainsKey("position"))
            {
                string sndName = objData["snd_name"];
                Vector3 position = ParseVector3(objData["position"]);

                GameObject emptyObject = new GameObject();
                emptyObject.name = sndName;
                emptyObject.transform.position = position;
                emptyObject.transform.parent = container.transform;

                if (objData.ContainsKey("max_dist") && objData.ContainsKey("min_dist"))
                {
                    float maxDist, minDist;

                    if (float.TryParse(objData["max_dist"], NumberStyles.Any, CultureInfo.InvariantCulture, out maxDist) &&
                        float.TryParse(objData["min_dist"], NumberStyles.Any, CultureInfo.InvariantCulture, out minDist))
                    {
                        AudioSource audioSource = emptyObject.AddComponent<AudioSource>();
                        audioSource.maxDistance = maxDist;
                        audioSource.minDistance = minDist;
                        audioSource.rolloffMode = AudioRolloffMode.Linear;
                        audioSource.outputAudioMixerGroup = _mixer;
                        audioSource.loop = true;
                        audioSource.spatialBlend = 1;
                        // Вы можете добавить код для выбора звукового файла аналогично предыдущему примеру.
                        AudioClip soundClip = AssetDatabase.LoadAssetAtPath<AudioClip>(soundPath + emptyObject.name+ ".ogg");
                        if (soundClip != null)
                            {
                                // Присвоение загруженного звукового файла компоненту AudioSource
                                audioSource.clip = soundClip;
                            }
                            else
                                {
                                Debug.LogError("Не удалось найти файл звука по пути: " + soundPath + emptyObject.name+ ".ogg");
                                }
                    }
                    else
                    {
                        Debug.LogError("Failed to parse max_dist or min_dist for object: " + sndName);
                    }
                }
            }
        }
    }
    private Vector3 ParseVector3(string vectorString)
    {
        string[] values = vectorString.Split(',');
        float x, y, z;

        if (float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
            float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
            float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out z))
        {
            if (x > 0 && z > 0)
                            {
                                x = -x;
                                z = -z;
                            }
                            else if (x < 0 && z > 0)
                            {
                                x = -x;
                                z = -z;
                            }
                            else if (x < 0 && z < 0)
                            {
                                x = -x;
                                z = -z;
                            }
                             else if (x > 0 && z < 0)
                            {
                                x = -x;
                                z = -z;
                            }

            return new Vector3(x, y, z);
        }
        else
        {
            Debug.LogError("Failed to parse vector: " + vectorString);
            return Vector3.zero; // or any other default value you want to use
        }
    }
    // Остальные методы без изменений.
}
public class ColliderMasterWindow : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.Label("Добавляет MeshCollider на объекты");
        GUILayout.Label("На декали, воду, листву и LOD идет пропуск.");
        if (GUILayout.Button("Добавить MeshCollider"))
        {
            AddMeshColliders();
        }
    }

    private static void AddMeshColliders()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (var selectedObject in selectedObjects)
        {
            AddMeshCollidersToChildren(selectedObject.transform);
        }

        Debug.Log("Mesh Colliders added to selected objects and their children.");
    }

    private static void AddMeshCollidersToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Проверяем, начинается ли материал объекта на "level", "decal" или "water"
            Renderer renderer = child.gameObject.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                string materialName = renderer.material.name;

                if (materialName.StartsWith("level") || materialName.StartsWith("decal") || materialName.StartsWith("water"))
                {
                    Debug.Log("Skipping object with material starting with 'level', 'decal', or 'water': " + child.gameObject.name);
                    continue;
                }

                // Проверяем, содержит ли материал "trees" и "bark" в его названии
                if (materialName.StartsWith("trees"))
                {
                    if (!materialName.Contains("_bark_"))
                    {
                        Debug.Log("Skipping object with material starting with 'trees' but not containing '_bark_': " + child.gameObject.name);
                        continue;
                    }
                }
            }

            // Если у объекта нет компонента Mesh Collider, добавляем его
            MeshCollider meshCollider = child.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                child.gameObject.AddComponent<MeshCollider>();
            }

            // Рекурсивно вызываем этот метод для всех дочерних объектов
            AddMeshCollidersToChildren(child);
        }
    }
}

public class MaterialMasterWindow : EditorWindow

{
    private ReorderableList materialsList;
    private SerializedObject serializedObject;

    private Vector2 scrollPosition = Vector2.zero;
    private bool showMaterialsList = false; // Переменная для отслеживания состояния списка
    private string _namelvl = "";

    public Material[] newMaterials;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);

        if (newMaterials == null)
        {
            newMaterials = new Material[0];
        }

        materialsList = new ReorderableList(serializedObject, serializedObject.FindProperty("newMaterials"), true, true, true, true);

        materialsList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Новые материалы");
        };

        materialsList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = materialsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Заменить материалы на объектах");
        _namelvl = EditorGUILayout.TextField("Префикс специального материала", _namelvl);
        serializedObject.Update();

        // Кнопка для показа/скрытия списка
        if (GUILayout.Button(showMaterialsList ? "Скрыть список" : "Показать список"))
        {
            showMaterialsList = !showMaterialsList;
        }

        if (showMaterialsList)
        {
            // Используйте EditorGUILayout.ScrollViewScope для области с прокруткой
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;

                // Отображаем список новых материалов в области с прокруткой
                materialsList.DoLayoutList();
            }
        }

        // Выбор нового материала
        if (GUILayout.Button("Добавить выбранные материалы"))
        {
            AddSelectedMaterials();
        }

        if (GUILayout.Button("Заменить материалы"))
        {
            ReplaceMaterials();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddSelectedMaterials()
    {
        Object[] selectedObjects = Selection.objects;

        foreach (var selectedObject in selectedObjects)
        {
            if (selectedObject is Material)
            {
                materialsList.serializedProperty.arraySize++;
                materialsList.serializedProperty.GetArrayElementAtIndex(materialsList.serializedProperty.arraySize - 1).objectReferenceValue = (Material)selectedObject;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

private void ReplaceMaterials()
{
    // Проходим по всем выбранным объектам
    foreach (var selectedObject in Selection.gameObjects)
    {
        // Проходим по всем дочерним и дочерним-дочерним объектам
        foreach (Transform child in selectedObject.transform)
        {
            ApplyMaterialChanges(child.gameObject);
        }
    }
}

private void ApplyMaterialChanges(GameObject gameObject)
{
    Renderer[] childRenderers = gameObject.GetComponentsInChildren<Renderer>();

    foreach (Renderer renderer in childRenderers)
    {
        Material[] materials = renderer.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            string old_mat_name = GetProcessedTextureName(materials[i].name);
            
            if (old_mat_name == "level_lods")
            {
                for (int j = 0; j < newMaterials.Length; j++)
                {
                    if (newMaterials[j].name == old_mat_name + "_" + _namelvl)
                    {
                        materials[i] = newMaterials[j];
                        break;
                    }
                }
            }

            for (int j = 0; j < newMaterials.Length; j++)
            {
                if (newMaterials[j].name == old_mat_name)
                {
                    materials[i] = newMaterials[j];
                    break;
                }
            }
        }

        renderer.sharedMaterials = materials;
    }
}
string GetProcessedTextureName(string materialName)
{
    string processedName = materialName.Replace(" ", "");
    string[] parts = processedName.Split('_');

    if (materialName.StartsWith("terrain"))
    {
        if (parts.Length >= 3)
        {
            string trimmedName = parts[0]; // Сохраняем первую часть
            trimmedName += "_" + parts[2]; // Добавляем третий элемент (после первого подчеркивания)

            // Проверяем последнюю часть, чтобы извлечь десятичную часть (если есть)
            if (parts.Length > 3)
            {
                string lastPart = parts[parts.Length - 1];
                int dotIndex = lastPart.IndexOf(".");
                if (dotIndex > 0)
                {
                    trimmedName += lastPart.Substring(dotIndex);
                }
            }

            return trimmedName;
        }
    }
    else if (parts.Length >= 3)
    {
        string trimmedName = parts[1];
        for (int i = 2; i < parts.Length; i++)
        {
            if (i == parts.Length - 1 && parts[i].Contains("."))
            {
                // Обрезаем десятичную часть
                int dotIndex = parts[i].IndexOf(".");
                if (dotIndex > 0)
                {
                    trimmedName += "_" + parts[i].Substring(0, dotIndex);
                }
            }
            else
            {
                trimmedName += "_" + parts[i];
            }
        }
        return trimmedName;
    }

    return processedName;
}
}

public class LODMasterWindow : EditorWindow
{
    private string lod0Transition = "43";
    private string lod1Transition = "16";
    
    private void OnGUI()
    {
        GUILayout.Label("Добавляет автоматически LOD на объекты");
        lod0Transition = EditorGUILayout.TextField("LOD0 Transition %", lod0Transition);
        lod1Transition = EditorGUILayout.TextField("LOD1 Transition %", lod1Transition);
        if (GUILayout.Button("Добавить LODSystem"))
        {
            AddLODMaster();
        }
    }

    void AddLODMaster()
    {
        // Проходим по всем объектам в сцене
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            // Проверяем, содержит ли имя объекта "lod" с последующими символами
            if (Regex.IsMatch(obj.name, @"lod(\.\d+)?"))
            {
                // Вызываем метод для обработки объекта с "lod" в названии
                ApplyLODMaterialChanges(obj);
            }

            // Рекурсивно вызываем метод для проверки дочерних объектов
            CheckChildObjectsForLOD(obj.transform);
        }
    }

    void CheckChildObjectsForLOD(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Проверяем, содержит ли имя дочернего объекта "lod" с последующими символами
            if (Regex.IsMatch(child.name, @"lod(\.\d+)?"))
            {
                // Вызываем метод для обработки дочернего объекта с "lod" в названии
                ApplyLODMaterialChanges(child.gameObject);
            }

            // Рекурсивно вызываем этот метод для всех дочерних объектов
            CheckChildObjectsForLOD(child);
        }
    }

    void ApplyLODMaterialChanges(GameObject lodObject)
    {
        // Добавляем компонент LOD Group к текущему объекту или получаем существующий
        LODGroup lodGroup = lodObject.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            lodGroup = lodObject.AddComponent<LODGroup>();
        }

        // Настраиваем параметры LOD Group
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;

        // Создаем массив для хранения LOD значения для каждого уровня
        LOD[] lods = new LOD[2]; // Два уровня LOD

        // Создаем LOD для LOD 0 (дочерние объекты)
        //float lod0ScreenRelativeTransitionHeight = 0.43f; // Пример значения
        float lod0ScreenRelativeTransitionHeight = float.Parse(lod0Transition)*0.01f;
        Renderer[] childRenderers = GetChildRenderers(lodObject.transform);
        lods[0] = new LOD(lod0ScreenRelativeTransitionHeight, childRenderers);

        // Создаем LOD для LOD 1 (родительский объект)
        //float lod1ScreenRelativeTransitionHeight = 0.16f; // Указанное значение
        float lod1ScreenRelativeTransitionHeight = float.Parse(lod1Transition)*0.01f;
        Renderer[] parentRenderer = { lodObject.GetComponent<Renderer>() }; // Получаем Renderer для родительского объекта
        lods[1] = new LOD(lod1ScreenRelativeTransitionHeight, parentRenderer);
        // Присваиваем массив LOD объекту LOD Group
        lodGroup.SetLODs(lods);
    }

    // Получить все Renderer для дочерних объектов
    Renderer[] GetChildRenderers(Transform parent)
    {
        Renderer[] childRenderers = new Renderer[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            childRenderers[i] = parent.GetChild(i).GetComponent<Renderer>();
        }

        return childRenderers;
    }
}
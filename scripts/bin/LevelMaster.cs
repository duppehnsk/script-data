using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Globalization;

using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

/// <summary>
[ExecuteInEditMode]
/// </summary>
public class LevelMaster : MonoBehaviour
{
    [Header("Мастер адаптации локации")]

    [Header("==========================")]
    [Header("Замена материалов")]
    public bool MaterialReplace = false;
    public string _namelvl = "undefined";
    public Material[] xrMaterials;

    [Header("==========================")]
    [Header("Создание LOD системы")]
    public bool GenerateLOD = false;
    [Header("==========================")]

    [Header("Парсинг SoundSRC объектов")]
    public bool ParseAndGenerateSoundSRC = false;
    public TextAsset documentTextAsset; // TextAsset containing the document
    public AudioMixerGroup mixer;
    public string soundPath = "Assets/gamedata/sounds/";

    [Header("==========================")]

    [Header("Генерация collider")]
    public bool GenerateColliders = false;

    // Start is called before the first frame update
    void Start()
    {  
        if(MaterialReplace == true)
        {
            StartMaterialReplace();
        }
        if(GenerateLOD == true)
        {
            GenerateLODGroup();
        }
        if(ParseAndGenerateSoundSRC == true)
        {
            ParseSoundSRC();
        }

        if(GenerateColliders == true)
        {
            GenerateCollide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateCollide()
    {
        GameObject parentObject1 = gameObject;

        // Проверка наличия родительского объекта
        if (parentObject1 != null)
        {
            // Вызываем метод поиска LOD объектов
            GenerateCollide2(parentObject1);
        }
        else
        {
            Debug.LogError("Родительский объект не найден.");
        }
    }

    void GenerateCollide2(GameObject parentObject1)
    {
        foreach (Transform child in parentObject1.transform)
        {
            // Проверяем, содержит ли имя объекта подстроку "lod"
            if (child.gameObject.name.StartsWith("sector_000"))
            {
                // Обработка найденного объекта
                //Debug.Log("Найден объект с именем: " + child.gameObject.name);
                AddMeshCollidersToChildren(child.gameObject);
            }

            // Рекурсивный вызов для поиска внутренних дочерних объектов
            FindLODObjectsInParent(child.gameObject);
        }
    }

    void AddMeshCollidersToChildren(GameObject parentObject1)
    {
        MeshCollider meshCollider = null;
        string temp1 = "trees";
        foreach (Transform child in parentObject1.transform)
        {
            // Добавляем меш-коллайдер к дочернему объекту
            if(child.gameObject.GetComponent<MeshRenderer>() != null)
            {
                string tempnamematmesh = child.gameObject.GetComponent<MeshRenderer>().material.name;
            string[] parts = tempnamematmesh.Split('_');

            if (tempnamematmesh.StartsWith(temp1))

    // Проверяем, что у нас есть как минимум три элемента
            if (parts.Length >= 3)
            {
        // Извлекаем второй элемент (индекс 1)
            string nameTREE = parts[1];

        // Проверяем, равен ли второй элемент "bark"
            if (nameTREE == "bark")
            {
                MeshCollider has = GetComponent<MeshCollider>();
                if(has != null)
                {
                    Destroy(has);
                }
                meshCollider = child.gameObject.AddComponent<MeshCollider>();
                Debug.Log("Добавлен MeshCollider для объекта с именем: " + child.gameObject.name);
            }
            }
            else
            {
            // В случае, если элементов меньше трех
            Debug.LogError("Недостаточно элементов в строке");
            }
        
        else
        {
        // Добавляем MeshCollider для всех объектов, чьи имена не начинаются с "trees"
        MeshCollider has1 = GetComponent<MeshCollider>();
        if(has1 != null)
        {
        Destroy(has1);
        }
        meshCollider = child.gameObject.AddComponent<MeshCollider>();
        Debug.Log("Добавлен MeshCollider для объекта с именем: " + child.gameObject.name);
        }

// Проверяем, был ли создан meshCollider, прежде чем добавить его



            //

            //
            // Настройки меш-коллайдера можно дополнительно настроить здесь

            // Рекурсивный вызов для поиска внутренних дочерних объектов
            AddMeshCollidersToChildren(child.gameObject);   
            }
            
        }
    }


    void ParseSoundSRC()
    {
    string documentText = documentTextAsset.text;
    List<Dictionary<string, string>> objects = ParseDocument(documentText);

    GameObject container = new GameObject("sound_src");

    CreateObjects(objects, container);
    }

    void GenerateLODGroup()
    {
        GameObject parentObject = gameObject;

        if (parentObject != null)
        {
            // Вызываем метод поиска LOD объектов
            FindLODObjectsInParent(parentObject);
        }

        else
        {
            Debug.LogError("Родительский объект не найден.");
        }
    }

    void FindLODObjectsInParent(GameObject parentObject)
    {
        // Проходим по всем дочерним объектам
        foreach (Transform child in parentObject.transform)
        {
            bool IsLODObject(GameObject obj)
            {
            string objName = child.gameObject.name.ToLower();
            return objName.StartsWith("lod") && Regex.IsMatch(objName.Substring(3), "^[0-9]+$");
            }
            // Проверяем, содержит ли имя объекта подстроку "lod"
            if (IsLODObject(child.gameObject))
            {
            
                // Обработка найденного объекта
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
            float lod0ScreenRelativeTransitionHeight = 0.31f; // Пример значения
            Renderer[] childRenderers = GetChildRenderers();
            lods[0] = new LOD(lod0ScreenRelativeTransitionHeight, childRenderers);

            // Создаем LOD для LOD 1 (родительский объект)
            float lod1ScreenRelativeTransitionHeight = 0.16f; // Указанное значение
            Renderer[] parentRenderer = { GetComponent<Renderer>() }; // Получаем Renderer для родительского объекта
            lods[1] = new LOD(lod1ScreenRelativeTransitionHeight, parentRenderer);

            // Присваиваем массив LOD объекту LOD Group
            lodGroup.SetLODs(lods);
            }

            // Рекурсивный вызов для поиска внутренних дочерних объектов
            FindLODObjectsInParent(child.gameObject);
        }
    }
    

    void StartMaterialReplace()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in childRenderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                string old_mat_name = GetProcessedTextureName(materials[i].name);
                if (old_mat_name == "level_lods")
                    {
                        for (int j = 0; j < xrMaterials.Length; j++)
                        {
                            if (xrMaterials[j].name == old_mat_name+"_"+_namelvl)
                                {
                                materials[i] = xrMaterials[j];
                                break;
                                }   
                        }
                    }
                for (int j = 0; j < xrMaterials.Length; j++)
                {
                    if (xrMaterials[j].name == old_mat_name)
                    {
                        materials[i] = xrMaterials[j];
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
    if(materialName.StartsWith("terrain"))
    {
        if (parts.Length >= 3)
        {
            string trimmedName = parts[0]; // Сохраняем первую часть
            trimmedName += "_" + parts[2]; // Добавляем третий элемент (после первого подчеркивания)

             // Проверяем последнюю часть, чтобы извлечь десятичную часть (если есть)
             if (parts.Length > 3) {
                string lastPart = parts[parts.Length - 1];
                int dotIndex = lastPart.IndexOf(".");
                if (dotIndex > 0) {
                    trimmedName += lastPart.Substring(dotIndex);
                    Debug.Log(trimmedName);
                }
            }

        return trimmedName;
    }
    }else
    if (parts.Length >= 3)
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

    private void OnDestroy()
    {
        if (!EditorApplication.isPlaying)
        {
            StartMaterialReplace();
        }
    }

    Renderer[] GetChildRenderers()
    {
        Renderer[] childRenderers = new Renderer[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childRenderers[i] = transform.GetChild(i).GetComponent<Renderer>();
        }

        return childRenderers;
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

    private void CreateObjects(List<Dictionary<string, string>> objects, GameObject container)
{
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
                    audioSource.outputAudioMixerGroup = mixer;
                    audioSource.loop = true;
                    audioSource.spatialBlend = 1;
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

}
public static class ApplyMaterialChangesOnExitPlayMode
{
    static ApplyMaterialChangesOnExitPlayMode()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class SpawnPartParser : MonoBehaviour
{
    [Header("Спавн объектов из spawn.part")]
    public TextAsset document;  // Ссылка на текстовый документ, который нужно спарсить
    public GameObject[] prefabs;

    private Dictionary<string, GameObject> createdObjects = new Dictionary<string, GameObject>();
    private Vector3 posobj;
    private Vector3 rotobj;

    void Start()
    {
        Debug.Log(prefabs[0].name);
        ParseDocument();
        
    }

    private void ParseDocument()
    {
        string[] lines = document.text.Split('\n');

        GameObject container = new GameObject("spawn_part");
        string currentObjectName = null; // Изменено имя переменной

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (line.StartsWith("[object_"))
            {
                // Извлекаем имя объекта из строки
                int startIndex = line.IndexOf("[object_") + 8;
                int endIndex = line.IndexOf("]");
                currentObjectName = line.Substring(startIndex, endIndex - startIndex);
            }
            
            else if (line.StartsWith("[") && line.EndsWith("]") && line.Contains("spawndata"))
            {
                // При встрече с новым объектом секции spawndata сбрасываем значение currentObjectName
                currentObjectName = null;
            }
            else if (line.StartsWith("name"))
            {
                string[] parts = line.Split('=');
                string objectName = parts[1].Trim();

                if (createdObjects.ContainsKey(objectName))
                {
                    Destroy(createdObjects[objectName]);
                    createdObjects.Remove(objectName);
                }
                else
                {
                    GameObject newObject = new GameObject(objectName);
                    newObject.transform.parent = container.transform;

                    createdObjects.Add(objectName, newObject);

                    // Поиск строки с координатами position
                    int positionIndex = -1;
                    int rotationIndex = -1;
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Trim().StartsWith("position"))
                        {
                            positionIndex = j;
                        }
                        else if (lines[j].Trim().StartsWith("rotation"))
                        {
                            rotationIndex = j;
                        }

                        if (positionIndex != -1 && rotationIndex != -1)
                        {
                            break;
                        }
                    }

                    // Если найдены строки с координатами position и rotation, извлекаем и применяем их к компонентам Transform
                    if (positionIndex != -1 && rotationIndex != -1)
                    {
                        string positionLine = lines[positionIndex].Trim();
                        string rotationLine = lines[rotationIndex].Trim();

                        string[] positionParts = positionLine.Split('=');
                        string[] rotationParts = rotationLine.Split('=');

                        string[] positionValues = positionParts[1].Trim().Split(',');
                        string[] rotationValues = rotationParts[1].Trim().Split(',');

                        // Извлечение и применение координат к компоненту Transform
                        if (positionValues.Length == 3 &&
                            float.TryParse(positionValues[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(positionValues[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                            float.TryParse(positionValues[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
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

                            newObject.transform.position = new Vector3(x, y, z);
                            posobj = new Vector3(x, y, z); //сохраняем позицию объекта для последующего спавна
                        }
                        else
                        {
                            Debug.LogWarning("Неправильный формат координат в строке position: " + positionLine);
                        }

                        // Извлечение и применение углов ротации к компоненту Transform
                        if (rotationValues.Length == 3 &&
                            float.TryParse(rotationValues[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float rotX) &&
                            float.TryParse(rotationValues[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float rotY) &&
                            float.TryParse(rotationValues[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float rotZ))
                        {
                            newObject.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                            rotobj = new Vector3(rotX, rotY, rotZ); //сохраняем ротацию объекта для последующего спавна
                        }
                        else
                        {
                            Debug.LogWarning("Неправильный формат углов ротации в строке rotation: " + rotationLine);
                        }
                    }
                }
            }
            else if (line.StartsWith("000027"))
            {
                // Получение значения строки "000027"
                string[] parts = line.Split('=');
                string value = parts[1].Trim();

                // Извлечение "название объекта" из значения строки "000027"
                
                string extractedValue = value.TrimStart('\"').TrimEnd('\"');
                string[] subparts = value.Split('\\');
                string extractedValue1 = subparts[subparts.Length - 1];
                string extractedValue2 = extractedValue1.TrimEnd('\"');
                Debug.Log(extractedValue2);
                //Debug.Log("Извлеченное значение: " + extractedValue1);
                GameObject prefab = GetPrefabByName(extractedValue2);
                if (prefab != null)
                {
                    GameObject spawnedObject = Instantiate(prefab);
                    spawnedObject.transform.position = posobj;
                    spawnedObject.transform.rotation = Quaternion.Euler(rotobj);
                    // Дополнительные настройки спавнутого объекта, если необходимо
                }
                else
                {
                     //Debug.LogWarning("Префаб с именем " + extractedValue1 + " не найден.");
                }
            }
            
        }
    }
    private GameObject GetPrefabByName(string name)
    {
        foreach (GameObject prefab in prefabs)
            {
                if (prefab.name == name)
                {
                return prefab;
                }
            }
        return null;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEditor;
using UnityEngine.Audio;

public class SoundSRCParser : MonoBehaviour
{
    public TextAsset documentTextAsset; // TextAsset containing the document
    public AudioMixerGroup mixer;
    private string soundPath = "Assets/gamedata/sounds/";

    void Start()
    {
    string documentText = documentTextAsset.text;
    List<Dictionary<string, string>> objects = ParseDocument(documentText);

    GameObject container = new GameObject("sound_src");

    CreateObjects(objects, container);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class MaterialReplace : MonoBehaviour
{
    public bool _enabled = true;
    public string _namelvl = "undefined";
    public Material[] xrMaterials;
    // Start is called before the first frame update
    private void Awake()
    {
        if(_enabled == true)
        {
        ApplyMaterialChanges();
        }
    }

    private void OnDestroy()
    {
        if (!EditorApplication.isPlaying)
        {
            ApplyMaterialChanges();
        }
    }

    public void  ApplyMaterialChanges()
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
}
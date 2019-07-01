using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UnityInternalEditor = UnityEditorInternal.InternalEditorUtility;

public class MVDUtils
{
    public static string[] RetrieveLayers()
    {
        List<string> layer_list = new List<string>();
        for (int i = 0; i <= UnityInternalEditor.layers.Length; i++) // There are 31 layers in unity, loop through them.
        {
            string layer_name = LayerMask.LayerToName(i);
            layer_list.Add(layer_name);
        }

        return layer_list.ToArray();
    }

    public static string[] RetrieveTags()
    {
        List<string> tag_list = new List<string>();
        for (int i = 0; i < UnityInternalEditor.tags.Length; i++)
        {
            tag_list.Add(UnityInternalEditor.tags[i]);
        }

        return tag_list.ToArray();
    }

    public static GameObject[] FindGameObjectsWithLayer(int layerIndex)
    {
        List<GameObject> finalObjects = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        for (var i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].layer == layerIndex)
                finalObjects.Add(allObjects[i]);
        }

        return finalObjects.ToArray();
    }

    public static List<PrefabInfo> LoadPrefabs()
    {
        List<PrefabInfo> prefabs = new List<PrefabInfo>();
        string[] prefabsGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in prefabsGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            prefabs.Add(new PrefabInfo(guid, assetPath));
        }

        return prefabs;
    }

    public static void ChangeAllMaterials(GameObject obj, Material mat)
    {
        Renderer rend = obj.GetComponent<Renderer>();

        if (rend)
            rend.sharedMaterial = mat;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;
            ChangeAllMaterials(child, mat);
        }
    }
}

public static class ExtensionMethods
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);

        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }

        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
}
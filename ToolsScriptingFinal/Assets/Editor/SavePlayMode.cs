using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SavePlayMode
{
    private static Dictionary<GameObject, GameObject> _changedGameObjects;

    // This method is called when class is initialized on launch
    static SavePlayMode()
    {
        _changedGameObjects = new Dictionary<GameObject, GameObject>();
        EditorApplication.playmodeStateChanged += OnChangePlayModeState;
    }

    // Update function on editor also
    static void Update()
    {
        Debug.Log("This print won't be shown");
        Debug.Log("We need ExecuteInEditMode to use update method within the editor");
    }

    // Event on play mode has changed
    static void OnChangePlayModeState()
    {
        if (!EditorApplication.isPlaying)
        {
            foreach(GameObject obj in _changedGameObjects.Keys)
                ReplaceComponents(obj, _changedGameObjects[obj]);

            _changedGameObjects.Clear();
            Debug.Log("editor status changed");
        }
    }

    [MenuItem("CONTEXT/Transform/Save gameobject")]
    static void SaveGameObject(MenuCommand cmd)
    {
        GameObject ctx_object = (cmd.context as Transform).gameObject;
        GameObject prefab = PrefabUtility.CreatePrefab("Assets/copy_" + ctx_object.name + ".prefab", ctx_object);

        _changedGameObjects.Add(ctx_object, prefab);
        Debug.Log("Object with name [" + ctx_object.name + "] saved to clipboard");
    }

    static void ReplaceComponents(GameObject original, GameObject prefab)
    {
        // Method 1, dirty but fast.
        GameObject ctx_copy_object = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        Vector3 local_pos = ctx_copy_object.transform.position;
        PrefabUtility.UnpackPrefabInstance(ctx_copy_object, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        ctx_copy_object.transform.SetParent(original.transform.parent.transform);
        ctx_copy_object.transform.name = original.name;
        ctx_copy_object.transform.SetSiblingIndex(original.transform.GetSiblingIndex());
        ctx_copy_object.transform.localPosition = local_pos;

        // Method 2, more elegant, but still need work to go.
        //GameObject ctx_copy_object = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        //ctx_copy_object.hideFlags = HideFlags.HideInHierarchy;
        //Component[] components = ctx_copy_object.GetComponents(typeof(Component));

        //// Loop through all the components and copy from prefab to gameobject
        //for (int i = 0; i < components.Length; i++)
        //{
        //    System.Type typed = components[i].GetType();
        //    Component copy_comp = original.GetComponent(typed);
        //    copy_comp.GetCopyOf(components[i]);

        //    if(typed.Name == "MeshFilter")
        //    {
        //        Debug.Log("meshfolter");
        //    }
        //}

        // Method 3, JSON Serialize.

        // Finally remove the temporal gameobjects and the prefab
        UnityEngine.Object.DestroyImmediate(original);
        string prefab_path = AssetDatabase.GetAssetPath(prefab);
        AssetDatabase.DeleteAsset(prefab_path);
    }
}

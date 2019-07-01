using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
    public int width = 8;
    public float offset = 0;
    public Vector2Int maxGridSize = new Vector2Int(16, 16);

    private List<GameObject> instances;
    private string prefabsPath = "Assets/External/3DGamekit/Prefabs/Environment";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Tile Sets")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MyWindow window = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
        window.title = "Custom tools";
        window.Show();
    }

    // Method used to 
    void OnGUI()
    {
        prefabsPath = EditorGUILayout.TextField("Prefabs path", prefabsPath);
        width = EditorGUILayout.IntField("Grid Width", width);
        offset = EditorGUILayout.FloatField("Grid Offset", offset);
        maxGridSize = EditorGUILayout.Vector2IntField("Maximum grid size", maxGridSize);

        if (GUILayout.Button("Build Tile sets"))
        {
            instances = new List<GameObject>();

            RetrievePrefabs(prefabsPath);
            SpawnPrefabs();
        }

        if (GUILayout.Button("Update shaders"))
        {
            instances = new List<GameObject>();

            RetrievePrefabs(prefabsPath);
            foreach (GameObject instance in instances)
            {

                UpdateShader(instance);
            }
        }
    }

    public void UpdateShader(GameObject instance)
    {
        Shader lightshader = Shader.Find("Lightweight Render Pipeline/Lit");
        MeshRenderer renderer = instance.GetComponent<MeshRenderer>();

        if (renderer && !renderer.sharedMaterial.shader.isSupported)
        {
            Debug.Log("Shader not supported!!");
            renderer.material.shader = lightshader;
        }
        else
        {
            for (int i = 0; i < instance.transform.childCount; i++)
                UpdateShader(instance.transform.GetChild(i).gameObject);
        } 
    }

    public void SpawnPrefabs()
    {
        int x_gridpos = 0, y_gridpos = 0;

        foreach (GameObject instance in instances)
        {
            // Get grid position
            float posx = x_gridpos * (maxGridSize.x + offset);
            float posy = y_gridpos * (maxGridSize.y + offset);

            // Update grid position
            x_gridpos = (x_gridpos + 1) % width;
            y_gridpos +=  x_gridpos % width == 0 ? 1 : 0;

            // Set instance position
            instance.transform.position = new Vector3(posx, 0, posy);
        }
    }

    public void RetrievePrefabs(string path)
    {
        string[] prefabs = AssetDatabase.FindAssets("", new[] { path });

        foreach (string assetGUID in prefabs)
        {
            string prefab_path = AssetDatabase.GUIDToAssetPath(assetGUID);

            if(AssetDatabase.IsValidFolder(prefab_path))
            {
                RetrievePrefabs(prefab_path);
                Debug.Log("Iterating through folder: " + prefab_path);
                continue;
            }

            Object prefab = AssetDatabase.LoadAssetAtPath(prefab_path, typeof(GameObject));
            GameObject new_tile = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Vector3 extents = GetExtents(new_tile);

            if (extents != Vector3.zero)
            {
                // Check its size, so that the piece is suitable for our purposes.
                if (extents.x < maxGridSize.x && extents.y < maxGridSize.y)
                {
                    instances.Add(new_tile);
                }
                else
                {
                    Debug.Log("Warning: " + new_tile.name + " extents bigger than the limit provided, prefab not instanced.");
                    DestroyImmediate(new_tile);
                }
            }
            else
            {
                Debug.Log("Warning: " + new_tile.name + " couldn't be placed at the scene, out of bounds.");
                DestroyImmediate(new_tile);
            }
        }
    }

    // Get the bounding box of the whole prefab
    public Vector3 GetExtents(GameObject new_tile)
    {
        Vector3 extents = Vector3.zero;
        MeshRenderer renderer = new_tile.GetComponent<MeshRenderer>();

        if (renderer)
            extents = new_tile.GetComponent<MeshRenderer>().bounds.extents;

        // We need to iterate through its children to know the exact size.
        for(int i = 0; i < new_tile.transform.childCount; i++)
        {
            Vector3 ext = GetExtents(new_tile.transform.GetChild(i).gameObject);

            if (ext.x > extents.x) extents.x = ext.x;
            if (ext.y > extents.y) extents.y = ext.y;
            if (ext.z > extents.z) extents.z = ext.z;
        }

        return extents;
    }
}
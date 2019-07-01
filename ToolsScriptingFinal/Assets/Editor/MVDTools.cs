using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Linq;

public class PanelMisc
{
    public int _tagIndex = 0;
    public int _layerIndex = 0;
    public int _namingIndex = 0;

    public string[] allTags;
    public string[] allLayers;
    public string[] namingSets;
    public string currentString;

    public void Init()
    {
        allTags = MVDUtils.RetrieveTags();
        allLayers = MVDUtils.RetrieveLayers();

        namingSets = new string[] { "Prefix", "Sufix", "Replace" };
    }
}

public class PanelPlacement
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    public Transform trans;
    public bool physicsOn;

    public List<PrefabInfo> prefabList;
    public Vector2 scroll;

    public float browserWidth = 128f;
    public float minSpacing = 1f;
    public float widthShift = 24f;

    public string search;
    public int currentLayer;
    public Dictionary<string, string> selectedGuids;

    public void Init()
    {
        prefabList = new List<PrefabInfo>();
        selectedGuids = new Dictionary<string, string>();

        prefabList = MVDUtils.LoadPrefabs();
    }
}

public struct PrefabInfo
{
    public string Guid;
    public string Path;

    public PrefabInfo(string guid, string path)
    {
        Guid = guid;
        Path = path;
    }
}

public class BRGEditor : EditorWindow
{
    // Editor version
    public static string version = "0.1a";

    public static GameObject rootEditor;

    // Misc variables
    private static PanelMisc miscValues;
    private static PanelPlacement placementValues;
    private static GUIStyle prefabElementStyle;

    // Panel toggles
    public bool showPanelMisc = true;
    public bool showPanelLight = true;
    public bool showPanelAI = false;

    public static bool inEditionMode = false;

    // Display textures
    public static Texture2D texture_logo;

    // Predefined sizes
    public Vector2 tx_logo_size = new Vector2(200, 75);

    // Predefined panel colors
    public Color colorPanelLight = new Color(153f / 255f, 153f / 255f, 153f / 255f);
    public Color colorPanelAI = new Color(153f / 255f, 153f / 153f, 153f / 255f);
    public Color colorPanelMisc = new Color(153f / 255f, 153f / 153f, 153f / 255f);

    // Contents
    public static GUIContent lightRemoveButton = new GUIContent("Remove All", "Removes all light colliders");
    public static GUIContent lightGenerateButton = new GUIContent("Generate All", "Removes all light colliders and regenerates all of them.");
    public static GUIContent raycastingToolOn = new GUIContent("Begin Edit Mode", "Starts raycasting mode tool in scene view.");
    public static GUIContent raycastingToolOff = new GUIContent("Stop Edit Mode", "Stops raycasting mode tool in scene view.");

    // Instantiate everything needed in this function
    [MenuItem("Tools/MVD Tools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BRGEditor window = (BRGEditor)EditorWindow.GetWindow(typeof(BRGEditor));
        window.Show();

        //texture_logo = Resources.Load("Editor/logo_lasalle") as Texture2D;
        inEditionMode = false;

        UpdateResources();
    }

    void OnDestroy() {
        if (rootEditor) {
            DestroyImmediate(rootEditor);
        }
    }

    static void UpdateResources()
    {
        // Initialize all sub variable classes
        miscValues = new PanelMisc();
        miscValues.Init();

        placementValues = new PanelPlacement();
        placementValues.Init();
    }

    void OnGUI()
    {
        LoadStyles();
        {
            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            //GUI.DrawTexture(new Rect(Screen.width * .5f - tx_logo_size.x * .5f, 15, tx_logo_size.x, tx_logo_size.y), texture_logo, ScaleMode.StretchToFill, true);
            //GUILayout.FlexibleSpace();
            //GUILayout.EndHorizontal();

            //GUILayout.Space(tx_logo_size.y + 10f);
            //DisplaySeparator(50);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Tools Version: " + version, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        // Display the given panels needed here.
        {
            DisplayPanel("Misc Tools", () => DisplayPanelMisc(), ref showPanelMisc, ref colorPanelMisc);
            DisplayPanel("Placement Tools", () => DisplayPanelPlacement(), ref showPanelLight, ref colorPanelLight);
            DisplayPanel("AI Tools", () => DisplayPanelAI(), ref showPanelAI, ref colorPanelAI);
        }
    }

    void DisplayPanelMisc()
    {
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Selection tools", EditorStyles.boldLabel);
            GUILayout.Space(3);

            // Determine selection configuration
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Tag selected   ", EditorStyles.label);
                miscValues._tagIndex = EditorGUILayout.Popup(miscValues._tagIndex, miscValues.allTags);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Layer selected", EditorStyles.label);
                miscValues._layerIndex = EditorGUILayout.Popup(miscValues._layerIndex, miscValues.allLayers);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Select"))
                {
                    CreateSelection(miscValues._layerIndex, miscValues._tagIndex);
                }
                if (GUILayout.Button("Unselect"))
                {
                    Selection.objects = new Object[0];
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Second 
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Naming tools", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Naming tools are selection dependent.", EditorStyles.helpBox);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);

            // Determine selection configuration
            GUILayout.BeginHorizontal();
            {
                miscValues.currentString = EditorGUILayout.TextField("Context string: ", miscValues.currentString);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Naming type", EditorStyles.label);
                miscValues._namingIndex = EditorGUILayout.Popup(miscValues._namingIndex, miscValues.namingSets);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Apply"))
                {
                    RenameObjects(miscValues.currentString, miscValues._namingIndex);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    static void DisplayPanelPlacement()
    {
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Misc settings", EditorStyles.boldLabel);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            placementValues.physicsOn = EditorGUILayout.Toggle("Enable physics", placementValues.physicsOn);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Layer filter  ", EditorStyles.label);
                placementValues.currentLayer = EditorGUILayout.Popup(placementValues.currentLayer, miscValues.allLayers);
            }
            GUILayout.EndHorizontal();

            placementValues.trans = (Transform)EditorGUILayout.ObjectField("Attach to parent", placementValues.trans, typeof(Transform));
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Transform settings", EditorStyles.boldLabel);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pos", EditorStyles.label);
            placementValues.position = EditorGUILayout.Vector3Field("", placementValues.position);
            GUILayout.Button("X");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rot", EditorStyles.label);
            placementValues.rotation = EditorGUILayout.Vector3Field("", placementValues.rotation);
            if (GUILayout.Button("R"))
            {
                placementValues.rotation = new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sct", EditorStyles.label);
            placementValues.scale = EditorGUILayout.Vector3Field("", placementValues.scale);
            if (GUILayout.Button("R"))
            {
                placementValues.scale = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        {
            bool someOption = true;

            GUILayout.Label("Prefab Browser", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            someOption = GUILayout.Toggle(someOption, "Filter search", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            placementValues.search = EditorGUILayout.TextField(GUIContent.none, placementValues.search, "ToolbarSeachTextField", GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button(GUIContent.none, string.IsNullOrEmpty(placementValues.search) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
            {
                placementValues.search = string.Empty;
                GUI.FocusControl("");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            {
                GUILayout.Space(300);
                GUILayout.BeginHorizontal();
                GUILayout.Space(300);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            {
                Rect trect = GUILayoutUtility.GetLastRect();
                Event current = Event.current;
                DrawPrefabList(trect, current);
            }
        }
        GUILayout.EndVertical();

        // Panel access
        GUILayout.BeginHorizontal();
        {
            if(!inEditionMode)
            {
                if (GUILayout.Button(raycastingToolOn, "Button"))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath(placementValues.selectedGuids.First().Value, typeof(UnityEngine.Object)) as GameObject;
                    rootEditor = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                    rootEditor.hideFlags = HideFlags.HideInHierarchy;
                    rootEditor.AddComponent<MeshRenderer>();
                    rootEditor.AddComponent<MeshFilter>();
                    rootEditor.AddComponent<ClickSpawn>();
                    rootEditor.GetComponent<ClickSpawn>().physics = placementValues.physicsOn;
                    rootEditor.GetComponent<ClickSpawn>().parent = placementValues.trans;
                    rootEditor.GetComponent<ClickSpawn>().scale = placementValues.scale;
                    rootEditor.GetComponent<ClickSpawn>().rotation = placementValues.rotation;
                    rootEditor.GetComponent<ClickSpawn>().position = placementValues.position;
                    rootEditor.GetComponent<ClickSpawn>().layerIndex = placementValues.currentLayer;
                    rootEditor.GetComponent<ClickSpawn>().prefab = obj;
                    rootEditor.layer = 12;

                    // Unpack prefab instance and change its materials.
                    PrefabUtility.UnpackPrefabInstance(rootEditor, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    MVDUtils.ChangeAllMaterials(rootEditor, Resources.Load("Materials/RootEditor") as Material);

                    inEditionMode = true;
                }
            }
            else
            {
                PlacementToolUpdate();

                GUIStyle ToggleButtonStyleToggled = new GUIStyle("Button");
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;

                if (GUILayout.Button(raycastingToolOff, ToggleButtonStyleToggled))
                {
                    DestroyImmediate(rootEditor);
                    inEditionMode = false;
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    void DisplayPanelAI()
    {

    }

    static void DisplaySeparator(int width)
    {
        string line = string.Empty;
        for (int i = 0; i < width; i++) line += "_";

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(line);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    static void DisplayPanel(string title, System.Action toexecute, ref bool foldout, ref Color color)
    {
        //GUI.color = color;
        EditorGUILayout.BeginVertical("Box");
        {
            GUI.color = Color.white;

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, title, foldout);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            if (foldout)
            {
                //GUILayout.BeginVertical("Box");
                toexecute();
                //GUILayout.EndVertical();
            }

        }
        GUILayout.EndVertical();
    }

    static void CreateSelection(int layerIndex, int tagindex)
    {
        GameObject[] layerSelection = MVDUtils.FindGameObjectsWithLayer(layerIndex);
        GameObject[] tagSelection = GameObject.FindGameObjectsWithTag(miscValues.allTags[tagindex]);
        GameObject[] listCommon = layerSelection.Intersect(tagSelection).ToArray();

        Selection.objects = listCommon;
    }

    static void PlacementToolUpdate()
    {
        // Update rooteditor if values were changed
        if(rootEditor)
        {
            rootEditor.transform.localScale = placementValues.scale;
            rootEditor.transform.rotation = Quaternion.Euler(placementValues.rotation);
        }
    }

    static void RenameObjects(string name, int type)
    {
        switch(type)
        {
            case 0:
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                        Selection.objects[i].name = name + Selection.objects[i].name;
                }
                break;
            case 1:
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                        Selection.objects[i].name = Selection.objects[i].name + name;
                }
                break;
            case 2:
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                        Selection.objects[i].name = name;
                }
                break;
        }
    }

    private static void LoadStyles()
    {
        if (prefabElementStyle == null)
        {
            prefabElementStyle = new GUIStyle(GUI.skin.FindStyle("ModuleTitle"));
            prefabElementStyle.imagePosition = ImagePosition.ImageOnly;
            prefabElementStyle.margin = new RectOffset(0, 0, 0, 0);
            prefabElementStyle.padding = new RectOffset(0, 0, 0, 0);
            prefabElementStyle.clipping = TextClipping.Clip;
            prefabElementStyle.contentOffset = Vector2.zero;
            prefabElementStyle.padding = new RectOffset(4, 4, 4, 4);
            prefabElementStyle.alignment = TextAnchor.MiddleCenter;
            prefabElementStyle.fixedHeight = 0;
            prefabElementStyle.border = new RectOffset(4, 4, 4, 4);
        }

    }

    private static void SetSelected(string guid, string path)
    {
        placementValues.selectedGuids.Clear();
        placementValues.selectedGuids.Add(guid, path);

        Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object));
    }

    public static GameObject SelectedPrefab
    {
        get
        {
            if (placementValues.selectedGuids.Count > 0)
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(placementValues.selectedGuids.First().Value), typeof(Object)) as GameObject;
            }
            return null;
        }
    }

    private static void DrawPrefabList(Rect prefabListRect, Event current)
    {
        int xCount = Mathf.FloorToInt((prefabListRect.width - placementValues.minSpacing) / (placementValues.browserWidth + placementValues.minSpacing));
        int yCount = Mathf.CeilToInt(placementValues.prefabList.Count / (float)xCount);

        float freeWidth = ((prefabListRect.width - placementValues.minSpacing) - 12) - (xCount * placementValues.browserWidth);
        float spacing = freeWidth / xCount;

        Rect contentRect = new Rect(prefabListRect.x, 0, prefabListRect.width - placementValues.widthShift, yCount * (placementValues.browserWidth + spacing + placementValues.widthShift) + spacing * 2);
        placementValues.scroll = GUI.BeginScrollView(prefabListRect, placementValues.scroll, contentRect);

        //disable the scrolling when dragging prefabs
        if (current.type == EventType.DragUpdated && DragAndDrop.GetGenericData("Prefab") != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            current.Use();
        }


        for (int row = 0; row < yCount; row++)
        {
            for (int x = 0; x < xCount; x++)
            {
                int i = row * xCount + x;

                if (i < placementValues.prefabList.Count)
                {
                    Rect elementPreviewRect = new Rect(prefabListRect.x + (placementValues.browserWidth + spacing) * x + 15, 
                        (placementValues.browserWidth + spacing + 15) * row + 15, placementValues.browserWidth, placementValues.browserWidth);

                    Rect fullElementRect = new Rect(elementPreviewRect.x, elementPreviewRect.y, elementPreviewRect.width, elementPreviewRect.height + 15);

                    if (fullElementRect.y <= prefabListRect.height + placementValues.scroll.y && fullElementRect.y + fullElementRect.height >= placementValues.scroll.y)
                    {
                        string assetPath = placementValues.prefabList[i].Path;
                        Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                        Texture2D preview = AssetPreview.GetAssetPreview(asset) ?? AssetPreview.GetMiniThumbnail(asset);
                        if (asset != null)
                        {
                            //events
                            if (fullElementRect.Contains(current.mousePosition))
                            {
                                if (current.type == EventType.MouseDown)
                                {
                                    GUI.FocusControl(placementValues.prefabList[i].Guid);
                                    SetSelected(placementValues.prefabList[i].Guid, placementValues.prefabList[i].Path);

                                    if (current.button == 0)
                                    {
                                        DragAndDrop.PrepareStartDrag();
                                        DragAndDrop.StartDrag("PrefabDrag");
                                        DragAndDrop.SetGenericData("Prefab", SelectedPrefab);
                                        DragAndDrop.objectReferences = new Object[0];
                                    }

                                    current.Use();
                                }
                                else if (current.type == EventType.ContextClick)
                                {
                                    GenericMenu contextMenu = new GenericMenu();
                                    //GeneratePrefabContextMenu(contextMenu, placementValues.prefabList[i]);
                                    current.Use();
                                }
                            }

                            bool selected = placementValues.selectedGuids.ContainsKey(placementValues.prefabList[i].Guid) ? true : false;
                            GUI.SetNextControlName(placementValues.prefabList[i].Guid);
                            GUI.Box(elementPreviewRect, new GUIContent(preview), prefabElementStyle);

                            EditorGUI.LabelField(
                                new Rect(elementPreviewRect.x, elementPreviewRect.y + placementValues.browserWidth + 3, placementValues.browserWidth, 18), 
                                new GUIContent(asset.name), selected ? "AssetLabel" : EditorStyles.label);

                            if (selected)
                            {
                                GUI.Box(new Rect(elementPreviewRect.x, elementPreviewRect.y, elementPreviewRect.width, elementPreviewRect.height - 2), GUIContent.none, "TL SelectionButton PreDropGlow");
                            }
                        }
                        else
                        {
                            GUI.Box(elementPreviewRect, new GUIContent("No Asset at: " + assetPath));
                        }
                    }
                }
            }
        }
        GUI.EndScrollView();

    }
}
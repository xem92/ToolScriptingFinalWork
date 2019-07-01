using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MenuSnippets
{
    [MenuItem("Assets/Load Additive Scene")]
    private static void LoadAdditiveScene()
    {
        var selected = Selection.activeObject;
        EditorApplication.OpenSceneAdditive(AssetDatabase.GetAssetPath(selected));
    }

    [MenuItem("Assets/ProcessTexture")]
    private static void NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
        //return Selection.activeObject.GetType() == typeof(Texture2D);
    }

}

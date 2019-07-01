using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyScript))]
public class InspectorTest : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Used to draw the default object values

        EditorGUILayout.HelpBox("This is a help box", MessageType.Info);
        EnemyScript myScript = (EnemyScript)target;

        if (GUILayout.Button("Build Object"))
        {

        }
    }
}
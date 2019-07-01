using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuItems : MonoBehaviour {
    [MenuItem("Tools/Rotate Right 45 _g")]
    private static void RotateRightOption() {
        GameObject obj = Selection.activeGameObject;
        obj.transform.Rotate(Vector3.up * 45);
    }
    [MenuItem("Tools/Rotate Left 45 _h")]
    private static void RotateLeftOption() {
        GameObject obj = Selection.activeGameObject;
        obj.transform.Rotate(Vector3.up * -45);
    }
    [MenuItem("Tools/Delete Selected _d")]
    private static void DeleteSelectedOption() {
        GameObject obj = Selection.activeGameObject;
        DestroyImmediate(obj);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Launcher))]
public class LauncherEditor : Editor
{
    [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
    static void DrawGizmosSelected(Launcher launcher, GizmoType gizmoType)
    {
        Vector3 offsetPosition = launcher.gunBody.transform.TransformPoint(launcher.offset);
        Handles.DrawDottedLine(launcher.gunBody.transform.position, offsetPosition, 3);
        Handles.Label(offsetPosition, "Offset");

        if (launcher.projectile != null)
        {
            float physicsStep = 0.1f;
            List<Vector3> positions = new List<Vector3>();

            Vector3 position = offsetPosition;
            Vector3 velocity = launcher.gunBody.transform.forward * launcher.velocity / launcher.projectile.mass;

            for (var i = 0f; i <= 1f; i += physicsStep)
            {
                positions.Add(position);
                position += velocity * physicsStep;
                velocity += Physics.gravity * physicsStep;
            }

            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.DrawAAPolyLine(positions.ToArray());
                Handles.Label(positions[positions.Count - 1], "Estimated Position (1 sec)");
                Gizmos.DrawWireSphere(positions[positions.Count - 1], 0.125f);
            }
        }
    }

    void OnSceneGUI()
    {
        Launcher launcher = target as Launcher;
        Transform transform = launcher.gunBody.transform;

        // Debug draw the offset handle and its state
        using (var cc = new EditorGUI.ChangeCheckScope())
        {
            Vector3 newOffset = transform.InverseTransformPoint(
            Handles.PositionHandle(transform.TransformPoint(launcher.offset), transform.rotation));

            if (cc.changed)
            {
                Undo.RecordObject(launcher, "Offset Change");
                launcher.offset = newOffset;
            }
        }

        // Buttons to access the methods needed from the launcher script
        Handles.BeginGUI();
        {
            Vector3 rectMin = Camera.current.WorldToScreenPoint(launcher.transform.position + launcher.offset);
            Rect rect = new Rect();
            rect.xMin = rectMin.x;
            rect.yMin = SceneView.currentDrawingSceneView.position.height - rectMin.y;
            rect.width = 64;
            rect.height = 18;

            GUILayout.BeginArea(rect);
            {
                using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
                {
                    if (GUILayout.Button("Fire"))
                        launcher.Fire();
                }
            }
            GUILayout.EndArea();
            rect.yMin = SceneView.currentDrawingSceneView.position.height - rectMin.y - 30;

            GUILayout.BeginArea(rect);
            {
                using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
                {
                    if (GUILayout.Button("Aim"))
                        launcher.Aim();
                }
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Used to draw the default object values
    }
}
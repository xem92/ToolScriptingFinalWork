using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    // Draw a wiere sphere based on the radius
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawGizmosSelected(Projectile projectile, GizmoType gizmoType)
    {
        Gizmos.DrawWireSphere(projectile.transform.position, projectile.damageRadius);
    }

    // Method that is always drawn on scene viewport
    void OnSceneGUI()
    {
        // Target method allow us to get the prefab gameobject reference
        Projectile projectile = (Projectile)target;
        Transform transform = projectile.transform;
        //projectile.damageRadius = Handles.RadiusHandle(transform.rotation, transform.position, projectile.damageRadius);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
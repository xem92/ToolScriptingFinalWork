using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ClickSpawn : MonoBehaviour
{
    public bool physics;
    public int layerIndex;

    public Vector3 scale;
    public Vector3 rotation;
    public Vector3 position;
    public Object prefab;
    public Transform parent;

    private float offsetDistance = 3f;

    private void OnEnable()
    {
        //Debug.Log("Started");
        if (!Application.isEditor)
        {
            Destroy(this);
        }

        Physics.autoSimulation = false;
        EditorApplication.update += Update;
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;

        // Disable the physics if they were enabled
        if(physics)
        {
            Physics.autoSimulation = true;
            EditorApplication.update -= Update;
        }
    }

    void Update()
    {
        if (physics)
            Physics.Simulate(Time.fixedDeltaTime);
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;
        Vector3 mousePos = e.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;
        mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
        mousePos.x *= ppp;

        Ray ray = scene.camera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 1 << layerIndex))
        {
            SceneView.lastActiveSceneView.Repaint();

            Vector3 final_pos = hit.point + hit.normal * offsetDistance;
            Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);
            Handles.SphereCap(0, final_pos, Quaternion.identity, 0.2f);
            Handles.DrawAAPolyLine(new Vector3[] { hit.point , final_pos});

            // Update the mesh
            Quaternion new_rotation = Quaternion.LookRotation(Vector3.forward, hit.normal);
            transform.position = hit.point;
            transform.rotation = new_rotation;

            if (e.type == EventType.MouseDown && e.button == 2) {

                Debug.Log(position);

                GameObject go = Instantiate(prefab) as GameObject;
                go.transform.position = hit.point + position;
                go.transform.rotation = Quaternion.Euler(rotation);
                go.transform.localScale = scale;
                go.transform.parent = parent != null ? parent : null;

                if (physics)
                {
                    go.AddComponent<Rigidbody>();
                    //go.AddComponent<EditorPhysics>();
                    go.transform.position = go.transform.position + hit.normal * offsetDistance;
                }

                e.Use();
            }
        }
    }
}
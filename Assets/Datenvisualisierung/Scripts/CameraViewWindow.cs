using UnityEngine;
using System.Collections;
using UnityEditor;

public class CameraViewWindow : EditorWindow
{

    [MenuItem("Tools/Camera View")]
    static void Init()
    {
        CreateInstance<CameraViewWindow>().Show();
    }

    public Camera cam;
    public Camera[] cameras = null;
    public string[] camNames;
    public int currentCam = -1;
    void UpdateCameras()
    {
        cameras = FindObjectsOfType<Camera>();
        if (camNames == null || camNames.Length != cameras.Length)
            camNames = new string[cameras.Length];
        for (int i = 0; i < cameras.Length; i++)
            camNames[i] = cameras[i].name;
    }
    void OnEnable()
    {
        cameras = null;
    }

    void OnHierarchyChange()
    {
        cameras = null;
    }

    void OnGUI()
    {
        if (cameras == null)
            UpdateCameras();
        GUILayout.BeginHorizontal();
        GUI.changed = false;
        cam = (Camera)EditorGUILayout.ObjectField(cam, typeof(Camera));
        if (GUI.changed)
        {
            currentCam = -1;
        }

        GUI.changed = false;
        currentCam = EditorGUILayout.Popup(currentCam, camNames);
        if (GUI.changed)
        {
            cam = cameras[currentCam];
        }
        GUILayout.EndHorizontal();
        Rect camArea = GUILayoutUtility.GetRect(1, 10000, 1, 10000);

        if (Event.current.type == EventType.Repaint)
        {
            if (cam != null)
            {
                Rect old = cam.pixelRect;
                camArea.y = position.height - camArea.yMax - 16;
                cam.pixelRect = camArea;
                cam.Render();
                cam.pixelRect = old;
            }
            else
                GUI.Label(camArea, "Set a camera to render", "button");
        }
    }
}

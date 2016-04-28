using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataController {

    private CSVDataObject data;
    private Vector3 origin = new Vector3(0.0f,25.0f,0.0f);
    private Vector3 length = new Vector3(5000f,1500f,2500f);

    public DataController(CSVDataObject csvData) {
        this.data = csvData;
    }

    private float safeGetValueFromMap(Dictionary<string,float> d,string s) {
        if (d.ContainsKey(s)) {
            Debug.Log("key: " + s + "\nvalue: " + d[s].ToString());
            return d[s];
        } else {
            d.Add(s, d.Keys.Count);
            Debug.Log("key: " + s + "\nvalue: " + d[s]);
            return d[s];
        }
    }

    public void generateGraphChart() {
        Dictionary<string, float> mapToFloatX = new Dictionary<string, float>();
        Dictionary<string, float> mapToFloatY = new Dictionary<string, float>();
        Dictionary<string, float> mapToFloatZ = new Dictionary<string, float>();

        List<GameObject> points = new List<GameObject>();

        foreach (MultidimensionalObject obj in data.getData()) {
            float x = (obj.getX() is float) ? (float)obj.getX() : safeGetValueFromMap(mapToFloatX, (string) obj.getX());
            float y = (obj.getY() is float) ? (float)obj.getY() : safeGetValueFromMap(mapToFloatY, (string) obj.getY());
            float z = (obj.getZ() is float) ? (float)obj.getZ() : safeGetValueFromMap(mapToFloatZ, (string )obj.getZ());


            float posX = length.x / (ListUtils.getHighestFloat(data.getAllX()) - ListUtils.getLowestFloat(data.getAllX()) + 1) * (x - ListUtils.getLowestFloat(data.getAllX()) + 1);
            float posY = length.y / (ListUtils.getHighestFloat(data.getAllY()) - ListUtils.getLowestFloat(data.getAllY()) + 1) * (y - ListUtils.getLowestFloat(data.getAllY()) + 1);
            float posZ = length.z / (ListUtils.getHighestFloat(data.getAllZ()) - ListUtils.getLowestFloat(data.getAllZ()) + 1) * (z - ListUtils.getLowestFloat(data.getAllZ()) + 1);
            Debug.Log("lenght" + length.z);
            Debug.Log("highest" + ListUtils.getHighestFloat(data.getAllZ()));
            Debug.Log("lowest" + ListUtils.getLowestFloat(data.getAllZ()) + 1);

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.position = new Vector3(posX,posY,posZ);
            temp.transform.localScale = new Vector3(100, 100, 100);
            PointScript script = temp.AddComponent<PointScript>();
            script.showX = obj.getX().ToString();
            script.showY = obj.getY().ToString();
            script.showZ = obj.getZ().ToString();
            script.titleX = data.getHeadlines()[0];
            script.titleY = data.getHeadlines()[1];
            script.titleZ = data.getHeadlines()[2];
            points.Add(temp);
        }

        for (int i = 1; i < points.Count; i++) {
            GameObject o1 = points[i];
            GameObject o2 = points[i - 1];
            LineRenderer lr = o2.AddComponent<LineRenderer>();
            lr.SetPosition(0, o1.transform.position );
            lr.SetPosition(1, o2.transform.position);
            lr.SetWidth(25, 25);
        }
    }
}

public class PointScript : MonoBehaviour {
    public string showX;
    public string showY;
    public string showZ;
    public string titleX;
    public string titleY;
    public string titleZ;
    private bool active = false;

    void OnMouseDown() {
        toggleTextRenderer();
    }

    private void toggleTextRenderer() {
        if (GameObject.Find(gameObject.GetInstanceID().ToString())) {
            GameObject.Destroy(GameObject.Find(gameObject.GetInstanceID().ToString()));
            return;
        }
        GameObject tem = new GameObject(gameObject.GetInstanceID().ToString());
        tem.transform.position = gameObject.transform.position;
        tem.transform.rotation = gameObject.transform.rotation;
        MeshRenderer mr = tem.AddComponent<MeshRenderer>();
        TextMesh tm = tem.AddComponent<TextMesh>();
        tm.text = titleX + ": " + showX + "\n" + titleY + ": " + showY + "\n" + titleZ + ": " + showZ;
        tm.fontSize = 50;
        tm.characterSize = 12;
    }
}
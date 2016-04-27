using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataController {

    private CSVDataObject data;
    private Vector3 origin = new Vector3(0.0f,25.0f,0.0f);
    private Vector3 length = new Vector3(5000f,1500f,5000f);

    public DataController(CSVDataObject csvData) {
        this.data = csvData;
    }

    public void generateGraphChart() {
        List<GameObject> points = new List<GameObject>();
    
        foreach (MultidimensionalObject obj in data.getData()) {
            float x = (obj.getX() is float) ? (float)obj.getX() : origin.x;
            float y = (obj.getY() is float) ? (float)obj.getY() : origin.y;
            float z = (obj.getZ() is float) ? (float)obj.getZ() : origin.z;

            float posX = length.x / (getUpperEndX() - getLowerEndX() + 1) * (x - getLowerEndX() + 1);
            float posY = length.y / (getUpperEndY() - getLowerEndY() + 1) * (y - getLowerEndY() + 1);
            float posZ = length.z / (getUpperEndZ() - getLowerEndZ() + 1) * (z - getLowerEndZ() + 1);

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.position = new Vector3(posX,posY,posZ);
            temp.transform.localScale = new Vector3(100, 100, 100);
            PointScript script = temp.AddComponent<PointScript>();
            script.showX = x;
            script.showY = y;
            script.showZ = z;
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

    private float getUpperEndX() {
        float high = 0f;
        foreach (MultidimensionalObject o in data.getData()) {
            
            if ((float)o.getX() > high) high = (float)o.getX();
        }
        return high;
    }

    private float getLowerEndX() {
        float low = 999999f;
        foreach (MultidimensionalObject o in data.getData()) {

            if ((float)o.getX() < low) low = (float)o.getX();
        }
        return low;
    }

    private float getUpperEndY() {
        float high = 0f;
        foreach (MultidimensionalObject o in data.getData()) {

            if ((float)o.getY() > high) high = (float)o.getY();
        }
        return high;
    }

    private float getLowerEndY() {
        float low = 999999f;
        foreach (MultidimensionalObject o in data.getData()) {

            if ((float)o.getY() < low) low = (float)o.getY();
        }
        return low;
    }

    private float getUpperEndZ() {
        float high = 0f;
        foreach (MultidimensionalObject o in data.getData()) {
            if ((float)o.getZ() > high) high = (float)o.getZ();
        }
        return high;
    }

    private float getLowerEndZ() {
        float low = 999999f;
        foreach (MultidimensionalObject o in data.getData()) {

            if ((float)o.getZ() < low) low = (float)o.getZ();
        }
        return low;
    }
}

public class PointScript : MonoBehaviour {
    public float showX;
    public float showY;
    public float showZ;
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
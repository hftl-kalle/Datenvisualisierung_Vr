﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class DataController : MonoBehaviour {

    private CSVDataObject data;
    private Vector3 origin = new Vector3(0.0f,25.0f,0.0f);
    private Vector3 length = new Vector3(2,2,2);
    private List<GameObject> points = new List<GameObject>();

    public void init(CSVDataObject csvData) {
        this.data = csvData;
        createPoints();
    }

    //return value from map or insert it if not found
    private float safeGetValueFromMap(Dictionary<string,float> d,string s) {
        if (d.ContainsKey(s)) {
            return d[s];
        } else {
            d.Add(s, d.Keys.Count);
            return d[s];
        }
    }

    public void createPoints() {

        //these maps are used to enumerate strings in the  lists
        Dictionary<string, float> mapX = new Dictionary<string, float>();
        Dictionary<string, float> mapY = new Dictionary<string, float>();
        Dictionary<string, float> mapZ = new Dictionary<string, float>();
        GameObject chartParent = GameObject.Find("chartParent");

        foreach (MultidimensionalObject obj in data.getData()) {

            //Get values
            float x = (obj.getX() is float) ? (float)obj.getX() : safeGetValueFromMap(mapX, (string) obj.getX());
            float y = (obj.getY() is float) ? (float)obj.getY() : safeGetValueFromMap(mapY, (string) obj.getY());
            float z = (obj.getZ() is float) ? (float)obj.getZ() : safeGetValueFromMap(mapZ, (string )obj.getZ());

            //calc position length axis / (highest avai. value - lowest avai. value + 1) * (value - lowest avai. value +1) 
            // 100 / (5-0+1) * (2.5 - 0 + 1) = 50
            //problem: lowest number is always at origin even if pretty big
            float posX = length.x / (ListUtils.getHighestFloat(data.getAllX()) - ListUtils.getLowestFloat(data.getAllX())) * (x - ListUtils.getLowestFloat(data.getAllX()));
            float posY = length.y / (ListUtils.getHighestFloat(data.getAllY()) - ListUtils.getLowestFloat(data.getAllY())) * (y - ListUtils.getLowestFloat(data.getAllY()));
            float posZ = length.z / (ListUtils.getHighestFloat(data.getAllZ()) - ListUtils.getLowestFloat(data.getAllZ())) * (z - ListUtils.getLowestFloat(data.getAllZ()));

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.parent = chartParent.transform;
            temp.transform.position = new Vector3(posX,posY,posZ);
            temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            temp.tag = "pointInCloud";
            Rigidbody rb = temp.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            PointScript script = temp.AddComponent<PointScript>();
            script.headlines = data.getHeadlines();
            script.data = obj.getObjectArray();
            points.Add(temp);
        }
    }

    public void createLineGraph() {
        for (int i = 1; i < points.Count; i++) {
            GameObject o1 = points[i];
            GameObject o2 = points[i - 1];
            LineRenderer lr = o2.AddComponent<LineRenderer>();
            lr.SetPosition(0, o1.transform.position);
            lr.SetPosition(1, o2.transform.position);
            lr.SetWidth(0.05f, 0.05f);
        }
    }

    public void createHeatMap() {

    }

    public void createBiMap() {
    }

    public void createMultiple2DGraphs() {
        Dictionary<float, GameObject> zValues = new Dictionary<float, GameObject>();
        for (int i = 1; i < points.Count; i++) {
            GameObject point = points[i];
            if (zValues.ContainsKey(point.transform.position.z)) {
                GameObject pointOld = zValues[point.transform.position.z];
                zValues.Remove(point.transform.position.z);
                LineRenderer lr = pointOld.AddComponent<LineRenderer>();
                lr.SetPosition(0, point.transform.position);
                lr.SetPosition(1, pointOld.transform.position);
                lr.SetWidth(0.05f, 0.05f);
                zValues.Add(point.transform.position.z, point);
            } else {
                zValues.Add(point.transform.position.z, point);
            }
        }
    }
}

public class PointScript : MonoBehaviour {
    public string[] headlines = new string[4];
    public object[] data = new object[4];
    private bool active = false;

    public void OnMouseDown() {
        toggleTextRenderer();
    }

    private void toggleTextRenderer() {
        if (GameObject.Find(gameObject.GetInstanceID().ToString())) {
            GameObject.Destroy(GameObject.Find(gameObject.GetInstanceID().ToString()));
            return;
        }
        /* GameObject tem = new GameObject(gameObject.GetInstanceID().ToString());
         tem.transform.position = gameObject.transform.position;
         MeshRenderer mr = tem.AddComponent<MeshRenderer>();
         TextMesh tm = tem.AddComponent<TextMesh>();
         tm.text = titleX + ": " + showX + "\n" + titleY + ": " + showY + "\n" + titleZ + ": " + showZ;
         tm.fontSize = 50;
         tm.characterSize = 12;*/
        GameObject Canvas = GameObject.Find("Canvas");
        GameObject textGO = new GameObject(gameObject.GetInstanceID().ToString());
        textGO.transform.parent = Canvas.transform;
        textGO.AddComponent<RectTransform>();
        Text textComponent = textGO.AddComponent<Text>();
        textGO.transform.position = gameObject.transform.position-new Vector3(0.2f,0,0.2f);
        textGO.transform.rotation = Quaternion.LookRotation(transform.position - GameObject.Find("Camera (eye)").transform.position);
        textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 160);
        textGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        for (int i=0; i < headlines.Length; i++) {
            if (headlines[i] != null && data[i] != null) textComponent.text = textComponent.text + Environment.NewLine + headlines[i] + ": " + data[i];
        }
        textComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        textComponent.fontSize = 20;
    }
}
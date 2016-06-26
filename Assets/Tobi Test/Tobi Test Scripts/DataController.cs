using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class DataController : MonoBehaviour {

    private enum CurrentRenderMode {
        LineGraph,
        HeatMap,
        BiMap,
        Multiple
    }
    private CSVDataObject data;
    public Vector3 scale = new Vector3(1, 1, 1);
    private Vector3 origin = new Vector3(1, 1, 1);

    private static Vector3 overallSize = new Vector3(2, 2, 2);
    private static Vector3 quadrantSize = overallSize / 2;

    CurrentRenderMode RenderMode;
    private List<GameObject> points = new List<GameObject>();
    enum HeatmapLayers : int { DarkBlue, LightBlue, Green, Yellow, Orange, Red };

    Dictionary<HeatmapLayers, float> heatmapColorThreshold = new Dictionary<HeatmapLayers, float>() {
        {HeatmapLayers.DarkBlue, 0.165f},
        {HeatmapLayers.LightBlue, 0.33f},
        {HeatmapLayers.Green, 0.495f},
        {HeatmapLayers.Yellow, 0.66f},
        {HeatmapLayers.Orange, 0.825f},
        {HeatmapLayers.Red, 1f}
    };

    public void init(CSVDataObject csvData) {
        this.data = csvData;
        GameObject.Find("chartParent").transform.localPosition = origin;
    }

    public void init(CSVDataObject csvData, Vector3 scaleParam) {
        init(csvData);
        scale = scaleParam;
    }

    public void setScale(Vector3 scaleParam)
    {
        scale += scaleParam;
        reRender();
    }

    public void reRender()
    {
        switch (RenderMode)
        {
            case CurrentRenderMode.LineGraph:
                createLineGraph();
                break;
            case CurrentRenderMode.HeatMap:
                createHeatMap();
                break;
            case CurrentRenderMode.BiMap:
                createBiMap();
                break;
            case CurrentRenderMode.Multiple:
                createMultiple2DGraphs();
                break;
            default:
                break;
        }
    }

    //return value from map or insert it if not found
    private float safeGetValueFromMap(Dictionary<string, float> d, string s) {
        if (d.ContainsKey(s)) {
            return d[s];
        } else {
            d.Add(s, d.Keys.Count);
            return d[s];
        }
    }

    public void createPoints(float heatmapHeightReference = 0) {
        if (heatmapHeightReference == 0) heatmapHeightReference = quadrantSize.y;
        clearGraph();
        createAxis();
        

        //TODO Comments and un-mess this
        //these maps are used to enumerate strings in the  lists
        Dictionary<string, float> stringValsX = new Dictionary<string, float>();
        Dictionary<string, float> stringValsY = new Dictionary<string, float>();
        Dictionary<string, float> stringValsZ = new Dictionary<string, float>();
        GameObject chartParent = GameObject.Find("chartParent");

        foreach (MultidimensionalObject obj in data.getData()) {

            //Get values
            float x = (obj.getX() is float) ? (float)obj.getX() : safeGetValueFromMap(stringValsX, (string)obj.getX());
            float y = (obj.getY() is float) ? (float)obj.getY() : safeGetValueFromMap(stringValsY, (string)obj.getY());
            float z = (obj.getZ() is float) ? (float)obj.getZ() : safeGetValueFromMap(stringValsZ, (string)obj.getZ());

            float scaleX = quadrantSize.x / ListUtils.getMaxAbsolutAmount(data.getAllX());
            float posX = x * scaleX;
            float scaleZ = quadrantSize.z / ListUtils.getMaxAbsolutAmount(data.getAllZ());
            float posZ = z * scaleZ * (-1);

            float scaleY = heatmapHeightReference / ListUtils.getMaxAbsolutAmount(data.getAllY());
            float posY = y * scaleY;

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.parent = chartParent.transform;
            temp.transform.localPosition = new Vector3(posX, posY, posZ);
            temp.transform.localScale = new Vector3(0.05f*overallSize.x * scale.x, 0.05f*overallSize.y * scale.y, 0.05f*overallSize.z * scale.z);
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
        RenderMode = CurrentRenderMode.LineGraph;
        createPoints();
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
        RenderMode = CurrentRenderMode.HeatMap;
        //float heatmapHeightReference = ListUtils.getHighestFloat(data.getAllW());
        float heatmapHeightReference = 0;
        createPoints(heatmapHeightReference);
        // set everything to 0
        float[,] htmap = new float[129, 129];
           for (int x = 0; x < htmap.GetLength(0); x++) {
               for (int z = 0; z < htmap.GetLength(1); z++) {
                   htmap[z, x] = 0;
               }
           }

        //create terrain
        GameObject terrainObj = new GameObject("TerrainObj");
        GameObject chartParent = GameObject.Find("chartParent");
        terrainObj.transform.parent = chartParent.transform;
        terrainObj.transform.localPosition = -quadrantSize;
        //center the terrain
        TerrainData terrainData = new TerrainData();
        //set terrain size
        terrainData.size = new Vector3(0.25f*overallSize.x * scale.x, 1f*overallSize.y * scale.y, 0.25f*overallSize.z * scale.z);
        //influences terrain size why so ever
        terrainData.heightmapResolution = 128;
        terrainData.baseMapResolution = 128;
        //dunno if we need this,  can cause lags if to high values
        terrainData.SetDetailResolution(64, 32);

        //terrainData.SetHeights(0, 0, htmap);

        int _heightmapWidth = htmap.GetUpperBound(0);
        int _heightmapHeight = htmap.GetUpperBound(0);

        TerrainCollider terrainCollider = terrainObj.AddComponent<TerrainCollider>();
        Terrain terrain = terrainObj.AddComponent<Terrain>();
        terrainCollider.terrainData = terrainData;
        terrain.terrainData = terrainData;
   
        Texture2D[] terrainTextures = new Texture2D[6];
        for (int o = 0; o < terrainTextures.Length; o++) {
            terrainTextures[o] = (Texture2D)Resources.Load("Textures/layer" + o);
        }

        SplatPrototype[] tex = new SplatPrototype[terrainTextures.Length];
        for (int i = 0; i < terrainTextures.Length; i++) {
            tex[i] = new SplatPrototype();
            tex[i].texture = terrainTextures[i];    //Sets the texture
            tex[i].tileSize = new Vector2(1, 1);    //Sets the size of the texture
        }
        terrainData.splatPrototypes = tex;
        terrainData.alphamapResolution = 128;


        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int x = 0; x < splatmapData.GetLength(0); x++) {
            for (int z = 0; z < splatmapData.GetLength(1); z++) {
                splatmapData[z, x, 0] = 1f;
            }
        }

        for (int k = 0; k < points.Count; k++) {
            //points[k].SetActive(false);
            if (points[k].transform.position.y > 0) {

                //best use multiple of 6
                int range = 6;
                float percentage = (float)Math.Round(1.0 / (range + 1), 2);

                int x1 = (int)((_heightmapWidth / 2) + (points[k].transform.localPosition.x * (_heightmapWidth / 2)));
                int z1 = (int)((_heightmapHeight / 2) + ( points[k].transform.localPosition.z * (_heightmapHeight / 2)));

                float pointHeigth = (points[k].transform.localPosition.y + quadrantSize.y) / overallSize.y;

                Debug.Log(x1);
                Debug.Log(z1);
                Debug.Log(pointHeigth);


                htmap[z1, x1] = pointHeigth;

                range++;
                for (int rx = -range; rx <= range; rx++) {
                    for (int rz = -range; rz <= range; rz++) {
                        //wenn wert +- den range wert immer noch im gültigen bereich
                        if ((x1 + rx > -1) && (z1 + rz > -1) && (x1 + rx <= _heightmapHeight) && (z1 + rz <= _heightmapWidth)) {
                            for (int ix = range; ix >= 0; ix--) {
                                if (Math.Abs(rx) == ix || Math.Abs(rz) == ix) {
                                    if (htmap[z1 + rz, x1 + rx] == 0) {
                                        float heigth = (range + 1 - ix) * percentage * pointHeigth;
                                        float[] splatWeights = new float[terrainData.alphamapLayers];
                                        if (heigth > heatmapColorThreshold[HeatmapLayers.Orange]) {
                                            splatWeights[5] = 1f;
                                        } else if (heigth > heatmapColorThreshold[HeatmapLayers.Yellow]) {
                                            splatWeights[4] = 1f;
                                        } else if (heigth > heatmapColorThreshold[HeatmapLayers.Green]) {
                                            splatWeights[3] = 1f;
                                        } else if (heigth > heatmapColorThreshold[HeatmapLayers.LightBlue]) {
                                            splatWeights[2] = 1f;
                                        } else if (heigth > heatmapColorThreshold[HeatmapLayers.DarkBlue]) {
                                            splatWeights[1] = 1f;
                                        } else {
                                            splatWeights[0] = 1f;
                                        }
                                        float z = splatWeights.Sum();
                                        for (int i = 0; i < terrainData.alphamapLayers; i++) {
                                            splatWeights[i] /= z;
                                            if ( (z1 + rz - 1 > -1) && (x1 + rx - 1 > -1)) splatmapData[z1 + rz-1, x1 + rx-1, i] = splatWeights[i];
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                range--;
                //iterate from - range  to + range
                for (int rx = -range; rx <= range; rx++) {                  
                    for (int rz = -range; rz <= range; rz++) {
                        //wenn wert +- den range wert immer noch im gültigen bereich
                        if ((x1 + rx > -1) && (z1 + rz > -1) && (x1 + rx <= _heightmapHeight) && (z1 + rz <= _heightmapWidth)) {
                            for (int ix = range; ix >= 0; ix--) {
                                if (Math.Abs(rx) == ix || Math.Abs(rz) == ix) {                    
                                    if (htmap[z1 + rz, x1 + rx] == 0) {
                                        float heigth = (range + 1 - ix) * percentage * pointHeigth;
                                        foreach (HeatmapLayers layer in Enum.GetValues(typeof(HeatmapLayers))) {
                                            if (heigth <= heatmapColorThreshold[layer]) {
                                                if (heatmapColorThreshold[layer] > pointHeigth) {
                                                    htmap[z1 + rz, x1 + rx] = heigth;
                                                    break;
                                                }
                                                htmap[z1 + rz, x1 + rx] = heatmapColorThreshold[layer];
                                                break;
                                            }
                                        }                               
                                    }
                                }
                            }
                        }
                    }
                }
            }
            terrain.terrainData.SetHeights(0, 0, htmap);
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }

    public void clearGraph() {
        foreach (GameObject point in points) {
            GameObject.Destroy(point);
        }
        GameObject.Destroy(GameObject.Find("TerrainObj"));
        GameObject.Destroy(GameObject.Find("Xaxis"));
        GameObject.Destroy(GameObject.Find("Yaxis"));
        GameObject.Destroy(GameObject.Find("Zaxis"));
        points = new List<GameObject>();
    }

    public void clearGraphImmediate() {
        foreach (GameObject point in points) {
            GameObject.DestroyImmediate(point);
        }
        GameObject.DestroyImmediate(GameObject.Find("TerrainObj"));
        GameObject.DestroyImmediate(GameObject.Find("Xaxis"));
        GameObject.DestroyImmediate(GameObject.Find("Yaxis"));
        GameObject.DestroyImmediate(GameObject.Find("Zaxis"));
        points = new List<GameObject>();
    }

    public void createBiMap() {
        RenderMode= CurrentRenderMode.BiMap;
        createPoints();
        for (int i = 1; i < points.Count; i++) {
            PointScript script = points[i].GetComponent<PointScript>();
            script.showAdditionalData = true;
        }
            
    }

    public void createMultiple2DGraphs() {
        RenderMode = CurrentRenderMode.Multiple;
        createPoints();
        Dictionary<float, GameObject> zValues = new Dictionary<float, GameObject>();
        for (int i = 1; i < points.Count; i++) {
            GameObject point = points[i];
            if (zValues.ContainsKey(point.transform.position.z)) {
                GameObject pointOld = zValues[point.transform.position.z];
                zValues.Remove(point.transform.position.z);
                LineRenderer lr = pointOld.AddComponent<LineRenderer>();
                lr.SetPosition(0, point.transform.position);
                lr.SetPosition(1, pointOld.transform.position);
                lr.SetWidth(0.029f * scale.magnitude, 0.029f * scale.magnitude);
                zValues.Add(point.transform.position.z, point);
            } else {
                zValues.Add(point.transform.position.z, point);
            }
        }
    }

    public void createAxis()
    {
        GameObject chartParent = GameObject.Find("chartParent");
        GameObject xaxis = new GameObject("Xaxis");
        Rigidbody xrb = xaxis.AddComponent<Rigidbody>();
        xrb.isKinematic = true;
        xaxis.transform.parent = chartParent.transform;
        xaxis.tag = "axis";
        BoxCollider xbc = xaxis.AddComponent<BoxCollider>();
        xbc.center = new Vector3(1, 1, 1);
        xbc.size = new Vector3(2, 0.05f, 0.05f);

        GameObject yaxis = new GameObject("Yaxis");
        Rigidbody yrb = yaxis.AddComponent<Rigidbody>();
        yrb.isKinematic = true;
        yaxis.transform.parent = chartParent.transform;
        yaxis.tag = "axis";
        BoxCollider ybc = yaxis.AddComponent<BoxCollider>();
        ybc.center = new Vector3(1, 1, 1);
        ybc.size = new Vector3(0.05f, 2, 0.05f);

        GameObject zaxis = new GameObject("Zaxis");
        Rigidbody zrb = zaxis.AddComponent<Rigidbody>();
        zrb.isKinematic = true;
        zaxis.transform.parent = chartParent.transform;
        zaxis.tag = "axis";
        BoxCollider zbc = zaxis.AddComponent<BoxCollider>();
        zbc.center = new Vector3(1, 1, 1);
        zbc.size = new Vector3(0.05f, 0.05f, 2);

        LineRenderer lrx = xaxis.AddComponent<LineRenderer>();
        lrx.material = new Material(Shader.Find("Sprites/Default"));
        lrx.SetWidth(0.05f, 0.05f);
        lrx.SetColors(Color.blue, Color.blue);
        lrx.SetPosition(0, new Vector3(2, 1, 1));
        lrx.SetPosition(1, new Vector3(0, 1, 1));
        lrx.useWorldSpace = false;
        LineRenderer lry = yaxis.AddComponent<LineRenderer>();
        lry.material = new Material(Shader.Find("Sprites/Default"));
        lry.SetWidth(0.05f, 0.05f);
        lry.SetColors(Color.blue, Color.blue);
        lry.SetPosition(0, new Vector3(1, 2, 1));
        lry.SetPosition(1, new Vector3(1, 0, 1));
        lry.useWorldSpace = false;
        LineRenderer lrz = zaxis.AddComponent<LineRenderer>();
        lrz.material = new Material(Shader.Find("Sprites/Default"));
        lrz.SetWidth(0.05f, 0.05f);
        lrz.SetColors(Color.blue, Color.blue);
        lrz.SetPosition(0, new Vector3(1, 1, 2));
        lrz.SetPosition(1, new Vector3(1, 1, 0));
        lrz.useWorldSpace = false;

        xaxis.transform.localPosition = new Vector3(-1,-1,-1);
        yaxis.transform.localPosition = new Vector3(-1, -1, -1);
        zaxis.transform.localPosition = new Vector3(-1, -1, -1);
    }
}
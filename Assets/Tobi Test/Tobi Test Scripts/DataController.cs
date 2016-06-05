using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class DataController : MonoBehaviour {
    private enum CurrentRenderMode
    {
        LineGraph,
        HeatMap,
        BiMap,
        Multiple
    }
    private CSVDataObject data;
    public Vector3 scale = new Vector3(1, 1, 1);
    private Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 length = new Vector3(2, 2, 2);
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
    }

    public void init(CSVDataObject csvData, Vector3 scaleParam)
    {
        this.data = csvData;
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

    public void createPoints(float heatmapHeightReference = -100) {
        clearGraph();
        createAxis();
        //TODO Comments and un-mess this
        //these maps are used to enumerate strings in the  lists
        Dictionary<string, float> mapX = new Dictionary<string, float>();
        Dictionary<string, float> mapY = new Dictionary<string, float>();
        Dictionary<string, float> mapZ = new Dictionary<string, float>();
        GameObject chartParent = GameObject.Find("chartParent");

        foreach (MultidimensionalObject obj in data.getData()) {

            //Get values
            float x = (obj.getX() is float) ? (float)obj.getX() : safeGetValueFromMap(mapX, (string)obj.getX());
            float y = (obj.getY() is float) ? (float)obj.getY() : safeGetValueFromMap(mapY, (string)obj.getY());
            float z = (obj.getZ() is float) ? (float)obj.getZ() : safeGetValueFromMap(mapZ, (string)obj.getZ());
            Debug.Log(ListUtils.getHighestFloat(data.getAllX()) - ListUtils.getLowestFloat(data.getAllX()));
            Debug.Log(ListUtils.getHighestFloat(data.getAllY()) - ListUtils.getLowestFloat(data.getAllY()));
            Debug.Log(ListUtils.getHighestFloat(data.getAllZ()) - ListUtils.getLowestFloat(data.getAllZ()));
            //calc position length axis / (highest avai. value - lowest avai. value + 1) * (value - lowest avai. value +1) 
            // 100 / (5-0) * (2.5 - 0) = 50
            //problem: lowest number is always at origin even if pretty big
            float posX = (length.x*scale.x) / (ListUtils.getHighestFloat(data.getAllX()) - ListUtils.getLowestFloat(data.getAllX())) * (x - ListUtils.getLowestFloat(data.getAllX()));
            float posZ = (length.z * scale.z) / (ListUtils.getHighestFloat(data.getAllZ()) - ListUtils.getLowestFloat(data.getAllZ())) * (z - ListUtils.getLowestFloat(data.getAllZ()));
            if (posX != posX) posX = (length.x * scale.x) / 2;
            if (posZ != posZ) posZ = (length.z * scale.z) / 2;
            float posY;
            if (heatmapHeightReference != -100) {
                posY = (length.y * scale.y) / heatmapHeightReference * y;
            } else {
                posY = (length.y * scale.y) / (ListUtils.getHighestFloat(data.getAllY()) - ListUtils.getLowestFloat(data.getAllY())) * (y - ListUtils.getLowestFloat(data.getAllY()));
            }
            
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.parent = chartParent.transform;
            temp.transform.position = new Vector3(posX, posY, posZ);
            temp.transform.localScale = new Vector3(0.05f*length.x * scale.x, 0.05f*length.y * scale.y, 0.05f*length.z * scale.z);
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
        float heatmapHeightReference = ListUtils.getHighestFloat(data.getAllW());
        createPoints(heatmapHeightReference);
        float[,] htmap = new float[129, 129];
           for (int x = 0; x < htmap.GetLength(0); x++) {
               for (int z = 0; z < htmap.GetLength(1); z++) {
                   htmap[z, x] = 0;
               }
           }

        //create terrain
        GameObject terrainObj = new GameObject("TerrainObj");
        terrainObj.transform.parent = GameObject.Find("chartParent").transform;
        TerrainData terrainData = new TerrainData();
        //set terrain size
        terrainData.size = new Vector3(0.25f*length.x * scale.x, 1f*length.y * scale.y, 0.25f*length.z * scale.z);
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
        terrainObj.transform.position = origin;
   
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

        /* smooth the heatmap raises */
        for (int k = 0; k < points.Count; k++) {
            //points[k].SetActive(false);
            if (points[k].transform.position.y > 0) {

                //best use multiple of 6
                int range = 12;
                float percentage = (float)Math.Round(1.0 / (range + 1), 2);
                int x1 =  (int)((points[k].transform.position.x / terrain.terrainData.size.x) * _heightmapWidth);
                int z1 = (int)((points[k].transform.position.z / terrain.terrainData.size.z) * _heightmapHeight);
                float pointHeigth = points[k].transform.position.y / (length.y * scale.y);
                htmap[z1, x1] = pointHeigth;

                range++;
                for (int rx = -range; rx <= range; rx++) {
                    for (int rz = -range; rz <= range; rz++) {
                        //wenn wert +- den range wert immer noch im gültigen bereich
                        if ((x1 + rx > -1) && (z1 + rz > -1) && (x1 + rx <= _heightmapHeight) && (z1 + rz <= _heightmapWidth)) {
                            for (int ix = range; ix >= 0; ix--) {
                                if (Math.Abs(rx) == ix || Math.Abs(rz) == ix) {
                                    if (htmap[z1 + rz, x1 + rx] == 0) {
                                        float heigth = (range + 1 - ix) * percentage * points[k].transform.position.y / (length.y * scale.y);
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
                                        float heigth = (range + 1 - ix) * percentage * points[k].transform.position.y / (length.y * scale.y);
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
            GameObject.DestroyImmediate(point);
        }
        GameObject.DestroyImmediate(GameObject.Find("TerrainObj"));
        GameObject.DestroyImmediate(GameObject.Find("Xaxis"));
        GameObject.DestroyImmediate(GameObject.Find("Yaxis"));
        GameObject.DestroyImmediate(GameObject.Find("Zaxis"));
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
    }
}

public class PointScript : MonoBehaviour {
    public string[] headlines = new string[4];
    public object[] data = new object[4];
    private bool active = false;
    public bool showAdditionalData = false;

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
        
        for (int i=0; i < ( (showAdditionalData) ? headlines.Length : (headlines.Length-1)); i++) {
            if (headlines[i] != null && data[i] != null) textComponent.text = textComponent.text + Environment.NewLine + headlines[i] + ": " + data[i];
        }
        textComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        textComponent.fontSize = 20;
    }
}
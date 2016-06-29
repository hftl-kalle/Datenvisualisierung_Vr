using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class DataController : MonoBehaviour {

    public enum CurrentRenderMode {
        LineGraph,
        HeatMap,
        BiMap,
        Multiple
    }
    private CSVDataObject data;

    private Vector3 origin = new Vector3(1, 1, 1);

    private static Vector3 overallSize = new Vector3(2, 2, 2);
    private static Vector3 quadrantSize=overallSize/2;

    public CurrentRenderMode RenderMode;
    private List<GameObject> points = new List<GameObject>();
    enum HeatmapLayers : int { DarkBlue, LightBlue, Green, Yellow, Orange, Red };

    Dictionary<HeatmapLayers, float> heatmapColorThreshold = new Dictionary<HeatmapLayers, float>() {
        {HeatmapLayers.DarkBlue, 0.25f},
        {HeatmapLayers.LightBlue, 0.45f},
        {HeatmapLayers.Green, 0.6f},
        {HeatmapLayers.Yellow, 0.8f},
        {HeatmapLayers.Orange, 0.95f},
        {HeatmapLayers.Red, 1f}
    };

    public void init(CSVDataObject csvData) {
        this.data = csvData;
        //GameObject.Find("chartParent").transform.localPosition = origin;
    }

    public void init(CSVDataObject csvData, Vector3 scaleParam) {
        init(csvData);
        overallSize = scaleParam;
        quadrantSize = overallSize / 2;
    }

    public void setScale(Vector3 scaleParam)
    {
        overallSize += scaleParam;
        quadrantSize = overallSize / 2;
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
        if (s == null) {
            return 0;
        }
        if (d.ContainsKey(s)) {
            return d[s];
        } else {
            d.Add(s, d.Keys.Count);
            return d[s];
        }
    }

    public void createPoints(float heatmapHeightReference = 0) {
        clearGraph();
        if (heatmapHeightReference == 0) heatmapHeightReference = quadrantSize.y;
        if(RenderMode!=CurrentRenderMode.HeatMap) createAxis();

        //TODO Comments and un-mess this
        //these maps are used to enumerate strings in the  lists
        Dictionary<string, float> stringValsX = new Dictionary<string, float>();
        Dictionary<string, float> stringValsY = new Dictionary<string, float>();
        Dictionary<string, float> stringValsZ = new Dictionary<string, float>();
        stringValsX.Add("",0);
        stringValsY.Add("", 0);
        stringValsZ.Add("", 0);
        GameObject chartParent = GameObject.Find("chartParent");

        foreach (MultidimensionalObject obj in data.getData()) {

            //Get values
            float x = (obj.getX() is float) ? (float)obj.getX() : safeGetValueFromMap(stringValsX, (string)obj.getX());
            float y = (obj.getY() is float) ? (float)obj.getY() : safeGetValueFromMap(stringValsY, (string)obj.getY());
            float z = (obj.getZ() is float) ? (float)obj.getZ() : safeGetValueFromMap(stringValsZ, (string)obj.getZ());

            float absoluteX = (ListUtils.getMaxAbsolutAmount(data.getAllX()) != 0f) ? ListUtils.getMaxAbsolutAmount(data.getAllX()) : 1;
            float scaleX = quadrantSize.x / absoluteX;
            float posX = x * scaleX;

            float absoluteZ = (ListUtils.getMaxAbsolutAmount(data.getAllZ()) != 0f) ? ListUtils.getMaxAbsolutAmount(data.getAllZ()) : 1;
            float scaleZ = quadrantSize.z / absoluteZ;
            float posZ = z * scaleZ * (-1);

            float absoluteY = (ListUtils.getMaxAbsolutAmount(data.getAllY()) != 0f) ? ListUtils.getMaxAbsolutAmount(data.getAllY()) : 1;
            float scaleY = heatmapHeightReference / absoluteY;
            float posY = y * scaleY;

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.parent = chartParent.transform;
            temp.transform.localPosition = new Vector3(posX, posY, posZ);
            temp.transform.localScale = new Vector3(0.05f*overallSize.x , 0.05f*overallSize.y , 0.05f*overallSize.z);
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
        float heatmapHeightReference = 1;
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
        terrainData.size = new Vector3(0.25f*overallSize.x , 1f*overallSize.y, 0.25f*overallSize.z);
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
            points[k].SetActive(false);
            if (points[k].transform.position.y > 0) {

                //best use multiple of 6
                int range = 4;
                float percentage = (float)Math.Round(1.0 / (range + 1), 2);

                int x1 = (int)((_heightmapWidth / 2) + (points[k].transform.localPosition.x * (_heightmapWidth / 2)));
                int z1 = (int)((_heightmapHeight / 2) + ( points[k].transform.localPosition.z * (_heightmapHeight / 2)));

                float pointHeigth = (points[k].transform.localPosition.y + quadrantSize.y) / overallSize.y;

                htmap[z1, x1] = pointHeigth;

                range++;
                for (int rx = -range; rx <= range; rx++) {
                    for (int rz = -range; rz <= range; rz++) {
                        //wenn wert +- den range wert immer noch im gültigen bereich
                        if ((x1 + rx > -1) && (z1 + rz > -1) && (x1 + rx <= _heightmapHeight) && (z1 + rz <= _heightmapWidth)) {
                            for (int ix = range; ix >= 0; ix--) {
                                if (Math.Abs(rx) == ix || Math.Abs(rz) == ix) {
                                    if (htmap[z1 + rz, x1 + rx] == 0) {
                                        /* use this for our example heatmap */
                                        float heigth = pointHeigth;

                                        /* use this for normal heatmap
                                        float heigth = (range + 1 - ix) * percentage * pointHeigth;
                                        */
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
                                        /* use this for our example heatmap */
                                        float heigth = pointHeigth;

                                        /* use this for normal heatmap
                                        float heigth = (range + 1 - ix) * percentage * pointHeigth;
                                        */
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
    public void clearCanvas()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        foreach (Transform text in canvas)
        {
            GameObject.Destroy(text.gameObject);
        }
    }

    public void clearGraph() {
        foreach (GameObject point in points) {
            GameObject.Destroy(point);
        }
        clearCanvas();
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
        Transform canvas = GameObject.Find("Canvas").transform;
        foreach (Transform text in canvas)
        {
            GameObject.Destroy(text.gameObject);
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
        Color[] colors = new Color[] { Color.black, Color.red, Color.yellow, Color.green,Color.magenta, new Color(1f/165f, 1f / 42f, 1f / 42f, 1), new Color(1f / 255f, 1f / 20f, 1f / 147f, 1)};
        RenderMode = CurrentRenderMode.Multiple;
        createPoints();
        Dictionary<float, GameObject> zValues = new Dictionary<float, GameObject>();
        Dictionary<float, Color> colorValues = new Dictionary<float, Color>();
        points[0].transform.localScale = new Vector3(0.01f * overallSize.x, 0.01f * overallSize.y, 0.01f * overallSize.z );
        for (int i = 1; i < points.Count; i++) {
            GameObject point = points[i];
            point.transform.localScale = new Vector3(0.01f * overallSize.x, 0.01f * overallSize.y , 0.01f * overallSize.z );
            if (zValues.ContainsKey(point.transform.localPosition.z)) {
                GameObject pointOld = zValues[point.transform.localPosition.z];
                zValues.Remove(point.transform.localPosition.z);
                LineRenderer lr = pointOld.AddComponent<LineRenderer>();
                lr.SetPosition(0, point.transform.position);
                lr.SetPosition(1, pointOld.transform.position);
                lr.SetColors(colorValues[point.transform.localPosition.z], colorValues[point.transform.localPosition.z]);
                Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
                lr.material = whiteDiffuseMat;
                lr.SetWidth(0.029f , 0.029f);
                zValues.Add(point.transform.localPosition.z, point);
            } else {
                zValues.Add(point.transform.localPosition.z, point);
                int count = colorValues.Keys.Count();
                if (count >= colors.Length) count = colors.Length - 1;
                colorValues.Add(point.transform.localPosition.z, colors[count]);
            }
        }
    }

    public void createAxis()
    {
        var listy = data.getAllY();
        var listx = data.getAllX();
        var listz = data.getAllZ();
        var minx = listx.Min();
        var maxx = listx.Max();
        var miny = listy.Min();
        var maxy = listy.Max();
        var minz = listz.Min();
        var maxz = listz.Max();
        Vector3 xstart, xend, ystart, yend,zstart, zend;

        //x
        float temp;
        if (float.TryParse(minx.ToString(),out temp)) {
            if (temp < 0) xstart = new Vector3((overallSize.x / 2 - overallSize.x), 0, 0);
            else xstart = new Vector3(0, 0, 0);
        }
        else
        {
            xstart = new Vector3(0, 0, 0);
        }

        if (float.TryParse(maxx.ToString(), out temp))
        {
            if (temp > 0) xend = new Vector3((overallSize.x - overallSize.x / 2), 0, 0);
            else xend = new Vector3(0, 0, 0);
        }
        else
        {
            xend = new Vector3(1, 0, 0);
        }

        //y
        if(float.TryParse(miny.ToString(), out temp))
        {
            if (temp < 0) ystart = new Vector3(0, (overallSize.y / 2 - overallSize.y), 0);
            else ystart = new Vector3(0, 0, 0);
        }
        else
        {
            ystart = new Vector3(0, 0, 0);
        }

        if (float.TryParse(maxy.ToString(), out temp))
        {
            if (temp > 0) yend = new Vector3(0, (overallSize.y - overallSize.y / 2), 0);
            else yend = new Vector3(0, 0, 0);
        }
        else
        {
            yend = new Vector3(0, 1, 0);
        }

        //z
        if (float.TryParse(minz.ToString(), out temp))
        {
            if (temp < 0) zstart = new Vector3(0, 0, (overallSize.z - overallSize.z / 2));
            else zstart = new Vector3(0, 0, 0);
        }
        else
        {
            zstart = new Vector3(0, 0, 0);
        }

        if (float.TryParse(maxz.ToString(), out temp))
        {
            if (temp > 0) zend = new Vector3(0, 0, (overallSize.z / 2 - overallSize.z));
            else zend = new Vector3(0, 0, 0);
        }
        else
        {
            zend = new Vector3(0, 0, -1);
        }


        GameObject chartParent = GameObject.Find("chartParent");
        GameObject xaxis = new GameObject("Xaxis");
        Rigidbody xrb = xaxis.AddComponent<Rigidbody>();
        xrb.isKinematic = true;
        xaxis.tag = "axis";
        BoxCollider xbc = xaxis.AddComponent<BoxCollider>();
        xbc.center = new Vector3(0, 0, 0);
        xbc.size = new Vector3(overallSize.x, 0.05f, 0.05f);
        xaxis.transform.position = chartParent.transform.position;
        xaxis.transform.rotation = chartParent.transform.rotation;
        xaxis.transform.parent = chartParent.transform;

        GameObject yaxis = new GameObject("Yaxis");
        Rigidbody yrb = yaxis.AddComponent<Rigidbody>();
        yrb.isKinematic = true;
        yaxis.tag = "axis";
        BoxCollider ybc = yaxis.AddComponent<BoxCollider>();
        ybc.center = new Vector3(0, 0, 0);
        ybc.size = new Vector3(0.05f, overallSize.y, 0.05f);
        yaxis.transform.position = chartParent.transform.position;
        yaxis.transform.rotation = chartParent.transform.rotation;
        yaxis.transform.parent = chartParent.transform;

        GameObject zaxis = new GameObject("Zaxis");
        Rigidbody zrb = zaxis.AddComponent<Rigidbody>();
        zrb.isKinematic = true;
        zaxis.tag = "axis";
        BoxCollider zbc = zaxis.AddComponent<BoxCollider>();
        zbc.center = new Vector3(0, 0, 0);
        zbc.size = new Vector3(0.05f, 0.05f, overallSize.z);
        zaxis.transform.position = chartParent.transform.position;
        zaxis.transform.rotation = chartParent.transform.rotation;
        zaxis.transform.parent = chartParent.transform;

        LineRenderer lrx = xaxis.AddComponent<LineRenderer>();
        lrx.material = new Material(Shader.Find("Sprites/Default"));
        lrx.SetWidth(0.05f, 0.05f);
        lrx.useWorldSpace = false;
        lrx.SetColors(Color.blue, Color.blue);
        lrx.SetPosition(0, xstart);
        lrx.SetPosition(1,xend);

        LineRenderer lry = yaxis.AddComponent<LineRenderer>();
        lry.material = new Material(Shader.Find("Sprites/Default"));
        lry.SetWidth(0.05f, 0.05f);
        lry.SetColors(Color.blue, Color.blue);
        lry.useWorldSpace = false;
        lry.SetPosition(0, ystart);
        lry.SetPosition(1, yend);

        LineRenderer lrz = zaxis.AddComponent<LineRenderer>();
        lrz.material = new Material(Shader.Find("Sprites/Default"));
        lrz.SetWidth(0.05f, 0.05f);
        lrz.SetColors(Color.blue, Color.blue);
        lrz.useWorldSpace = false;
        lrz.SetPosition(0, zstart);
        lrz.SetPosition(1, zend);        

        string[] headlines = data.getHeadlines();
        GameObject Canvas = GameObject.Find("Canvas");
        GameObject xtextGO = new GameObject(gameObject.GetInstanceID().ToString());
        xtextGO.transform.parent = Canvas.transform;
        xtextGO.AddComponent<RectTransform>();
        Text xtextComponent = xtextGO.AddComponent<Text>();
        xtextGO.transform.position = chartParent.transform.TransformPoint(new Vector3((overallSize.x - overallSize.x / 2), 0, 0));
        LookAt xLA = xtextGO.AddComponent<LookAt>();
        xLA.target = GameObject.Find("Camera (eye)").transform;
        xtextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 160);
        xtextGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        xtextComponent.text = headlines[0];
        xtextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        xtextComponent.fontSize = 20;

        GameObject ytextGO = new GameObject(gameObject.GetInstanceID().ToString());
        ytextGO.transform.parent = Canvas.transform;
        ytextGO.AddComponent<RectTransform>();
        Text ytextComponent = ytextGO.AddComponent<Text>();
        ytextGO.transform.position = chartParent.transform.TransformPoint(new Vector3(0, (overallSize.y - overallSize.y / 2), 0));
        LookAt yLA = ytextGO.AddComponent<LookAt>();
        yLA.target = GameObject.Find("Camera (eye)").transform;
        ytextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 160);
        ytextGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        ytextComponent.text = headlines[1];
        ytextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        ytextComponent.fontSize = 20;

        GameObject ztextGO = new GameObject(gameObject.GetInstanceID().ToString());
        ztextGO.transform.parent = Canvas.transform;
        ztextGO.AddComponent<RectTransform>();
        Text ztextComponent = ztextGO.AddComponent<Text>();
        // negativ end of z with offset of .25
        ztextGO.transform.position = chartParent.transform.TransformPoint(new Vector3(0, 0, (overallSize.z / 2 - overallSize.z) -0.25f));
        LookAt zLA = ztextGO.AddComponent<LookAt>();
        zLA.target = GameObject.Find("Camera (eye)").transform;
        ztextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 160);
        ztextGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        ztextComponent.text = headlines[2];
        ztextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        ztextComponent.fontSize = 20;
    }
}
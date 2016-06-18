using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

class ClipboardScript : MonoBehaviour {
    public FileInfo file;
    private Vector3 offset = new Vector3(0,0,2);
    private GameObject player;
    private bool hold = false;

    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnMouseDown() {
        Cursor.visible = false;
        hold = true;
    }

    public void Update() {
        if (hold) {
            transform.position = player.transform.position + offset;
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }
        
    }

    public void OnMouseUp() {
        transform.position = transform.position;
        Cursor.visible = true;
        hold = false;
    }

    public void setHold(bool holding) {
        hold = holding;
    }

    public void loadFile() {
        CSVDataObject csvData = CSVParser.loadCsv(file.FullName);
        //create chart parent
        GameObject chartParent = GameObject.Find("chartParent");
        if (chartParent != null) {
            Destroy(chartParent);
            foreach (Transform child in GameObject.Find("Canvas").transform) {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        // create chart parent with axis

        chartParent = new GameObject("chartParent");
        //attach data controller to chart parent and init it

        DataController dc = chartParent.AddComponent<DataController>();
        dc.init(csvData);

        //creating graphs can be called with
        // ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
        //also possible .createHeatMap() // .createBiMap // .createMultiple2DGraphs
        dc.createHeatMap();

        if (csvData == null) {
            EditorUtility.DisplayDialog("Error", "Your csv seems to be invalid. Atleast x and y are required", "Ok");
        }
    }

    public void unloadFile() {
        GameObject chartParent = GameObject.Find("chartParent");
        DataController dc = chartParent.GetComponent<DataController>();
        dc.clearGraph();
        if (chartParent != null) {
            GameObject.Destroy(chartParent);
            foreach (Transform child in GameObject.Find("Canvas").transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
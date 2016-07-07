using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

/// <summary>
/// class attached to every clipboard object
/// </summary>
class ClipboardScript : MonoBehaviour {
    public FileInfo file;
    private Vector3 offset = new Vector3(0,0,2);
    private GameObject player;
    private bool hold = false;

    /// <summary>
    /// init vars 
    /// </summary>
    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// if clicked enable holding
    /// </summary>
    public void OnMouseDown() {
        Cursor.visible = false;
        hold = true;
    }

    /// <summary>
    /// while holding active, the clipboard is offset away fom the player
    /// </summary>
    public void Update() {
        if (hold) {
            transform.position = player.transform.position + offset;
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }
        
    }

    /// <summary>
    /// if mouse up disable holding and active cursor
    /// </summary>
    public void OnMouseUp() {
        transform.position = transform.position;
        Cursor.visible = true;
        hold = false;
    }

    /// <summary>
    /// whether the clipboard should be moving with the player or not
    /// </summary>
    /// <param name="holding"></param>
    public void setHold(bool holding) {
        hold = holding;
    }

    /// <summary>
    /// load a csv file and display standard graph
    /// </summary>
    public void loadFile() {
        CSVDataObject csvData = CSVParser.loadCsv(file.FullName);
        //create chart parent
        GameObject chartParent = GameObject.Find("chartParent");
        if (chartParent != null) {
            Destroy(chartParent);
            foreach (Transform child in GameObject.Find("Canvas").transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
        // create chart parent with axis

        chartParent = new GameObject("chartParent");
        //attach data controller to chart parent and init it
        chartParent.transform.position = new Vector3(-1, 1, 0);
        chartParent.transform.Rotate(new Vector3(0, 1, 0), 90f);
        DataController dc = chartParent.AddComponent<DataController>();
        dc.init(csvData);

        //creating graphs can be called with
        // ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
        //also possible .createHeatMap() // .createBiMap // .createMultiple2DGraphs
        

        if (csvData == null) {
            EditorUtility.DisplayDialog("Error", "Your csv seems to be invalid. Atleast x and y are required", "Ok");
        }
    }

    /// <summary>
    /// unload csv and destroy graph
    /// </summary>
    public void unloadFile() {
        GameObject chartParent = GameObject.Find("chartParent");
        DataController dc = chartParent.GetComponent<DataController>();
        dc.clearGraph();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("holding_button")) {
            go.GetComponent<ButtonPressed>().deactivateButton();
        }
        if (chartParent != null) {
            GameObject.Destroy(chartParent);
            foreach (Transform child in GameObject.Find("Canvas").transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
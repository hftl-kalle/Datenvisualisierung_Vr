using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

class ClipboardScript : MonoBehaviour {
    public FileInfo file;

    public void OnMouseDown() {
        CSVDataObject csvData = CSVParser.loadCsv(file.FullName);
        //create chart parent
        GameObject chartParent = GameObject.Find("chartParent");
        if (chartParent != null) {
            DestroyImmediate(chartParent);
            foreach (Transform child in GameObject.Find("Canvas").transform) {
                GameObject.Destroy(child.gameObject);
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
        dc.createMultiple2DGraphs();

        if (csvData == null) {
            EditorUtility.DisplayDialog("Error", "Your csv seems to be invalid. Atleast x and y are required", "Ok");
        }
    }
}
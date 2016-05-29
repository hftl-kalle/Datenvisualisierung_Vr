using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class CSVReader : MonoBehaviour {
 
    void Start() {
        openCsvFromFolder();
    }

    public void openCsvFromFolder() {
        //path to .exe or document root + csv directory
        string directory = @Application.dataPath + "/../csv/";

        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        //get all csv's
        FileInfo[] files = dirInfo.GetFiles("*.csv");
        System.Random rnd = new System.Random();

        for (int i = 0; i < files.Length; i++) {
            FileInfo f = files[i];
            //GameObject inst = (GameObject) Instantiate(Resources.Load("Clipboard"), new Vector3((float) rnd.Next((int)transform.position.x-350, (int)transform.position.x + 350), (int)transform.position.y + i * 250f, (float)rnd.Next((int)transform.position.z - 350, (int)transform.position.z + 350)) , Quaternion.Euler((float)rnd.Next(-30, 30), (float)rnd.Next(-120, -60), (float)rnd.Next(-30, 30)));
            GameObject tableTop = GameObject.Find("tabletop");
            GameObject clipBoard = Instantiate(Resources.Load("Clipboard")) as GameObject;

            //get x- and z- length of the tabletop and crop it a bit so no clipboards fall off
            float tableLengthX = tableTop.GetComponent<Renderer>().bounds.size.x * 0.8f;
            float tableLengthZ = tableTop.GetComponent<Renderer>().bounds.size.z * 0.6f;

            //randomize position of clipboards on the table
            Vector3 vec = new Vector3(tableLengthX * (float) rnd.NextDouble() - (tableLengthX / 2),
                0.1f,
               tableLengthZ * (float)rnd.NextDouble() - (tableLengthZ / 2));
            clipBoard.transform.position= tableTop.transform.position+vec;
            clipBoard.transform.Rotate(new Vector3(1,0,0),10f);

            //add clipboards click script
            ClipboardScript script = clipBoard.AddComponent<ClipboardScript>();
            script.file = f;
        }
        
    }

}

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
        chartParent = new GameObject("chartParent");

        //attach data controller to chart parent and init it

        DataController dc = chartParent.AddComponent<DataController>();
        dc.init(csvData);

        //creating graphs can be called with
        // ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
        //also possible .createHeatMap() // .createBiMap // .createMultiple2DGraphs
        dc.createHeatMap();

        if (csvData == null) {
            EditorUtility.DisplayDialog("Error","Your csv seems to be invalid. Atleast x and y are required","Ok");
        }
    }
}

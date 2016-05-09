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
        string directory = @Application.dataPath + "/../csv/";

        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles("*.csv");
        System.Random rnd = new System.Random();

        for (int i = 0; i < files.Length; i++) {
            FileInfo f = files[i];
            //GameObject inst = (GameObject) Instantiate(Resources.Load("Clipboard"), new Vector3((float) rnd.Next((int)transform.position.x-350, (int)transform.position.x + 350), (int)transform.position.y + i * 250f, (float)rnd.Next((int)transform.position.z - 350, (int)transform.position.z + 350)) , Quaternion.Euler((float)rnd.Next(-30, 30), (float)rnd.Next(-120, -60), (float)rnd.Next(-30, 30)));
            GameObject table = GameObject.Find("clipboardtable");
            GameObject clipBoard = Instantiate(Resources.Load("Clipboard")) as GameObject;
            clipBoard.transform.position= table.transform.position+new Vector3(0,0.1f,0);
            clipBoard.transform.Rotate(new Vector3(1,0,0),10f);
            ClipboardScript script = clipBoard.AddComponent<ClipboardScript>();
            script.file = f;
        }
        
    }

}

class ClipboardScript : MonoBehaviour {
    public FileInfo file;

    public void OnMouseDown() {
        CSVDataObject csvData = CSVParser.loadCsv(file.FullName);
        DataController dc = new DataController(csvData);
        dc.generateGraphChart();
        if (csvData == null) {
            EditorUtility.DisplayDialog("Error","Your csv seems to be invalid. Atleast x and y are required","Ok");
        }
    }
}

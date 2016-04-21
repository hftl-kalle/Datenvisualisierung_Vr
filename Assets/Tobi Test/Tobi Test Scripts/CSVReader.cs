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

        for (int i = 0; i < files.Length; i++) {
            FileInfo f = files[i];
            GameObject inst = (GameObject) Instantiate(Resources.Load("Clipboard"), new Vector3(-150.0f + i * 150.0f,0.0f,-300.0f) , Quaternion.Euler(0.0f,270.0f,0.0f));
            ClipboardScript script = inst.AddComponent<ClipboardScript>();
            script.file = f;
        }
        
    }

}

class ClipboardScript : MonoBehaviour {
    public FileInfo file;

    void OnMouseDown() {
        CSVDataObject csvData = CSVParser.loadCsv(file.FullName);
    }
}

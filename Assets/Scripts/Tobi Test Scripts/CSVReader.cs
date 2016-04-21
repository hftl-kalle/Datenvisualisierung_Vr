using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class CSVReader : MonoBehaviour {

    public void openCsvFromFolder() {
        CSVDataObject csvData;
        string directory = @Application.dataPath + "/../csv/";
        Debug.Log(directory);

        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles("*.csv");

        foreach (FileInfo f in files) {
            Debug.Log(f.Name);
            csvData = CSVParser.loadCsv(f.FullName);
        }
        
    }

}

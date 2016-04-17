using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CSVReader : MonoBehaviour {

    public void openCsvFromFileChooser() {
        CSVDataObject csvData;
        string file = EditorUtility.OpenFilePanel("Choose csv", Environment.GetFolderPath(Environment.SpecialFolder.MyComputer), "csv");
        if (file.Length != 0) {
            csvData = CSVParser.loadCsv(file);

        } else {
            EditorUtility.DisplayDialog("Select csv", "You must select a valid csv", "Ok");
        }
    }

}

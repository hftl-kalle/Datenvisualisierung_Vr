using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public static class CSVParser {

    public static CSVDataObject loadCsv(string file) {

        if (File.Exists(file)) {

            string SEMICOLON_SANATZER = "/u003B/";

            string content = System.IO.File.ReadAllText(file);

            string[] lines = content.Replace('\r', '\n').Replace("\r\n", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string regex = "\"(.*?)\"";

            for (int i = 0; i < lines.GetLength(0); i++) {
                foreach (Match match in Regex.Matches(lines[i], regex))
                    lines[i] = lines[i].Replace(match.Value, match.Value.Replace(";", SEMICOLON_SANATZER));
            }
            int rowCount = lines.GetLength(0);
            int colCount = lines[0].Split(';').GetLength(0);
            string[,] data = new string[rowCount,colCount];

            for (int r = 0; r < rowCount; r++) {
                for (int c = 0; c < colCount; c++) {
                    data[r,c] = lines[r].Split(';')[c].Replace(SEMICOLON_SANATZER, ";");
                }
            }

            //<<<<<<<DEBUG
            for (int m = 0; m < data.GetLength(0); ++m) {
                for (int n = 0; n < data.GetLength(1); ++n) {
                    Debug.Log(data[m, n]);
                }  
            }

            return new CSVDataObject(file, data);

            //<<<<<<<DEBUG
        }

        return null;

    }

    
}

public class CSVDataObject {

    private int rowCount = 0;
    private int colCount = 0;
    private string file;
    private string[,] data;

    public CSVDataObject(string file, string[,] data) {
        this.data = data;
        this.file = file;
        this.rowCount = data.GetLength(0);
        this.colCount = data.GetLength(1);
    }

    public string[,] getData() {
        return data;
    }

    public int getRowCount() {
        return rowCount;
    }

    public int getColCount() {
        return colCount;
    }
}
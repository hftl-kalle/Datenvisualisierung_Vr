using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

public static class CSVParser {

    public static CSVDataObject loadCsv(string file) {

        if (File.Exists(file)) {

            string SEMICOLON_SANATIZER = "/u003B/";

            string content = System.IO.File.ReadAllText(file);
            string regex = "\"(.*?)\"";
            foreach (Match match in Regex.Matches(content, regex))
                content = content.Replace(match.Value, match.Value.Replace(";", SEMICOLON_SANATIZER));
            content = content.Replace("\"", "");

            string[] lines = content.Replace('\r', '\n').Replace("\r\n", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<MultidimensionalObject> dataList = new List<MultidimensionalObject>();
            for (int l = 0; l < lines.Length; l++) {
                object[] temp = new object[4];
                if (lines[l].Split(';').Length < 2 && lines[l].Length > 0) {
                    return null;
                }
                for (int i = 0; i < lines[l].Split(';').Length; i++) {
                    if (i >= 4) break;
                    float f;
                    if (float.TryParse(lines[l].Split(';')[i].Replace(SEMICOLON_SANATIZER, ";"), out f))
                        temp[i] = f;
                    else
                        temp[i] = lines[l].Split(';')[i].Replace(SEMICOLON_SANATIZER, ";");
                }
                dataList.Add(new MultidimensionalObject(temp[0],temp[1],temp[2],temp[3]));
            }

            MultidimensionalObject headlines = dataList[0];
            dataList.Remove(dataList[0]);

            CSVDataObject obj = new CSVDataObject(file, dataList, (string) headlines.getX(), (string) headlines.getY(), (string) headlines.getZ());
            Debug.Log(obj.toString());
            return obj;
        }
        return null;
    }
   
}

public class CSVDataObject {

    private List<MultidimensionalObject> dataList = new List<MultidimensionalObject>();
    private string headlineX;
    private string headlineY;
    private string headlineZ;
    private string file;

    public CSVDataObject(string file, List<MultidimensionalObject> data, string headX, string headY, string headZ) {
        this.dataList = data;
        this.file = file;
        this.headlineX = headX;
        this.headlineY = headY;
        this.headlineZ = headZ;
    }

    public List<MultidimensionalObject> getData() {
        return dataList;
    }

    public string[] getHeadlines() {
        return new string[3] { this.headlineX, this.headlineY, this.headlineZ };
    }

    public string toString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("x: ").Append(headlineX);
        sb.Append(" y: ").Append(headlineY);
        sb.Append(" z: ").Append(headlineZ).Append("\n");
        foreach (MultidimensionalObject obj3D in dataList)
            sb.Append(obj3D.toString()).Append("\n");
        return sb.ToString();
    }

}

public class MultidimensionalObject {
    private object x;
    private object y;
    private object z;
    private object range;

    public MultidimensionalObject(object x, object y, object z = null, object range = null) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.range = range;
    }

    public object getX() {
        return this.x;
    }

    public object getY() {
        return this.y;
    }

    public object getZ() {
        return this.z;
    }

    public object getRange() {
        return this.range;
    }

    public string toString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("x: ").Append(this.x != null ? this.x.ToString() : "Null");
        sb.Append(" y: ").Append(this.y != null ? this.y.ToString() : "Null");
        sb.Append(" z: ").Append(this.z != null ? this.z.ToString() : "Null");
        sb.Append(" range: ").Append(this.range != null ? this.range.ToString() : "Null");
        return sb.ToString();
    }
}
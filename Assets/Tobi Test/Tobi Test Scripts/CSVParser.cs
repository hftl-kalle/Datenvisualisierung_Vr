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

            List<Object3D> dataList = new List<Object3D>();
            for (int r = 0; r < lines.Length; r++) {
                dataList.Add(new Object3D(lines[r].Split(';')[0].Replace(SEMICOLON_SANATIZER, ";"),
                    lines[r].Split(';')[1].Replace(SEMICOLON_SANATIZER, ";"),
                    lines[r].Split(';')[2].Replace(SEMICOLON_SANATIZER, ";")));
            }

            Object3D headlines = dataList[0];
            dataList.Remove(dataList[0]);

            CSVDataObject obj = new CSVDataObject(file, dataList, (string) headlines.getX(), (string) headlines.getY(), (string) headlines.getZ());
            Debug.Log(obj.toString());
            return obj;
        }
        return null;
    }
   
}

public class CSVDataObject {

    private List<Object3D> dataList = new List<Object3D>();
    private string headlineX;
    private string headlineY;
    private string headlineZ;
    private string file;

    public CSVDataObject(string file, List<Object3D> data, string headX, string headY, string headZ) {
        this.dataList = data;
        this.file = file;
        this.headlineX = headX;
        this.headlineY = headY;
        this.headlineZ = headZ;
    }

    public List<Object3D> getData() {
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
        foreach (Object3D obj3D in dataList)
            sb.Append(obj3D.toString()).Append("\n");
        return sb.ToString();
    }

}

public class Object3D {
    private object x;
    private object y;
    private object z;

    public Object3D(object x, object y, object z) {
        this.x = x;
        this.y = y;
        this.z = z;
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

    public string toString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("x: ").Append( (string) x);
        sb.Append(" y: ").Append( (string) y);
        sb.Append(" z: ").Append( (string) z);
        return sb.ToString();
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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

    public List<object> getAllX() {
        List<object> objects = new List<object>();
        dataList.ForEach(x => objects.Add(x.getX()));
        return objects;
    }

    public List<object> getAllY() {
        List<object> objects = new List<object>();
        dataList.ForEach(x => objects.Add(x.getY()));
        return objects;
    }

    public List<object> getAllZ() {
        List<object> objects = new List<object>();
        dataList.ForEach(x => objects.Add(x.getZ()));
        return objects;
    }

    public List<object> getAllRange() {
        List<object> objects = new List<object>();
        dataList.ForEach(x => objects.Add(x.getRange()));
        return objects;
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSVDataObject {

    private List<MultidimensionalObject> dataList = new List<MultidimensionalObject>();
    private string headlineX;
    private string headlineY;
    private string headlineZ;
    private string headlineW;
    private string file;

    public CSVDataObject(string file, List<MultidimensionalObject> data, string headX, string headY, string headZ, string headW = null) {
        this.dataList = data;
        this.file = file;
        this.headlineX = headX;
        this.headlineY = headY;
        this.headlineZ = headZ;
        this.headlineW = headW;
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

    public List<object> getAllW() {
        List<object> objects = new List<object>();
        dataList.ForEach(x => objects.Add(x.getW()));
        return objects;
    }

    public string[] getHeadlines() {
        return new string[4] { this.headlineX, this.headlineY, this.headlineZ, this.headlineW };
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

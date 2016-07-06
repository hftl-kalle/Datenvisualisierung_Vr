using UnityEngine;
using System.Collections;
using System.Text;

public class MultidimensionalObject {

    private object x;
    private object y;
    private object z;
    private object w;

    public MultidimensionalObject(object x, object y, object z = null, object w = null) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
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

    public object getW() {
        return this.w;
    }

    public void setX(object x) {
        this.x = x;
    }

    public void setY(object y) {
        this.y = y;
    }
    public void setZ(object z) {
        this.z = z;
    }
    public void setW(object w) {
        this.w = w;
    }
    public object[] getObjectArray() {
        return new object[4] { this.x, this.y, this.z, this.w };
    }

    public string toString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("x: ").Append(this.x != null ? this.x.ToString() : "Null");
        sb.Append("\ny: ").Append(this.y != null ? this.y.ToString() : "Null");
        sb.Append("\nz: ").Append(this.z != null ? this.z.ToString() : "Null");
        sb.Append("\nw: ").Append(this.w != null ? this.w.ToString() : "Null");
        return sb.ToString();
    }
}

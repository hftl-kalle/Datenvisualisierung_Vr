using UnityEngine;
using System.Collections;
using System.Text;

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

    public void setX(object x) {
        this.x = x;
    }

    public void setY(object y) {
        this.y = y;
    }
    public void setZ(object z) {
        this.z = z;
    }
    public void setRange(object range) {
        this.range = range;
    }
    public object[] getObjectArray() {
        return new object[4] { this.x, this.y, this.z, this.range };
    }

    public string toString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("x: ").Append(this.x != null ? this.x.ToString() : "Null");
        if (this.x is string) sb.Append("\n is string");
        else sb.Append("\n is float");
        sb.Append("\ny: ").Append(this.y != null ? this.y.ToString() : "Null");
        if (this.y is string) sb.Append("\n is string");
        else sb.Append("\n is float");
        sb.Append("\nz: ").Append(this.z != null ? this.z.ToString() : "Null");
        if (this.z is string) sb.Append("\n is string");
        else sb.Append("\n is float");
        sb.Append("\nrange: ").Append(this.range != null ? this.range.ToString() : "Null");
        if (this.range is string) sb.Append("\n is string");
        else sb.Append("\n is float");
        return sb.ToString();
    }
}

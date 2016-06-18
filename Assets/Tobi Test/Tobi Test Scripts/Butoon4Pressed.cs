using UnityEngine;
using System.Collections;

public class Butoon4Pressed : MonoBehaviour {

    public void OnMouseDown() {
        if (GameObject.Find("chartParent") != null)
            ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createHeatMap();
    }
}

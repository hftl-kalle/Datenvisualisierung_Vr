using UnityEngine;
using System.Collections;

public class Button1Pressed : MonoBehaviour {

    public void OnMouseDown() {
        if (GameObject.Find("chartParent") != null)
            ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
    }
}

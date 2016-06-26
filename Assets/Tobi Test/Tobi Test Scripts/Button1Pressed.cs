using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Button1Pressed : ButtonPressed {

    public Renderer renderer;
    public Material activeMaterial;
    public Material inactiveMaterial;

    public void Start() {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = inactiveMaterial;
    }

    public void OnMouseDown() {
        activateButton();
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("1 pressed");
        activateButton();
    }

    public override void deactivateButton() {
        if (GameObject.Find("chartParent") != null) {
           // ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
            renderer.sharedMaterial = inactiveMaterial;
        }
    }

    public override void activateButton() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("holding_button")) {
            if (!go.Equals(gameObject)) {
                go.GetComponent<ButtonPressed>().deactivateButton();
            }
        }
        if (GameObject.Find("chartParent") != null) {
            ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createLineGraph();
            renderer.sharedMaterial = activeMaterial;
        }
        GameObject.Find("holding").GetComponent<InitHoldingObject>().getAnim().SetTrigger("Button1");
    }
}
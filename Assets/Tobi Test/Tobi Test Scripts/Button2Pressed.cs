using UnityEngine;
using System.Collections;

public class Button2Pressed : ButtonPressed {

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
        Debug.Log("2 pressed");
        activateButton();
    }

    public override void deactivateButton() {
            renderer.sharedMaterial = inactiveMaterial;
    }

    public override void activateButton() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("holding_button")) {
            if (!go.Equals(gameObject)) {
                go.GetComponent<ButtonPressed>().deactivateButton();
            }
        }
        if (GameObject.Find("chartParent") != null) {
            ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createMultiple2DGraphs();
            renderer.sharedMaterial = activeMaterial;
        }
        GameObject.Find("holding").GetComponent<InitHoldingObject>().getAnim().SetTrigger("Button2");
    }
}
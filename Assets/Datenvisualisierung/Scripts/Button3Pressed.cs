using UnityEngine;

public class Button3Pressed : ButtonPressed {

    public new Renderer renderer;
    public Material activeMaterial;
    public Material inactiveMaterial;

    /// <summary>
    /// initialize variabels
    /// </summary>
    public void Start() {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = inactiveMaterial;
    }

    /// <summary>
    /// activate the button on click
    /// </summary>
    public void OnMouseDown() {
        activateButton();
    }

    /// <summary>
    /// activate button on trigger with controller
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter(Collider coll)
    {
        activateButton();
    }

    /// <summary>
    /// deactivate the button, set material appropriate
    /// </summary>
    public override void deactivateButton() {
            renderer.sharedMaterial = inactiveMaterial;
    }

    /// <summary>
    /// activate the button, switching material and refresh diagramms
    /// </summary>
    public override void activateButton() {

        //deactivate all other buttons
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("holding_button")) {
            if (!go.Equals(gameObject)) {
                go.GetComponent<ButtonPressed>().deactivateButton();
            }
        }

        //set material for the button
        if (GameObject.Find("chartParent") != null) {
            ((DataController)GameObject.Find("chartParent").GetComponent(typeof(DataController))).createBiMap();
            renderer.sharedMaterial = activeMaterial;
        }

        //trigger the visualisation changes
        GameObject.Find("holding").GetComponent<InitHoldingObject>().getAnim().SetTrigger("Button3");
    }
}

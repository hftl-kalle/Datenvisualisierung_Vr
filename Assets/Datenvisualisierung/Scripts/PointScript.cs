using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Script attached to the Datapoints in the Diagrammview, Used to store the position aswell as Headlines and Values. 
/// Used to display the headline and values to the user.
/// </summary>
public class PointScript : MonoBehaviour {

    /// <summary>
    /// Data headlines [x,y,z,w]
    /// </summary>
    public string[] headlines = new string[4];
    /// <summary>
    /// Data values [x,y,z,w]
    /// </summary>
    public object[] data = new object[4];
    /// <summary>
    /// whether or not the fourth value should be displayed
    /// this value is used for additional information
    /// </summary>
    public bool showAdditionalData = false;

    /// <summary>
    /// show text information on click
    /// </summary>
    public void OnMouseDown() {
        toggleTextRenderer();
    }

    /// <summary>
    /// toggle the informational text for the sepcific data point
    /// </summary>
    private void toggleTextRenderer() {

        /// if the text is already displayed , disable it
        if (GameObject.Find(gameObject.GetInstanceID().ToString())) {
            GameObject.Destroy(GameObject.Find(gameObject.GetInstanceID().ToString()));
            return;
        }


        GameObject Canvas = GameObject.Find("Canvas");
        GameObject chartParent = GameObject.Find("chartParent");

        #region configure and display the text renderer
        GameObject textGO = new GameObject(gameObject.GetInstanceID().ToString());
        textGO.transform.parent = Canvas.transform;
        textGO.AddComponent<RectTransform>();
        Text textComponent = textGO.AddComponent<Text>();        
        textGO.transform.position = gameObject.transform.position - new Vector3(0.10f, 0, 0.03f);
        textGO.transform.rotation = Quaternion.LookRotation(transform.position - GameObject.Find("Camera (eye)").transform.position);
        textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 160);
        textGO.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        for (int i = 0; i <  headlines.Length - 1; i++) {
            if (headlines[i] != null && data[i] != null) textComponent.text = textComponent.text + Environment.NewLine + headlines[i] + ": " + data[i];
        }

        if (showAdditionalData) {
            if (headlines[3] != null && data[3] != null) textComponent.text = headlines[3] + ": " + data[3];
        }

        textComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        textComponent.fontSize = 33;
        #endregion
    }
}

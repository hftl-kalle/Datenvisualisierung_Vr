﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PointScript : MonoBehaviour {
    public string[] headlines = new string[4];
    public object[] data = new object[4];
    private bool active = false;
    public bool showAdditionalData = false;

    public void OnMouseDown() {
        toggleTextRenderer();
    }

    private void toggleTextRenderer() {
        if (GameObject.Find(gameObject.GetInstanceID().ToString())) {
            GameObject.Destroy(GameObject.Find(gameObject.GetInstanceID().ToString()));
            return;
        }
        /* GameObject tem = new GameObject(gameObject.GetInstanceID().ToString());
         tem.transform.position = gameObject.transform.position;
         MeshRenderer mr = tem.AddComponent<MeshRenderer>();
         TextMesh tm = tem.AddComponent<TextMesh>();
         tm.text = titleX + ": " + showX + "\n" + titleY + ": " + showY + "\n" + titleZ + ": " + showZ;
         tm.fontSize = 50;
         tm.characterSize = 12;*/
        GameObject Canvas = GameObject.Find("Canvas");
        GameObject textGO = new GameObject(gameObject.GetInstanceID().ToString());
        textGO.transform.parent = Canvas.transform;
        textGO.AddComponent<RectTransform>();
        Text textComponent = textGO.AddComponent<Text>();
        textGO.transform.position = gameObject.transform.position - new Vector3(0.2f, 0, 0.2f);
        textGO.transform.rotation = Quaternion.LookRotation(transform.position - GameObject.Find("Camera (eye)").transform.position);
        textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 160);
        textGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        for (int i = 0; i < ((showAdditionalData) ? headlines.Length : (headlines.Length - 1)); i++) {
            if (headlines[i] != null && data[i] != null) textComponent.text = textComponent.text + Environment.NewLine + headlines[i] + ": " + data[i];
        }
        textComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        textComponent.fontSize = 20;
    }
}

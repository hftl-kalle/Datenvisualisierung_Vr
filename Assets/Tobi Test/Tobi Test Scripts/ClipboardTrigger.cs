using UnityEngine;
using System.Collections;

public class ClipboardTrigger : MonoBehaviour {

    /*if clipboard moved inside box colldier
        move clipboard to final position inside the holding
        disable kinematic while doing so to reset applied forces
        */
    public void OnTriggerEnter(Collider other) {
        other.gameObject.GetComponent<ClipboardScript>().setHold(false);
        //load the csv;
        other.gameObject.GetComponent<ClipboardScript>().loadFile();
        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //position the clipboard inside the holding structure
        // rotation and position of cliboard depend on rotation of holder in scene.
        Debug.Log("Application.loadedLevelName: "+ Application.loadedLevelName);

        if (Application.loadedLevelName == "HMDScene")
        {
            other.gameObject.transform.position = gameObject.transform.position + new Vector3(+0.14f, 0.15f, 0.1f);
            other.gameObject.transform.rotation = Quaternion.Euler(0, 0, 44);
        }
        else
        {

            other.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0.15f, -0.22f);
            other.gameObject.transform.rotation = Quaternion.Euler(0, -18, 315);
        }

        other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    /* if the clipboard is moved out of the bx collider unload the csv and disable trigger for 2 seconds to allow moving the clipboard out of the way */
    IEnumerator OnTriggerExit(Collider other) {
        Debug.Log("Exit");
        other.gameObject.GetComponent<ClipboardScript>().unloadFile();
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
}

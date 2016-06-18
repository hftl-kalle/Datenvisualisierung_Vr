using UnityEngine;
using System.Collections;

public class ClipboardTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other) {
        other.gameObject.GetComponent<ClipboardScript>().setHold(false);
        //load;
        other.gameObject.GetComponent<ClipboardScript>().loadFile();
        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        other.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0.15f, -0.22f);
        other.gameObject.transform.rotation = Quaternion.Euler(0, -18, 315);
        other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    IEnumerator OnTriggerExit(Collider other) {
        Debug.Log("Exit");
        other.gameObject.GetComponent<ClipboardScript>().unloadFile();
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
}

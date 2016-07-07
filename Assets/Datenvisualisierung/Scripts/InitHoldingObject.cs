using UnityEngine;

public class InitHoldingObject : MonoBehaviour {

    public Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }

    public Animator getAnim() {
        return anim;
    }
	

}

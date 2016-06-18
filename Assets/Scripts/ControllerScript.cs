using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerScript : MonoBehaviour
{
    public GameObject currentCollisionObject;
    public Rigidbody attachPoint;
    public Vector3 currentCollisionPoint;
    Transform attachTransfrom = null;
    Vector3 currPosition, lastPosition;
    public float scaleValue=0.5f;
    public float xyzscale = 2;


    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        lastPosition = transform.position;
        currPosition = transform.position;
    }

    void Start()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        currentCollisionObject = coll.gameObject;
        try
        {
            currentCollisionObject.GetComponent<Renderer>().material.shader = Shader.Find("Custom/TestShader");
        }
        catch
        {
            currentCollisionObject.transform.Find("Cube").GetComponent<Renderer>().material.shader = Shader.Find("Custom/TestShader");
        }
        
    }

    void OnTriggerExit(Collider coll)
    {
        try
        {
            if(currentCollisionObject.tag=="axis") currentCollisionObject.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
            else currentCollisionObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        }
        catch
        {
            currentCollisionObject.transform.Find("Cube").GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        }
        currentCollisionObject = null;
    }

    void OnCollisionEnter(Collision coll)
    {
        ContactPoint contactPoint = coll.contacts[0];
        currentCollisionPoint=contactPoint.point; //this is the Vector3 position of the point of contact
    }


    void FixedUpdate()
    {
        lastPosition = currPosition;
        currPosition = transform.position;

        if (!attachTransfrom)
        {
            try
            {
                attachTransfrom = transform.FindChild("Model").FindChild("tip").FindChild("attach");
                Rigidbody rigidbody = attachTransfrom.gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                attachPoint = rigidbody;
            }
            catch
            {
                Debug.Log("did not find transform or something");
            }

        }

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        // check if clipboard is attached and if the trackpad is pressed. if so create 
        if (joint != null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            ClipboardScript script = joint.gameObject.GetComponent<ClipboardScript>();
            script.loadFile();
        }

        // check if trigger is pressed
        if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            // if a clipboard is found to be collided with pick it up and create a joint
            if (currentCollisionObject && currentCollisionObject.tag == "clipboard")
            {
                currentCollisionObject.transform.position = attachPoint.transform.position- (attachPoint.transform.position- currentCollisionObject.transform.position);

                joint = currentCollisionObject.AddComponent<FixedJoint>();
                joint.connectedBody = attachPoint;
            }else{
                //if a point in a cloud is found display text
                if(currentCollisionObject&& currentCollisionObject.tag == "pointInCloud")
                {
                    currentCollisionObject.GetComponent<PointScript>().OnMouseDown();
                }
                else
                {
                    if (currentCollisionObject && currentCollisionObject.tag == "axis")
                    {
                        GameObject chartParent = GameObject.Find("chartParent");
                        Vector3 heading = lastPosition - currPosition;
                        heading = heading / heading.magnitude;
                        //check which axis
                        BoxCollider bc = currentCollisionObject.GetComponent<BoxCollider>();
                        DataController dc = currentCollisionObject.GetComponent<DataController>();
                        if (bc.size.x == 2)
                        {                            
                            dc.setScale (new Vector3(heading.x,0,0));
                            //chartParent.transform.FindChild("TerrainObj").GetComponent<Terrain>().terrainData.size=new();
                        }
                        else
                        {
                            if (bc.size.y == 2)
                            {
                                dc.setScale (new Vector3(0,heading.y, 0));
                            }
                            else
                            {
                                dc.setScale (new Vector3(0,0, heading.z));
                            }
                        }
                    }
                }
            }
        }
        else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            // destroy joint
            var go = joint.gameObject;
            var rigidbody = go.GetComponent<Rigidbody>();
            Object.DestroyImmediate(joint);
            joint = null;

            // add force after letting go
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
                rigidbody.velocity = device.velocity;
                rigidbody.angularVelocity = device.angularVelocity;
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
        }
    }
}

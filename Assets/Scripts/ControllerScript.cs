using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerScript : MonoBehaviour
{
    public GameObject currentCollisionObject,scalingAxis;
    public Rigidbody attachPoint;
    public Vector3 currentCollisionPoint;
    Transform attachTransfrom = null;
    Vector3 currPosition, lastPosition;
    public float scaleValue=0.5f;
    public float xyzscale = 2;
    bool scaling = false;


    SteamVR_TrackedObject trackedObj;
    public FixedJoint joint;

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
            if (currentCollisionObject.tag == "axis") currentCollisionObject.GetComponent<Renderer>().material.shader = Shader.Find("Custom/TestShader");
            else currentCollisionObject.transform.Find("Cube").GetComponent<Renderer>().material.shader = Shader.Find("Custom/TestShader");
        }
        catch
        {
        }
        
    }

    void OnTriggerExit(Collider coll)
    {
        try
        {
            if(currentCollisionObject.tag=="axis") currentCollisionObject.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
            else currentCollisionObject.transform.Find("Cube").GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        }
        catch
        {
            
        }
        currentCollisionObject = null;
    }

    void OnCollisionEnter(Collision coll)
    {
        ContactPoint contactPoint = coll.contacts[0];
        currentCollisionPoint=contactPoint.point; //this is the Vector3 position of the point of contact
    }

    public void destroyJoint()
    {
        var go = joint.gameObject;
        Object.Destroy(joint);
        joint = null;
    }

    void FixedUpdate()
    {
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
        if (joint==null&&device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            GameObject chartParent = GameObject.Find("chartParent");
            chartParent.transform.position = attachPoint.transform.position - (attachPoint.transform.position - chartParent.transform.position);
            joint = chartParent.AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;
        }
        if (joint&& device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            // destroy joint
            var go = joint.gameObject;
            var rigidbody = go.GetComponent<Rigidbody>();
            DataController dc = go.GetComponent<DataController>();
            Object.DestroyImmediate(joint);
            joint = null;
            Destroy(rigidbody);
            dc.reRender();            
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
                        Debug.Log("Found Axis");
                        if (!scaling)
                        {
                            scalingAxis = currentCollisionObject;
                            lastPosition = currPosition;
                            scaling = true;
                            Debug.Log("Scale on");
                        }
                    }
                }
            }
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (scaling) {
                Vector3 scaleVec;
                GameObject chartParent = GameObject.Find("chartParent");
                Vector3 heading = lastPosition - currPosition;
                Debug.Log("lastPostion: "+lastPosition+ " currPostion: "+currPosition);
                Debug.Log("heading: " + heading);
                heading*=1.5f;
                heading=chartParent.transform.InverseTransformDirection(heading);
                Debug.Log("heading: " + heading);
                //check which axis
                BoxCollider bc = scalingAxis.GetComponent<BoxCollider>();
                DataController dc = chartParent.GetComponent<DataController>();
                if (bc.gameObject.name == "Xaxis")
                {
                    scaleVec = new Vector3(-heading.x, 0, 0);
                    Debug.Log("scaling x: "+scaleVec);
                    dc.setScale(scaleVec);
                    //chartParent.transform.FindChild("TerrainObj").GetComponent<Terrain>().terrainData.size=new();
                }
                else
                {
                    if (bc.gameObject.name == "Yaxis")
                    {
                        scaleVec = new Vector3(0, -heading.y, 0);
                        Debug.Log("scaling y: " + scaleVec);
                        dc.setScale(scaleVec);
                    }
                    else
                    {
                        scaleVec = new Vector3(0, 0, heading.z);
                        Debug.Log("scaling z: "+ scaleVec);
                        dc.setScale(scaleVec);
                    }
                }
                Debug.Log("Scale Off");
                scaling = false;
            }
            if (joint != null)
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
}

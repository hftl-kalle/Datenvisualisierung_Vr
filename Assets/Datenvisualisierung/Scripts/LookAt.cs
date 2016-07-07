using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Rotate every frame so it keeps looking at the target 
        if(target) transform.rotation = Quaternion.LookRotation(transform.position -target.position);
       
    }
}
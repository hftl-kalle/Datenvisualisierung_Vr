using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Rotate every frame so it keeps looking at the target 
        if(target) transform.LookAt(target);
    }
}
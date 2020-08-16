using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public int direction = -1;
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * (rotateSpeed* direction));     
    }
}

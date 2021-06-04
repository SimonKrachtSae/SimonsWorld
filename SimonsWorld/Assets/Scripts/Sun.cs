using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public float ShiftTime = 0;
    public GameObject world;
    private void Start()
    {

    }
    private void Update()
    {
        transform.RotateAround(world.transform.position, Vector3.forward, 5);
    }

}

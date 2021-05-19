using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        int MoveSpeed = 3;
        transform.position += (transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * MoveSpeed;

        if(Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * Time.deltaTime * MoveSpeed;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * Time.deltaTime * MoveSpeed;
        }


    }
}

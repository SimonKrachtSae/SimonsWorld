using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rigidBody;
    private bool isGrounded;
    void Start()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
    }
    void Update()
    {
        int MoveSpeed = 3;
        transform.position += (transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * MoveSpeed;


       
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 250);
            isGrounded = false;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Block"))
        {
            isGrounded = true;
        }
    }
}

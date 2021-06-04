using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuilder : MonoBehaviour
{
    private List<Cube> inventoryCubes = new List<Cube>();
    [SerializeField] private GameObject kamera;
    private World world;
    [SerializeField] private AudioSource buildSound;

    private void Start()
    {
        world = World.Instance;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }
        if (Input.GetMouseButtonDown(1))
        {
            BuildBlock();
        }
    }
    void BuildBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(kamera.transform.position, kamera.transform.forward * 5, out hit))
        {
            if (hit.collider.CompareTag("Block"))
            {
                buildSound.Play();
                Vector3 position = hit.collider.transform.position;
                Vector3 direction = hit.normal.normalized;
                int x = Mathf.RoundToInt(position.x + direction.x);
                int y = Mathf.RoundToInt(position.y + direction.y);
                int z = Mathf.RoundToInt(position.z + direction.z);
                
                world.CreateBlock(x, y,z);
            }
        }
    }
    void DestroyBlock()
    {

        RaycastHit hit;
        if (Physics.Raycast(kamera.transform.position, kamera.transform.forward * 5, out hit))
        {
            buildSound.Play();
            if(hit.collider.CompareTag("Block"))
            {
                world.DestroyBlock(hit.collider.gameObject.GetComponent<Cube>());
            }
        }
    }
}

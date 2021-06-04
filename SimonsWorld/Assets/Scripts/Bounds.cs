using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{

    private void Start()
    {
        PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        int scale = perlinNoise.GetWorldSize();
        boxCollider.size = new Vector3(scale + 2,50, scale + 2);
        int halfScale = Mathf.RoundToInt(scale / 2.0f);
        boxCollider.center = new Vector3(halfScale, halfScale, halfScale);
    }
    private void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }
}

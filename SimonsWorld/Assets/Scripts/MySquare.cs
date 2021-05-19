using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySquare : MonoBehaviour
{
    public Vector3[] Corners;
    public Vector3 TopLeft;
    public Vector3 TopRight;
    public Vector3 BottomLeft;
    public Vector3 BottomRight;

    private List<Vector3> Vertices = new List<Vector3>();
    private List<int> Indices = new List<int>();

    private void Awake()
    {
        TopLeft = transform.position + Vector3.up * 0.5f + Vector3.left * 0.5f;
        TopRight = transform.position + Vector3.up * 0.5f + Vector3.right * 0.5f;
        BottomLeft = transform.position + Vector3.down * 0.5f + Vector3.left * 0.5f;
        BottomRight = transform.position + Vector3.down * 0.5f + Vector3.right * 0.5f;

        Corners = new Vector3[] { BottomLeft, TopLeft, TopRight, BottomRight };
    }
    void Start()
    {
        GenerateMesh();
    }
    void GenerateMesh()
    {
        CreateTriangle(1, 2, 3);
        CreateTriangle(0, 1, 3);
        Vertices.Add(BottomLeft);
        Vertices.Add(TopLeft);
        Vertices.Add(TopRight);
        Vertices.Add(BottomRight);
        Mesh mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Indices.ToArray();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void CreateTriangle(int a, int b, int c)
    {
        Indices.Add(a);
        Indices.Add(b);
        Indices.Add(c);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCube : MonoBehaviour
{
    public List<MyCube> surroundingCells;

    [SerializeField] private protected Vector2Int m_NumberOfTexturesPerRow;

    [SerializeField, Tooltip("front, top, back, left, right,bottom")] public List<int> m_NumberOfTexture;

    private protected List<Vector2> m_UVs;

    public int X_Index;
    public int Y_Index;
    public int Z_Index;

    public bool hasNode = false;
    public Node nodeOnTop;

    public  Vector3 Position;
            
    private protected Vector3 FrontTopLeft;
    private protected Vector3 FrontTopRight;
    private protected Vector3 FrontBottomLeft;
    private protected Vector3 FrontBottomRight;
            
    private protected Vector3 BackTopLeft;
    private protected Vector3 BackTopRight;
    private protected Vector3 BackBottomLeft;
    private protected Vector3 BackBottomRight;
            
    private protected List<Vector3> Vertices = new List<Vector3>();
    private protected List<int> Indices = new List<int>();

    private protected void Awake()
    {
        FrontTopLeft = Vector3.up * 0.5f + Vector3.left * 0.5f + Vector3.forward * 0.5f;
        FrontTopRight = Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        FrontBottomLeft = Vector3.down * 0.5f + Vector3.left * 0.5f + Vector3.forward * 0.5f;
        FrontBottomRight = Vector3.down * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;

        BackTopLeft = Vector3.up * 0.5f + Vector3.left * 0.5f + Vector3.back * 0.5f;
        BackTopRight = Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.back * 0.5f;
        BackBottomLeft = Vector3.down * 0.5f + Vector3.left * 0.5f + Vector3.back * 0.5f;
        BackBottomRight = Vector3.down * 0.5f + Vector3.right * 0.5f + Vector3.back * 0.5f;
    }
    private protected void OnDestroy()
    {
        for(int i = 0; i <surroundingCells.Count;i++)
        {
            surroundingCells[i].surroundingCells.Remove(this);
            surroundingCells[i].GenerateMesh(); 
        }
    }
    public virtual void GenerateMesh()
    {
        Position = new Vector3(X_Index, Y_Index, Z_Index);

        Vertices = new List<Vector3>();
        Indices = new List<int>();
        m_UVs = new List<Vector2>();

        

        //Front
        if (!SurroundingCellsContains(X_Index, Y_Index, Z_Index + 1))
        { 
            Vertices.Add(FrontBottomRight);
            Vertices.Add(FrontTopRight);
            Vertices.Add(FrontTopLeft);
            Vertices.Add(FrontBottomLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[0]);
        }

        //Top
        if (!SurroundingCellsContains(X_Index, Y_Index + 1,Z_Index))
        {
            Vertices.Add(FrontTopRight);
            Vertices.Add(BackTopRight);
            Vertices.Add(BackTopLeft);
            Vertices.Add(FrontTopLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[1]);    
        }

        //Back
        if (!SurroundingCellsContains(X_Index, Y_Index, Z_Index - 1))
        { 
            Vertices.Add(BackBottomLeft);
            Vertices.Add(BackTopLeft);
            Vertices.Add(BackTopRight);
            Vertices.Add(BackBottomRight);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[2]);    
        }


        //Left
        if (!SurroundingCellsContains(X_Index - 1, Y_Index, Z_Index))
        {
            Vertices.Add(FrontBottomLeft);
            Vertices.Add(FrontTopLeft);
            Vertices.Add(BackTopLeft);
            Vertices.Add(BackBottomLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[3]);    
        }

        //Right
        if (!SurroundingCellsContains(X_Index + 1, Y_Index, Z_Index))
        {
            Vertices.Add(BackBottomRight);
            Vertices.Add(BackTopRight);
            Vertices.Add(FrontTopRight);
            Vertices.Add(FrontBottomRight);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[4]);    
        }

        //Bottom
        if (!SurroundingCellsContains(X_Index, Y_Index - 1, Z_Index))
        {
            Vertices.Add(BackBottomRight);
            Vertices.Add(FrontBottomRight);
            Vertices.Add(FrontBottomLeft);
            Vertices.Add(BackBottomLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[5]);    
        }
        Mesh mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Indices.ToArray();

        mesh.uv = m_UVs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    private protected void CalculateUVs(int textureNumber)
    {
        float uSize = 1.0f / m_NumberOfTexturesPerRow.x;
        float vSize = 1.0f / m_NumberOfTexturesPerRow.y;

        float vStart = (textureNumber / m_NumberOfTexturesPerRow.x) * uSize;
        float uStart = (textureNumber % m_NumberOfTexturesPerRow.x) * vSize;

        m_UVs.Add(new Vector2(uStart, vStart));
        m_UVs.Add(new Vector2(uStart, vStart + vSize));
        m_UVs.Add(new Vector2(uStart + uSize, vStart + vSize));
        m_UVs.Add(new Vector2(uStart + uSize, vStart));
    }
    private protected void CalculateIndices()
    {
        int count = Vertices.Count;

        Indices.Add(count - 4);
        Indices.Add(count - 3);
        Indices.Add(count - 2);

        Indices.Add(count - 2);
        Indices.Add(count - 1);
        Indices.Add(count - 4);
    }
    private bool SurroundingCellsContains(int X_Index, int Y_Index, int Z_Index)
    {
        for(int i = 0; i < surroundingCells.Count; i++)
        {
            if(surroundingCells[i].X_Index == X_Index && surroundingCells[i].Y_Index == Y_Index && surroundingCells[i].Z_Index == Z_Index)
            {
                return true;
            }
        }
        return false;
    }
    private protected MyCube GetSurroundingCell(int X_Index, int Y_Index, int Z_Index)
    {
        MyCube cube = null;
        for (int i = 0; i < surroundingCells.Count; i++)
        {
            if (surroundingCells[i].X_Index == X_Index && surroundingCells[i].Y_Index == Y_Index && surroundingCells[i].Z_Index == Z_Index)
            {
                cube = surroundingCells[i];
            }
        }
        return cube;
    }
}

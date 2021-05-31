using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWaterCube : MyCube
{
    public List<MyWaterCube> surroundingWaterCubes = new List<MyWaterCube>();
    public override void GenerateMesh()
    {
        Position = new Vector3(X_Index, Y_Index, Z_Index);

        Vertices = new List<Vector3>();
        Indices = new List<int>();
        m_UVs = new List<Vector2>();



        //Front
        if (!SurroundingWaterCellsContains(X_Index, Y_Index, Z_Index + 1))
        {
            Vertices.Add(FrontBottomRight);
            Vertices.Add(FrontTopRight);
            Vertices.Add(FrontTopLeft);
            Vertices.Add(FrontBottomLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[0]);
        }

        //Top
        if (!SurroundingWaterCellsContains(X_Index, Y_Index + 1, Z_Index))
        {
            Vertices.Add(FrontTopRight);
            Vertices.Add(BackTopRight);
            Vertices.Add(BackTopLeft);
            Vertices.Add(FrontTopLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[1]);
        }

        //Back
        if (!SurroundingWaterCellsContains(X_Index, Y_Index, Z_Index - 1))
        {
            Vertices.Add(BackBottomLeft);
            Vertices.Add(BackTopLeft);
            Vertices.Add(BackTopRight);
            Vertices.Add(BackBottomRight);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[2]);
        }


        //Left
        if (!SurroundingWaterCellsContains(X_Index - 1, Y_Index, Z_Index))
        {
            Vertices.Add(FrontBottomLeft);
            Vertices.Add(FrontTopLeft);
            Vertices.Add(BackTopLeft);
            Vertices.Add(BackBottomLeft);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[3]);
        }

        //Right
        if (!SurroundingWaterCellsContains(X_Index + 1, Y_Index, Z_Index))
        {
            Vertices.Add(BackBottomRight);
            Vertices.Add(BackTopRight);
            Vertices.Add(FrontTopRight);
            Vertices.Add(FrontBottomRight);
            CalculateIndices();
            CalculateUVs(m_NumberOfTexture[4]);
        }

        //Bottom
        if (!SurroundingWaterCellsContains(X_Index, Y_Index - 1, Z_Index))
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
    private bool SurroundingWaterCellsContains(int X_Index, int Y_Index, int Z_Index)
    {
        for (int i = 0; i < surroundingWaterCubes.Count; i++)
        {
            if (surroundingWaterCubes[i].X_Index == X_Index && surroundingWaterCubes[i].Y_Index == Y_Index && surroundingWaterCubes[i].Z_Index == Z_Index)
            {
                return true;
            }
        }
        return false;
    }
}

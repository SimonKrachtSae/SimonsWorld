using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCube : Cube
{
    public List<WaterCube> surroundingWaterCubes = new List<WaterCube>();
    public override void GenerateMesh()
    {

        Vertices = new List<Vector3>();
        Indices = new List<int>();
        UVs = new List<Vector2>();


        World world = World.Instance;
        //Front
        int scale = world.GetScale();

        if (Z_Index + 1 < scale)
        {
            if (world.GetWorldMapAtPos(X_Index, Y_Index, Z_Index + 1) != 5)
            {
                GenerateFront();
            }
        }
        if(Z_Index == scale - 1)
        {
            GenerateFront();
        }

        //Top
        if (Y_Index + 1 < scale)
        {
            if (world.GetWorldMapAtPos(X_Index, Y_Index + 1, Z_Index) != 5)
            {
                GenerateTop();
            }

        }
        if (Y_Index == scale - 1)
        {
            GenerateTop();
        }

        //Back
        if (Z_Index - 1 >= 0)
        {
            if (world.GetWorldMapAtPos(X_Index, Y_Index, Z_Index - 1) != 5)
            {
                GenerateBack();
            }
        }

        if (Z_Index == 0 )
        {
            GenerateBack();
        }

        //Left
        if (X_Index - 1 >= 0)
        {
            if (world.GetWorldMapAtPos(X_Index - 1, Y_Index, Z_Index) != 5)
            {
                GenerateLeft();
            }
        }
        if (X_Index == 0)
        {
            GenerateLeft();
        }

        //Right
        if (X_Index + 1 < scale)
        {
            if (world.GetWorldMapAtPos(X_Index + 1, Y_Index, Z_Index) != 5)
            {
                GenerateRight();
            }
        }
        if (X_Index == scale - 1)
        {
            GenerateRight();
        }

        //Bottom
        if (Y_Index - 1 >= 0)
        {
            if (world.GetWorldMapAtPos(X_Index, Y_Index - 1, Z_Index) != 5)
            {
                GenerateBottom();
            }

        }
        if (Y_Index == 0)
        {
            GenerateBottom();
        }

            Mesh mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Indices.ToArray();

        mesh.uv = UVs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void GenerateFront()
    {
        Vertices.Add(FrontBottomRight);
        Vertices.Add(FrontTopRight);
        Vertices.Add(FrontTopLeft);
        Vertices.Add(FrontBottomLeft);
        CalculateIndices();
        CalculateUVs(numberOfTexture[0]);
    }
    private void GenerateBack()
    {
        Vertices.Add(BackBottomLeft);
        Vertices.Add(BackTopLeft);
        Vertices.Add(BackTopRight);
        Vertices.Add(BackBottomRight);
        CalculateIndices();
        CalculateUVs(numberOfTexture[2]);
    }
    private void GenerateLeft()
    {
        Vertices.Add(FrontBottomLeft);
        Vertices.Add(FrontTopLeft);
        Vertices.Add(BackTopLeft);
        Vertices.Add(BackBottomLeft);
        CalculateIndices();
        CalculateUVs(numberOfTexture[3]);
    }
    private void GenerateRight()
    {
        Vertices.Add(BackBottomRight);
        Vertices.Add(BackTopRight);
        Vertices.Add(FrontTopRight);
        Vertices.Add(FrontBottomRight);
        CalculateIndices();
        CalculateUVs(numberOfTexture[4]);
    }
    private void GenerateTop()
    {
        Vertices.Add(FrontTopRight);
        Vertices.Add(BackTopRight);
        Vertices.Add(BackTopLeft);
        Vertices.Add(FrontTopLeft);
        CalculateIndices();
        CalculateUVs(numberOfTexture[1]);
    }
    private void GenerateBottom()
    {
        Vertices.Add(BackBottomRight);
        Vertices.Add(FrontBottomRight);
        Vertices.Add(FrontBottomLeft);
        Vertices.Add(BackBottomLeft);
        CalculateIndices();
        CalculateUVs(numberOfTexture[5]);
    }
}

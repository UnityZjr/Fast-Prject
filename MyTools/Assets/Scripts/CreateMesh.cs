using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "testMesh";
        //顶点数组
        mesh.vertices = GetVertices();
        //三角形序列
        mesh.triangles = GetTriangles();
        mesh.uv = GetUV();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        
    }

    private Vector3[] GetVertices()
    {
        return new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0)

        };

    }
    private int[] GetTriangles()
    {
        return new int[]
        {
            0,1,2,
            2,1,3

        };
    }
    private Vector2[] GetUV()
    {
        return new Vector2[] 
        {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,1),
        new Vector2(1,0)
        };

       
    }

}

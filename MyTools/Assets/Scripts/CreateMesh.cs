using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    [Tooltip("测试材质球")]
    [SerializeField]
    private Material mat;

    [Tooltip("法向量")]
    [SerializeField]
    private Vector3[] normals;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "testMesh";
        //顶点数组
        mesh.vertices = GetVertices();
        //三角形序列
        mesh.triangles = GetTriangles();

        mesh.normals = GetNormals();
        mesh.uv = GetUV();


     
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = mat;

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
    private Vector3[] GetNormals()
    {
        if (normals.Length != 0) return normals;
        return new Vector3[]
        {
           Vector3.up,
            Vector3.up,
             Vector3.up,
              Vector3.up,
        };

    }
    private Vector2[] GetUV()
    {
        return new Vector2[] 
        {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1)
        };

       
    }

}

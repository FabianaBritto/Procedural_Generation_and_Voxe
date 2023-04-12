using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh : MonoBehaviour
{
    UnityEngine.Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Color[] colors;
    public int xSize = 20;
    public int zSize = 20;

    public float height;
    public float entropy;
    public Gradient gradient;
    [SerializeField] private bool actualize;
    void Start()
    {
        mesh = new UnityEngine.Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateShape();
    }
    private void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float noiseX = (x / (float)xSize) * entropy;
                float noiseZ = (z / (float)zSize) * entropy;
                float y = Mathf.PerlinNoise(noiseX, noiseZ) * height;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        //Texture
        /*
        uvs = new Vector2[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2(x / (float)xSize, z / (float)zSize);
                i++;
            }
        }
        */

        //VertexColor
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float _height = vertices[i].y / height;

                colors[i] = gradient.Evaluate(_height);
                i++;
            }
        }

        // HARD CODANDO
        /*      
        vertices = new Vector3[]{
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1)
        };
        triangles = new int[]{
            0, 1, 2,
            1, 3, 2
        };
        */
    }
    private void Update()
    {
        if (actualize)
        {
            CreateShape();
            UpdateShape();
        }
    }
    private void UpdateShape()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        // Texture
        // mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }
}

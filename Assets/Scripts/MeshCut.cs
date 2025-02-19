using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshCut : MonoBehaviour
{
    void Start()
    {
        CreateSplitMesh();
    }

    void CreateSplitMesh()
    {
        Mesh mesh = new Mesh();

        // Define vertices for the quad
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-1, 0, -1), // Bottom left
            new Vector3(1, 0, -1),  // Bottom right
            new Vector3(1, 0, 1),   // Top right
            new Vector3(-1, 0, 1)   // Top left
        };

        // Define triangles for two triangles (0-1-2 and 0-2-3)
        int[] triangles = new int[6]
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals(); // Calculate normals for lighting

        GetComponent<MeshFilter>().mesh = mesh;
    }
}

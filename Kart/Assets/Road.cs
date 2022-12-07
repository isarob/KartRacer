

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Road : MonoBehaviour
{
    public CompositeBeizer bezier;
    [Range(0.2f, 10.0f)] 
    public float spacing = 0.5f;
    [Range(0f, 15f)]
    public float cleanup = 0.5f;
    public GameObject road;

    float prevSpacing;
    float prevCleanup;

    public void GenerateRoad() 
    {

        List<Vector3> points = bezier.GetPoints(spacing);
        List<Vector3> tangents = bezier.GetTangents(spacing);
        List<Vector3> ups = bezier.GetUps(spacing);
        //GenerateCubesAtVectors(points, tangents);

        GenerateRoadSurface(points,  ups,tangents, 4);
    }
public void GenerateRoadSurface(List<Vector3> points, List<Vector3> ups, List<Vector3> tangents, float width) 
{
    MeshFilter filter;
    MeshRenderer renderer;
    MeshCollider collider;
    if(road == null)
    {
      road = new GameObject("Road");
    road.transform.parent = this.transform;  
    filter = road.AddComponent<MeshFilter>();
    renderer = road.AddComponent<MeshRenderer>();
    collider = road.AddComponent<MeshCollider>();
    }else{
    
    filter = road.GetComponent<MeshFilter>();
    renderer = road.GetComponent<MeshRenderer>();
    collider = road.GetComponent<MeshCollider>();
    }

    Mesh mesh = new Mesh();
    filter.mesh = mesh;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();

    for (int i = 0; i < points.Count; i++) 
    {
        Vector3 side = Vector3.Cross(tangents[i], ups[i]).normalized * (width / 2f);
        Vector3 v1 = points[i] + side;
        Vector3 v2 = points[i] - side;

             // Ignore points if the difference between the current tangent and one of the previous/next is too large
        if (i > 1) 
        {
            Vector3 prevDiff = tangents[i] - tangents[i-1];
            if( prevDiff.magnitude > cleanup) 
            {
                continue;
            }
        }

        verts.Add(v1);
        verts.Add(v2);

        if(i > 0) 
        {
            // Connecting the previous point and current point
            int prevIndexV1 = verts.Count - 4;
            int prevIndexV2 = verts.Count - 3;
            int currIndexV1 = verts.Count - 2;
            int currIndexV2 = verts.Count - 1;

            tris.Add(prevIndexV1);
            tris.Add(currIndexV1); 
            tris.Add(prevIndexV2); 
            tris.Add(currIndexV1); 
            tris.Add(currIndexV2);
            tris.Add(prevIndexV2);
        }
    }

    mesh.vertices = verts.ToArray();
    mesh.triangles = tris.ToArray(); 
    mesh.RecalculateNormals(); 
    collider.sharedMesh = mesh;


}
   
public void GenerateCubesAtVectors(List<Vector3> vectors, List<Vector3> tangents) 
{
    if(road != null)
    {
        road.SetActive(false);
        DestroyImmediate(road);

    }
    road = new GameObject("Road");
    road.transform.parent = this.transform;

    for (int i = 0; i < vectors.Count; i++) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = vectors[i];
        cube.transform.rotation = Quaternion.LookRotation(tangents[i]);
        Debug.DrawLine(vectors[i], vectors[i]+tangents[i]*5, Color.red, 10);
        cube.transform.parent = road.transform;
    }
}

public void GenerateCubesAtVectors(List<Vector3> vectors, List<Quaternion> rotations) 
{
    if(road != null)
    {
        DestroyImmediate(road);
        road.SetActive(false);
    }
    road = new GameObject("Road");
    road.transform.parent = this.transform;

    for (int i = 0; i < vectors.Count; i++) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = vectors[i];
        cube.transform.rotation = rotations[i];
        cube.transform.parent = road.transform;
    }
}

    void OnEnable() 
    {
        bezier = GetComponent<CompositeBeizer>();
        GenerateRoad();
    }

    void OnValidate() 
    {
        if (spacing != prevSpacing) {
            prevSpacing = spacing;
            GenerateRoad();
        }
        if(cleanup != prevCleanup) {
            prevCleanup = cleanup;
            GenerateRoad();
            

        }
        

    }

public void DeleteRoads() 
    {
        foreach (Transform child in transform) 
        {
            if (child.name == "Road") 
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

}
[CustomEditor(typeof(Road))]
class RoadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate Road"))
            ((Road)target).GenerateRoad();
        if (GUILayout.Button("Delete Road"))
            ((Road)target).DeleteRoads();

        DrawDefaultInspector();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class BeizerCurve : MonoBehaviour
{
    public GameObject start;
    public GameObject control1;
    public GameObject control2;
    public GameObject end;
    public LineRenderer lr;
    public GameObject TMPObject;
    public Camera cam;
    private bool showKnobs = true;
    public bool useRotation;
    

    //static functions
        public static Vector3 CalculateBeizerCurve(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float interpolationRatio)
    {
        Vector3 point1 = Vector3.Lerp(startPoint, control1, interpolationRatio);
        Vector3 point2 = Vector3.Lerp(control1, control2, interpolationRatio);
        Vector3 point3 = Vector3.Lerp(control2, endPoint, interpolationRatio);

        Vector3 midPoint1 = Vector3.Lerp(point1, point2, interpolationRatio);
        Vector3 midPoint2 = Vector3.Lerp(point2, point3, interpolationRatio);

        return Vector3.Lerp(midPoint1, midPoint2, interpolationRatio);
    }

    public static float GetLength(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint) 
    {
    float totalLength = 0f;

    Vector3 currentPosition = startPoint;
    Vector3 lastPosition = startPoint;

    // Iterate through each point on the bezier curve.
    for (int i = 1; i <= 20; ++i) {
        float t = i * 0.05f;  
        currentPosition = CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
        totalLength += Vector3.Distance(currentPosition, lastPosition);
        lastPosition = currentPosition;
    }

    return totalLength;
    }

    public static Vector3 GetPointAlong(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float along)
    {
    float t = 0f;
    float totalLength = GetLength(startPoint, control1, control2, endPoint);

    Vector3 currentPosition = startPoint;
    Vector3 lastPosition = startPoint;

    if (totalLength == 0f) {
        return startPoint;
    }
    float offset = 0;
    // Iterate through each point on the bezier curve.
    for (int i = 1; i <= 20; ++i)
    {
        t = i * 0.05f;
        currentPosition = CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
        float distanceSinceLastPosition = Vector3.Distance(currentPosition, lastPosition);
        if (distanceSinceLastPosition + offset >= along)
        {
            // We have reached our target point.
            break;
        }
        // Move on to the next point.
        lastPosition = currentPosition;
        offset += distanceSinceLastPosition;
    }

    // Do a linear interpolation between our last two points.
    float interpolationRatio = (along - offset) / Vector3.Distance(currentPosition, lastPosition);
    return Vector3.Lerp(lastPosition, currentPosition, interpolationRatio);
}
public static List<Vector3> GetPoints(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, ref float offset, float spacing) 
{
    List<Vector3> points = new List<Vector3>();
    Vector3 lastPosition = GetPointAlong(startPoint, control1, control2, endPoint, offset);
    
    points.Add(lastPosition);

    float currentOffset = 0f;
    while ((currentOffset = offset + spacing) < GetLength(startPoint, control1, control2, endPoint)) 
    {
        Vector3 newPosition = GetPointAlong(startPoint, control1, control2, endPoint, currentOffset);
        points.Add(newPosition);

        offset = currentOffset;
        lastPosition = newPosition;
    }

    offset = Vector3.Distance(lastPosition, endPoint);
    return points;
}

public List<Vector3> GetPoints(ref float offset, float spacing)
{
    return GetPoints(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position, ref offset, spacing);
}
public static float GetLength(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float interpolationRatio) 
{
    float totalLength = 0f;

    Vector3 currentPosition = startPoint;
    Vector3 lastPosition = startPoint;

    // Iterate through each point on the bezier curve.
    for (int i = 0; i <= interpolationRatio; ++i) {
        float t = i * 0.05f;  
        currentPosition = CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
        totalLength += Vector3.Distance(currentPosition, lastPosition);
        lastPosition = currentPosition;
    }

    return totalLength;
}

public float GetLength()
{
    return GetLength(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position);
}

public static Vector3 GetTangent(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float interpolationRatio) 
    {
        Vector3 tangent;
        Vector3 point1 = CalculateBeizerCurve(startPoint, control1, control2, endPoint, interpolationRatio-0.001f);
        Vector3 point2 = CalculateBeizerCurve(startPoint, control1, control2, endPoint, interpolationRatio+0.001f);


        //Calculate tangent by finding the direction vector between the two MidPoints
        tangent = Vector3.Normalize(point2 - point1);
        return tangent;
    }

   public static List<Vector3> GetTangents(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, ref float offset, float spacing) 
{
    List<Vector3> points = new List<Vector3>();
    Vector3 lastPosition = GetPointAlong(startPoint, control1, control2, endPoint, offset);
    
    points.Add(lastPosition);

    float currentOffset = 0f;
    while ((currentOffset = offset + spacing) < GetLength(startPoint, control1, control2, endPoint)) 
    {
        Vector3 newPosition = GetPointAlong(startPoint, control1, control2, endPoint, currentOffset);
        points.Add(GetTangent(startPoint, control1, control2, endPoint, currentOffset/GetLength(startPoint, control1, control2, endPoint)));

        offset = currentOffset;
        lastPosition = newPosition;
    }

    offset = Vector3.Distance(lastPosition, endPoint);
    return points;
}


public List<Vector3> GetTangents(ref float offset, float spacing)
{
    return GetTangents(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position, ref offset, spacing);
}

public static List<float> GetInterpolationRatios(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, ref float offset, float spacing) 
{
    List<float> ratios = new List<float>();
    Vector3 lastPosition = GetPointAlong(startPoint, control1, control2, endPoint, offset);
    
    ratios.Add(offset);

    float currentOffset = 0f;
    while ((currentOffset = offset + spacing) < GetLength(startPoint, control1, control2, endPoint)) 
    {
        Vector3 newPosition = GetPointAlong(startPoint, control1, control2, endPoint, currentOffset);
        ratios.Add(currentOffset/GetLength(startPoint, control1, control2, endPoint));

        offset = currentOffset;
        lastPosition = newPosition;
    }

    offset = Vector3.Distance(lastPosition, endPoint);
    foreach(float ratio in ratios){
        Debug.Log(ratio);

    }
    return ratios;
}

public List<float> GetInterpolationRatios(ref float offset, float spacing)
{
    return GetInterpolationRatios(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position, ref offset, spacing);
}

public static List<Quaternion> GetRotations(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, Quaternion startRotation, Quaternion endRotation, ref float offset, float spacing) 
{
    List<Quaternion> rotations = new List<Quaternion>();
    List<float> ratios = GetInterpolationRatios(startPoint, control1, control2, endPoint, ref offset, spacing);

    foreach (float ratio in ratios) 
    {
        rotations.Add(Quaternion.Lerp(startRotation, endRotation, ratio));
    }

    return rotations;
}

public List<Quaternion> GetRotations(ref float offset, float spacing)
{
    return GetRotations(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position, start.transform.rotation, end.transform.rotation, ref offset, spacing);
}

public static List<Vector3> GetUps(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, Vector3 startUp, Vector3 endUp, ref float offset, float spacing) 
{
    List<Vector3> rotations = new List<Vector3>();
    List<float> ratios = GetInterpolationRatios(startPoint, control1, control2, endPoint, ref offset, spacing);

    foreach (float ratio in ratios) 
    {
        rotations.Add(Vector3.Lerp(startUp, endUp, ratio));
    }

    return rotations;
}

public List<Vector3> GetUps(ref float offset, float spacing)
{
    return GetUps(start.transform.position, control1.transform.position, control2.transform.position, end.transform.position, start.transform.up, end.transform.up, ref offset, spacing);
}


    public void Init()
    {
        CreateSpheres();
        //CreateLabels();
        RenamePoints();
		SetColors();
        //GenerateMesh();
    }

    void Update()
    {
        DrawBeizerCurve(start, control1, control2, end, lr);
        if(showKnobs)
            DrawLines();
        //UpdateMesh();
        
    }
    

    public void DrawLines()
    {
        Debug.DrawLine(start.transform.position, control1.transform.position, Color.white);
        Debug.DrawLine(control2.transform.position, end.transform.position, Color.white);
    }

    public void RenamePoints()
{
    start.name = "Start";
    control1.name = "Control1";
    control2.name = "Control2";
    end.name = "End";
}

   public void CreateSpheres()
    {
        //Instantiate 4 spheres and assign them to start, control1, control2, and end
        start = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        start.AddComponent<Drag>();
        start.transform.parent = this.transform;
        start.transform.localPosition = new Vector3(0,1,0);
        control1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        control1.AddComponent<Drag>();
        control1.transform.parent = this.transform;
        control1.transform.localPosition = new Vector3(1,1,0);
        control2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        control2.AddComponent<Drag>();
        control2.transform.parent = this.transform;
        control2.transform.localPosition = new Vector3(1,0,0);

        end = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        end.AddComponent<Drag>();
        end.transform.parent = this.transform;
        end.transform.localPosition = new Vector3(0,0,0);


       
    }

    public void CreateLabels()
    {
        //Create a textMeshPro to label the spheres 
        TextMeshPro txtmeshpro = Instantiate(TMPObject).GetComponent<TextMeshPro>();
        txtmeshpro.text = "Start";
        TextCamFollow.Init(txtmeshpro, start.transform, cam);
        txtmeshpro.transform.parent = start.transform;
        txtmeshpro.transform.localPosition = Vector3.zero;
        txtmeshpro.gameObject.SetActive(true);

        TextMeshPro txtmeshpro1 = Instantiate(TMPObject).GetComponent<TextMeshPro>();
        txtmeshpro1.text = "Control1";
        TextCamFollow.Init(txtmeshpro1, control1.transform, cam);
        txtmeshpro1.transform.parent = control1.transform;
        txtmeshpro1.transform.localPosition = Vector3.zero;
        txtmeshpro1.gameObject.SetActive(true);


        TextMeshPro txtmeshpro2 = Instantiate(TMPObject).GetComponent<TextMeshPro>();
        txtmeshpro2.text = "Control2";
        TextCamFollow.Init(txtmeshpro2, control2.transform, cam);
        txtmeshpro2.transform.parent = control2.transform;
        txtmeshpro2.transform.localPosition = Vector3.zero;
        txtmeshpro2.gameObject.SetActive(true);


        TextMeshPro txtmeshpro3 = Instantiate(TMPObject).GetComponent<TextMeshPro>();
        txtmeshpro3.text = "End";
        TextCamFollow.Init(txtmeshpro3, end.transform, cam);
        txtmeshpro3.transform.parent = end.transform;
        txtmeshpro3.transform.localPosition = Vector3.zero;
        txtmeshpro3.gameObject.SetActive(true);
    }
	
	public void SetColors()
    {
		start.GetComponent<Renderer>().material.color = Color.red;
		control1.GetComponent<Renderer>().material.color = Color.grey;
		control2.GetComponent<Renderer>().material.color = Color.grey;
		end.GetComponent<Renderer>().material.color = Color.blue;


        control1.transform.localScale /= 2;
        control2.transform.localScale /= 2;

	}
    
    MeshCollider meshCollider;
    public void GenerateMesh()
    {
        meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        UpdateMesh();
        
    }

    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }

    public void ShowKnobs(bool state)
    {
        start.SetActive(state);
        end.SetActive(state);
        control1.SetActive(state);
        control2.SetActive(state);
        showKnobs = state;

    }

    void OnMouseDown()
    {
        ShowKnobs(!start.activeSelf);
        Debug.Log("Beizer Clicked");
    }


    public void DrawBeizerCurve(GameObject startPoint, GameObject control1, GameObject control2, GameObject endPoint, LineRenderer lineRenderer)
    {
        int numPoints = 50; //number of points in between start and end point
        Vector3[] positions = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            float interpolationRatio = (float) i / (numPoints - 1);
            positions[i] = CalculateBeizerCurve(startPoint.transform.position, control1.transform.position, control2.transform.position, endPoint.transform.position, interpolationRatio);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class CompositeBeizer : MonoBehaviour
{

    public List<BeizerCurve> curves = new List<BeizerCurve>();
    public BeizerCurve curvePrefab;

    // Adds a bezier curve near the last bezier curve in the list.
   public void AddCurve()
    {
    // Instantiate the bezier curve
    BeizerCurve newCurve = Instantiate(curvePrefab);
    newCurve.Init();
    newCurve.gameObject.SetActive(true);
    newCurve.transform.SetParent(transform);

    if(curves.Count > 0) {
        DestroyImmediate(newCurve.start);
        newCurve.start = curves[curves.Count - 1].end;
        newCurve.transform.localPosition = curves[curves.Count - 1].end.transform.localPosition;

        newCurve.end.transform.position = curves[curves.Count - 1].end.transform.position + new Vector3(5, 0, 0);

        // Calculate the evenly spaced points for the control nodes
        float a = Vector3.Distance(newCurve.start.transform.position, newCurve.end.transform.position)/3;

        newCurve.control1.transform.position = newCurve.start.transform.position + new Vector3(a, 0, 0);
        newCurve.control2.transform.position = newCurve.end.transform.position - new Vector3(a, 0, 0);
    } else {
        newCurve.start.transform.localPosition = -Vector3.right * 2;
        newCurve.end.transform.localPosition = Vector3.right * 2;
        newCurve.control1.transform.localPosition = Vector3.right * 0.5f;
        newCurve.control2.transform.localPosition = Vector3.right * -0.5f;
    }

    // Add the new curve to the list
    curves.Add(newCurve);
}

    // Makes the first and last curves in the list joined by setting the last curves end to be equal to the first curve's start.
    public void MakeLoop()
    {
        
        DestroyImmediate(curves[curves.Count - 1].end);
        curves[curves.Count - 1].end = curves[0].start;

    }

    public void RemoveCurve()
    {
    if(curves.Count > 1)
        for (int i = curves[curves.Count - 1].transform.childCount; i > 0; --i)
            DestroyImmediate(curves[curves.Count - 1].transform.GetChild(0).gameObject);
        DestroyImmediate(curves[curves.Count-1].gameObject);
        curves.RemoveAt(curves.Count-1);
    }

    public void ShowCurveKnobs(bool state)
    {
        for (int i = 0; i < curves.Count; i++)
        {
            curves[i].ShowKnobs(state); 
        }
    }


    public List<Vector3> GetPoints (float spacing)
{
    List<Vector3> points = new List<Vector3>();
    float offset = 0;

    // Iterate through each bezier curve in the list.
    for (int i = 0; i < curves.Count; i++) 
    {
        BeizerCurve curve = curves[i];

        if(offset > curve.GetLength()){
            offset = offset-curve.GetLength();
        }else{
        points.AddRange(curve.GetPoints(ref offset, spacing));
        
        offset = spacing-offset;
        }
    }

    return points;
}

public List<Vector3> GetTangents(float spacing)
{
    List<Vector3> pointsTangents = new List<Vector3>();
    float offset = 0;

    // Iterate through each bezier curve in the list.
    for (int i = 0; i < curves.Count; i++) 
    {
        BeizerCurve curve = curves[i];
        pointsTangents.AddRange(curve.GetTangents(ref offset, spacing));
        offset = spacing-offset;
    }

    return pointsTangents;
} 


public List<Quaternion> GetRotations(float spacing)
{
    List<Quaternion> pointsRotations = new List<Quaternion>();
    float offset = 0;

    // Iterate through each bezier curve in the list.
    for (int i = 0; i < curves.Count; i++) 
    {
        BeizerCurve curve = curves[i];
        pointsRotations.AddRange(curve.GetRotations(ref offset, spacing));
        offset = spacing-offset;
    }

    return pointsRotations;
}

public List<Vector3> GetUps(float spacing)
{
    List<Vector3> pointsUps = new List<Vector3>();
    float offset = 0;

    // Iterate through each bezier curve in the list.
    for (int i = 0; i < curves.Count; i++) 
    {
        BeizerCurve curve = curves[i];
        pointsUps.AddRange(curve.GetUps(ref offset, spacing));
        offset = spacing-offset;
    }

    return pointsUps;
} 
}



[CustomEditor(typeof(CompositeBeizer))]
class CompositeBeizerEditor : Editor
{
    private bool knob = true;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add Curve"))
            ((CompositeBeizer)target).AddCurve();
        if (GUILayout.Button("Remove Curve"))
            ((CompositeBeizer)target).RemoveCurve();
        if (GUILayout.Button("Make Loop"))
            ((CompositeBeizer)target).MakeLoop();
        if(GUILayout.Button("Toggle Knobs"))
            {
            ((CompositeBeizer)target).ShowCurveKnobs(!knob);
            knob = !knob;
            }
        DrawDefaultInspector();
    }
}

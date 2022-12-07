using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeizerCurveCalculator
{
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
        currentPosition = BeizerCurveCalculator.CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
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
        currentPosition = BeizerCurveCalculator.CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
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
public static float GetLength(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float interpolationRatio) 
{
    float totalLength = 0f;

    Vector3 currentPosition = startPoint;
    Vector3 lastPosition = startPoint;

    // Iterate through each point on the bezier curve.
    for (int i = 0; i <= interpolationRatio; ++i) {
        float t = i * 0.05f;  
        currentPosition = BeizerCurveCalculator.CalculateBeizerCurve(startPoint, control1, control2, endPoint, t);
        totalLength += Vector3.Distance(currentPosition, lastPosition);
        lastPosition = currentPosition;
    }

    return totalLength;
}

public static Vector3 GetTangent(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, float interpolationRatio) 
    {
        Vector3 tangent;
        Vector3 point1 = Vector3.Lerp(startPoint, control1, interpolationRatio);
        Vector3 point2 = Vector3.Lerp(control1, control2, interpolationRatio);
        Vector3 point3 = Vector3.Lerp(control2, endPoint, interpolationRatio);

        Vector3 midPoint1 = Vector3.Lerp(point1, point2, interpolationRatio);
        Vector3 midPoint2 = Vector3.Lerp(point2, point3, interpolationRatio);

        //Calculate tangent by finding the direction vector between the two MidPoints
        tangent = Vector3.Normalize(midPoint2 - midPoint1);
        return tangent;
    }

   public static List<Vector3> GetTangents(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, ref float offset, float spacing) 
{
    List<Vector3> points = new List<Vector3>();
    Vector3 lastPosition = GetPointAlong(startPoint, control1, control2, endPoint, offset);
    
    points.Add(BeizerCurveCalculator.GetTangent(startPoint, control1, control2, endPoint, offset));

    float currentOffset = 0f;
    while ((currentOffset = offset + spacing) < GetLength(startPoint, control1, control2, endPoint)) 
    {
        Vector3 newPosition = GetPointAlong(startPoint, control1, control2, endPoint, currentOffset);
        points.Add(BeizerCurveCalculator.GetTangent(startPoint, control1, control2, endPoint, currentOffset));

        offset = currentOffset;
        lastPosition = newPosition;
    }

    offset = Vector3.Distance(lastPosition, endPoint);
    return points;
}
}
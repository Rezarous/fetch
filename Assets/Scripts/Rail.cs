using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rail : MonoBehaviour
{

    [SerializeField]
    public Vector3[] points;
    
    private Color gizmoCol;
    private float currentPercentage;
    private float railLength;
    private bool wasInside;

    void Start()
    {
        railLength = getRailLength();
        gizmoCol = Color.blue;
        wasInside = false;
    }
    public float getDistToRail(Vector2 point, out Vector2 closestPoint)
    {
        
        float outdist = Mathf.Infinity;
        int closest = 0;
        bool inside = isInside(point);
        float tempPercentage = 0;
        float tempLength = 0;

        if (inside)
        {
            if (currentPercentage < 0.5)
            {
                closestPoint = points[0];
                currentPercentage = 0;
            }
            else
            {
                closestPoint = points[points.Length - 1];
                currentPercentage = 1;
            }
        }
        else
        {
            float tempCurrentPercentage = 0;
            for (int i = 0; i < points.Length - 1; i++)
            {
                float segmentLength = toVec2(points[i] - points[i+1]).magnitude;
                float tempdist = (closestPointOnLine(point, toVec2(points[i]), toVec2(points[i + 1]), out tempPercentage) - point).magnitude;
                if (tempdist < outdist)
                {
                    closest = i;
                    outdist = tempdist;
                    tempCurrentPercentage = (tempLength + segmentLength * tempPercentage) / railLength;
                }
                tempLength += segmentLength;
            }
            railLength = tempLength;
            if(Mathf.Abs(currentPercentage - tempCurrentPercentage) < 0.01)
            {
                wasInside = false;
            }

            if (!wasInside)
            {
                currentPercentage = tempCurrentPercentage;
                closestPoint = closestPointOnLine(point, toVec2(points[closest]), toVec2(points[closest + 1]), out tempPercentage);
            }
            else
            {
                if (currentPercentage < 0.5)
                {
                    closestPoint = points[0];
                    currentPercentage = 0;
                }
                else
                {
                    closestPoint = points[points.Length - 1];
                    currentPercentage = 1;
                }
            }

        }        
        gizmoCol =  inside ? Color.red : Color.blue;
        return outdist;
    }

    private Vector2 closestPointOnLine(Vector2 point, Vector2 a, Vector2 b, out float segmentPercentage)
    {
        Vector2 ap = point - a, ab = b - a;
        float mul = Mathf.Clamp01(Vector2.Dot(ap, ab) / (ab.magnitude * ab.magnitude));
        segmentPercentage = mul;
        return a + (mul * ab); 
    }

    private bool isInside(Vector2 point)
    {
        bool inside = true;
        Vector3 prevPoint = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            Vector2 normal = toVec2(points[i] - prevPoint);
            normal = new Vector2(-normal.y, normal.x).normalized;
            prevPoint = points[i];
            inside = inside && Vector2.Dot((point - toVec2(points[i])).normalized, normal) < 0;
        }
        if (inside) wasInside = true;
        return inside;
    }

    private float getRailLength()
    {
        float length = 0;
        Vector2 lastPoint = toVec2(points[0]);
        for(int i = 1; i < points.Length; i++)
        {
            length += (toVec2(points[i]) - lastPoint).magnitude;
            lastPoint = toVec2(points[i]);
        }
        return length;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoCol;
        if (points.Length > 1)
        {
            Vector3 prevPoint = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                Gizmos.DrawLine(points[i], prevPoint);

                Vector2 dir = toVec2(points[i] - prevPoint);
                Vector2 center = toVec2(prevPoint) + dir * 0.5f;
                dir = new Vector2(-dir.y, dir.x).normalized;

                Gizmos.DrawLine(center, center + dir * 0.5f);

                prevPoint = points[i];
            }
        }
    }

    private Vector2 toVec2(Vector3 vin)
    {
        return new Vector2(vin.x, vin.y);
    }

    private Vector3 toVec3(Vector2 vin)
    {
        return new Vector3(vin.x, vin.y, 0);
    }
}

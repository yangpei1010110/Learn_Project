using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class CameraCreateLine : MonoBehaviour
{
    public List<LineIntersect2D.Line> Lines        = new();
    public bool                       isFirstPoint = true;
    public LineIntersect2D.Line?      Temp;

    void Update()
    {
        var main = Camera.main;
        if (Input.GetMouseButtonDown(0))
        {
            if (isFirstPoint)
            {
                Temp = new LineIntersect2D.Line { top = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)) };

                isFirstPoint = false;
            }
            else
            {
                Temp = new LineIntersect2D.Line { top = Temp.Value.top, bottom = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)) };
                Lines.Add(Temp.Value);
                Temp = null;
                isFirstPoint = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var point1 = main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0) + Vector3.forward * (main.nearClipPlane * 10));
            var point2 = main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0) + Vector3.forward * (main.nearClipPlane * 10));
            var line = new LineIntersect2D.Line { top = point1, bottom = point2, };
            Lines.Add(line);
        }
    }

    void OnDrawGizmos()
    {
        var main = Camera.main;
        var z = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)).z;
        var zV = new Vector3(0, 0, z);
        if (Temp != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3)Temp.Value.top + zV, 0.01f);
            Gizmos.DrawLine((Vector3)Temp.Value.top + zV, main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)));
        }

        foreach (var line in Lines)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector3)line.top + zV, (Vector3)line.bottom + zV);
        }

        var sw = Stopwatch.StartNew();

        var drawList = new LineIntersect2D().IntersectPoint(Lines.ToArray());

        sw.Stop();
        if (Time.frameCount % 100 == 0)
        {
            Debug.Log($"count:{Lines.Count} Intersect:{sw.Elapsed.TotalMilliseconds}ms");
        }

        foreach (var e in drawList)
        {
            var s = e.point;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(s.x, s.y, zV.z), 0.01f);
        }
    }
}
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

        // load debug data
        if (Input.GetKeyDown(KeyCode.D))
        {
            var center = (Vector2)main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0) + Vector3.forward * (main.nearClipPlane * 10));
            var debugLine1 = new LineIntersect2D.Line() { top = center + new Vector2(0f, 1f), bottom = center + new Vector2(0f, -1f), };
            var debugLine2 = new LineIntersect2D.Line() { top = center + new Vector2(0.5f, 0.7f), bottom = center + new Vector2(0.5f, -1.3f), };
            var debugLine3 = new LineIntersect2D.Line() { top = center + new Vector2(-1f, 0.6f), bottom = center + new Vector2(1f, -1.2f), };

            Lines.Add(debugLine1);
            Lines.Add(debugLine2);
            Lines.Add(debugLine3);
        }
    }

    void OnDrawGizmos()
    {
        var main = Camera.main;
        var mousePos = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10));


        var z = mousePos.z;
        var zV = new Vector3(0, 0, z);
        if (Temp != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3)Temp.Value.top + zV, 0.05f);
            Gizmos.DrawLine((Vector3)Temp.Value.top + zV, main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)));
        }

        foreach (var line in Lines)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector3)line.top + zV, (Vector3)line.bottom + zV);
        }

        var linesArray = Lines.ToArray();
        // var li2d = new LineIntersect2D();
        // li2d.SortedArrayByPointY(ref linesArray, mousePos - new Vector3(0, float.Epsilon * 100f,0));
        // for (int i = 0; i < linesArray.Length; i++)
        // {
        //     var line = linesArray[i];
        //     Gizmos.color = Color.red;
        //     Handles.Label(new Vector3(line.top.x, line.top.y, z), $"{i} {li2d.PointLineDistance(mousePos, line):F2}");
        // }

        var sw = Stopwatch.StartNew();
        // if (DebugData.Lines.TryPeek(out var data) || DebugData.Lines.Count == 0)
        // {
        //     if (DebugData.Lines.Count == 0 || data.Length != linesArray.Length)
        //     {
        //         if (linesArray.Length != 0)
        //         {
        //             DebugData.Lines.Push(linesArray);
        //             AssetDatabase.SaveAssets();
        //         }
        //     }
        // }
        //
        var drawList = new LineIntersect2D().IntersectPoint3(linesArray);
        //
        sw.Stop();
        if (Time.frameCount % 100 == 0)
        {
            Debug.Log($"count:{Lines.Count} IntersectCount:{drawList.Length} Intersect:{sw.Elapsed.TotalMilliseconds}ms");
        }
        
        foreach (var e in drawList)
        {
            var s = e.point;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(s.x, s.y, z), 0.05f);
        }
    }
}
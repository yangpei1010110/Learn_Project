#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CameraCreateLine : MonoBehaviour
{
    public List<Line2D.Line> Lines        = new();
    public bool                  isFirstPoint = true;
    public Line2D.Line?      Temp;

    private void Update()
    {
        Camera main = Camera.main;
        if (Input.GetMouseButtonDown(0))
        {
            if (isFirstPoint)
            {
                Temp = new Line2D.Line { top = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)), };

                isFirstPoint = false;
            }
            else
            {
                Temp = new Line2D.Line { top = Temp.Value.top, bottom = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)), };
                Lines.Add(Temp.Value);
                Temp = null;
                isFirstPoint = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 point1 = main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0) + Vector3.forward * (main.nearClipPlane * 10));
            Vector3 point2 = main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0) + Vector3.forward * (main.nearClipPlane * 10));
            Line2D.Line line = new() { top = point1, bottom = point2, };
            Lines.Add(line);
        }

        // load debug data
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector2 center = (Vector2)main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0) + Vector3.forward * (main.nearClipPlane * 10));
            Line2D.Line debugLine1 = new() { top = center + new Vector2(0f, 1f), bottom = center + new Vector2(0f, -1f), };
            Line2D.Line debugLine2 = new() { top = center + new Vector2(0.5f, 0.7f), bottom = center + new Vector2(0.5f, -1.3f), };
            Line2D.Line debugLine3 = new() { top = center + new Vector2(-1f, 0.6f), bottom = center + new Vector2(1f, -1.2f), };

            Lines.Add(debugLine1);
            Lines.Add(debugLine2);
            Lines.Add(debugLine3);
        }
    }

    private void OnDrawGizmos()
    {
        Camera main = Camera.main;
        Vector3 mousePos = main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10));


        float z = mousePos.z;
        Vector3 zV = new(0, 0, z);
        if (Temp != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3)Temp.Value.top + zV, 0.05f);
            Gizmos.DrawLine((Vector3)Temp.Value.top + zV, main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * (main.nearClipPlane * 10)));
        }

        foreach (Line2D.Line line in Lines)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector3)line.top + zV, (Vector3)line.bottom + zV);
        }

        Line2D.Line[] linesArray = Lines.ToArray();
        // var li2d = new LineIntersect2D();
        // li2d.SortedArrayByPointY(ref linesArray, mousePos - new Vector3(0, float.Epsilon * 100f,0));
        // for (int i = 0; i < linesArray.Length; i++)
        // {
        //     var line = linesArray[i];
        //     Gizmos.color = Color.red;
        //     Handles.Label(new Vector3(line.top.x, line.top.y, z), $"{i} {li2d.PointLineDistance(mousePos, line):F2}");
        // }

        Stopwatch sw = Stopwatch.StartNew();
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
        LineIntersect2D.IntersectEvent[] drawList = new LineIntersect2D().IntersectPoint3(linesArray);
        //
        sw.Stop();
        if (Time.frameCount % 100 == 0)
        {
            Debug.Log($"count:{Lines.Count} IntersectCount:{drawList.Length} Intersect:{sw.Elapsed.TotalMilliseconds}ms");
        }

        foreach (LineIntersect2D.IntersectEvent e in drawList)
        {
            Vector2 s = e.point;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(s.x, s.y, z), 0.05f);
        }
    }
}
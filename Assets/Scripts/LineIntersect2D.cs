using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineIntersect2D
{
    public struct Line
    {
        public Vector2 top;
        public Vector2 bottom;
    }

    public struct IntersectEvent
    {
        public Vector2 point;
        public Line[]  lines;
    }

    private struct PointLineSet
    {
        public Vector2       point;
        public HashSet<Line> lineSet;
    }

    private List<Vector2>                     EventQueue = new();
    private Dictionary<Vector2, PointLineSet> Status     = new();
    private List<Line>                        LineTemp   = new();
    public  HashSet<Vector2>                  Result     = new();

    /// <summary>
    /// 重新排序线段，使得线段的顶点总是在底点的上方
    /// </summary>
    public void ReOrderLines(ref Line[] lineSet)
    {
        for (int i = 0; i < lineSet.Length; i++)
        {
            Line line = lineSet[i];
            if (line.top.y < line.bottom.y)
            {
                (line.top, line.bottom) = (line.bottom, line.top);
            }
            else if (Math.Abs(line.top.y - line.bottom.y) < float.Epsilon)
            {
                if (line.top.x > line.bottom.x)
                {
                    (line.top, line.bottom) = (line.bottom, line.top);
                }
            }

            lineSet[i] = line;
        }
    }

    /// <summary>
    /// 线段集合相交测试
    /// </summary>
    public IntersectEvent[] IntersectPoint(Line[] lineSet)
    {
        // init
        {
            ReOrderLines(ref lineSet);

            EventQueue = lineSet.SelectMany(l => new[] { l.top, l.bottom, })
                                .OrderByDescending(p => p.y)
                                .ThenBy(p => p.x)
                                .ToList();
            Status.Clear();
            foreach (Line line in lineSet)
            {
                if (Status.TryGetValue(line.top, out PointLineSet ls))
                {
                    ls.lineSet.Add(line);
                }
                else
                {
                    Status.Add(line.top, new PointLineSet()
                    {
                        point = line.top,
                        lineSet = new HashSet<Line>() { line, },
                    });
                }

                if (Status.TryGetValue(line.bottom, out ls))
                {
                    ls.lineSet.Add(line);
                }
                else
                {
                    Status.Add(line.bottom, new PointLineSet()
                    {
                        point = line.bottom,
                        lineSet = new HashSet<Line>() { line, },
                    });
                }
            }

            LineTemp = new List<Line>();
        }

        // 扫描线
        while (EventQueue.Count > 0)
        {
            Vector2 eventPoint = EventQueue[0];
            EventQueue.RemoveAt(0);
            HandleEventPoint(eventPoint);
        }

        // 收集结果
        return Result.Select(p => new IntersectEvent()
        {
            point = p,
            lines = Status[p].lineSet.ToArray(),
        }).ToArray();
    }


    /// <summary>
    /// 2d 线段求交点
    /// </summary>
    public Vector2? Intersect(Line l1, Line l2)
    {
        Vector2 p1 = l1.top;
        Vector2 p2 = l1.bottom;
        Vector2 p3 = l2.top;
        Vector2 p4 = l2.bottom;

        Vector2 p4p3 = p4 - p3;
        Vector2 p1p3 = p1 - p3;
        Vector2 p2p1 = p2 - p1;

        float denominator = p4p3.y * p2p1.x - p4p3.x * p2p1.y;
        float ua = (p4p3.x * p1p3.y - p4p3.y * p1p3.x) / denominator;
        float ub = (p2p1.x * p1p3.y - p2p1.y * p1p3.x) / denominator;

        float x = p1.x + ua * p2p1.x;
        float y = p1.y + ua * p2p1.y;

        if (0f <= ua && ua <= 1f && 0f <= ub && ub <= 1f)
        {
            return new Vector2(x, y);
        }

        return null;
    }

    /// <summary>
    /// 点与线段垂直的距离
    /// </summary>
    public float PointLineDistance(Vector2 point, Line line)
    {
        if (line.top == line.bottom)
        {
            return Vector2.Distance(point, line.top);
        }

        Vector2 p1 = line.top;
        Vector2 p2 = line.bottom;
        Vector2 p3 = point;

        Vector2 p3p1 = p3 - p1;
        Vector2 p2p1 = p2 - p1;

        float sqrMagnitude = p2p1.x * p2p1.x + p2p1.y * p2p1.y;
        float u = (p3p1.x * p2p1.x + p3p1.y * p2p1.y) / sqrMagnitude;

        if (u < 0f)
        {
            return Vector2.Distance(p1, p3);
        }
        else if (u > 1f)
        {
            return Vector2.Distance(p2, p3);
        }
        else
        {
            Vector2 p = p1 + u * p2p1;
            return Vector2.Distance(p, p3);
        }
    }

    /// <summary>
    /// 检查事件点
    /// </summary>
    private void HandleEventPoint(Vector2 point)
    {
        Line[] cpSet = Status[point].lineSet.ToArray();
        Line[] upSet = cpSet.Where(l => l.top == point).ToArray();
        Line[] lpSet = LineTemp.FindAll(l => l.bottom == point).ToArray();

        // 求交集
        Line[] intersectSet = upSet.Intersect(lpSet).Intersect(cpSet).ToArray();
        if (intersectSet.Length > 0)
        {
            Result.Add(point);
        }

        // 移除下端点
        foreach (Line line in lpSet)
        {
            LineTemp.Remove(line);
        }
    }

    /// <summary>
    /// 找到新事件
    /// </summary>
    private void FindNewEvent(Line l1, Line l2, Vector2 eventPoint)
    {
        Vector2? intersectPoint = Intersect(l1, l2);
        if (!intersectPoint.HasValue)
        {
            return;
        }

        if (intersectPoint.Value.y > eventPoint.y)
        {
            return;
        }

        if (intersectPoint.Value.x < eventPoint.x)
        {
            return;
        }

        // 添加新事件
        PushEvent(intersectPoint.Value);
        // 修正状态
        if (Status.TryGetValue(intersectPoint.Value, out PointLineSet lineSet))
        {
            lineSet.lineSet.Add(l1);
            lineSet.lineSet.Add(l2);
        }
        else
        {
            Status.Add(intersectPoint.Value, new PointLineSet()
            {
                point = intersectPoint.Value,
                lineSet = new HashSet<Line>() { l1, l2, },
            });
        }
    }

    /// <summary>
    /// 添加事件点
    /// </summary>
    private void PushEvent(Vector2 point)
    {
        int insertIndex = EventQueue.FindIndex(v => v.y <= point.y && v.x >= point.x);
        if (insertIndex == -1)
        {
            insertIndex = EventQueue.Count;
        }

        EventQueue.Insert(insertIndex, point);
    }
}
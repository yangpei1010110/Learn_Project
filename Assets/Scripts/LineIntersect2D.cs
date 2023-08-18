using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LineIntersect2D
{
    class EventComparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            if (x.y > y.y || (x.y.Equals(y.y) && x.x < y.x))
            {
                return -1;
            }
            else if (x.y < y.y || (x.y.Equals(y.y) && x.x > y.x))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    [Serializable]
    public struct Line
    {
        public Vector2 top;
        public Vector2 bottom;
    }

    [Serializable]
    public struct IntersectEvent
    {
        public Vector2 point;
        public List<Line> lines;
    }

    // 结果集合
    private Dictionary<Vector2, IntersectEvent> Result = new();

    private static int lastCount = 0;

    private Dictionary<Vector2, HashSet<Line>> _upperPointCache      = new();
    private Dictionary<Vector2, HashSet<Line>> _centerPointCache     = new();
    private Dictionary<Vector2, HashSet<Line>> _lowerPointCache      = new();
    private SortedList<Vector2, Vector2>       _lineSortedEventCache = new(new EventComparer());
    private List<Line>                         _lineSweepCache       = new();

    public IntersectEvent[] IntersectPoint3(Line[] lineSet)
    {
        ReOrderLines(ref lineSet);
        AddPointLineToCache(lineSet);

        while (_lineSortedEventCache.Count >0)
        {
            var e = PopEvent()!.Value;
            if (_upperPointCache.ContainsKey(e))
            {
                AddEvent(e);
                continue;
            }

            if (_lowerPointCache.TryGetValue(e,out var lines))
            {
                foreach (Line line in lines)
                {
                    _lineSweepCache.Remove(line);
                }
            }
        }
        
        return Result.Values.ToArray();
    }

    private void AddEvent(Vector2 point)
    {
        if (_upperPointCache.TryGetValue(point, out var lines))
        {
            _lineSweepCache.AddRange(lines);
            foreach (Line line1 in lines)
            {
                foreach (Line line2 in _lineSweepCache)
                {
                    if (!line1.Equals(line2))
                    {
                        var intersectPoint = Intersect(line1, line2);
                        if (intersectPoint.HasValue)
                        {
                            if (Result.TryGetValue(intersectPoint.Value,out var result))
                            {
                                result.lines.Add(line1);
                                result.lines.Add(line2);
                            }
                            else
                            {
                                Result.Add(intersectPoint.Value, new IntersectEvent()
                                {
                                    point = intersectPoint.Value,
                                    lines =  new List<Line>() { line1, line2, },
                                });
                            }
                        }
                    }
                }
            }
        }
    }
    public IntersectEvent[] IntersectPoint2(Line[] lineSet)
    {
        ReOrderLines(ref lineSet);
        AddPointLineToCache(lineSet);

        int count = 0;
        int maxCount = 1000;
        while (_lineSortedEventCache.Count > 0 && count < maxCount)
        {
            count++;
            if (lastCount != lineSet.Length && lineSet.Length != 0)
            {
                Debug.Log("debug breakpoint");
                lastCount = lineSet.Length;
            }

            HandleEventPoint(PopEvent()!.Value);
        }

        if (count >= maxCount)
        {
            Debug.Log($"event too much {count}");
            Debug.Break();
        }

        return Result.Values.ToArray();
    }

    public void AddPointLineToCache(Line[] lineSet)
    {
        for (int i = 0; i < lineSet.Length; i++)
        {
            Line line = lineSet[i];
            if (_upperPointCache.TryGetValue(line.top, out HashSet<Line> set))
            {
                set.Add(line);
            }
            else
            {
                _upperPointCache.Add(line.top, new HashSet<Line>() { line, });
            }

            if (_lowerPointCache.TryGetValue(line.bottom, out set))
            {
                set.Add(line);
            }
            else
            {
                _lowerPointCache.Add(line.bottom, new HashSet<Line>() { line, });
            }

            PushEvent(line.top);
            PushEvent(line.bottom);
        }
    }

    /// <summary>
    /// 重新排序线段，使得线段的顶点总是在底点的上方
    /// </summary>
    private void ReOrderLines(ref Line[] lines)
    {
        // top.y > bottom.y
        // top.x < bottom.x
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].top.y < lines[i].bottom.y
             || (lines[i].top.y.Equals(lines[i].bottom.y) && lines[i].top.x > lines[i].bottom.x))
            {
                var temp = lines[i].top;
                lines[i].top = lines[i].bottom;
                lines[i].bottom = temp;
            }
        }
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
    /// 点与线段的距离
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

        if (u <= 0f)
        {
            return -1f;
        }
        else if (u >= 1f)
        {
            return -1f;
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
        // 1. 找到所有以 point 为上顶点的线段
        Line[] upperSet = Array.Empty<Line>();
        Line[] centerSet = Array.Empty<Line>();
        Line[] lowerSet = Array.Empty<Line>();

        if (_upperPointCache.TryGetValue(point, out HashSet<Line> upperS))
        {
            upperSet = upperS.ToArray();
        }

        if (_centerPointCache.TryGetValue(point, out HashSet<Line> centerS))
        {
            centerSet = centerS.ToArray();
        }

        if (_lowerPointCache.TryGetValue(point, out HashSet<Line> lowerS))
        {
            lowerSet = lowerS.ToArray();
        }

        // 2. 在状态中找到所有以 point 为下顶点或包含 point 的线段
        Line[] centerAndLowerSet = lowerSet.Union(centerSet).ToArray();
        Line[] upperAndCenterSet = upperSet.Union(centerSet).ToArray();
        SortedArrayByPointY(ref centerAndLowerSet, point );
        SortedArrayByPointY(ref upperAndCenterSet, point );
        Array.Reverse(upperAndCenterSet);

        // 3. upper, center ,lower 其中包含不止一条线段，说明事件点是交点
        {
            if (upperSet.Length + centerSet.Length + lowerSet.Length > 1)
            {
                Line[] unionSet = upperSet.Union(centerSet).Union(lowerSet).ToArray();
                // 4. 报告发现交点 返回线段集合
                if (Result.TryGetValue(point, out IntersectEvent resultSet))
                {
                    resultSet.lines.AddRange( unionSet);
                }
                else
                {
                    Result.Add(point, new IntersectEvent()
                    {
                        point = point,
                        lines = unionSet.ToList(),
                    });
                }
            }
        }

        // 5. 将 L(p) C(p) 中的线段从 LineTemp 中移除
        int lastDeleteIndex = -1;
        if (centerAndLowerSet.Length > 0)
        {
            for (int i = 0; i < centerAndLowerSet.Length; i++)
            {
                if (lastDeleteIndex == -1)
                {
                    Line line = _lineSweepCache[i];
                    if (centerAndLowerSet.Contains(line))
                    {
                        lastDeleteIndex = _lineSweepCache.IndexOf(line);

                        _lineSweepCache.RemoveAt(lastDeleteIndex);
                    }
                }
                else
                {
                    Line line = _lineSweepCache[lastDeleteIndex];
                    if (centerAndLowerSet.Contains(line))
                    {
                        _lineSweepCache.RemoveAt(lastDeleteIndex);
                    }
                }
            }
        }

        // 6. 将 U(p) C(p) 中的线段添加到 LineTemp 中
        // 修正 firstIndex
        int firstIndex = FindLastNearIndexByPoint(point) + 1;
        firstIndex = math.max(0, firstIndex);
        // Array.Reverse(upperAndCenterSet);
        for (int i = 0; i < upperAndCenterSet.Length; i++)
        {
            _lineSweepCache.Insert(firstIndex + i, upperAndCenterSet[i]);
        }
        // 7. 找到新事件
        if (upperAndCenterSet.Length == 0)
        {
            int nearIndex = FindLastNearIndexByPoint(point);
            // 进入这里说明事件点是只有下顶点
            // lastDeleteIndex 是删除线段后的右边线段的索引 找到左右两边的 2 条线段
            if (0 <= nearIndex && nearIndex + 1 < _lineSweepCache.Count)
            {
                Line lLine = _lineSweepCache[nearIndex];
                Line rLine = _lineSweepCache[nearIndex + 1];
                FindNewEvent(lLine, rLine, point);
            }
        }
        else
        {
            // 找到距离 sl 最近的左边的线段 firstIndex - 1
            int nearIndex = FindLastNearIndexByPoint(point);
            if (nearIndex >= 0)
            {
                foreach (Line line in upperAndCenterSet)
                {
                    FindNewEvent(_lineSweepCache[nearIndex], line, point);
                }
            }

            // 找到距离 sr 最近的右边的线段 firstIndex + upperAndCenterSet.Length
            if (firstIndex + upperAndCenterSet.Length < _lineSweepCache.Count)
            {
                foreach (Line line in upperAndCenterSet)
                {
                    FindNewEvent(line, _lineSweepCache[firstIndex + upperAndCenterSet.Length], point);
                }
            }
        }
    }

    private int FindLastNearIndexByPoint(Vector2 point)
    {
        int index = -1;
        for (int i = 0; i < _lineSweepCache.Count; i++)
        {
            Line line = _lineSweepCache[i];
            if (line.top.Equals(point) || line.bottom.Equals(point))
            {
                continue;
            }

            Vector2 lineDir = line.top - line.bottom;
            float lineSlope = lineDir.y / lineDir.x;
            float lineX = line.bottom.x + (point.y - line.bottom.y) / lineSlope;

            if (lineX > point.x + float.Epsilon)
            {
                break;
            }
            else if (lineX < point.x - float.Epsilon )
            {
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    /// 找到新交点事件
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
        else if (intersectPoint.Value.y.Equals(eventPoint.y) && intersectPoint.Value.x < eventPoint.x)
        {
            return;
        }
        else if (intersectPoint.Value.y.Equals(eventPoint.y) && intersectPoint.Value.x.Equals(eventPoint.x))
        {
            return;
        }


        // 添加新事件
        PushEvent(intersectPoint.Value);
        if (_centerPointCache.TryGetValue(intersectPoint.Value, out HashSet<Line> set))
        {
            set.Add(l1);
            set.Add(l2);
        }
        else
        {
            _centerPointCache.Add(intersectPoint.Value, new HashSet<Line>() { l1, l2, });
        }
    }

    private Vector2? PopEvent()
    {
        if (_lineSortedEventCache.Count == 0)
        {
            return null;
        }

        Vector2 result = _lineSortedEventCache.Values[0];
        _lineSortedEventCache.RemoveAt(0);
        return result;
    }

    /// <summary>
    /// 添加事件点
    /// </summary>
    private void PushEvent(Vector2 point)
    {
        if (_lineSortedEventCache.ContainsKey(point))
        {
            return;
        }

        _lineSortedEventCache.Add(point, point); 
    }

    public void SortedArrayByPointY(ref Line[] arr, Vector2 point)
    {
        Array.Sort(arr, (l1, l2) =>
        {
            Vector2 l1Dir = l1.top - l1.bottom;
            float l1Slope = l1Dir.y / l1Dir.x;
            float l1X = l1.bottom.x + (point.y - l1.bottom.y) / l1Slope;

            Vector2 l2Dir = l2.top - l2.bottom;
            float l2Slope = l2Dir.y / l2Dir.x;
            float l2X = l2.bottom.x + (point.y - l2.bottom.y) / l2Slope;

            return l1X.CompareTo(l2X);
        });
    }
}
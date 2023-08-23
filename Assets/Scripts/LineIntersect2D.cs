#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Unity.Mathematics;
using UnityEngine;

public class LineIntersect2D
{
    private static int                                           lastCount             = 0;
    private        Dictionary<Vector2, HashSet<LineTool2D.Line>> _centerPointCache     = new();
    private        SortedList<Vector2, Vector2>                  _lineSortedEventCache = new(new EventComparer());
    private        List<LineTool2D.Line>                         _lineSweepCache       = new();
    private        Dictionary<Vector2, HashSet<LineTool2D.Line>> _lowerPointCache      = new();

    private Dictionary<Vector2, HashSet<LineTool2D.Line>> _upperPointCache = new();

    // 结果集合
    private Dictionary<Vector2, IntersectEvent> Result = new();

    /// <summary>
    ///     简单粗暴的扫描线算法 计算线段集合交点
    /// </summary>
    public IntersectEvent[] IntersectPoint3(LineTool2D.Line[] lineSet)
    {
        LineTool2D.ReOrderLines(ref lineSet);
        AddPointLineToCache(lineSet);

        while (_lineSortedEventCache.Count > 0)
        {
            Vector2 e = PopEvent()!.Value;
            if (_upperPointCache.ContainsKey(e))
            {
                AddEvent(e);
                continue;
            }

            if (_lowerPointCache.TryGetValue(e, out HashSet<LineTool2D.Line> lines))
            {
                foreach (LineTool2D.Line line in lines)
                {
                    _lineSweepCache.Remove(line);
                }
            }
        }

        return Result.Values.ToArray();
    }

    private void AddEvent(Vector2 point)
    {
        if (_upperPointCache.TryGetValue(point, out HashSet<LineTool2D.Line> lines))
        {
            _lineSweepCache.AddRange(lines);
            foreach (LineTool2D.Line line1 in lines)
            {
                foreach (LineTool2D.Line line2 in _lineSweepCache)
                {
                    if (!line1.Equals(line2))
                    {
                        Vector2? intersectPoint = LineTool2D.Intersect(line1, line2);
                        if (intersectPoint.HasValue)
                        {
                            if (Result.TryGetValue(intersectPoint.Value, out IntersectEvent result))
                            {
                                result.lines.Add(line1);
                                result.lines.Add(line2);
                            }
                            else
                            {
                                Result.Add(intersectPoint.Value, new IntersectEvent()
                                {
                                    point = intersectPoint.Value,
                                    lines = new List<LineTool2D.Line>() { line1, line2, },
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     理论上更快 但是有 bug 暂时不用
    /// </summary>
    public IntersectEvent[] IntersectPoint2(LineTool2D.Line[] lineSet)
    {
        LineTool2D.ReOrderLines(ref lineSet);
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

    private void AddPointLineToCache(LineTool2D.Line[] lineSet)
    {
        for (int i = 0; i < lineSet.Length; i++)
        {
            LineTool2D.Line line = lineSet[i];
            if (_upperPointCache.TryGetValue(line.top, out HashSet<LineTool2D.Line> set))
            {
                set.Add(line);
            }
            else
            {
                _upperPointCache.Add(line.top, new HashSet<LineTool2D.Line>() { line, });
            }

            if (_lowerPointCache.TryGetValue(line.bottom, out set))
            {
                set.Add(line);
            }
            else
            {
                _lowerPointCache.Add(line.bottom, new HashSet<LineTool2D.Line>() { line, });
            }

            PushEvent(line.top);
            PushEvent(line.bottom);
        }
    }

    /// <summary>
    ///     检查事件点
    /// </summary>
    private void HandleEventPoint(Vector2 point)
    {
        // 1. 找到所有以 point 为上顶点的线段
        LineTool2D.Line[] upperSet = Array.Empty<LineTool2D.Line>();
        LineTool2D.Line[] centerSet = Array.Empty<LineTool2D.Line>();
        LineTool2D.Line[] lowerSet = Array.Empty<LineTool2D.Line>();

        if (_upperPointCache.TryGetValue(point, out HashSet<LineTool2D.Line> upperS))
        {
            upperSet = upperS.ToArray();
        }

        if (_centerPointCache.TryGetValue(point, out HashSet<LineTool2D.Line> centerS))
        {
            centerSet = centerS.ToArray();
        }

        if (_lowerPointCache.TryGetValue(point, out HashSet<LineTool2D.Line> lowerS))
        {
            lowerSet = lowerS.ToArray();
        }

        // 2. 在状态中找到所有以 point 为下顶点或包含 point 的线段
        LineTool2D.Line[] centerAndLowerSet = lowerSet.Union(centerSet).ToArray();
        LineTool2D.Line[] upperAndCenterSet = upperSet.Union(centerSet).ToArray();
        LineTool2D.SortedArrayByPointY(ref centerAndLowerSet, point.y);
        LineTool2D.SortedArrayByPointY(ref upperAndCenterSet, point.y);
        Array.Reverse(upperAndCenterSet);

        // 3. upper, center ,lower 其中包含不止一条线段，说明事件点是交点
        {
            if (upperSet.Length + centerSet.Length + lowerSet.Length > 1)
            {
                LineTool2D.Line[] unionSet = upperSet.Union(centerSet).Union(lowerSet).ToArray();
                // 4. 报告发现交点 返回线段集合
                if (Result.TryGetValue(point, out IntersectEvent resultSet))
                {
                    resultSet.lines.AddRange(unionSet);
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
                    LineTool2D.Line line = _lineSweepCache[i];
                    if (centerAndLowerSet.Contains(line))
                    {
                        lastDeleteIndex = _lineSweepCache.IndexOf(line);

                        _lineSweepCache.RemoveAt(lastDeleteIndex);
                    }
                }
                else
                {
                    LineTool2D.Line line = _lineSweepCache[lastDeleteIndex];
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
                LineTool2D.Line lLine = _lineSweepCache[nearIndex];
                LineTool2D.Line rLine = _lineSweepCache[nearIndex + 1];
                FindNewEvent(lLine, rLine, point);
            }
        }
        else
        {
            // 找到距离 sl 最近的左边的线段 firstIndex - 1
            int nearIndex = FindLastNearIndexByPoint(point);
            if (nearIndex >= 0)
            {
                foreach (LineTool2D.Line line in upperAndCenterSet)
                {
                    FindNewEvent(_lineSweepCache[nearIndex], line, point);
                }
            }

            // 找到距离 sr 最近的右边的线段 firstIndex + upperAndCenterSet.Length
            if (firstIndex + upperAndCenterSet.Length < _lineSweepCache.Count)
            {
                foreach (LineTool2D.Line line in upperAndCenterSet)
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
            LineTool2D.Line line = _lineSweepCache[i];
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
            else if (lineX < point.x - float.Epsilon)
            {
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    ///     找到新交点事件
    /// </summary>
    private void FindNewEvent(LineTool2D.Line l1, LineTool2D.Line l2, Vector2 eventPoint)
    {
        Vector2? intersectPoint = LineTool2D.Intersect(l1, l2);
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
        if (_centerPointCache.TryGetValue(intersectPoint.Value, out HashSet<LineTool2D.Line> set))
        {
            set.Add(l1);
            set.Add(l2);
        }
        else
        {
            _centerPointCache.Add(intersectPoint.Value, new HashSet<LineTool2D.Line>() { l1, l2, });
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
    ///     添加事件点
    /// </summary>
    private void PushEvent(Vector2 point)
    {
        if (_lineSortedEventCache.ContainsKey(point))
        {
            return;
        }

        _lineSortedEventCache.Add(point, point);
    }

    private class EventComparer : IComparer<Vector2>
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
    public struct IntersectEvent
    {
        public Vector2               point;
        public List<LineTool2D.Line> lines;
    }
}
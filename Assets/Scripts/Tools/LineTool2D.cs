#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Tools
{
    public static class LineTool2D
    {
        public static void LineCast2(in Vector2Int p0, in Vector2Int p1, in Func<Vector2Int, bool> plot)
        {
            int x0 = p0.x;
            int x1 = p1.x;
            int y0 = p0.y;
            int y1 = p1.y;

            if (!plot(p0))
            {
                return;
            }

            Vector2Int temp = Vector2Int.zero;
            Vector2Int offset = p1 - p0;
            if (offset == Vector2Int.zero)
            {
                return;
            }

            int xStep = offset.x >= 0 ? 1 : -1;
            int yStep = offset.y >= 0 ? 1 : -1;
            if (offset.x == 0)
            {
                // y step
                while (temp != offset)
                {
                    temp.y += yStep;
                    if (!plot(p0 + temp))
                    {
                        return;
                    }
                }

                return;
            }
            else if (offset.y == 0)
            {
                // x step
                while (temp != offset)
                {
                    temp.x += xStep;
                    if (!plot(p0 + temp))
                    {
                        return;
                    }
                }

                return;
            }
            else
            {
                // normal step
                int absY = math.abs(offset.y);
                int absX = math.abs(offset.x);

                if (absY == absX)
                {
                    // 45 degree
                    Vector2Int step = new Vector2Int(xStep, yStep);
                    while (temp != offset)
                    {
                        temp += step;
                        if (!plot(p0 + temp))
                        {
                            return;
                        }
                    }

                    return;
                }

                float slope = offset.y / (float)offset.x;
                if (absY > absX)
                {
                    // y longer
                    while (math.abs(temp.x) < math.abs(offset.x))
                    {
                        temp.y += yStep;
                        if (math.abs(temp.x - temp.y / slope) >= 1f)
                        {
                            temp.x += xStep;
                        }

                        if (!plot(p0 + temp))
                        {
                            return;
                        }
                    }

                    return;
                }
                else
                {
                    // x longer
                    while (math.abs(temp.y) < math.abs(offset.y))
                    {
                        temp.x += xStep;
                        if (math.abs(temp.y - temp.x * slope) >= 1f)
                        {
                            temp.y += yStep;
                        }

                        if (!plot(p0 + temp))
                        {
                            return;
                        }
                    }

                    return;
                }
            }
        }

        public static Vector2Int[] LineCast(in Vector2Int p0, in Vector2Int p1)
        {
            List<Vector2Int> result = new();
            int x0 = p0.x;
            int x1 = p1.x;
            int y0 = p0.y;
            int y1 = p1.y;
            bool steep = math.abs(y1 - y0) > math.abs(x1 - x0);
            if (steep)
            {
                x0 = p0.y;
                x1 = p1.y;
                y0 = p0.x;
                y1 = p1.x;
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            int deltaX = x1 - x0;
            int deltaY = math.abs(y1 - y0);
            int error = deltaX / 2;
            int yStep = y0 < y1 ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    result.Add(new Vector2Int(y, x));
                }
                else
                {
                    result.Add(new Vector2Int(x, y));
                }

                error -= deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }

            if (p0.x < p1.x)
            {
                if (p0.y < p1.y)
                {
                    return result.OrderBy(v => v.x)
                                 .ThenBy(v => v.y)
                                 .ToArrayPooled();
                }
                else if (p0.y > p1.y)
                {
                    return result.OrderBy(v => v.x)
                                 .ThenByDescending(v => v.y)
                                 .ToArrayPooled();
                }
                else
                {
                    return result.OrderBy(v => v.x)
                                 .ToArrayPooled();
                }
            }
            else if (p0.x > p1.x)
            {
                if (p0.y < p1.y)
                {
                    return result.OrderByDescending(v => v.x)
                                 .ThenBy(v => v.y)
                                 .ToArrayPooled();
                }
                else if (p0.y > p1.y)
                {
                    return result.OrderByDescending(v => v.x)
                                 .ThenByDescending(v => v.y)
                                 .ToArrayPooled();
                }
                else
                {
                    return result.OrderByDescending(v => v.x)
                                 .ToArrayPooled();
                }
            }
            else
            {
                if (p0.y < p1.y)
                {
                    return result.OrderBy(v => v.y)
                                 .ToArrayPooled();
                }
                else if (p0.y > p1.y)
                {
                    return result.OrderByDescending(v => v.y)
                                 .ToArrayPooled();
                }
                else
                {
                    return result.ToArrayPooled();
                }
            }
        }

        /// <summary>
        ///     Bresenham 算法绘制线段 if plot return false, stop draw
        /// </summary>
        public static void DrawLine(in Vector2Int p0, in Vector2Int p1, in Func<Vector2Int, bool> plot)
        {
            int x0 = p0.x;
            int x1 = p1.x;
            int y0 = p0.y;
            int y1 = p1.y;
            bool steep = math.abs(y1 - y0) > math.abs(x1 - x0);
            if (steep)
            {
                x0 = p0.y;
                x1 = p1.y;
                y0 = p0.x;
                y1 = p1.x;
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            int deltaX = x1 - x0;
            int deltaY = math.abs(y1 - y0);
            int error = deltaX / 2;
            int yStep = y0 < y1 ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    if (!plot(new Vector2Int(y, x)))
                    {
                        return;
                    }
                }
                else
                {
                    if (!plot(new Vector2Int(x, y)))
                    {
                        return;
                    }
                }

                error -= deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }
        }

        /// <summary>
        ///     2d 线段求交点
        /// </summary>
        public static Vector2? Intersect(Line l1, Line l2)
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
        ///     点与线段的距离
        /// </summary>
        public static float PointLineDistance(Vector2 point, Line line)
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
        ///     重排线段 使得线段在 y 轴上的投影是从左到右的
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="yValue"></param>
        public static void SortedArrayByPointY(ref Line[] arr, float yValue)
        {
            Array.Sort(arr, (l1, l2) =>
            {
                Vector2 l1Dir = l1.top - l1.bottom;
                float l1Slope = l1Dir.y / l1Dir.x;
                float l1X = l1.bottom.x + (yValue - l1.bottom.y) / l1Slope;

                Vector2 l2Dir = l2.top - l2.bottom;
                float l2Slope = l2Dir.y / l2Dir.x;
                float l2X = l2.bottom.x + (yValue - l2.bottom.y) / l2Slope;

                return l1X.CompareTo(l2X);
            });
        }

        /// <summary>
        ///     重新排序线段，使得线段的顶点总是在底点的上方
        /// </summary>
        private static void ReOrderLine(ref Line line)
        {
            if (line.top.y < line.bottom.y
             || (line.top.y.Equals(line.bottom.y) && line.top.x > line.bottom.x))
            {
                Vector2 temp = line.top;
                line.top = line.bottom;
                line.bottom = temp;
            }
        }

        /// <summary>
        ///     重新排序线段，使得线段的顶点总是在底点的上方
        /// </summary>
        public static void ReOrderLines(ref Line[] lines)
        {
            // top.y > bottom.y
            // top.x < bottom.x
            for (int i = 0; i < lines.Length; i++)
            {
                ReOrderLine(ref lines[i]);
            }
        }

        [Serializable]
        public struct Line
        {
            public Vector2 top;
            public Vector2 bottom;
        }
    }
}
#nullable enable
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Tools
{
    public static class Line2D
    {
        public static int LineCastNonAlloc(in Vector2Int p0, in Vector2Int p1, ref Vector2Int[] results)
        {
            int resultCount = 0;
            int resultMaxLength = results.Length;

            if (resultMaxLength == 0)
            {
                return resultCount;
            }

            int x0 = p0.x;
            int x1 = p1.x;
            int y0 = p0.y;
            int y1 = p1.y;

            Vector2Int offset = p1 - p0;
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
                    results[resultCount].x = y;
                    results[resultCount].y = x;
                    resultCount++;
                }
                else
                {
                    results[resultCount].x = x;
                    results[resultCount].y = y;
                    resultCount++;
                }

                if (resultCount >= resultMaxLength)
                {
                    if ((offset.x <= 0 && offset.y <= 0)
                     || (offset.x <= 0 && offset.y >= 0 && math.abs(offset.x) >= math.abs(offset.y))
                     || (offset.x >= 0 && offset.y <= 0 && math.abs(offset.x) < math.abs(offset.y)))
                    {
                        Array.Reverse(results, 0, resultCount);
                    }

                    return resultCount;
                }

                error -= deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }

            if ((offset.x <= 0 && offset.y <= 0)
             || (offset.x <= 0 && offset.y >= 0 && math.abs(offset.x) >= math.abs(offset.y))
             || (offset.x >= 0 && offset.y <= 0 && math.abs(offset.x) < math.abs(offset.y)))
            {
                Array.Reverse(results, 0, resultCount);
            }

            return resultCount;
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
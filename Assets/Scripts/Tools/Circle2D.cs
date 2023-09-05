#nullable enable
using UnityEngine;

namespace Tools
{
    public static class Circle2D
    {
        /// <summary>
        ///     绘制空心圆形
        /// </summary>
        public static int DrawCircleNonAlloc(in Vector2Int center, int radius, ref Vector2Int[] results)
        {
            int resultCount = 0;
            int resultMaxLength = results.Length;

            if (resultMaxLength == 0)
            {
                return resultCount;
            }

            if (radius < 0)
            {
                return resultCount;
            }
            else if (radius == 0)
            {
                results[resultCount++] = center;
            }

            if (resultMaxLength <= resultCount)
            {
                return resultCount;
            }

            Draw4PointNonAlloc(center, radius, ref results, ref resultCount);
            if (resultMaxLength <= resultCount)
            {
                return resultCount;
            }

            int r2 = radius * radius;
            Vector2Int offset = new(radius, 0);
            do
            {
                offset.y += 1;
                while (offset.x * offset.x + offset.y * offset.y > r2)
                {
                    offset.x -= 1;

                    if (offset.y >= offset.x)
                    {
                        break;
                    }
                }

                if (offset.y > offset.x)
                {
                    break;
                }
                else if (offset.y == offset.x)
                {
                    DrawMirrorPointNonAlloc(center, offset, ref results, ref resultCount);
                    if (resultMaxLength <= resultCount)
                    {
                        return resultCount;
                    }
                }
                else
                {
                    DrawMirrorPointNonAlloc(center, offset, ref results, ref resultCount);
                    if (resultMaxLength <= resultCount)
                    {
                        return resultCount;
                    }

                    DrawMirrorPointNonAlloc(center, SlopePoint(offset), ref results, ref resultCount);
                    if (resultMaxLength <= resultCount)
                    {
                        return resultCount;
                    }
                }
            } while (offset.y <= offset.x);

            return resultCount;
        }

        private static Vector2Int SlopePoint(in Vector2Int offset) => new(offset.y, offset.x);

        private static void DrawMirrorPointNonAlloc(in Vector2Int center, in Vector2Int offset, ref Vector2Int[] results, ref int startIndex)
        {
            if (results.Length > startIndex + 4)
            {
                results[startIndex++] = center + new Vector2Int(offset.x, offset.y);
                results[startIndex++] = center + new Vector2Int(-offset.x, offset.y);
                results[startIndex++] = center + new Vector2Int(offset.x, -offset.y);
                results[startIndex++] = center + new Vector2Int(-offset.x, -offset.y);
            }
        }

        private static void Draw4PointNonAlloc(in Vector2Int center, int radius, ref Vector2Int[] results, ref int startIndex)
        {
            if (results.Length > startIndex + 4)
            {
                results[startIndex++] = new Vector2Int(center.x - radius, center.y);
                results[startIndex++] = new Vector2Int(center.x + radius, center.y);
                results[startIndex++] = new Vector2Int(center.x, center.y + radius);
                results[startIndex++] = new Vector2Int(center.x, center.y - radius);
            }
        }
    }
}
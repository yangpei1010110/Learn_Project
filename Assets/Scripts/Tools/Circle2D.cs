#nullable enable
using System;
using UnityEngine;

namespace Tools
{
    public static class Circle2D
    {
        /// <summary>
        ///     绘制空心圆形
        /// </summary>
        public static void DrawCircle(in Vector2Int center, int radius, in Action<Vector2Int> setPixel)
        {
            if (radius < 0)
            {
                return;
            }
            else if (radius == 0)
            {
                setPixel.Invoke(center);
            }

            Draw4Point(center, radius, setPixel);
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
                    DrawMirrorPoint(center, offset, setPixel);
                }
                else
                {
                    DrawMirrorPoint(center, offset, setPixel);
                    DrawMirrorPoint(center, SlopePoint(offset), setPixel);
                }
            } while (offset.y <= offset.x);
        }

        private static Vector2Int SlopePoint(in Vector2Int offset) => new(offset.y, offset.x);

        private static void DrawMirrorPoint(in Vector2Int center, in Vector2Int offset, in Action<Vector2Int> setPixel)
        {
            setPixel.Invoke(center + new Vector2Int(offset.x, offset.y));
            setPixel.Invoke(center + new Vector2Int(-offset.x, offset.y));
            setPixel.Invoke(center + new Vector2Int(offset.x, -offset.y));
            setPixel.Invoke(center + new Vector2Int(-offset.x, -offset.y));
        }

        private static void Draw4Point(in Vector2Int center, int radius, in Action<Vector2Int> setPixel)
        {
            setPixel.Invoke(new Vector2Int(center.x - radius, center.y));
            setPixel.Invoke(new Vector2Int(center.x + radius, center.y));
            setPixel.Invoke(new Vector2Int(center.x, center.y + radius));
            setPixel.Invoke(new Vector2Int(center.x, center.y - radius));
        }
    }
}
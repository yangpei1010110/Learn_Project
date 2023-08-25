#nullable enable

using UnityEngine;

namespace SandBox.Map
{
    /// <summary>
    ///     地图相对坐标计算
    /// </summary>
    public static class MapOffset
    {
        /// <summary>
        ///     Unity 世界坐标获取地图本地坐标
        /// </summary>
        public static Vector2Int WorldToLocal(in Vector2 worldPosition, in int mapLocalSizePerUnit, in float mapWorldSizePerUnit)
        {
            Vector2Int globalPosition = WorldToGlobal(worldPosition, mapLocalSizePerUnit, mapWorldSizePerUnit);
            Vector2Int localPosition = GlobalToLocal(globalPosition, mapLocalSizePerUnit);
            return localPosition;
        }

        /// <summary>
        ///     Unity 世界坐标获取地图全局坐标
        /// </summary>
        public static Vector2Int WorldToGlobal(in Vector2 worldPosition, in int mapLocalSizePerUnit, in float mapWorldSizePerUnit)
        {
            float x = worldPosition.x;
            float y = worldPosition.y;

            x /= mapWorldSizePerUnit;
            y /= mapWorldSizePerUnit;

            x *= mapLocalSizePerUnit;
            y *= mapLocalSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        /// <summary>
        ///     地图全局坐标获得地图块索引
        /// </summary>
        public static Vector2Int GlobalToBlock(in Vector2Int globalIndex, in int mapLocalSizePerUnit)
        {
            float x = globalIndex.x;
            float y = globalIndex.y;

            x /= mapLocalSizePerUnit;
            y /= mapLocalSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        /// <summary>
        ///     从 Unity 世界坐标获取地图块索引
        /// </summary>
        public static Vector2Int WorldToBlock(in Vector2 worldPosition, in float mapWorldSizePerUnit)
        {
            float x = worldPosition.x;
            float y = worldPosition.y;

            x /= mapWorldSizePerUnit;
            y /= mapWorldSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        /// <summary>
        ///     根据相对地图索引和地图全局坐标获得地图局部坐标
        /// </summary>
        public static Vector2Int GlobalToLocal(in Vector2Int globalIndex, in int mapLocalSizePerUnit)
        {
            int x = globalIndex.x;
            int y = globalIndex.y;

            x %= mapLocalSizePerUnit;
            y %= mapLocalSizePerUnit;

            x = x < 0 ? x + mapLocalSizePerUnit : x;
            y = y < 0 ? y + mapLocalSizePerUnit : y;

            return new Vector2Int(x, y);
        }

        /// <summary>
        ///     根据地图块索引和地图局部坐标获得地图全局坐标
        /// </summary>
        public static Vector2Int LocalToGlobal(in Vector2Int blockIndex, in Vector2Int localIndex, in int mapSize)
        {
            Vector2Int mapOriginIndex = blockIndex * mapSize;
            return mapOriginIndex + localIndex;
        }
    }
}
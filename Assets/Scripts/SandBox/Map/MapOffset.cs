using UnityEngine;

namespace SandBox.Map
{
    /// <summary>
    ///     地图相对坐标计算
    /// </summary>
    public static class MapOffset
    {
        public static Vector2Int WorldToLocal(Vector2 worldPosition, int mapLocalSizePerUnit, float mapWorldSizePerUnit)
        {
            var globalPosition = WorldToGlobal(worldPosition, mapLocalSizePerUnit, mapWorldSizePerUnit);
            var localPosition = GlobalToLocal(globalPosition, mapLocalSizePerUnit);
            return localPosition;
        }

        /// <summary>
        ///     Unity 世界坐标获取地图全局坐标
        /// </summary>
        public static Vector2Int WorldToGlobal(Vector2 worldPosition, int mapLocalSizePerUnit, float mapWorldSizePerUnit)
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
        public static Vector2Int BlockIndex(Vector2Int globalPosition, int mapLocalSizePerUnit)
        {
            float x = globalPosition.x;
            float y = globalPosition.y;

            x /= mapLocalSizePerUnit;
            y /= mapLocalSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        /// <summary>
        ///     从 Unity 世界坐标获取地图块索引
        /// </summary>
        public static Vector2Int BlockIndex(Vector2 worldPosition, float mapWorldSizePerUnit)
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
        public static Vector2Int GlobalToLocal(Vector2Int globalPosition, int mapLocalSizePerUnit)
        {
            int x = globalPosition.x;
            int y = globalPosition.y;

            x %= mapLocalSizePerUnit;
            y %= mapLocalSizePerUnit;

            globalPosition.x = x < 0 ? x + mapLocalSizePerUnit : x;
            globalPosition.y = y < 0 ? y + mapLocalSizePerUnit : y;

            return globalPosition;
        }

        /// <summary>
        ///     根据地图块索引和地图局部坐标获得地图全局坐标
        /// </summary>
        public static Vector2Int LocalToGlobal(Vector2Int mapBlockIndex, Vector2Int localIndex, int mapSize)
        {
            Vector2Int mapOriginIndex = mapBlockIndex * mapSize;
            return mapOriginIndex + localIndex;
        }
    }
}
#nullable enable

using UnityEngine;

namespace SandBox.Map
{
    /// <summary>
    ///     地图相对坐标计算
    ///     World: Unity 世界坐标
    ///     Local: 地图块本地坐标
    ///     Global: 地图块全局坐标
    ///     Block: 地图块索引坐标
    /// </summary>
    public static class MapOffset
    {
        #region LocalTo

        public static Vector2Int LocalToGlobal(in Vector2Int blockIndex, in Vector2Int localIndex) => blockIndex * MapSetting.MapLocalSizePerUnit + localIndex;

        #endregion

        #region BlockTo

        public static Vector2 BlockToWorld(in Vector2Int blockIndex) => (blockIndex + MapSetting.SpritePivot) * MapSetting.MapWorldSizePerUnit;

        #endregion

        #region WorldTo

        public static Vector2Int WorldToLocal(in Vector2 worldPosition)
        {
            Vector2Int globalPosition = WorldToGlobal(worldPosition);
            Vector2Int localPosition = GlobalToLocal(globalPosition);
            return localPosition;
        }

        public static Vector2Int WorldToGlobal(in Vector2 worldPosition)
        {
            float x = worldPosition.x;
            float y = worldPosition.y;

            x /= MapSetting.MapWorldSizePerUnit;
            y /= MapSetting.MapWorldSizePerUnit;

            x *= MapSetting.MapLocalSizePerUnit;
            y *= MapSetting.MapLocalSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public static Vector2Int WorldToBlock(in Vector2 worldPosition)
        {
            float x = worldPosition.x;
            float y = worldPosition.y;

            x /= MapSetting.MapWorldSizePerUnit;
            y /= MapSetting.MapWorldSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        #endregion

        #region GlobalTo

        public static Vector2Int GlobalRound(in Vector2 floatGlobalIndex)
        {
            float x = floatGlobalIndex.x;
            float y = floatGlobalIndex.y;

            // x /= MapSetting.MapLocalSizePerUnit;
            // y /= MapSetting.MapLocalSizePerUnit;

            return new Vector2Int((int)(x + 0.5f), (int)(y + 0.5f));
        }

        public static Vector2Int GlobalToBlock(in Vector2Int globalIndex)
        {
            float x = globalIndex.x;
            float y = globalIndex.y;

            x /= MapSetting.MapLocalSizePerUnit;
            y /= MapSetting.MapLocalSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }


        public static Vector2Int GlobalToLocal(in Vector2Int globalIndex)
        {
            int x = globalIndex.x;
            int y = globalIndex.y;

            x %= MapSetting.MapLocalSizePerUnit;
            y %= MapSetting.MapLocalSizePerUnit;

            x = x < 0 ? x + MapSetting.MapLocalSizePerUnit : x;
            y = y < 0 ? y + MapSetting.MapLocalSizePerUnit : y;

            return new Vector2Int(x, y);
        }

        #endregion
    }
}
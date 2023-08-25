#nullable enable

using SandBox.Elements.Liquid;
using SandBox.Elements.Solid;
using SandBox.Map;
using Tools;
using Unity.Mathematics;
using UnityEngine;

namespace SandBox
{
    public class SandBoxWorld : MonoBehaviour
    {
        public SandBoxMap SandBoxMap;

        private void Start()
        {
            SandBoxMap = new SandBoxMap();
        }

        private void Update()
        {
            UpdateInput();
            UpdateTexture();
        }

        private void OnDrawGizmos()
        {
            // draw dirty rect
            Gizmos.color = Color.red;
            foreach (MapBlock block in SparseSandBoxMap.Instance._mapBlocks.Values)
            {
                MapSetting mapSetting = MapSetting.Instance;
                Vector2Int minRect = new(block._dirtyRectMinX, block._dirtyRectMinY);
                Vector2Int maxRect = new(block._dirtyRectMaxX, block._dirtyRectMaxY);

                Vector2 globalMinRect = (Vector2)MapOffset.LocalToGlobal(block.BlockIndex, minRect, mapSetting.MapLocalSizePerUnit);
                Vector2 globalMaxRect = (Vector2)MapOffset.LocalToGlobal(block.BlockIndex, maxRect, mapSetting.MapLocalSizePerUnit);

                if (globalMaxRect.y < globalMinRect.y || globalMaxRect.x < globalMinRect.x)
                {
                    continue;
                }

                float pixelSize = mapSetting.MapWorldSizePerUnit / mapSetting.MapLocalSizePerUnit;
                globalMinRect *= pixelSize;
                globalMaxRect *= pixelSize;

                globalMinRect.x += pixelSize;
                globalMinRect.y -= pixelSize;
                globalMaxRect.x += pixelSize;
                globalMaxRect.x += pixelSize;
                // Vector2 worldRectMin = (Vector2)block.BlockIndex * mapSetting.MapWorldSizePerUnit + (Vector2)minRect / mapSetting.MapLocalSizePerUnit;
                // Vector2 worldRectMax = (Vector2)block.BlockIndex * mapSetting.MapWorldSizePerUnit + (Vector2)maxRect / mapSetting.MapLocalSizePerUnit;

                Vector2 topLeft = new(globalMinRect.x - pixelSize, globalMaxRect.y + pixelSize);
                Vector2 topRight = new(globalMaxRect.x - pixelSize, globalMaxRect.y + pixelSize);
                Vector2 bottomLeft = new(globalMinRect.x - pixelSize, globalMinRect.y + pixelSize);
                Vector2 bottomRight = new(globalMaxRect.x - pixelSize, globalMinRect.y + pixelSize);

                Gizmos.DrawLine(topLeft, topRight);
                Gizmos.DrawLine(topRight, bottomRight);
                Gizmos.DrawLine(bottomRight, bottomLeft);
                Gizmos.DrawLine(bottomLeft, topLeft);
            }
        }


        private void UpdateInput()
        {
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    SandBoxMap[mousePosition] = new Sand();
                }
                else if (Input.GetMouseButton(1))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    SandBoxMap[mousePosition] = new Water();
                }
            }
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    mouseCircleRadius--;
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    mouseCircleRadius++;
                }

                mouseCircleRadius = math.clamp(mouseCircleRadius, 1, 10);
            }
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition, MapSetting.Instance.MapLocalSizePerUnit, MapSetting.Instance.MapWorldSizePerUnit);
                UpdateCircleSprite(mouseGlobalIndex);
            }
        }

        private void UpdateTexture()
        {
            SandBoxMap.UpdateMap();
        }

        #region 鼠标范围显示

        private int         mouseCircleRadius = 1;
        private Vector2Int? lastUpdateSpriteIndex;

        private void UpdateCircleSprite(in Vector2Int mouseGlobalIndex)
        {
            // 更新最后一帧的区块
            if (lastUpdateSpriteIndex.HasValue)
            {
                SparseSpriteMap.Instance.UpdateColorFormMapBlock(lastUpdateSpriteIndex.Value);
            }

            // 鼠标指向的区块不存在
            if (!SandBoxMap.Exist(mouseGlobalIndex))
            {
                return;
            }

            Vector2Int blockIndex = MapOffset.GlobalToBlock(mouseGlobalIndex, MapSetting.Instance.MapLocalSizePerUnit);
            Vector2Int localIndex = MapOffset.GlobalToLocal(mouseGlobalIndex, MapSetting.Instance.MapLocalSizePerUnit);
            Texture2D texture = SparseSpriteMap.Instance._mapBlockTexture[blockIndex];
            CircleTool2D.DrawCircle(localIndex, mouseCircleRadius, pixel => texture.SetPixel(pixel.x, pixel.y, Color.white));
            texture.Apply();
            lastUpdateSpriteIndex = blockIndex;
        }

        #endregion
    }
}
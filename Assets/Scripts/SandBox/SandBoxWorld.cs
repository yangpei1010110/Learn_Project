#nullable enable

using System.Collections.Generic;
using SandBox.Elements.Liquid;
using SandBox.Elements.Solid;
using SandBox.Map;
using SandBox.Map.SpriteMap;
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
                Vector2Int minRect = new(block._dirtyRectMinX, block._dirtyRectMinY);
                Vector2Int maxRect = new(block._dirtyRectMaxX, block._dirtyRectMaxY);

                Vector2 globalMinRect = (Vector2)MapOffset.LocalToGlobal(block.BlockIndex, minRect);
                Vector2 globalMaxRect = (Vector2)MapOffset.LocalToGlobal(block.BlockIndex, maxRect);

                if (globalMaxRect.y < globalMinRect.y || globalMaxRect.x < globalMinRect.x)
                {
                    continue;
                }

                float pixelSize = MapSetting.MapWorldSizePerUnit / MapSetting.MapLocalSizePerUnit;
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
                mouseCircleRadiusY = math.clamp(mouseCircleRadiusY + Input.mouseScrollDelta.y, 0f, maxMouseCircleRadius);
                mouseCircleRadius = (int)mouseCircleRadiusY;
            }
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition);
                UpdateCircleSprite(mouseGlobalIndex);
            }
        }

        private void UpdateTexture()
        {
            SandBoxMap.UpdateMap();
        }

        #region 鼠标范围显示

        private float               mouseCircleRadiusY    = 1f;
        private int                 mouseCircleRadius     = 1;
        private int                 maxMouseCircleRadius  = 100;
        private HashSet<Vector2Int> lastUpdateSpriteIndex = new();

        private void UpdateCircleSprite(in Vector2Int mouseGlobalIndex)
        {
            // 更新最后一帧的区块
            foreach (Vector2Int blockIndex in lastUpdateSpriteIndex)
            {
                SparseSpriteMap2.Instance.UpdateColorFormMapBlock(blockIndex);
            }

            CircleTool2D.DrawCircle(mouseGlobalIndex, mouseCircleRadius, pixel =>
            {
                SparseSpriteMap2.Instance.SafeSet(pixel, Color.white);
                lastUpdateSpriteIndex.Add(MapOffset.GlobalToBlock(pixel));
            });
        }

        #endregion
    }
}
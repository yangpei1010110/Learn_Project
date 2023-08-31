#nullable enable

using System.Collections.Generic;
using SandBox.Elements.Interface;
using SandBox.Elements.Liquid;
using SandBox.Elements.Solid;
using SandBox.Map;
using SandBox.Map.SandBox;
using SandBox.Map.Sprite;
using SandBox.Physics;
using Tools;
using Unity.Mathematics;
using UnityEngine;

namespace SandBox
{
    public class SandBoxWorld : MonoBehaviour
    {
        // public SandBoxMap SandBoxMap;
        //
        // private void Start()
        // {
        //     SandBoxMap = new SandBoxMap();
        // }

        private void Update()
        {
            UpdateInput();
            // UpdateTexture();
            UpdateParticle(Time.deltaTime);
            ElementPhysicsSetting.GravityPoint = null;
        }

        private void OnDrawGizmos()
        {
            // draw dirty rect
            Gizmos.color = Color.red;
            foreach (MapBlock2<IElement> block in SparseSandBoxMap2.Instance._mapBlocks.Values)
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
                    Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition);
                    IElement element = new Sand();
                    SparseSandBoxMap2.Instance[mouseGlobalIndex] = element;
                    SparseSandBoxMap2.Instance.SetDirty(mouseGlobalIndex);
                    SparseSpriteMap.Instance[mouseGlobalIndex] = element.Color;
                }
                else if (Input.GetMouseButton(1))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition);
                    IElement element = new Water();
                    SparseSandBoxMap2.Instance[mouseGlobalIndex] = element;
                    SparseSandBoxMap2.Instance.SetDirty(mouseGlobalIndex);
                    SparseSpriteMap.Instance[mouseGlobalIndex] = element.Color;
                }
                else if (Input.GetMouseButton(2))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition);
                    ElementPhysicsSetting.GravityPoint = mouseGlobalIndex;
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
                UpdateLine(mouseGlobalIndex);
            }
        }

        private void UpdateParticle(in float deltaTime)
        {
            SparseSandBoxMap2.Instance.UpdateParticle(deltaTime);
            SparseSpriteMap.Instance.Flush();
        }

        // private void UpdateTexture()
        // {
        //     SandBoxMap.UpdateMap();
        // }

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
                SparseSpriteMap.Instance.UpdateColorFormMapBlock(blockIndex);
            }

            CircleTool2D.DrawCircle(mouseGlobalIndex, mouseCircleRadius, pixel =>
            {
                SparseSpriteMap.Instance.SafeSet(pixel, Color.white);
                lastUpdateSpriteIndex.Add(MapOffset.GlobalToBlock(pixel));
            });
        }

        private void UpdateLine(in Vector2Int mouseGlobalIndex)
        {
            LineTool2D.DrawLine(Vector2Int.zero, mouseGlobalIndex, pixel =>
            {
                SparseSpriteMap.Instance.SafeSet(pixel, Color.red);
                lastUpdateSpriteIndex.Add(MapOffset.GlobalToBlock(pixel));
                return true;
            });
        }

        #endregion
    }
}
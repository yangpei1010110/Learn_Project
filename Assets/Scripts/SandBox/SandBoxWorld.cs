#nullable enable

using System;
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
        private static SparseSandBoxMap _cacheSparseSandBoxMap = SparseSandBoxMap.Instance;
        private static SparseSpriteMap  cacheSparseSpriteMap   = SparseSpriteMap.Instance;
        public static  Type?            Selected;

        private void Update()
        {
            UpdateInput();
            UpdateParticle(Time.deltaTime);
            ElementPhysicsSetting.GravityPoint = null;
        }

        private void OnDrawGizmos()
        {
            // draw dirty rect
            Gizmos.color = Color.red;
            foreach (MapBlock<IElement> block in _cacheSparseSandBoxMap._mapBlocks.Values)
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
                if (Input.GetMouseButton(0) && Selected != null && Selected.GetInterface(nameof(IElement)) != null)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2Int mouseGlobalIndex = MapOffset.WorldToGlobal(mousePosition);
                    for (int y = -mouseCircleRadius; y <= mouseCircleRadius; y++)
                    for (int x = -mouseCircleRadius; x <= mouseCircleRadius; x++)
                    {
                        if (y * y + x * x <= mouseCircleRadius * mouseCircleRadius)
                        {
                            Vector2Int elementGlobalIndex = mouseGlobalIndex + new Vector2Int(x, y);
                            IElement element = (IElement)Activator.CreateInstance(Selected);
                            _cacheSparseSandBoxMap[elementGlobalIndex] = element;
                            _cacheSparseSandBoxMap.SetDirty(elementGlobalIndex);
                            cacheSparseSpriteMap[elementGlobalIndex] = element.Color;
                        }
                    }
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
                #if UNITY_EDITOR
                UpdateLine(mouseGlobalIndex);
                #endif
            }
        }

        private void UpdateParticle(in float deltaTime)
        {
            _cacheSparseSandBoxMap.UpdateParticle(deltaTime);
            cacheSparseSpriteMap.Flush();
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
                cacheSparseSpriteMap.UpdateColorFormMapBlock(blockIndex);
            }

            CircleTool2D.DrawCircle(mouseGlobalIndex, mouseCircleRadius, pixel =>
            {
                cacheSparseSpriteMap.SafeSet(pixel, Color.white);
                lastUpdateSpriteIndex.Add(MapOffset.GlobalToBlock(pixel));
            });
        }

        private void UpdateLine(in Vector2Int mouseGlobalIndex)
        {
            LineTool2D.LineCast2(Vector2Int.zero, mouseGlobalIndex, pixel =>
            {
                cacheSparseSpriteMap.SafeSet(pixel, Color.red);
                lastUpdateSpriteIndex.Add(MapOffset.GlobalToBlock(pixel));
                return true;
            });
        }

        #endregion
    }
}
#nullable enable

using SandBox.Elements.Liquid;
using SandBox.Elements.Solid;
using SandBox.Map;
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

        private void UpdateTexture()
        {
            SandBoxMap.UpdateMap();
        }
    }
}
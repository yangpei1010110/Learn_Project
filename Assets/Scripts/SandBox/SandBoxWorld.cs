using System;
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

        private void OnDrawGizmos()
        {
            // draw dirty rect
            Gizmos.color = Color.red;
            foreach (var block in SparseSandBoxMap.Instance._mapBlocks.Values)
            {
                var minRect = new Vector2(block._dirtyRectMinX, block._dirtyRectMinY);
                var maxRect = new Vector2(block._dirtyRectMaxX, block._dirtyRectMaxY);

                if (maxRect.y < minRect.y || maxRect.x < minRect.x)
                {
                    continue;
                }

                var mapSetting = MapSetting.Instance;
                Vector2 worldRectMin = (Vector2)block.MapIndex * mapSetting.MapWorldSizePerUnit + (Vector2)minRect / mapSetting.MapLocalSizePerUnit;
                Vector2 worldRectMax = (Vector2)block.MapIndex * mapSetting.MapWorldSizePerUnit + (Vector2)maxRect / mapSetting.MapLocalSizePerUnit;

                var topLeft = new Vector2(worldRectMin.x, worldRectMax.y);
                var topRight = new Vector2(worldRectMax.x, worldRectMax.y);
                var bottomLeft = new Vector2(worldRectMin.x, worldRectMin.y);
                var bottomRight = new Vector2(worldRectMax.x, worldRectMin.y);

                Gizmos.DrawLine(topLeft, topRight);
                Gizmos.DrawLine(topRight, bottomRight);
                Gizmos.DrawLine(bottomRight, bottomLeft);
                Gizmos.DrawLine(bottomLeft, topLeft);
            }
        }
    }
}
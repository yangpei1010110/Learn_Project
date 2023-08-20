using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements.Solid
{
    public struct Sand : IElement
    {
        public Vector2Int  Position { get; set; }
        public Color       Color    => Color.yellow;
        public float       Density  { get; set; }
        public ElementType Type     => ElementType.Solid;

        // public Sand()
        // {
        //     Color = Color.yellow;
        //     Density = 1;
        //     Type = ElementType.Solid;
        // }

        public void UpdateElement(SandBoxMap map, Vector2Int globalPosition)
        {
            Vector2Int downPosition = globalPosition + Vector2Int.down;
            if (!map.Exist(downPosition))
            {
                return;
            }

            if (map[downPosition].Type == ElementType.Void)
            {
                IElement temp = map[downPosition];
                map[downPosition] = map[globalPosition];
                map[globalPosition] = temp;
                return;
            }

            Vector2Int downLeftPosition = globalPosition + Vector2Int.down + Vector2Int.left;
            if (map[downLeftPosition].Type == ElementType.Void)
            {
                IElement temp = map[downLeftPosition];
                map[downLeftPosition] = map[globalPosition];
                map[globalPosition] = temp;
                return;
            }

            Vector2Int downRightPosition = globalPosition + Vector2Int.down + Vector2Int.right;
            if (map[downRightPosition].Type == ElementType.Void)
            {
                IElement temp = map[downRightPosition];
                map[downRightPosition] = map[globalPosition];
                map[globalPosition] = temp;
                return;
            }
        }
    }
}
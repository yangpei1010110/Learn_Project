using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements.Solid
{
    public struct Sand : IElement
    {
        public Vector2Int  Position { get; set; }
        public Color       Color    { get; }
        public float       Density  { get; set; }
        public ElementType Type     { get; }

        public Sand(Vector2Int position)
        {
            Position = position;
            Color = Color.yellow;
            Density = 1;
            Type = ElementType.Solid;
        }

        public void UpdateElement(SandBoxMap map, Vector2Int particlePosition)
        {
            var downPosition = Position + Vector2Int.down;
            if (map[downPosition].Type == ElementType.Void)
            {
                map.Swap(downPosition, particlePosition);
                return;
            }

            var downLeftPosition = Position + Vector2Int.down + Vector2Int.left;
            if (map[downLeftPosition].Type == ElementType.Void)
            {
                map.Swap(downLeftPosition, particlePosition);
                return;
            }

            var downRightPosition = Position + Vector2Int.down + Vector2Int.right;
            if (map[downRightPosition].Type == ElementType.Void)
            {
                map.Swap(downRightPosition, particlePosition);
                return;
            }
        }
    }
}
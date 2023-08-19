using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements.Void
{
    public struct Void : IElement
    {
        public Vector2Int  Position { get; set; }
        public Color       Color    { get; }
        public float       Density { get; set; }
        public ElementType Type     { get; }

        public Void(Vector2Int position)
        {
            Position = position;
            Color = Color.black;
            Density = 0;
            Type = ElementType.Void;
        }

        public void UpdateElement(SandBoxMap map, Vector2Int particlePosition)
        {
        }
    }
}
using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements.Void
{
    public struct Void : IElement
    {
        public Vector2Int  Position { get; set; }
        public Color       Color    => Color.black;
        public float       Density  { get; set; }
        public ElementType Type     => ElementType.Void;

        public void UpdateElement(SandBoxMap map, Vector2Int globalPosition)
        {
        }
    }
}
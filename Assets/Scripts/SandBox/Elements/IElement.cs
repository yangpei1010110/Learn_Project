using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements
{
    public interface IElement
    {
        public Vector2Int  Position { get; set; }
        public Color       Color    { get; }
        public float       Density  { get; set; }
        public ElementType Type     { get; }

        public void UpdateElement(SandBoxMap map, Vector2Int globalPosition);
    }
}
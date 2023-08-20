using UnityEngine;

namespace SandBox.Elements
{
    public interface IElement
    {
        public long        Step     { get; set; }
        public Vector2Int  Position { get; set; }
        public Color       Color    { get; }
        public float       Density  { get; }
        public ElementType Type     { get; }

        public void UpdateElement(ref IElement element, Vector2Int globalPosition);
    }
}
using UnityEngine;

namespace SandBox.Elements
{
    public interface IElement
    {
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          { get; }
        public float       Density        { get; }
        public ElementType Type           { get; }
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }
        public void        UpdateElement(ref IElement element, Vector2Int globalPosition);
    }
}
#nullable enable

using UnityEngine;

namespace SandBox.Elements.Interface
{
    public interface IElement
    {
        public bool        IsStatic       { get; }
        public float       Life           { get; set; }
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          { get; }
        public float       Density        { get; }
        public ElementType Type           { get; }
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }
        public int         StableStep     { get; set; }

        public void StatusUpdate(in Vector2Int globalIndex);
    }
}
using SandBox.Elements.Interface;
using UnityEngine;

namespace SandBox.Elements.Gas
{
    public struct Smoke : IElement
    {
        public float       Life           { get; set; }
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.gray;
        public float       Density        => -0.5f;
        public ElementType Type           => ElementType.Gas;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }

        public void StatusUpdate(in Vector2Int globalIndex)
        {
        }
    }
}
#nullable enable

using SandBox.Elements.Interface;
using UnityEngine;

namespace SandBox.Elements.Solid
{
    public struct Sand : IElement
    {
        public bool IsStatic => false;
        public float       Life           { get; set; }
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.yellow;
        public float       Density        => 10f;
        public ElementType Type           => ElementType.Solid;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }

        public void StatusUpdate(in Vector2Int globalIndex)
        {
        }
    }
}
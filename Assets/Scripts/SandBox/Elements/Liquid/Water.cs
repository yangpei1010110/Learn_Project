#nullable enable

using SandBox.Elements.Interface;
using UnityEngine;

namespace SandBox.Elements.Liquid
{
    public struct Water : IElement
    {
        public bool        IsStatic       => false;
        public float       Life           { get; set; }
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.cyan;
        public float       Density        => 1f;
        public ElementType Type           => ElementType.Liquid;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }
        public int StableStep { get; set; }

        public void StatusUpdate(in Vector2Int globalIndex)
        {
        }
    }
}
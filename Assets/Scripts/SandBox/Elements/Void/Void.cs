using SandBox.Map;
using UnityEngine;

namespace SandBox.Elements.Void
{
    public struct Void : IElement
    {
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.black;
        public float       Density        => -1f;
        public ElementType Type           => ElementType.Void;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }

        public void UpdateElement(ref IElement element, Vector2Int globalPosition)
        {
            if (Step == SparseSandBoxMap.Instance.Step)
            {
                return;
            }
            else
            {
                Step = SparseSandBoxMap.Instance.Step;
            }
        }
    }
}
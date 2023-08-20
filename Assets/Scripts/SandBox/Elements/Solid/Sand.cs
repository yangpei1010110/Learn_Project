using SandBox.Map;
using Tools;
using UnityEngine;

namespace SandBox.Elements.Solid
{
    public struct Sand : IElement
    {
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.yellow;
        public float       Density        => 10f;
        public ElementType Type           => ElementType.Solid;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }

        /// <summary>
        ///     can continue then return true
        /// </summary>
        public void UpdateElement(ref IElement element, Vector2Int globalPosition)
        {
            if (element.Step == SparseSandBoxMap.Instance.Step)
            {
                return;
            }
            else
            {
                element.Step = SparseSandBoxMap.Instance.Step;
            }

            Vector2Int downPosition = globalPosition + Vector2Int.down;
            if (!SandBoxMap.Instance.Exist(downPosition))
            {
                return;
            }

            SandBoxMap.Instance[downPosition] = SandBoxMap.Instance[downPosition];
            if (SandBoxMap.Instance[downPosition].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[downPosition];
                SandBoxTool.SwapPosition(ref swapElement, ref element);
                SandBoxMap.Instance[globalPosition] = swapElement;
                SandBoxMap.Instance[downPosition] = element;
                return;
            }

            Vector2Int downLeftPosition = globalPosition + Vector2Int.down + Vector2Int.left;
            if (!SandBoxMap.Instance.Exist(downLeftPosition))
            {
                return;
            }

            SandBoxMap.Instance[downLeftPosition] = SandBoxMap.Instance[downLeftPosition];
            if (SandBoxMap.Instance[downLeftPosition].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[downLeftPosition];
                SandBoxTool.SwapPosition(ref swapElement, ref element);
                SandBoxMap.Instance[globalPosition] = swapElement;
                SandBoxMap.Instance[downLeftPosition] = element;
                return;
            }

            Vector2Int downRightPosition = globalPosition + Vector2Int.down + Vector2Int.right;
            if (!SandBoxMap.Instance.Exist(downRightPosition))
            {
                return;
            }

            SandBoxMap.Instance[downRightPosition] = SandBoxMap.Instance[downRightPosition];
            if (SandBoxMap.Instance[downRightPosition].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[downRightPosition];
                SandBoxTool.SwapPosition(ref swapElement, ref element);
                SandBoxMap.Instance[globalPosition] = swapElement;
                SandBoxMap.Instance[downRightPosition] = element;
                return;
            }
        }
    }
}
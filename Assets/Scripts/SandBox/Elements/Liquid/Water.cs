#nullable enable

using SandBox.Elements.Interface;
using SandBox.Map;
using SandBox.Map.SandBox;
using Tools;
using UnityEngine;

namespace SandBox.Elements.Liquid
{
    public struct Water : IElement
    {
        public float       Life           { get; set; }
        public long        Step           { get; set; }
        public Vector2Int  Position       { get; set; }
        public Color       Color          => Color.cyan;
        public float       Density        => 1f;
        public ElementType Type           => ElementType.Liquid;
        public Vector2     Velocity       { get; set; }
        public Vector2     PositionOffset { get; set; }

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

            Vector2Int? canMoveGlobalPosition = GetCanMoveGlobalPosition(element, globalPosition);
            if (canMoveGlobalPosition != null)
            {
                IElement swapElement = SandBoxMap.Instance[canMoveGlobalPosition.Value];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, canMoveGlobalPosition.Value, globalPosition);
                SandBoxMap.Instance[globalPosition] = swapElement;
                SandBoxMap.Instance[canMoveGlobalPosition.Value] = element;
                return;
            }
        }

        private static Vector2Int? GetCanMoveGlobalPosition(IElement element, Vector2Int globalPosition)
        {
            // can down
            Vector2Int downGlobalPosition = globalPosition + Vector2Int.down;
            if (!SandBoxMap.Instance.Exist(downGlobalPosition))
            {
                return null;
            }

            SandBoxMap.Instance[downGlobalPosition] = SandBoxMap.Instance[downGlobalPosition];
            if (SandBoxMap.Instance[downGlobalPosition].Density < element.Density)
            {
                return downGlobalPosition;
            }

            // can down range 5 gird
            bool isLeft = Random.Range(0, 2) == 0;
            for (int i = 0; i < 5; i++)
            {
                Vector2Int newDownGlobalPosition = isLeft ? downGlobalPosition + Vector2Int.left * i : downGlobalPosition + Vector2Int.right * i;
                isLeft = !isLeft;

                if (!SandBoxMap.Instance.Exist(newDownGlobalPosition))
                {
                    continue;
                }

                SandBoxMap.Instance[newDownGlobalPosition] = SandBoxMap.Instance[newDownGlobalPosition];
                if (SandBoxMap.Instance[newDownGlobalPosition].Density < element.Density)
                {
                    return newDownGlobalPosition;
                }
            }

            // random left or right move
            int random = Random.Range(-2, 3);
            Vector2Int randomMoveGlobalPosition = globalPosition + Vector2Int.right * random;
            if (!SandBoxMap.Instance.Exist(randomMoveGlobalPosition))
            {
                return null;
            }

            SandBoxMap.Instance[randomMoveGlobalPosition] = SandBoxMap.Instance[randomMoveGlobalPosition];
            if (SandBoxMap.Instance[randomMoveGlobalPosition].Density < element.Density)
            {
                return randomMoveGlobalPosition;
            }

            return null;
        }
    }
}
#nullable enable

using System;
using SandBox.Elements.Interface;
using SandBox.Map;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SandBox.Elements
{
    public static class ElementSimulation
    {
        private static void UpdateVelocity(in MapBlock mapBlock, in Vector2Int localIndex, in int deltaTime)
        {
            mapBlock[localIndex].Velocity += MapSetting.GravityForce * deltaTime;
        }

        private static Vector2Int? DetectCollision(in MapBlock mapBlock, in Vector2Int localIndex, in int deltaTime)
        {
            throw new NotImplementedException();
        }

        private static void UpdatePosition(in MapBlock mapBlock, in Vector2Int localIndex, in int deltaTime)
        {
            var element = mapBlock[localIndex];
            var velocity = element.Velocity;
            var globalIndex = MapOffset.LocalToGlobal(mapBlock.BlockIndex, localIndex);
            var nextWorldPosition = globalIndex + velocity * deltaTime;
            var nextGlobalIndex = MapOffset.WorldToGlobal(nextWorldPosition);
            var offset = nextWorldPosition - nextGlobalIndex;
            element.PositionOffset = offset;
        }

        public static bool Run(in MapBlock mapBlock, ref IElement element)
        {
            switch (element.Type)
            {
                case ElementType.Solid:
                    return SolidSimulation(mapBlock, ref element);
                case ElementType.Liquid:
                    return LiquidSimulation(mapBlock, ref element);
                case ElementType.Gas:
                    return GasSimulation(mapBlock, ref element);
                default:
                    return false;
            }
        }

        public static bool SolidSimulation(in MapBlock mapBlock, ref IElement element)
        {
            if (element.Step == SparseSandBoxMap.Instance.Step)
            {
                return false;
            }
            else
            {
                element.Step = SparseSandBoxMap.Instance.Step;
            }

            Vector2Int elementGlobal = MapOffset.LocalToGlobal(mapBlock.BlockIndex, element.Position);
            Vector2Int downGlobal = elementGlobal + Vector2Int.down;
            Vector2Int downLeftGlobal = elementGlobal + Vector2Int.down + Vector2Int.left;
            Vector2Int downRightGlobal = elementGlobal + Vector2Int.down + Vector2Int.right;

            if (!SandBoxMap.Instance.Exist(downGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[downGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[downGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, downGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[downGlobal] = element;
                return true;
            }

            bool isLeft = Random.Range(0, 2) == 0;
            Vector2Int newDownGlobal = isLeft ? downLeftGlobal : downRightGlobal;
            if (!SandBoxMap.Instance.Exist(newDownGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newDownGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newDownGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newDownGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newDownGlobal] = element;
                return true;
            }

            newDownGlobal = !isLeft ? downLeftGlobal : downRightGlobal;
            if (!SandBoxMap.Instance.Exist(newDownGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newDownGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newDownGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newDownGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newDownGlobal] = element;
                return true;
            }

            return false;
        }

        public static bool LiquidSimulation(in MapBlock mapBlock, ref IElement element)
        {
            if (element.Step == SparseSandBoxMap.Instance.Step)
            {
                return false;
            }
            else
            {
                element.Step = SparseSandBoxMap.Instance.Step;
            }

            Vector2Int elementGlobal = MapOffset.LocalToGlobal(mapBlock.BlockIndex, element.Position);
            Vector2Int downGlobal = elementGlobal + Vector2Int.down;
            Vector2Int downLeftGlobal = elementGlobal + Vector2Int.down + Vector2Int.left;
            Vector2Int downRightGlobal = elementGlobal + Vector2Int.down + Vector2Int.right;
            Vector2Int leftGlobal = elementGlobal + Vector2Int.left;
            Vector2Int rightGlobal = elementGlobal + Vector2Int.right;

            if (!SandBoxMap.Instance.Exist(downGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[downGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[downGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, downGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[downGlobal] = element;
                return true;
            }

            bool isLeft = Random.Range(0, 2) == 0;
            Vector2Int newDownGlobal = isLeft ? downLeftGlobal : downRightGlobal;
            if (!SandBoxMap.Instance.Exist(newDownGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newDownGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newDownGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newDownGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newDownGlobal] = element;
                return true;
            }

            newDownGlobal = !isLeft ? downLeftGlobal : downRightGlobal;
            if (!SandBoxMap.Instance.Exist(newDownGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newDownGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newDownGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newDownGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newDownGlobal] = element;
                return true;
            }

            Vector2Int newGlobal = isLeft ? leftGlobal : rightGlobal;
            if (!SandBoxMap.Instance.Exist(newGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newGlobal] = element;
                return true;
            }

            newGlobal = !isLeft ? leftGlobal : rightGlobal;
            if (!SandBoxMap.Instance.Exist(newGlobal))
            {
                return false;
            }
            else if (SandBoxMap.Instance[newGlobal].Density < element.Density)
            {
                IElement swapElement = SandBoxMap.Instance[newGlobal];
                SandBoxTool.SwapGlobalIndex(ref swapElement, ref element, newGlobal, elementGlobal);
                SandBoxMap.Instance[elementGlobal] = swapElement;
                SandBoxMap.Instance[newGlobal] = element;
                return true;
            }

            return false;
        }

        public static bool GasSimulation(in MapBlock mapBlock, ref IElement element) => false;
    }
}
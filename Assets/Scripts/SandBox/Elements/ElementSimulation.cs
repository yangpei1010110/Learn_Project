#nullable enable

using System;
using SandBox.Elements.Interface;
using SandBox.Map;
using SandBox.Map.SandBox;
using SandBox.Physics;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SandBox.Elements
{
    // TODO 将粒子更新修改为不需要储存变量地址的方式
    public static class ElementSimulation
    {
        private static void UpdateVelocity(in Vector2Int globalIndex, in int deltaTime)
        {
            SparseSandBoxMap2<IElement>.Instance[globalIndex].Velocity += MapSetting.GravityForce * deltaTime;
        }

        private static Vector2Int? DetectCollision(Vector2Int globalIndex, in int deltaTime)
        {
            var element = SparseSandBoxMap2<IElement>.Instance[globalIndex];
            Vector2 nextWorldPosition = globalIndex + element.PositionOffset + element.Velocity * deltaTime;
            Vector2Int nextGlobalIndex = MapOffset.WorldToGlobal(nextWorldPosition);
            Vector2 offset = nextWorldPosition - nextGlobalIndex;
            element.PositionOffset = offset;

            Vector2Int? elementGlobalIndex = globalIndex;
            LineTool2D.LineCast(globalIndex, nextGlobalIndex, tempGlobalIndex =>
            {
                var existElement = SparseSandBoxMap2<IElement>.Instance.Exist(tempGlobalIndex);
                if (!existElement)
                {
                    // collision edge
                    var collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, tempGlobalIndex);
                    return false;
                }

                var tempElement = SparseSandBoxMap2<IElement>.Instance[tempGlobalIndex];
                if (tempElement.Type != ElementType.Void)
                {
                    var collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, tempGlobalIndex);
                    return false;
                }

                elementGlobalIndex = tempGlobalIndex;
                return true;
            });
        }

        private static void GetNextPosition(in MapBlock2<IElement> mapBlock, in Vector2Int localIndex, in int deltaTime)
        {
            IElement element = mapBlock[localIndex];
            Vector2 velocity = element.Velocity;
            Vector2Int globalIndex = MapOffset.LocalToGlobal(mapBlock.BlockIndex, localIndex);
            Vector2 nextWorldPosition = globalIndex + velocity * deltaTime;
            Vector2Int nextGlobalIndex = MapOffset.WorldToGlobal(nextWorldPosition);
            Vector2 offset = nextWorldPosition - nextGlobalIndex;
            element.PositionOffset = offset;
        }

        public static bool Run(in MapBlock2<IElement> mapBlock, ref IElement element)
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

        public static bool SolidSimulation(in MapBlock2<IElement> mapBlock, ref IElement element)
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

        public static bool LiquidSimulation(in MapBlock2<IElement> mapBlock, ref IElement element)
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

        public static bool GasSimulation(in MapBlock2<IElement> mapBlock, ref IElement element) => false;
    }
}
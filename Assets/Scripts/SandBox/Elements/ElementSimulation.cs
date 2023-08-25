#nullable enable

using SandBox.Elements.Interface;
using SandBox.Map;
using Tools;
using UnityEngine;

namespace SandBox.Elements
{
    public static class ElementSimulation
    {
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

            Vector2Int elementGlobal = MapOffset.LocalToGlobal(mapBlock.BlockIndex, element.Position, MapSetting.Instance.MapLocalSizePerUnit);
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

            Vector2Int elementGlobal = MapOffset.LocalToGlobal(mapBlock.BlockIndex, element.Position, MapSetting.Instance.MapLocalSizePerUnit);
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
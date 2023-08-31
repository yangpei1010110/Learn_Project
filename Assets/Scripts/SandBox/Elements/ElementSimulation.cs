#nullable enable

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
        private static void UpdateVelocity(in Vector2Int globalIndex, in float deltaTime)
        {
            var element = SparseSandBoxMap2.Instance[globalIndex];
            if (element.Type != ElementType.Void)
            {
                element.Velocity += MapSetting.GravityForce * deltaTime;
                SparseSandBoxMap2.Instance[globalIndex] = element;
            }
        }

        private static void UpdateByCollision(Vector2Int globalIndex, in float deltaTime)
        {
            var element = SparseSandBoxMap2.Instance[globalIndex];
            Vector2 nextWorldPosition = globalIndex + element.PositionOffset + element.Velocity * deltaTime;
            Vector2Int nextGlobalIndex = MapOffset.GlobalRound(nextWorldPosition);
            Vector2 offset = nextWorldPosition - nextGlobalIndex;
            element.PositionOffset = offset;
            // if (element.Type == ElementType.Solid && globalIndex != nextGlobalIndex)
            // {
            //     Debug.Log($"break point");
            // }

            Vector2Int? elementGlobalIndex = globalIndex;
            // TODO 这里待完善 此处出现碰撞后立刻结束 但是应继续运行
            if (globalIndex != nextGlobalIndex)
            {
                var path = LineTool2D.LineCast(globalIndex, nextGlobalIndex);
                foreach (Vector2Int stepGlobalIndex in path)
                {
                    var collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, stepGlobalIndex);
                    if (collisionData.Type != ElementPhysics.CollisionType.Empty
                     && collisionData.Type != ElementPhysics.CollisionType.Block)
                    {
                        // is move
                        SandBoxTool.MoveTo(elementGlobalIndex.Value, collisionData.NextGlobalIndex);
                        elementGlobalIndex = collisionData.NextGlobalIndex;

                        // if (collisionData.Type == ElementPhysics.CollisionType.Swap)
                        // {
                        //     SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                        // }
                        // else if (collisionData.Type == ElementPhysics.CollisionType.Slip)
                        // {
                        //     SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                        // }
                    }
                    else if (collisionData.Type == ElementPhysics.CollisionType.Block)
                    {
                        SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity = Vector2.zero;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                // LineTool2D.DrawLine(globalIndex, nextGlobalIndex, stepGlobalIndex =>
                // {
                //     // Debug.Log($"Step:{SparseSandBoxMap2.Instance.Step}, " +
                //     //           $"globalIndex:{globalIndex}, " +
                //     //           $"nextGlobalIndex:{nextGlobalIndex}, " +
                //     //           $"elementGlobalIndex:{elementGlobalIndex}, " +
                //     //           $"stepGlobalIndex:{stepGlobalIndex}");
                //     var collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, stepGlobalIndex);
                //     // if (globalIndex != stepGlobalIndex)
                //     // {
                //     //     Debug.Log($"Step:{SparseSandBoxMap2.Instance.Step}");
                //     //     Debug.Log($"Collision: globalIndex{globalIndex}, nextGlobalIndex{nextGlobalIndex}");
                //     //     Debug.Log($"Collision: elementGlobalIndex{elementGlobalIndex}, stepGlobalIndex{stepGlobalIndex}");
                //     //     Debug.Log($"CollisionData: Type:{collisionData.Type}");
                //     // }
                //
                //     if (collisionData.Type != ElementPhysics.CollisionType.Empty
                //      && collisionData.Type != ElementPhysics.CollisionType.Block)
                //     {
                //         // is move
                //         SandBoxTool.MoveTo(elementGlobalIndex.Value, stepGlobalIndex);
                //         elementGlobalIndex = stepGlobalIndex;
                //
                //         if (collisionData.Type == ElementPhysics.CollisionType.Swap)
                //         {
                //             SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                //             return true;
                //         }
                //         else if (collisionData.Type == ElementPhysics.CollisionType.Block)
                //         {
                //             SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity = Vector2.zero;
                //             return false;
                //         }
                //         else if (collisionData.Type == ElementPhysics.CollisionType.Slip)
                //         {
                //             SparseSandBoxMap2.Instance[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping * 0.5f;
                //             return true;
                //         }
                //     }
                //
                //     return false;
                // });
            }

            if (!elementGlobalIndex.HasValue)
            {
                return;
            }

            // dirty check
            var nextElement = SparseSandBoxMap2.Instance[elementGlobalIndex.Value];
            nextElement.Velocity *= ElementPhysicsSetting.VelocityDamping;
            if (nextElement.Velocity.sqrMagnitude > 0.1f)
            {
                SparseSandBoxMap2.Instance.SetDirty(elementGlobalIndex.Value);
            }
        }

        public static bool Run(in Vector2Int globalIndex, in float deltaTime)
        {
            // if (Time.frameCount % 100 == 0)
            // {
            //     Debug.Log(@"ElementSimulation.Run");
            // }

            UpdateVelocity(globalIndex, deltaTime);
            UpdateByCollision(globalIndex, deltaTime);
            return true;

            // switch (element.Type)
            // {
            //     case ElementType.Solid:
            //         return SolidSimulation(mapBlock, ref element);
            //     case ElementType.Liquid:
            //         return LiquidSimulation(mapBlock, ref element);
            //     case ElementType.Gas:
            //         return GasSimulation(mapBlock, ref element);
            //     default:
            //         return false;
            // }
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
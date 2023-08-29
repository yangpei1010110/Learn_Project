using System;
using SandBox.Elements.Interface;
using SandBox.Map.SandBox;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SandBox.Physics
{
    public static class ElementPhysics
    {
        public struct CollisionInfo
        {
            public bool          IsCollision;
            public CollisionType Type;
            public Vector2Int    NextGlobalIndex;
        }

        public enum CollisionType
        {
            Empty,
            Swap,
            Slip,
            Block,
        }

        private static CollisionInfo SimpleElementCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        {
            CollisionInfo result = default(CollisionInfo);
            if (elementGlobalIndex == blockGlobalIndex)
            {
                return default(CollisionInfo);
            }

            Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
            if (math.abs(normal.x) != 1 || math.abs(normal.y) != 1)
            {
                // elements not near
                return default(CollisionInfo);
            }

            bool isCanCollision = SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex)
                               && SparseSandBoxMap2<IElement>.Instance.Exist(blockGlobalIndex);

            if (!isCanCollision)
            {
                return default(CollisionInfo);
            }
            else
            {
                // can block
                result.IsCollision = true;
                result.Type = CollisionType.Block;
            }

            var element = SparseSandBoxMap2<IElement>.Instance[elementGlobalIndex];
            var block = SparseSandBoxMap2<IElement>.Instance[blockGlobalIndex];

            // swap test
            if (element.Density < block.Density)
            {
                result.Type = CollisionType.Swap;
                result.NextGlobalIndex = blockGlobalIndex;
                return result;
            }

            // slip test
            if (math.abs(normal.x) + math.abs(normal.y) == 2)
            {
                // is diagonal
                // test x move
                var xMoveGlobalIndex = elementGlobalIndex + new Vector2Int(normal.x, 0);
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }

                // test y move
                var yMoveGlobalIndex = elementGlobalIndex + new Vector2Int(0, normal.y);
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }
            }
            else if (math.abs(normal.x) == 1)
            {
                // test y move
                bool isPositive = Random.Range(0, 2) == 0;
                var offset = isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
                var yMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }

                offset = !isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
                yMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }
            }
            else if (math.abs(normal.y) == 1)
            {
                // test x move
                bool isPositive = Random.Range(0, 2) == 0;
                var offset = isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
                var xMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }

                offset = !isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
                xMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }
            }
        }

        /// <summary>
        ///  元素与边缘发生碰撞
        /// </summary>
        private static CollisionInfo SimpleWallCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        {
            CollisionInfo result = default(CollisionInfo);
            if (elementGlobalIndex == blockGlobalIndex)
            {
                return default(CollisionInfo);
            }

            Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
            if (math.abs(normal.x) != 1 || math.abs(normal.y) != 1)
            {
                // elements not near
                return default(CollisionInfo);
            }

            bool isCanCollision = SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex)
                               && !SparseSandBoxMap2<IElement>.Instance.Exist(blockGlobalIndex);

            if (!isCanCollision)
            {
                return default(CollisionInfo);
            }
            else
            {
                // can block
                result.IsCollision = true;
                result.Type = CollisionType.Block;
            }

            var element = SparseSandBoxMap2<IElement>.Instance[elementGlobalIndex];
            if (math.abs(normal.x) + math.abs(normal.y) == 2)
            {
                // is diagonal
                // test x move
                var xMoveGlobalIndex = elementGlobalIndex + new Vector2Int(normal.x, 0);
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }

                // test y move
                var yMoveGlobalIndex = elementGlobalIndex + new Vector2Int(0, normal.y);
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }
            }
            else if (math.abs(normal.x) == 1)
            {
                // test y move
                bool isPositive = Random.Range(0, 2) == 0;
                var offset = isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
                var yMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }

                offset = !isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
                yMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
                {
                    var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
                    if (yMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = yMoveGlobalIndex;
                        return result;
                    }
                }
            }
            else if (math.abs(normal.y) == 1)
            {
                // test x move
                bool isPositive = Random.Range(0, 2) == 0;
                var offset = isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
                var xMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }

                offset = !isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
                xMoveGlobalIndex = elementGlobalIndex + offset;
                if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
                {
                    var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
                    if (xMoveElement.Density <= element.Density)
                    {
                        result.Type = CollisionType.Slip;
                        result.NextGlobalIndex = xMoveGlobalIndex;
                        return result;
                    }
                }
            }

            throw new Exception("impossible collision");
        }

        /// <summary>
        /// 仅有简单的穿过与碰撞检测
        /// </summary>
        public static CollisionInfo SimpleCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        {
            CollisionInfo result = default(CollisionInfo);
            if (elementGlobalIndex == blockGlobalIndex)
            {
                return result;
            }

            Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
            if (math.abs(normal.x) != 1 || math.abs(normal.y) != 1)
            {
                // elements not near
                return result;
            }

            if (!SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex))
            {
                // element not exist
                return result;
            }

            if (!SparseSandBoxMap2<IElement>.Instance.Exist(blockGlobalIndex))
            {
                // is wall collision
                return SimpleWallCollision(elementGlobalIndex, blockGlobalIndex);
            }
            else
            {
                return SimpleElementCollision(elementGlobalIndex, blockGlobalIndex);
            }
        }

        // private static void WallDetectCollision(in Vector2Int elementGlobalIndex, in Vector2Int wallGlobalIndex)
        // {
        //     if (elementGlobalIndex == wallGlobalIndex)
        //     {
        //         return;
        //     }
        //
        //
        //     Vector2Int e1e2Normal = elementGlobalIndex - wallGlobalIndex;
        //     if (math.abs(e1e2Normal.x) != 1 || math.abs(e1e2Normal.y) != 1)
        //     {
        //         // elements not near
        //         return;
        //     }
        //
        //     if (!SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex)
        //      || SparseSandBoxMap2<IElement>.Instance.Exist(wallGlobalIndex))
        //     {
        //         // element not exist or wall exist
        //         return;
        //     }
        //
        //     IElement element1 = SparseSandBoxMap2<IElement>.Instance[elementGlobalIndex];
        //     if (element1.Type == ElementType.Void)
        //     {
        //         // not collision
        //         return;
        //     }
        //
        //     Vector2 v1 = element1.Velocity;
        //
        //     // compute new velocity direction
        //     if (e1e2Normal.y >= 1f)
        //     {
        //         // e1 up e2 down
        //         if (v1.y < 0f)
        //         {
        //             v1.y = -v1.y;
        //         }
        //     }
        //     else if (e1e2Normal.y <= -1f)
        //     {
        //         // e1 down e2 up
        //         if (v1.y > 0f)
        //         {
        //             v1.y = -v1.y;
        //         }
        //     }
        //
        //     if (e1e2Normal.x >= 1f)
        //     {
        //         // e1 left e2 right
        //         if (v1.x > 0f)
        //         {
        //             v1.x = -v1.x;
        //         }
        //     }
        //     else if (e1e2Normal.x <= -1f)
        //     {
        //         // e1 right e2 left
        //         if (v1.x < 0f)
        //         {
        //             v1.x = -v1.x;
        //         }
        //     }
        //
        //     element1.Velocity = v1;
        // }
        //
        // /// <summary>
        // ///     简单粒子碰撞检测
        // /// </summary>
        // public static void DetectCollision(in Vector2Int element1GlobalIndex, in Vector2Int element2GlobalIndex)
        // {
        //     if (element1GlobalIndex == element2GlobalIndex)
        //     {
        //         return;
        //     }
        //
        //     Vector2Int e1e2Normal = element2GlobalIndex - element1GlobalIndex;
        //     if (math.abs(e1e2Normal.x) != 1 || math.abs(e1e2Normal.y) != 1)
        //     {
        //         // elements not near
        //         return;
        //     }
        //
        //     if (SparseSandBoxMap2<IElement>.Instance.Exist(element1GlobalIndex)
        //      || !SparseSandBoxMap2<IElement>.Instance.Exist(element2GlobalIndex))
        //     {
        //         // is wall collision
        //         WallDetectCollision(element1GlobalIndex, element2GlobalIndex);
        //         return;
        //     }
        //     else if (!SparseSandBoxMap2<IElement>.Instance.Exist(element1GlobalIndex)
        //           || !SparseSandBoxMap2<IElement>.Instance.Exist(element2GlobalIndex))
        //     {
        //         // element not exist
        //         return;
        //     }
        //
        //     IElement element1 = SparseSandBoxMap2<IElement>.Instance[element1GlobalIndex];
        //     IElement element2 = SparseSandBoxMap2<IElement>.Instance[element2GlobalIndex];
        //
        //     if (element1.Type == ElementType.Void || element2.Type == ElementType.Void)
        //     {
        //         // not collision
        //         return;
        //     }
        //
        //     float m1 = element1.Density;
        //     float m2 = element2.Density;
        //
        //     if (m1 <= 0f || m2 <= 0f)
        //     {
        //         return;
        //     }
        //
        //     Vector2 v1 = element1.Velocity;
        //     Vector2 v2 = element2.Velocity;
        //
        //     float energy1 = m1 * v1.sqrMagnitude;
        //     float energy2 = m2 * v2.sqrMagnitude;
        //
        //     float totalEnergy = energy1 + energy2;
        //     float halfTotalEnergy = totalEnergy / 2f;
        //
        //     Vector2 newV1 = v1.normalized * (halfTotalEnergy / m1);
        //     Vector2 newV2 = v2.normalized * (halfTotalEnergy / m2);
        //
        //     // compute new velocity direction
        //     if (e1e2Normal.y >= 1f)
        //     {
        //         // e1 up e2 down
        //         if (newV1.y < 0f)
        //         {
        //             newV1.y = -newV1.y;
        //         }
        //
        //         if (newV2.y > 0f)
        //         {
        //             newV2.y = -newV2.y;
        //         }
        //     }
        //     else if (e1e2Normal.y <= -1f)
        //     {
        //         // e1 down e2 up
        //         if (newV1.y > 0f)
        //         {
        //             newV1.y = -newV1.y;
        //         }
        //
        //         if (newV2.y < 0f)
        //         {
        //             newV2.y = -newV2.y;
        //         }
        //     }
        //
        //     if (e1e2Normal.x >= 1f)
        //     {
        //         // e1 left e2 right
        //         if (newV1.x > 0f)
        //         {
        //             newV1.x = -newV1.x;
        //         }
        //
        //         if (newV2.x < 0f)
        //         {
        //             newV2.x = -newV2.x;
        //         }
        //     }
        //     else if (e1e2Normal.x <= -1f)
        //     {
        //         // e1 right e2 left
        //         if (newV1.x < 0f)
        //         {
        //             newV1.x = -newV1.x;
        //         }
        //
        //         if (newV2.x > 0f)
        //         {
        //             newV2.x = -newV2.x;
        //         }
        //     }
        //
        //     element1.Velocity = newV1;
        //     element2.Velocity = newV2;
        // }
    }
}
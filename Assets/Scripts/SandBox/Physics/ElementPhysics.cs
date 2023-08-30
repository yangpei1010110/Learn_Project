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
        private struct CollisionRegularData
        {
            public Vector2Int ElementGlobalIndex;
            public Vector2Int ElementLeftGlobalIndex;
            public Vector2Int ElementRightGlobalIndex;
            public Vector2Int BlockGlobalIndex;
            public Vector2Int BlockLeftGlobalIndex;
            public Vector2Int BlockRightGlobalIndex;
        }

        private struct CollisionDiagonalData
        {
            public Vector2Int ElementGlobalIndex;
            public Vector2Int LeftGlobalIndex;
            public Vector2Int RightGlobalIndex;
            public Vector2Int BlockGlobalIndex;
        }

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


        /// <summary>
        /// 碰撞处在 y 平面或 x 平面
        /// </summary>
        private static CollisionInfo SimpleRegularCollision(in CollisionRegularData collisionData)
        {
            var existElement = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.ElementGlobalIndex);
            if (!existElement)
            {
                return default(CollisionInfo);
            }

            var existBlock = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.BlockGlobalIndex);
            if (existBlock)
            {
                // test swap
                var element = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementGlobalIndex];
                var block = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockGlobalIndex];

                if (element.Type == ElementType.Void)
                {
                    return default(CollisionInfo);
                }

                if (block.Density < element.Density)
                {
                    return new CollisionInfo()
                    {
                        IsCollision = true,
                        Type = CollisionType.Swap,
                        NextGlobalIndex = collisionData.BlockGlobalIndex,
                    };
                }


                // test block left or right
                var existBlockLeft = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.BlockLeftGlobalIndex);
                var existBlockRight = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.BlockRightGlobalIndex);
                bool isBlockLeft = Random.Range(0, 2) == 0;
                if (isBlockLeft)
                {
                    if (existBlockLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockLeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.BlockLeftGlobalIndex,
                            };
                        }
                    }

                    if (existBlockRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockRightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.BlockRightGlobalIndex,
                            };
                        }
                    }
                }
                else
                {
                    if (existBlockRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockRightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.BlockRightGlobalIndex,
                            };
                        }
                    }

                    if (existBlockLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockLeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.BlockLeftGlobalIndex,
                            };
                        }
                    }
                }

                // test element left or right
                var existElementLeft = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.ElementLeftGlobalIndex);
                var existElementRight = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.ElementRightGlobalIndex);
                bool isElementLeft = Random.Range(0, 2) == 0;
                if (isElementLeft)
                {
                    if (existElementLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementLeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.ElementLeftGlobalIndex,
                            };
                        }
                    }

                    if (existElementRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementRightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.ElementRightGlobalIndex,
                            };
                        }
                    }
                }
                else
                {
                    if (existElementRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementRightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.ElementRightGlobalIndex,
                            };
                        }
                    }

                    if (existElementLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementLeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.ElementLeftGlobalIndex,
                            };
                        }
                    }
                }
            }

            return default(CollisionInfo);
        }

        /// <summary>
        /// 碰撞处在对角线
        /// </summary>
        private static CollisionInfo SimpleDiagonalCollision(in CollisionDiagonalData collisionData)
        {
            var existElement = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.ElementGlobalIndex);
            if (!existElement)
            {
                return default(CollisionInfo);
            }

            var existBlock = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.BlockGlobalIndex);
            if (existBlock)
            {
                // test swap
                var element = SparseSandBoxMap2<IElement>.Instance[collisionData.ElementGlobalIndex];
                var block = SparseSandBoxMap2<IElement>.Instance[collisionData.BlockGlobalIndex];

                if (element.Type == ElementType.Void)
                {
                    return default(CollisionInfo);
                }

                if (block.Density < element.Density)
                {
                    return new CollisionInfo()
                    {
                        IsCollision = true,
                        Type = CollisionType.Swap,
                        NextGlobalIndex = collisionData.BlockGlobalIndex,
                    };
                }

                // test left or right
                var existLeft = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.LeftGlobalIndex);
                var existRight = SparseSandBoxMap2<IElement>.Instance.Exist(collisionData.RightGlobalIndex);
                bool isLeft = Random.Range(0, 2) == 0;
                if (isLeft)
                {
                    if (existLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.LeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.LeftGlobalIndex,
                            };
                        }
                    }

                    if (existRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.RightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.RightGlobalIndex,
                            };
                        }
                    }
                }
                else
                {
                    if (existRight)
                    {
                        var right = SparseSandBoxMap2<IElement>.Instance[collisionData.RightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.RightGlobalIndex,
                            };
                        }
                    }

                    if (existLeft)
                    {
                        // test swap
                        var left = SparseSandBoxMap2<IElement>.Instance[collisionData.LeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Swap,
                                NextGlobalIndex = collisionData.LeftGlobalIndex,
                            };
                        }
                    }
                }
            }

            return default(CollisionInfo);
        }

        // private static CollisionInfo SimpleElementCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        // {
        //     CollisionInfo result = default(CollisionInfo);
        //     if (elementGlobalIndex == blockGlobalIndex)
        //     {
        //         return default(CollisionInfo);
        //     }
        //
        //     Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
        //     if (math.abs(normal.x) != 1 || math.abs(normal.y) != 1)
        //     {
        //         // elements not near
        //         return default(CollisionInfo);
        //     }
        //
        //     bool isCanCollision = SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex)
        //                        && SparseSandBoxMap2<IElement>.Instance.Exist(blockGlobalIndex);
        //
        //     if (!isCanCollision)
        //     {
        //         return default(CollisionInfo);
        //     }
        //     else
        //     {
        //         // can block
        //         result.IsCollision = true;
        //         result.Type = CollisionType.Block;
        //     }
        //
        //     var element = SparseSandBoxMap2<IElement>.Instance[elementGlobalIndex];
        //     var block = SparseSandBoxMap2<IElement>.Instance[blockGlobalIndex];
        //
        //     // swap test
        //     if (element.Density < block.Density)
        //     {
        //         result.Type = CollisionType.Swap;
        //         result.NextGlobalIndex = blockGlobalIndex;
        //         return result;
        //     }
        //
        //     // slip test
        //     if (math.abs(normal.x) + math.abs(normal.y) == 2)
        //     {
        //         // is diagonal
        //         // test x move
        //         var xMoveGlobalIndex = elementGlobalIndex + new Vector2Int(normal.x, 0);
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         // test y move
        //         var yMoveGlobalIndex = elementGlobalIndex + new Vector2Int(0, normal.y);
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        //     else if (math.abs(normal.x) == 1)
        //     {
        //         // test y move
        //         bool isPositive = Random.Range(0, 2) == 0;
        //         var offset = isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
        //         var yMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         offset = !isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
        //         yMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        //     else if (math.abs(normal.y) == 1)
        //     {
        //         // test x move
        //         bool isPositive = Random.Range(0, 2) == 0;
        //         var offset = isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
        //         var xMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         offset = !isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
        //         xMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        // }

        // /// <summary>
        // ///  元素与边缘发生碰撞
        // /// </summary>
        // private static CollisionInfo SimpleWallCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        // {
        //     CollisionInfo result = default(CollisionInfo);
        //     if (elementGlobalIndex == blockGlobalIndex)
        //     {
        //         return default(CollisionInfo);
        //     }
        //
        //     Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
        //     if (math.abs(normal.x) != 1 || math.abs(normal.y) != 1)
        //     {
        //         // elements not near
        //         return default(CollisionInfo);
        //     }
        //
        //     bool isCanCollision = SparseSandBoxMap2<IElement>.Instance.Exist(elementGlobalIndex)
        //                        && !SparseSandBoxMap2<IElement>.Instance.Exist(blockGlobalIndex);
        //
        //     if (!isCanCollision)
        //     {
        //         return default(CollisionInfo);
        //     }
        //     else
        //     {
        //         // can block
        //         result.IsCollision = true;
        //         result.Type = CollisionType.Block;
        //     }
        //
        //     var element = SparseSandBoxMap2<IElement>.Instance[elementGlobalIndex];
        //     if (math.abs(normal.x) + math.abs(normal.y) == 2)
        //     {
        //         // is diagonal
        //         // test x move
        //         var xMoveGlobalIndex = elementGlobalIndex + new Vector2Int(normal.x, 0);
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         // test y move
        //         var yMoveGlobalIndex = elementGlobalIndex + new Vector2Int(0, normal.y);
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        //     else if (math.abs(normal.x) == 1)
        //     {
        //         // test y move
        //         bool isPositive = Random.Range(0, 2) == 0;
        //         var offset = isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
        //         var yMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         offset = !isPositive ? new Vector2Int(normal.x, 1) : new Vector2Int(normal.x, -1);
        //         yMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(yMoveGlobalIndex))
        //         {
        //             var yMoveElement = SparseSandBoxMap2<IElement>.Instance[yMoveGlobalIndex];
        //             if (yMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = yMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        //     else if (math.abs(normal.y) == 1)
        //     {
        //         // test x move
        //         bool isPositive = Random.Range(0, 2) == 0;
        //         var offset = isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
        //         var xMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //
        //         offset = !isPositive ? new Vector2Int(1, normal.y) : new Vector2Int(-1, normal.y);
        //         xMoveGlobalIndex = elementGlobalIndex + offset;
        //         if (SparseSandBoxMap2<IElement>.Instance.Exist(xMoveGlobalIndex))
        //         {
        //             var xMoveElement = SparseSandBoxMap2<IElement>.Instance[xMoveGlobalIndex];
        //             if (xMoveElement.Density <= element.Density)
        //             {
        //                 result.Type = CollisionType.Slip;
        //                 result.NextGlobalIndex = xMoveGlobalIndex;
        //                 return result;
        //             }
        //         }
        //     }
        //
        //     throw new Exception("impossible collision");
        // }

        /// <summary>
        /// 仅有简单的穿过与碰撞检测
        /// </summary>
        public static CollisionInfo SimpleCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        {
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

            if (math.abs(normal.x) == 1 && math.abs(normal.y) == 1)
            {
                // Diagonal Collision
                CollisionDiagonalData collisionData = default(CollisionDiagonalData);
                collisionData.ElementGlobalIndex = elementGlobalIndex;
                collisionData.BlockGlobalIndex = blockGlobalIndex;

                if (normal.x == 1 && normal.y == 1)
                {
                    collisionData.LeftGlobalIndex = elementGlobalIndex + Vector2Int.down;
                    collisionData.RightGlobalIndex = elementGlobalIndex + Vector2Int.left;
                }
                else if (normal.x == 1 && normal.y == -1)
                {
                    collisionData.LeftGlobalIndex = elementGlobalIndex + Vector2Int.left;
                    collisionData.RightGlobalIndex = elementGlobalIndex + Vector2Int.up;
                }
                else if (normal.x == -1 && normal.y == -1)
                {
                    collisionData.LeftGlobalIndex = elementGlobalIndex + Vector2Int.up;
                    collisionData.RightGlobalIndex = elementGlobalIndex + Vector2Int.right;
                }
                else if (normal.x == -1 && normal.y == 1)
                {
                    collisionData.LeftGlobalIndex = elementGlobalIndex + Vector2Int.right;
                    collisionData.RightGlobalIndex = elementGlobalIndex + Vector2Int.down;
                }
                else
                {
                    throw new Exception("Error Diagonal Collision Data");
                }

                return SimpleDiagonalCollision(collisionData);
            }
            else
            {
                // Regular Collision
                CollisionRegularData collisionData = default(CollisionRegularData);
                collisionData.ElementGlobalIndex = elementGlobalIndex;
                collisionData.BlockGlobalIndex = blockGlobalIndex;

                // TODO
                if (math.abs(normal.x) == 1)
                {
                    // x collision
                }
                else if (math.abs(normal.y) == 1)
                {
                    // y collision
                }
                else
                {
                    throw new Exception("Error Regular Collision Data");
                }

                return SimpleRegularCollision(collisionData);
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
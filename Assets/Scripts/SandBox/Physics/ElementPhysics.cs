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
        public enum CollisionType
        {
            Empty = 0,
            Swap  = 1 << 0,
            Slip  = 1 << 1,
            Block = 1 << 2,
        }

        private static SparseSandBoxMap _cacheSparseSandBoxMap = SparseSandBoxMap.Instance;

        /// <summary>
        ///     碰撞处在 y 平面或 x 平面
        /// </summary>
        private static CollisionInfo SimpleRegularCollision(in CollisionRegularData collisionData)
        {
            bool existElement = _cacheSparseSandBoxMap.Exist(collisionData.ElementGlobalIndex);
            if (!existElement)
            {
                return default(CollisionInfo);
            }

            bool existBlock = _cacheSparseSandBoxMap.Exist(collisionData.BlockGlobalIndex);
            if (!existBlock)
            {
                return default(CollisionInfo);
            }

            // test swap
            IElement element = _cacheSparseSandBoxMap[collisionData.ElementGlobalIndex];
            IElement block = _cacheSparseSandBoxMap[collisionData.BlockGlobalIndex];

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
            bool existBlockLeft = _cacheSparseSandBoxMap.Exist(collisionData.BlockLeftGlobalIndex);
            bool existBlockRight = _cacheSparseSandBoxMap.Exist(collisionData.BlockRightGlobalIndex);
            bool isBlockLeft = Random.Range(0, 2) == 0;
            if (isBlockLeft)
            {
                if (existBlockLeft)
                {
                    // test swap
                    IElement left = _cacheSparseSandBoxMap[collisionData.BlockLeftGlobalIndex];
                    if (left.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.BlockLeftGlobalIndex,
                        };
                    }
                }

                if (existBlockRight)
                {
                    IElement right = _cacheSparseSandBoxMap[collisionData.BlockRightGlobalIndex];
                    if (right.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.BlockRightGlobalIndex,
                        };
                    }
                }
            }
            else
            {
                if (existBlockRight)
                {
                    IElement right = _cacheSparseSandBoxMap[collisionData.BlockRightGlobalIndex];
                    if (right.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.BlockRightGlobalIndex,
                        };
                    }
                }

                if (existBlockLeft)
                {
                    // test swap
                    IElement left = _cacheSparseSandBoxMap[collisionData.BlockLeftGlobalIndex];
                    if (left.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.BlockLeftGlobalIndex,
                        };
                    }
                }
            }

            if (element.Type == ElementType.Solid)
            {
                return new CollisionInfo()
                {
                    IsCollision = true,
                    Type = CollisionType.Block,
                    NextGlobalIndex = collisionData.ElementGlobalIndex,
                };
            }

            // test element left or right
            bool existElementLeft = _cacheSparseSandBoxMap.Exist(collisionData.ElementLeftGlobalIndex);
            bool existElementRight = _cacheSparseSandBoxMap.Exist(collisionData.ElementRightGlobalIndex);
            bool isElementLeft = Random.Range(0, 2) == 0;
            if (isElementLeft)
            {
                if (existElementLeft)
                {
                    // test swap
                    IElement left = _cacheSparseSandBoxMap[collisionData.ElementLeftGlobalIndex];
                    if (left.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.ElementLeftGlobalIndex,
                        };
                    }
                }

                if (existElementRight)
                {
                    IElement right = _cacheSparseSandBoxMap[collisionData.ElementRightGlobalIndex];
                    if (right.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.ElementRightGlobalIndex,
                        };
                    }
                }
            }
            else
            {
                if (existElementRight)
                {
                    IElement right = _cacheSparseSandBoxMap[collisionData.ElementRightGlobalIndex];
                    if (right.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.ElementRightGlobalIndex,
                        };
                    }
                }

                if (existElementLeft)
                {
                    // test swap
                    IElement left = _cacheSparseSandBoxMap[collisionData.ElementLeftGlobalIndex];
                    if (left.Density < element.Density)
                    {
                        return new CollisionInfo()
                        {
                            IsCollision = true,
                            Type = CollisionType.Slip,
                            NextGlobalIndex = collisionData.ElementLeftGlobalIndex,
                        };
                    }
                }
            }

            return new CollisionInfo()
            {
                IsCollision = true,
                Type = CollisionType.Block,
                NextGlobalIndex = collisionData.ElementGlobalIndex,
            };
        }

        /// <summary>
        ///     碰撞处在对角线
        /// </summary>
        private static CollisionInfo SimpleDiagonalCollision(in CollisionDiagonalData collisionData)
        {
            bool existElement = _cacheSparseSandBoxMap.Exist(collisionData.ElementGlobalIndex);
            if (!existElement)
            {
                return default(CollisionInfo);
            }

            bool existBlock = _cacheSparseSandBoxMap.Exist(collisionData.BlockGlobalIndex);
            if (existBlock)
            {
                // test swap
                IElement element = _cacheSparseSandBoxMap[collisionData.ElementGlobalIndex];
                IElement block = _cacheSparseSandBoxMap[collisionData.BlockGlobalIndex];

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
                bool existLeft = _cacheSparseSandBoxMap.Exist(collisionData.LeftGlobalIndex);
                bool existRight = _cacheSparseSandBoxMap.Exist(collisionData.RightGlobalIndex);
                bool isLeft = Random.Range(0, 2) == 0;
                if (isLeft)
                {
                    if (existLeft)
                    {
                        // test swap
                        IElement left = _cacheSparseSandBoxMap[collisionData.LeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Slip,
                                NextGlobalIndex = collisionData.LeftGlobalIndex,
                            };
                        }
                    }

                    if (existRight)
                    {
                        IElement right = _cacheSparseSandBoxMap[collisionData.RightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Slip,
                                NextGlobalIndex = collisionData.RightGlobalIndex,
                            };
                        }
                    }

                }
                else
                {
                    if (existRight)
                    {
                        IElement right = _cacheSparseSandBoxMap[collisionData.RightGlobalIndex];
                        if (right.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Slip,
                                NextGlobalIndex = collisionData.RightGlobalIndex,
                            };
                        }
                    }

                    if (existLeft)
                    {
                        // test swap
                        IElement left = _cacheSparseSandBoxMap[collisionData.LeftGlobalIndex];
                        if (left.Density < element.Density)
                        {
                            return new CollisionInfo()
                            {
                                IsCollision = true,
                                Type = CollisionType.Slip,
                                NextGlobalIndex = collisionData.LeftGlobalIndex,
                            };
                        }
                    }
                }
            }

            return new CollisionInfo()
            {
                IsCollision = true,
                Type = CollisionType.Block,
                NextGlobalIndex = collisionData.ElementGlobalIndex,
            };
        }

        /// <summary>
        ///     仅有简单的穿过与碰撞检测
        /// </summary>
        public static CollisionInfo SimpleCollision(in Vector2Int elementGlobalIndex, in Vector2Int blockGlobalIndex)
        {
            if (elementGlobalIndex == blockGlobalIndex)
            {
                return default(CollisionInfo);
            }

            Vector2Int normal = blockGlobalIndex - elementGlobalIndex;
            if (math.abs(normal.x) > 1 || math.abs(normal.y) > 1)
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

                if (normal.x == 1)
                {
                    collisionData.BlockLeftGlobalIndex = blockGlobalIndex + Vector2Int.down;
                    collisionData.BlockRightGlobalIndex = blockGlobalIndex + Vector2Int.up;
                    collisionData.ElementLeftGlobalIndex = elementGlobalIndex + Vector2Int.up;
                    collisionData.ElementRightGlobalIndex = elementGlobalIndex + Vector2Int.down;
                }
                else if (normal.x == -1)
                {
                    collisionData.BlockLeftGlobalIndex = blockGlobalIndex + Vector2Int.up;
                    collisionData.BlockRightGlobalIndex = blockGlobalIndex + Vector2Int.down;
                    collisionData.ElementLeftGlobalIndex = elementGlobalIndex + Vector2Int.down;
                    collisionData.ElementRightGlobalIndex = elementGlobalIndex + Vector2Int.up;
                }
                else if (normal.y == 1)
                {
                    collisionData.BlockLeftGlobalIndex = blockGlobalIndex + Vector2Int.right;
                    collisionData.BlockRightGlobalIndex = blockGlobalIndex + Vector2Int.left;
                    collisionData.ElementLeftGlobalIndex = elementGlobalIndex + Vector2Int.left;
                    collisionData.ElementRightGlobalIndex = elementGlobalIndex + Vector2Int.right;
                }
                else if (normal.y == -1)
                {
                    collisionData.BlockLeftGlobalIndex = blockGlobalIndex + Vector2Int.left;
                    collisionData.BlockRightGlobalIndex = blockGlobalIndex + Vector2Int.right;
                    collisionData.ElementLeftGlobalIndex = elementGlobalIndex + Vector2Int.right;
                    collisionData.ElementRightGlobalIndex = elementGlobalIndex + Vector2Int.left;
                }
                else
                {
                    throw new Exception("Error Regular Collision Data");
                }

                return SimpleRegularCollision(collisionData);
            }
        }

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
    }
}
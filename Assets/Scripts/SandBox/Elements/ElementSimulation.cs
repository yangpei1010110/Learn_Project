#nullable enable

using SandBox.Elements.Interface;
using SandBox.Map;
using SandBox.Map.SandBox;
using SandBox.Physics;
using Tools;
using Unity.Mathematics;
using UnityEngine;

namespace SandBox.Elements
{
    // TODO 将粒子更新修改为不需要储存变量地址的方式
    public static class ElementSimulation
    {
        private static SparseSandBoxMap _cacheSparseSandBoxMap = SparseSandBoxMap.Instance;

        private static void UpdateVelocity(in Vector2Int globalIndex, in float deltaTime)
        {
            IElement element = _cacheSparseSandBoxMap[globalIndex];
            if (element.Type == ElementType.Void)
            {
                return;
            }

            element.Velocity += MapSetting.GravityForce * deltaTime;
            if (ElementPhysicsSetting.GravityPoint.HasValue)
            {
                Vector2 direction = ElementPhysicsSetting.GravityPoint.Value - (Vector2)globalIndex;
                Vector2 normalized = direction.normalized;
                float Magnitude = math.max(0.001f, direction.sqrMagnitude);
                Vector2 force = normalized * 100000f / Magnitude;
                element.Velocity += force * deltaTime;
            }

            _cacheSparseSandBoxMap[globalIndex] = element;
        }

        private static void UpdateByCollision(Vector2Int globalIndex, in float deltaTime)
        {
            IElement element = _cacheSparseSandBoxMap[globalIndex];
            if (element.Type == ElementType.Void)
            {
                return;
            }

            Vector2 nextWorldPosition = globalIndex + element.PositionOffset + element.Velocity * deltaTime;
            Vector2Int nextGlobalIndex = MapOffset.GlobalRound(nextWorldPosition);
            Vector2 offset = nextWorldPosition - nextGlobalIndex;
            element.PositionOffset = offset;

            Vector2Int? elementGlobalIndex = globalIndex;
            if (globalIndex != nextGlobalIndex)
            {
                LineTool2D.LineCast2(globalIndex, nextGlobalIndex, stepGlobalIndex =>
                {
                    ElementPhysics.CollisionInfo collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, stepGlobalIndex);
                    if (collisionData.Type == ElementPhysics.CollisionType.Swap
                     || collisionData.Type == ElementPhysics.CollisionType.Slip)
                    {
                        // is move
                        SandBoxTool.MoveTo(elementGlobalIndex.Value, collisionData.NextGlobalIndex);
                        elementGlobalIndex = collisionData.NextGlobalIndex;

                        // if (collisionData.Type == ElementPhysics.CollisionType.Swap)
                        // {
                        //     cacheSparseSandBoxMap2[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                        // }
                        // else if (collisionData.Type == ElementPhysics.CollisionType.Slip)
                        // {
                        //     cacheSparseSandBoxMap2[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                        // }
                    }
                    else if (collisionData.Type == ElementPhysics.CollisionType.Block)
                    {
                        _cacheSparseSandBoxMap[elementGlobalIndex.Value].Velocity = Vector2.zero;
                        return false;
                    }

                    return true;
                });
            }


            // dirty check
            IElement nextElement = _cacheSparseSandBoxMap[elementGlobalIndex.Value];
            // edge detect
            if (!_cacheSparseSandBoxMap.Exist(elementGlobalIndex.Value + Vector2Int.up)
             || !_cacheSparseSandBoxMap.Exist(elementGlobalIndex.Value + Vector2Int.down)
             || !_cacheSparseSandBoxMap.Exist(elementGlobalIndex.Value + Vector2Int.left)
             || !_cacheSparseSandBoxMap.Exist(elementGlobalIndex.Value + Vector2Int.right))
            {
                // on edge
                nextElement.Velocity = Vector2.zero;
            }

            // nextElement.Velocity *= ElementPhysicsSetting.VelocityDamping;
            if (nextElement.Velocity.sqrMagnitude > 0.1f)
            {
                _cacheSparseSandBoxMap.SetDirty(elementGlobalIndex.Value);
            }
        }

        public static bool Run(in Vector2Int globalIndex, in float deltaTime)
        {
            IElement element = _cacheSparseSandBoxMap[globalIndex];
            if (element.Type == ElementType.Void)
            {
                return false;
            }

            UpdateVelocity(globalIndex, deltaTime);
            UpdateByCollision(globalIndex, deltaTime);
            return true;
        }
    }
}
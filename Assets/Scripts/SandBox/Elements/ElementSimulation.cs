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

        private static Vector2Int[] LineCastResults = new Vector2Int[128];

        private static void UpdateVelocity(in Vector2Int globalIndex, in float deltaTime)
        {
            IElement element = _cacheSparseSandBoxMap[globalIndex];
            if (element.Type == ElementType.Void)
            {
                return;
            }

            element.Velocity += element.Density >= 0f ? MapSetting.GravityForce * deltaTime : -MapSetting.GravityForce * deltaTime;
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
                int resultCount = Line2D.LineCastNonAlloc(globalIndex, nextGlobalIndex, ref LineCastResults);
                for (int i = 0; i < resultCount; i++)
                {
                    if (i >= ElementPhysicsSetting.maxStepDistance)
                    {
                        break;
                    }

                    Vector2Int stepGlobalIndex = LineCastResults[i];
                    ElementPhysics.CollisionInfo collisionData = ElementPhysics.SimpleCollision(elementGlobalIndex.Value, stepGlobalIndex);
                    if (collisionData.Type == ElementPhysics.CollisionType.Swap
                     || collisionData.Type == ElementPhysics.CollisionType.Slip)
                    {
                        // is move
                        SandBoxTool.MoveTo(elementGlobalIndex.Value, collisionData.NextGlobalIndex);
                        elementGlobalIndex = collisionData.NextGlobalIndex;

                        if (collisionData.Type == ElementPhysics.CollisionType.Swap)
                        {
                            // _cacheSparseSandBoxMap[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.VelocityDamping;
                        }
                        else if (collisionData.Type == ElementPhysics.CollisionType.Slip)
                        {
                            // _cacheSparseSandBoxMap[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.CollisionDamping;
                        }
                    }
                    else if (collisionData.Type == ElementPhysics.CollisionType.Block)
                    {
                        _cacheSparseSandBoxMap[elementGlobalIndex.Value].Velocity *= ElementPhysicsSetting.CollisionDamping;
                        break;
                    }
                    // else if (collisionData.Type == ElementPhysics.CollisionType.Empty)
                    // {
                    //     _cacheSparseSandBoxMap[elementGlobalIndex.Value].Velocity *= 0.5f;
                    //     break;
                    // }
                }
            }


            // dirty check
            var nextValueIndex = elementGlobalIndex.Value;
            IElement nextElement = _cacheSparseSandBoxMap[nextValueIndex];
            nextElement.Velocity *= ElementPhysicsSetting.VelocityDamping;

            if (nextValueIndex == globalIndex)
            {
                nextElement.StableStep += 1;
            }
            else
            {
                nextElement.StableStep = math.max(0, nextElement.StableStep - 100);
            }

            // // edge detect
            // var up = nextValueIndex + Vector2Int.up;
            // var down = nextValueIndex + Vector2Int.down;
            // var left = nextValueIndex + Vector2Int.left;
            // var right = nextValueIndex + Vector2Int.right;
            // if ((!_cacheSparseSandBoxMap.Exist(up))
            //  || (!_cacheSparseSandBoxMap.Exist(down))
            //  || (!_cacheSparseSandBoxMap.Exist(left))
            //  || (!_cacheSparseSandBoxMap.Exist(right)))
            // {
            //     // on edge
            //     nextElement.Velocity *= 0.001f;
            // }

            // else if (nextElement.Velocity.sqrMagnitude > MapSetting.GravityForce.sqrMagnitude)
            // {
            //     nextElement.StableStep = math.max(0, nextElement.StableStep - 10);
            // }

            // if (elementGlobalIndex.Value == nextGlobalIndex)
            // {
            //     nextElement.StableStep += 1;
            // }
            // else
            // {
            //     nextElement.StableStep = 0;
            // }


            if (nextElement.StableStep < ElementPhysicsSetting.StableStepSleep)
            {
                _cacheSparseSandBoxMap.SetDirty(nextValueIndex);
            }
            else if (nextElement.Type != ElementType.Solid && nextElement.StableStep < ElementPhysicsSetting.StableStepSleep * 2)
            {
                _cacheSparseSandBoxMap.SetDirty(nextValueIndex);
            }
            
            if (nextElement.Type == ElementType.Void)
            {
                nextElement.StableStep = ElementPhysicsSetting.StableStepSleep;
            }
            else if (nextElement.Type == ElementType.Solid)
            {
                nextElement.StableStep = math.clamp(nextElement.StableStep, 0, ElementPhysicsSetting.StableStepSleep);
            }
            else
            {
                nextElement.StableStep = math.clamp(nextElement.StableStep, 0, ElementPhysicsSetting.StableStepSleep * 2);
            }

            // nextElement.Velocity *= ElementPhysicsSetting.VelocityDamping;
            // if (nextElement.Velocity.sqrMagnitude > 0.1f)
            // {
            //     _cacheSparseSandBoxMap.SetDirty(elementGlobalIndex.Value);
            // }
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
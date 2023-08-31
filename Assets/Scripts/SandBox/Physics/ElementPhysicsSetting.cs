#nullable enable
using UnityEngine;

namespace SandBox.Physics
{
    public static class ElementPhysicsSetting
    {
        public static float       CollisionDamping = 0.95f;
        public static float       VelocityDamping  = 0.99f;
        public static Vector2Int? GravityPoint;
    }
}
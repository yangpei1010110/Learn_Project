#nullable enable
using SandBox.Map;
using UnityEngine;

namespace SandBox.Physics
{
    public static class ElementPhysicsSetting
    {
        public static int         maxStepDistance  = MapSetting.MapLocalSizePerUnit / 2;
        public static float       CollisionDamping = 0.995f;
        public static float       VelocityDamping  = 0.999f;
        public static Vector2Int? GravityPoint;
        public static int         StableStepSleep = 1000;
    }
}
#nullable enable

using UnityEngine;

namespace SandBox.Map
{
    public static class MapSetting
    {
        public static readonly int     MapDirtyOutRange    = 2;
        public static readonly int     MapLocalSizePerUnit = 64;
        public static readonly float   MapWorldSizePerUnit = 1f;
        public static readonly Vector2 SpritePivot         = new(0.5f, 0.5f);
        public static readonly Vector2 GravityForce        = Physics2D.gravity;
    }
}
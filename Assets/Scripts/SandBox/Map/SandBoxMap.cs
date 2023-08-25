#nullable enable

using SandBox.Elements.Interface;
using UnityEngine;

namespace SandBox.Map
{
    public class SandBoxMap
    {
        public IElement this[Vector2 worldPosition]
        {
            get => this[MapOffset.WorldToGlobal(
                            worldPosition,
                            MapSetting.Instance.MapLocalSizePerUnit,
                            MapSetting.Instance.MapWorldSizePerUnit)];
            set => this[MapOffset.WorldToGlobal(
                            worldPosition,
                            MapSetting.Instance.MapLocalSizePerUnit,
                            MapSetting.Instance.MapWorldSizePerUnit)] = value;
        }
        public IElement this[Vector2Int globalPosition]
        {
            get => SparseSandBoxMap.Instance[globalPosition];
            set
            {
                value.Position = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                SparseSandBoxMap.Instance[globalPosition] = value;
                SparseSpriteMap.Instance[globalPosition] = SparseSandBoxMap.Instance[globalPosition].Color;
            }
        }

        public bool Exist(Vector2Int globalPosition) => SparseSandBoxMap.Instance.ContainKey(MapOffset.GlobalToBlock(globalPosition, MapSetting.Instance.MapLocalSizePerUnit));

        public void UpdateMap()
        {
            SparseSandBoxMap.Instance.UpdateParticles();
            SparseSpriteMap.Instance.Flush();
        }

        #region Instance

        private static SandBoxMap? _instance;
        public static  SandBoxMap  Instance => _instance ??= new SandBoxMap();

        #endregion
    }
}
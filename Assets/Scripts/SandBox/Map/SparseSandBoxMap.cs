using System.Collections.Generic;
using Extensions;
using JetBrains.Annotations;
using SandBox.Elements;
using UnityEngine;

namespace SandBox.Map
{
    public class SparseSandBoxMap
    {
        #region Data

        public Dictionary<Vector2Int, MapBlock> _mapBlocks = new();

        #endregion

        public IElement this[Vector2Int globalPosition]
        {
            get
            {
                Vector2Int mapBlockIndex = MapOffset.BlockIndex(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                return map[mapLocalIndex];
            }
            set
            {
                Vector2Int mapBlockIndex = MapOffset.BlockIndex(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                map[mapLocalIndex] = value;
            }
        }

        private MapBlock CreateMapBlock(Vector2Int mapBlockIndex) => new(mapBlockIndex);

        #region Instance

        [CanBeNull] private static SparseSandBoxMap _instance;
        public static              SparseSandBoxMap Instance => _instance ??= new SparseSandBoxMap();

        #endregion
    }
}
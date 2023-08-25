#nullable enable

using System.Collections.Generic;
using System.Linq;
using Extensions;
using SandBox.Elements.Interface;
using UnityEngine;

namespace SandBox.Map
{
    public class SparseSandBoxMap
    {
        public IElement this[in Vector2Int globalIndex]
        {
            get
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalIndex);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalIndex);
                return map[mapLocalIndex];
            }
            set
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalIndex);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalIndex);
                map[mapLocalIndex] = value;
            }
        }

        public void SetDirtyPoint(in Vector2Int globalIndex)
        {
            int mapLocalSizePerUnit = MapSetting.MapLocalSizePerUnit;
            int mapDirtyOutRange = MapSetting.MapDirtyOutRange;

            Vector2Int dirtyMin = new(
                Mathf.Clamp(globalIndex.x - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(globalIndex.y - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
            Vector2Int dirtyMax = new(
                Mathf.Clamp(globalIndex.x + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(globalIndex.y + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
        }

        public bool ContainKey(in Vector2Int mapBlockIndex) => _mapBlocks.ContainsKey(mapBlockIndex);

        private MapBlock CreateMapBlock(Vector2Int mapBlockIndex) => new(mapBlockIndex);

        public void UpdateParticles()
        {
            Step += 1;
            foreach (MapBlock mapBlock in _mapBlocks.Values.ToArray())
            {
                mapBlock.UpdateElement();
            }
        }

        #region Data

        public long Step { get; set; }

        public Dictionary<Vector2Int, MapBlock> _mapBlocks = new();

        #endregion

        #region Instance

        private static SparseSandBoxMap? _instance;
        public static  SparseSandBoxMap  Instance => _instance ??= new SparseSandBoxMap();

        #endregion
    }
}
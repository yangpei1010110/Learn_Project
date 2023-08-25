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
        public IElement this[Vector2Int globalPosition]
        {
            get
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                return map[mapLocalIndex];
            }
            set
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                MapBlock map = _mapBlocks.GetOrNew(mapBlockIndex, CreateMapBlock, mapBlockIndex);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                map[mapLocalIndex] = value;
            }
        }

        public void SetDirtyPoint(in Vector2Int globalIndex)
        {
            int mapLocalSizePerUnit = MapSetting.Instance.MapLocalSizePerUnit;
            int mapDirtyOutRange = MapSetting.Instance.MapDirtyOutRange;

            Vector2Int dirtyMin = new(
                Mathf.Clamp(globalIndex.x - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(globalIndex.y - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
            Vector2Int dirtyMax = new(
                Mathf.Clamp(globalIndex.x + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(globalIndex.y + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
        }

        public bool ContainKey(Vector2Int mapBlockIndex) => _mapBlocks.ContainsKey(mapBlockIndex);

        private MapBlock CreateMapBlock(Vector2Int mapBlockIndex) => new(mapBlockIndex);

        public void UpdateParticles()
        {
            Instance.Step += 1;
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
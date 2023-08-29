#nullable enable
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace SandBox.Map.SandBox
{
    public class SparseSandBoxMap2<T>
    {
        public bool Exist(in Vector2Int globalPosition) => ContainKey(MapOffset.GlobalToBlock(globalPosition));

        public bool ContainKey(in Vector2Int mapBlockIndex) => _mapBlocks.ContainsKey(mapBlockIndex);

        public ref T this[in Vector2Int globalIndex] => ref _mapBlocks.GetOrNew(MapOffset.GlobalToBlock(globalIndex), CreateMapBlock, globalIndex)[globalIndex];

        public void SetDirty(in Vector2Int globalIndex) => _mapBlocks[MapOffset.GlobalToBlock(globalIndex)].SetDirtyPoint(MapOffset.GlobalToLocal(globalIndex));

        private MapBlock2<T> CreateMapBlock(Vector2Int globalIndex)
        {
            MapBlock2<T> result = new(globalIndex);
            Vector2Int blockIndex = result.BlockIndex;

            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<T>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }

            return result;
        }

        #region Data

        public long Step { get; set; }

        public Dictionary<Vector2Int, MapBlock2<T>> _mapBlocks = new();

        #endregion

        #region Instance

        private static SparseSandBoxMap2<T>? _instance;
        public static  SparseSandBoxMap2<T>  Instance => _instance ??= new SparseSandBoxMap2<T>();

        #endregion
    }
}
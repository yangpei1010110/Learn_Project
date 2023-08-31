#nullable enable
using System.Collections.Generic;
using Extensions;
using SandBox.Elements.Interface;
using SandBox.Elements.Void;
using Unity.VisualScripting;
using UnityEngine;

namespace SandBox.Map.SandBox
{
    public class SparseSandBoxMap2
    {
        public void UpdateParticle(in float deltaTime)
        {
            Step += 1;
            foreach (MapBlock2<IElement> mapBlock in _mapBlocks.Values.ToArrayPooled())
            {
                mapBlock.UpdateElement(deltaTime);
            }
        }

        public bool Exist(in Vector2Int globalPosition) => ContainKey(MapOffset.GlobalToBlock(globalPosition));

        public bool ContainKey(in Vector2Int mapBlockIndex) => _mapBlocks.ContainsKey(mapBlockIndex);

        public ref IElement this[in Vector2Int globalIndex] => ref _mapBlocks.GetOrNew(MapOffset.GlobalToBlock(globalIndex), CreateMapBlock, globalIndex)[globalIndex];

        public void SetDirty(in Vector2Int globalIndex) => _mapBlocks[MapOffset.GlobalToBlock(globalIndex)].SetDirtyPoint(MapOffset.GlobalToLocal(globalIndex));

        private MapBlock2<IElement> CreateBlock(in Vector2Int globalIndex)
        {
            var block = new MapBlock2<IElement>(globalIndex);
            for (int i = 0; i < MapSetting.MapLocalSizePerUnit; i++)
            for (int j = 0; j < MapSetting.MapLocalSizePerUnit; j++)
            {
                block._mapElements[i + j * MapSetting.MapLocalSizePerUnit] = new Void();
            }

            return block;
        }

        private MapBlock2<IElement> CreateMapBlock(Vector2Int globalIndex)
        {
            MapBlock2<IElement> result = CreateBlock(globalIndex);
            Vector2Int blockIndex = result.BlockIndex;

            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock2<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }

            return result;
        }

        #region Data

        public long Step { get; set; }

        public Dictionary<Vector2Int, MapBlock2<IElement>> _mapBlocks = new();

        #endregion

        #region Instance

        private static SparseSandBoxMap2? _instance;
        public static  SparseSandBoxMap2  Instance => _instance ??= new SparseSandBoxMap2();

        #endregion
    }
}
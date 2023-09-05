#nullable enable
using System.Collections.Generic;
using Extensions;
using SandBox.Elements.Interface;
using SandBox.Elements.Void;
using Unity.VisualScripting;
using UnityEngine;

namespace SandBox.Map.SandBox
{
    public class SparseSandBoxMap
    {
        public ref IElement this[in Vector2Int globalIndex] => ref _mapBlocks.GetOrNew(MapOffset.GlobalToBlock(globalIndex), CreateMapBlock, globalIndex)[globalIndex];

        public void UpdateParticle(in float deltaTime)
        {
            Step += 1;
            foreach (MapBlock<IElement> mapBlock in _mapBlocks.Values.ToArrayPooled())
            {
                mapBlock.UpdateElement(deltaTime);
            }
        }

        public bool Exist(in Vector2Int globalPosition) => ContainKey(MapOffset.GlobalToBlock(globalPosition));

        public bool ContainKey(in Vector2Int mapBlockIndex) => _mapBlocks.ContainsKey(mapBlockIndex);

        public void SetDirty(in Vector2Int globalIndex) => _mapBlocks[MapOffset.GlobalToBlock(globalIndex)].SetDirtyPoint(MapOffset.GlobalToLocal(globalIndex));

        private MapBlock<IElement> CreateBlock(in Vector2Int globalIndex)
        {
            MapBlock<IElement> block = new(globalIndex);
            for (int i = 0; i < MapSetting.MapLocalSizePerUnit; i++)
            for (int j = 0; j < MapSetting.MapLocalSizePerUnit; j++)
            {
                block._mapElements[i + j * MapSetting.MapLocalSizePerUnit] = new Void();
            }

            return block;
        }

        private MapBlock<IElement> CreateMapBlock(Vector2Int globalIndex)
        {
            MapBlock<IElement> result = CreateBlock(globalIndex);
            Vector2Int blockIndex = result.BlockIndex;

            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.up + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.left;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }
            {
                Vector2Int neighborBlockIndex = blockIndex + Vector2Int.down + Vector2Int.right;
                if (_mapBlocks.TryGetValue(neighborBlockIndex, out MapBlock<IElement>? value))
                {
                    value.InsertMapBlockCache(result);
                }
            }

            return result;
        }

        #region Data

        public long Step { get; set; }

        public Dictionary<Vector2Int, MapBlock<IElement>> _mapBlocks = new();

        #endregion

        #region Instance

        private static SparseSandBoxMap? _instance;
        public static  SparseSandBoxMap  Instance => _instance ??= new SparseSandBoxMap();

        #endregion
    }
}
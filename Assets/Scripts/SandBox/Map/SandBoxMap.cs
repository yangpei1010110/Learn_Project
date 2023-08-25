#nullable enable

using SandBox.Elements.Interface;
using SandBox.Map.SandBox;
using SandBox.Map.Sprite;
using UnityEngine;

namespace SandBox.Map
{
    public class SandBoxMap
    {
        public IElement this[in Vector2 worldPosition]
        {
            get => this[MapOffset.WorldToGlobal(worldPosition)];
            set => this[MapOffset.WorldToGlobal(worldPosition)] = value;
        }
        public IElement this[in Vector2Int globalPosition]
        {
            get => SparseSandBoxMap.Instance[globalPosition];
            set
            {
                value.Position = MapOffset.GlobalToLocal(globalPosition);
                SparseSandBoxMap.Instance[globalPosition] = value;
                SparseSpriteMap.Instance[globalPosition] = SparseSandBoxMap.Instance[globalPosition].Color;
            }
        }

        public bool Exist(in Vector2Int globalPosition) => SparseSandBoxMap.Instance.ContainKey(MapOffset.GlobalToBlock(globalPosition));

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
using JetBrains.Annotations;
using SandBox.Elements;
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
            get => _sparseSandBoxMap[globalPosition];
            set
            {
                value.Position = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                _sparseSandBoxMap[globalPosition] = value;
                _sparseSpriteMap[globalPosition] = value.Color;
            }
        }

        public bool Exist(Vector2Int globalPosition) => _sparseSandBoxMap._mapBlocks.ContainsKey(MapOffset.BlockIndex(globalPosition, MapSetting.Instance.MapLocalSizePerUnit));

        public void Swap(Vector2Int globalPosition1, Vector2Int globalPosition2)
        {
            IElement temp1 = this[globalPosition1];
            IElement temp2 = this[globalPosition2];

            Vector2Int positionTemp = temp1.Position;
            temp1.Position = temp2.Position;
            temp2.Position = positionTemp;

            this[globalPosition1] = temp2;
            this[globalPosition2] = temp1;
        }

        public void UpdateSprite()
        {
            _sparseSpriteMap.Flush();
        }

        #region Instance

        [CanBeNull] private static SandBoxMap _instance;
        public static              SandBoxMap Instance => _instance ??= new SandBoxMap();

        #endregion

        #region Data

        private SparseSandBoxMap _sparseSandBoxMap = new();
        private SparseSpriteMap  _sparseSpriteMap  = new();

        #endregion
    }
}
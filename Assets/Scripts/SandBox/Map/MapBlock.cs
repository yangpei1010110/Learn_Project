#nullable enable

using SandBox.Elements;
using Tools.DataStruct;
using UnityEngine;

namespace SandBox.Map
{
    public class MapBlock<T>
    {
        public MapBlock(in Vector2Int globalIndex)
        {
            BlockIndex = MapOffset.GlobalToBlock(globalIndex);
            _mapElements = new ZArray<T>(MapSetting.MapLocalSizePerUnit);
        }

        public ref T this[in Vector2Int globalIndex]
        {
            get
            {
                Vector2Int localIndex = MapOffset.GlobalToLocal(globalIndex);
                return ref _mapElements[localIndex.x, localIndex.y];
            }
        }

        public void UpdateElement(in float deltaTime)
        {
            int xMin = _dirtyRectMinX;
            int xMax = _dirtyRectMaxX;
            int yMin = _dirtyRectMinY;
            int yMax = _dirtyRectMaxY;
            ClearDirtyRect();
            for (int j = yMin; j <= yMax; j++)
            for (int i = xMin; i <= xMax; i++)
            {
                Vector2Int globalIndex = MapOffset.LocalToGlobal(BlockIndex, new Vector2Int(i, j));
                ElementSimulation.Run(globalIndex, deltaTime);
            }
        }

        #region MapBlockNeighborCache

        private MapBlock<T>? _upBlock;
        private MapBlock<T>? _downBlock;
        private MapBlock<T>? _leftBlock;
        private MapBlock<T>? _rightBlock;
        private MapBlock<T>? _upLeftBlock;
        private MapBlock<T>? _upRightBlock;
        private MapBlock<T>? _downLeftBlock;
        private MapBlock<T>? _downRightBlock;

        public void InsertMapBlockCache(in MapBlock<T> newMapBlock)
        {
            Vector2Int indexOffset = newMapBlock.BlockIndex - BlockIndex;
            switch (indexOffset.x)
            {
                case 1:
                    switch (indexOffset.y)
                    {
                        case 1:
                            _upRightBlock = newMapBlock;
                            break;
                        case 0:
                            _rightBlock = newMapBlock;
                            break;
                        case -1:
                            _downRightBlock = newMapBlock;
                            break;
                    }

                    break;
                case 0:
                    switch (indexOffset.y)
                    {
                        case 1:
                            _upBlock = newMapBlock;
                            break;
                        case -1:
                            _downBlock = newMapBlock;
                            break;
                    }

                    break;
                case -1:
                    switch (indexOffset.y)
                    {
                        case 1:
                            _upLeftBlock = newMapBlock;
                            break;
                        case 0:
                            _leftBlock = newMapBlock;
                            break;
                        case -1:
                            _downLeftBlock = newMapBlock;
                            break;
                    }

                    break;
            }
        }

        #endregion

        #region Data

        public readonly ZArray<T>  _mapElements;
        public readonly Vector2Int BlockIndex;

        #endregion

        #region DirtyRect

        public int _dirtyRectMinX = int.MaxValue;
        public int _dirtyRectMinY = int.MaxValue;
        public int _dirtyRectMaxX = int.MinValue;
        public int _dirtyRectMaxY = int.MinValue;


        public void SetDirtyPoint(in Vector2Int dirtyGlobalIndex)
        {
            Vector2Int dirtyLocalIndex = MapOffset.GlobalToLocal(dirtyGlobalIndex);

            int dirtyMinX = Mathf.Clamp(dirtyLocalIndex.x - MapSetting.MapDirtyOutRange, 0, MapSetting.MapLocalSizePerUnit - 1);
            int dirtyMinY = Mathf.Clamp(dirtyLocalIndex.y - MapSetting.MapDirtyOutRange, 0, MapSetting.MapLocalSizePerUnit - 1);
            int dirtyMaxX = Mathf.Clamp(dirtyLocalIndex.x + MapSetting.MapDirtyOutRange, 0, MapSetting.MapLocalSizePerUnit - 1);
            int dirtyMaxY = Mathf.Clamp(dirtyLocalIndex.y + MapSetting.MapDirtyOutRange, 0, MapSetting.MapLocalSizePerUnit - 1);

            _dirtyRectMinX = Mathf.Min(_dirtyRectMinX, dirtyMinX);
            _dirtyRectMinY = Mathf.Min(_dirtyRectMinY, dirtyMinY);
            _dirtyRectMaxX = Mathf.Max(_dirtyRectMaxX, dirtyMaxX);
            _dirtyRectMaxY = Mathf.Max(_dirtyRectMaxY, dirtyMaxY);
        }

        public void ClearDirtyRect()
        {
            _dirtyRectMinX = int.MaxValue;
            _dirtyRectMinY = int.MaxValue;
            _dirtyRectMaxX = int.MinValue;
            _dirtyRectMaxY = int.MinValue;
        }

        public void SetFullDirtyRect()
        {
            _dirtyRectMinX = 0;
            _dirtyRectMinY = 0;
            _dirtyRectMaxX = MapSetting.MapLocalSizePerUnit - 1;
            _dirtyRectMaxY = MapSetting.MapLocalSizePerUnit - 1;
        }

        #endregion
    }
}
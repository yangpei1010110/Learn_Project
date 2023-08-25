#nullable enable

using SandBox.Elements;
using SandBox.Elements.Interface;
using SandBox.Elements.Void;
using UnityEngine;

namespace SandBox.Map
{
    public class MapBlock
    {
        public MapBlock(in Vector2Int blockIndex) => BlockIndex = blockIndex;

        public IElement this[in Vector2Int localPosition]
        {
            get => MapElements[localPosition.x + localPosition.y * MapSetting.Instance.MapLocalSizePerUnit];
            set
            {
                MapElements[localPosition.x + localPosition.y * MapSetting.Instance.MapLocalSizePerUnit] = value;
                SetDirtyPoint(localPosition);
            }
        }

        public void SetDirtyPoint(in Vector2Int localDirtyPoint)
        {
            int mapLocalSizePerUnit = MapSetting.Instance.MapLocalSizePerUnit;
            int mapDirtyOutRange = MapSetting.Instance.MapDirtyOutRange;

            Vector2Int dirtyMin = new(
                Mathf.Clamp(localDirtyPoint.x - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(localDirtyPoint.y - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
            Vector2Int dirtyMax = new(
                Mathf.Clamp(localDirtyPoint.x + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1),
                Mathf.Clamp(localDirtyPoint.y + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
            // SetDirtyPoint(dirtyX);
            // SetDirtyPoint(dirtyY);

            _dirtyRectMinX = Mathf.Min(_dirtyRectMinX, dirtyMin.x);
            _dirtyRectMinY = Mathf.Min(_dirtyRectMinY, dirtyMin.y);
            // _dirtyRectMaxX = Mathf.Max(_dirtyRectMaxX, dirtyMin.x);
            // _dirtyRectMaxY = Mathf.Max(_dirtyRectMaxY, dirtyMin.y);
            // _dirtyRectMinX = Mathf.Min(_dirtyRectMinX, dirtyMax.x);
            // _dirtyRectMinY = Mathf.Min(_dirtyRectMinY, dirtyMax.y);
            _dirtyRectMaxX = Mathf.Max(_dirtyRectMaxX, dirtyMax.x);
            _dirtyRectMaxY = Mathf.Max(_dirtyRectMaxY, dirtyMax.y);
        }

        public void UpdateElement()
        {
            int mapLocalSizePerUnit = MapSetting.Instance.MapLocalSizePerUnit;
            int mapDirtyOutRange = MapSetting.Instance.MapDirtyOutRange;
            int xMin = _dirtyRectMinX;
            int xMax = _dirtyRectMaxX;
            int yMin = _dirtyRectMinY;
            int yMax = _dirtyRectMaxY;
            // int xMin = _dirtyRectMinX - mapDirtyOutRange < 0 ? 0 : _dirtyRectMinX - mapDirtyOutRange;
            // int xMax = _dirtyRectMaxX + mapDirtyOutRange >= mapLocalSizePerUnit ? mapLocalSizePerUnit - 1 : _dirtyRectMaxX + mapDirtyOutRange;
            // int yMin = _dirtyRectMinY - mapDirtyOutRange < 0 ? 0 : _dirtyRectMinY - mapDirtyOutRange;
            // int yMax = _dirtyRectMaxY + mapDirtyOutRange >= mapLocalSizePerUnit ? mapLocalSizePerUnit - 1 : _dirtyRectMaxY + mapDirtyOutRange;
            ClearDirtyRect();
            for (int j = yMin; j <= yMax; j++)
            for (int i = xMin; i <= xMax; i++)
            {
                int localIndex = i + j * mapLocalSizePerUnit;
                IElement element = MapElements[localIndex];
                MapBlock mapBlock = this;
                bool isChange = ElementSimulation.Run(mapBlock, ref element);
                if (isChange)
                {
                    SetDirtyPoint(new Vector2Int(i, j));
                    // var dirtyX = new Vector2Int(Mathf.Clamp(i - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1), Mathf.Clamp(j - mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
                    // var dirtyY = new Vector2Int(Mathf.Clamp(i + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1), Mathf.Clamp(j + mapDirtyOutRange, 0, mapLocalSizePerUnit - 1));
                    // SetDirtyPoint(dirtyX);
                    // SetDirtyPoint(dirtyY);
                }
            }
        }

        private IElement[] Create()
        {
            IElement[] elements = new IElement[MapSetting.Instance.MapLocalSizePerUnit * MapSetting.Instance.MapLocalSizePerUnit];
            for (int i = 0; i < MapSetting.Instance.MapLocalSizePerUnit; i++)
            {
                for (int j = 0; j < MapSetting.Instance.MapLocalSizePerUnit; j++)
                {
                    elements[i + j * MapSetting.Instance.MapLocalSizePerUnit] = new Void()
                    {
                        Position = new Vector2Int(i, j),
                    };
                }
            }

            {
                Vector2Int originBlockIndex = BlockIndex + Vector2Int.left + Vector2Int.down;
                for (int j = 0; j < 3; j++)
                for (int i = 0; i < 3; i++)
                {
                    Vector2Int testBlockIndex = originBlockIndex + new Vector2Int(i, j);
                    if (SparseSandBoxMap.Instance.ContainKey(testBlockIndex))
                    {
                        SparseSandBoxMap.Instance._mapBlocks[testBlockIndex].SetFullDirtyRect();
                    }
                }
            }

            return elements;
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
            int mapLocalSizePerUnit = MapSetting.Instance.MapLocalSizePerUnit;
            SetDirtyPoint(new Vector2Int(0, 0));
            SetDirtyPoint(new Vector2Int(mapLocalSizePerUnit - 1, mapLocalSizePerUnit - 1));
        }

        #region MapBlockNeighborCache

        private MapBlock? _upBlock;
        private MapBlock? _downBlock;
        private MapBlock? _leftBlock;
        private MapBlock? _rightBlock;
        private MapBlock? _upLeftBlock;
        private MapBlock? _upRightBlock;
        private MapBlock? _downLeftBlock;
        private MapBlock? _downRightBlock;

        public void InsertMapBlockCache(in MapBlock newMapBlock)
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

        private IElement[]? _mapElements;
        public  IElement[]  MapElements => _mapElements ??= Create();
        public  Vector2Int  BlockIndex;

        #endregion

        #region DirtyRect

        public int _dirtyRectMinX = int.MaxValue;
        public int _dirtyRectMinY = int.MaxValue;
        public int _dirtyRectMaxX = int.MinValue;
        public int _dirtyRectMaxY = int.MinValue;

        #endregion
    }
}
using JetBrains.Annotations;
using SandBox.Elements;
using SandBox.Elements.Void;
using UnityEngine;

namespace SandBox.Map
{
    public class MapBlock
    {
        public MapBlock(Vector2Int mapIndex) => MapIndex = mapIndex;

        public IElement this[Vector2Int localPosition]
        {
            get => MapElements[localPosition.x + localPosition.y * MapSetting.Instance.MapLocalSizePerUnit];
            set
            {
                MapElements[localPosition.x + localPosition.y * MapSetting.Instance.MapLocalSizePerUnit] = value;
                _dirtyRectMinX = Mathf.Min(_dirtyRectMinX, localPosition.x);
                _dirtyRectMinY = Mathf.Min(_dirtyRectMinY, localPosition.y);
                _dirtyRectMaxX = Mathf.Max(_dirtyRectMaxX, localPosition.x);
                _dirtyRectMaxY = Mathf.Max(_dirtyRectMaxY, localPosition.y);
            }
        }

        public void UpdateElement()
        {
            int xMin = _dirtyRectMinX;
            int xMax = _dirtyRectMaxX;
            int yMin = _dirtyRectMinY;
            int yMax = _dirtyRectMaxY;
            ClearDirtyRect();
            for (int j = yMin; j <= yMax; j++)
            for (int i = xMin; i <= xMax; i++)
            {
                IElement element = MapElements[i + j * MapSetting.Instance.MapLocalSizePerUnit];
                element.UpdateElement(ref element, MapOffset.LocalToGlobal(MapIndex, element.Position, MapSetting.Instance.MapLocalSizePerUnit));
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
                Vector2Int originBlockIndex = MapIndex + Vector2Int.left + Vector2Int.down;
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
            _dirtyRectMinX = 0;
            _dirtyRectMinY = 0;
            _dirtyRectMaxX = MapSetting.Instance.MapLocalSizePerUnit - 1;
            _dirtyRectMaxY = MapSetting.Instance.MapLocalSizePerUnit - 1;
        }

        #region Data

        [CanBeNull] private IElement[] _mapElements;
        public              IElement[] MapElements => _mapElements ??= Create();
        public              Vector2Int MapIndex;

        #endregion

        #region DirtyRect

        public int _dirtyRectMinX = int.MaxValue;
        public int _dirtyRectMinY = int.MaxValue;
        public int _dirtyRectMaxX = int.MinValue;
        public int _dirtyRectMaxY = int.MinValue;

        #endregion
    }
}
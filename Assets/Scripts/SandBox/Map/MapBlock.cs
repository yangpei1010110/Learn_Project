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
            set => MapElements[localPosition.x + localPosition.y * MapSetting.Instance.MapLocalSizePerUnit] = value;
        }

        public void UpdateElement()
        {
            for (int j = 0; j < MapSetting.Instance.MapLocalSizePerUnit; j++)
            for (int i = 0; i < MapSetting.Instance.MapLocalSizePerUnit; i++)
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

            return elements;
        }

        #region Data

        [CanBeNull] private IElement[] _mapElements;
        public              IElement[] MapElements => _mapElements ??= Create();
        public              Vector2Int MapIndex;

        #endregion
    }
}
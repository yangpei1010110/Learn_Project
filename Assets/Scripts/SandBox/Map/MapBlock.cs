using System.Collections.Generic;
using JetBrains.Annotations;
using SandBox.Elements;
using UnityEngine;
using Void = SandBox.Elements.Void.Void;

namespace SandBox.Map
{
    public class MapBlock
    {
        public bool       IsInit;
        public bool       IsSleep;
        public Vector2Int MapIndex;
        public int        MapSize;

        public              Texture2D         RuntimeTexture;
        [CanBeNull] private IElement[]        _mapElements;
        public              Stack<Vector2Int> _changedElements = new();

        public MapBlock(Vector2Int mapIndex, int mapSize, Texture2D runtimeTexture)
        {
            MapIndex = mapIndex;
            MapSize = mapSize;

            IsInit = true;
            IsSleep = true;

            RuntimeTexture = runtimeTexture;
        }

        public IElement this[int x, int y]
        {
            get
            {
                InitIfNot();
                return _mapElements[x + y * MapSize];
            }
            set
            {
                InitIfNot();
                if (_mapElements[x + y * MapSize].Type == ElementType.Void)
                {
                    _mapElements[x + y * MapSize] = value;
                    _changedElements.Push(new Vector2Int(x, y));
                }
            }
        }

        private void InitIfNot()
        {
            if (!IsSleep) return;

            _mapElements = new IElement[MapSize * MapSize];

            for (int j = 0; j < MapSize; j++)
            for (int i = 0; i < MapSize; i++)
            {
                _mapElements[i + j * MapSize] = new Void(new Vector2Int(i, j));
            }

            IsInit = true;
        }

        public void UpdateTexture()
        {
            bool isUpdate = false;
            while (_changedElements.TryPop(out var result))
            {
                isUpdate = true;
                RuntimeTexture.SetPixel(result.x,result.y, _mapElements[result.x + result.y * MapSize].Color);
            }

            if (isUpdate)
            {
                RuntimeTexture.Apply();
            }
        }
    }
}
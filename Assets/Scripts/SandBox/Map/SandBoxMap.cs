using System.Collections.Generic;
using JetBrains.Annotations;
using SandBox.Elements;
using SandBox.Elements.Solid;
using UnityEngine;

namespace SandBox.Map
{
    public class SandBoxMap
    {
        public int   TotalMapSize        { get; private set; }
        public int   MapSize             { get; private set; } = 64;
        public int   MapPixelPerUnit     { get; private set; } = 64;
        public float MapWorldSizePerUnit { get; private set; } = 1f;

        public Dictionary<Vector2Int, MapBlock>  _mapBlockIndex   = new();
        private Dictionary<Vector2Int, Texture2D> _mapBlockTexture = new();

        public IElement this[int x, int y]
        {
            get => this[new Vector2Int(x, y)];
            set => this[new Vector2Int(x, y)] = value;
        }

        public IElement this[Vector2 worldPosition]
        {
            get => GetElement(worldPosition);
            set => SetElement(worldPosition, value);
        }
        public IElement this[Vector2Int mapPosition]
        {
            get => GetElement(mapPosition);
            set => SetElement(mapPosition, value);
        }

        public void Swap(Vector2Int position1, Vector2Int position2)
        {
            var temp1 = this[position1];
            var temp2 = this[position2];

            var positionTemp = temp1.Position;
            temp1.Position = temp2.Position;
            temp2.Position = positionTemp;

            this[position1] = temp2;
            this[position2] = temp1;
        }

        private IElement GetElement(Vector2 worldPosition)
        {
            var mapBlockIndex = GetMapBlockIndex(worldPosition);
            CheckOrCreateMap(mapBlockIndex);
            var mapBlockOffset = GetLocalPosition(worldPosition);
            // var mapBlockOffset = new Vector2Int(mapPosition.x % MapSize, mapPosition.y % MapSize);
            return _mapBlockIndex[mapBlockIndex][mapBlockOffset.x, mapBlockOffset.y];
            // if (_mapBlockIndex.TryGetValue(mapBlockIndex, out MapBlock value))
            // {
            //     return value[mapBlockOffset.x, mapBlockOffset.y];
            // }
            // else
            // {
            //     CheckOrCreateMap(mapBlockIndex);
            //     MapBlock mapBlock = new(mapBlockIndex, MapSize);
            //     _mapBlockIndex.Add(mapBlockIndex, mapBlock);
            //     return mapBlock[mapBlockOffset.x, mapBlockOffset.y];
            // }
        }

        private void SetElement(Vector2 worldPosition, IElement element)
        {
            var mapBlockIndex = GetMapBlockIndex(worldPosition);
            CheckOrCreateMap(mapBlockIndex);
            var mapBlockOffset = GetLocalPosition(worldPosition);
            element.Position = mapBlockOffset;
            _mapBlockIndex[mapBlockIndex][mapBlockOffset.x, mapBlockOffset.y] = element;
            // var mapBlockOffset = new Vector2Int(mapPosition.x % MapSize, mapPosition.y % MapSize);
            // if (_mapBlockIndex.TryGetValue(mapBlockIndex, out MapBlock value))
            // {
            //     value[mapBlockOffset.x, mapBlockOffset.y] = new Sand(mapPosition);
            // }
            // else
            // {
            //     MapBlock mapBlock = new(mapBlockIndex, MapSize);
            //     _mapBlockIndex.Add(mapBlockIndex, mapBlock);
            //     mapBlock[mapBlockOffset.x, mapBlockOffset.y] = new Sand(mapPosition);
            // }
        }
        
        private IElement GetElement(Vector2Int mapPosition)
        {
            var mapBlockIndex = GetMapBlockIndex(mapPosition);
            CheckOrCreateMap(mapBlockIndex);
           var mapBlockOffset = GetLocalPosition(mapPosition);
            // var mapBlockOffset = new Vector2Int(mapPosition.x % MapSize, mapPosition.y % MapSize);
            return _mapBlockIndex[mapBlockIndex][mapBlockOffset.x, mapBlockOffset.y];
            // if (_mapBlockIndex.TryGetValue(mapBlockIndex, out MapBlock value))
            // {
            //     return value[mapBlockOffset.x, mapBlockOffset.y];
            // }
            // else
            // {
            //     CheckOrCreateMap(mapBlockIndex);
            //     MapBlock mapBlock = new(mapBlockIndex, MapSize);
            //     _mapBlockIndex.Add(mapBlockIndex, mapBlock);
            //     return mapBlock[mapBlockOffset.x, mapBlockOffset.y];
            // }
        }

        private void SetElement(Vector2Int mapPosition, IElement element)
        {
            var mapBlockIndex = GetMapBlockIndex(mapPosition);
            CheckOrCreateMap(mapBlockIndex);
            var mapBlockOffset = GetLocalPosition(mapPosition);
            element.Position = mapBlockOffset;
            _mapBlockIndex[mapBlockIndex][mapBlockOffset.x, mapBlockOffset.y] = element;
            // var mapBlockOffset = new Vector2Int(mapPosition.x % MapSize, mapPosition.y % MapSize);
            // if (_mapBlockIndex.TryGetValue(mapBlockIndex, out MapBlock value))
            // {
            //     value[mapBlockOffset.x, mapBlockOffset.y] = new Sand(mapPosition);
            // }
            // else
            // {
            //     MapBlock mapBlock = new(mapBlockIndex, MapSize);
            //     _mapBlockIndex.Add(mapBlockIndex, mapBlock);
            //     mapBlock[mapBlockOffset.x, mapBlockOffset.y] = new Sand(mapPosition);
            // }
        }

        public Vector2Int GetMapBlockIndex(Vector2Int mapPosition)
        {
            var x = (float)mapPosition.x;
            var y = (float)mapPosition.y;

            x /= MapSize;
            y /= MapSize;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public Vector2Int GetMapBlockIndex(Vector2 worldPosition)
        {
            var x = worldPosition.x;
            var y = worldPosition.y;

            x /= MapWorldSizePerUnit;
            y /= MapWorldSizePerUnit;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public Vector2Int GetGlobalPosition(Vector2 worldPosition)
        {
            var x = worldPosition.x;
            var y = worldPosition.y;

            x /= MapWorldSizePerUnit;
            y /= MapWorldSizePerUnit;

            x *= MapSize;
            y *= MapSize;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }
        
        public Vector2Int GetLocalPosition(Vector2Int mapPosition)
        {
            var x = mapPosition.x;
            var y = mapPosition.y;

            mapPosition.x = x < 0 ? x % MapSize + MapSize : x % MapSize;
            mapPosition.y = y < 0 ? y % MapSize + MapSize : y % MapSize;

            return mapPosition;
        }

        public Vector2Int GetLocalPosition(Vector2 worldPosition)
        {
            var x = worldPosition.x;
            var y = worldPosition.y;

            x /= MapWorldSizePerUnit;
            y /= MapWorldSizePerUnit;

            x = x < 0 ? x % 1f + 1f : x % 1f;
            y = y < 0 ? y % 1f + 1f : y % 1f;

            x *= MapSize;
            y *= MapSize;

            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public void CheckOrCreateMap(Vector2Int mapBlockIndex)
        {
            if (_mapBlockIndex.ContainsKey(mapBlockIndex))
            {
                return;
            }

            CreateMap(mapBlockIndex);
        }

        [CanBeNull]
        public GameObject CreateMap(Vector2Int mapBlockIndex)
        {
            if (_mapBlockIndex.ContainsKey(mapBlockIndex))
            {
                return null;
            }


            var texture2D = new Texture2D(MapSize, MapSize);
            texture2D.filterMode = FilterMode.Point;
            texture2D.wrapMode = TextureWrapMode.Clamp;
            MapBlock mapBlock = new(mapBlockIndex, MapSize, texture2D);
            _mapBlockIndex.Add(mapBlockIndex, mapBlock);
            _mapBlockTexture.Add(mapBlockIndex, texture2D);
            var go = new GameObject($"child-map-{mapBlockIndex}", typeof(SpriteRenderer));
            go.transform.position = new Vector3((mapBlockIndex.x + 0.5f) * MapWorldSizePerUnit, (mapBlockIndex.y + 0.5f) * MapWorldSizePerUnit, 0);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(texture2D, new Rect(0, 0, MapSize, MapSize), 0.5f * Vector2.one, MapSize);
            for (int j = 0; j < texture2D.height; j++)
            {
                for (int i = 0; i < texture2D.width; i++)
                {
                    texture2D.SetPixel(i, j, Color.black);
                }
            }

            texture2D.Apply();
            return go;
        }
    }
}
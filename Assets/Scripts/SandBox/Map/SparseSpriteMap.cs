#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace SandBox.Map
{
    /// <summary>
    ///     稀疏精灵地图
    /// </summary>
    public class SparseSpriteMap
    {
        /// <summary>
        ///     获取或设置颜色
        /// </summary>
        public Color this[in Vector2Int globalPosition]
        {
            get
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                CheckOrCreateByBlockIndex(mapBlockIndex);
                return _mapBlockTexture[mapBlockIndex].GetPixel(mapLocalIndex.x, mapLocalIndex.y);
            }
            set
            {
                Vector2Int mapBlockIndex = MapOffset.GlobalToBlock(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                Vector2Int mapLocalIndex = MapOffset.GlobalToLocal(globalPosition, MapSetting.Instance.MapLocalSizePerUnit);
                CheckOrCreateByBlockIndex(mapBlockIndex);
                _mapBlockTexture[mapBlockIndex].SetPixel(mapLocalIndex.x, mapLocalIndex.y, value);
                _dirtyBlocks.Add(mapBlockIndex);
            }
        }

        public void Init()
        {
            _dirtyBlocks.Clear();
            _mapBlockSprite.Clear();
            _mapBlockTexture.Clear();
        }

        private void CheckOrCreateByBlockIndex(in Vector2Int blockIndex)
        {
            if (_mapBlockSprite.ContainsKey(blockIndex))
            {
                return;
            }
            else
            {
                GameObject go = new($"child-map-{blockIndex}", typeof(SpriteRenderer));
                go.transform.SetParent(MapSetParent);
                go.transform.position =
                    new Vector3((blockIndex.x + 0.5f) * MapSetting.Instance.MapWorldSizePerUnit,
                                (blockIndex.y + 0.5f) * MapSetting.Instance.MapWorldSizePerUnit,
                                0);

                Texture2D tex2D = CreateTexture();
                Sprite sprite = CreateSprite(tex2D);

                go.GetComponent<SpriteRenderer>().sprite = sprite;

                _mapBlockSprite.Add(blockIndex, sprite);
                _mapBlockTexture.Add(blockIndex, tex2D);
            }
        }

        private Sprite CreateSprite(in Texture2D texture) =>
            Sprite.Create(texture,
                          new Rect(0, 0, texture.width, texture.height),
                          MapSetting.Instance.SpritePivot,
                          MapSetting.Instance.MapPixelPerUnit);

        private Texture2D CreateTexture()
        {
            int size = MapSetting.Instance.MapPixelPerUnit;
            Texture2D texture = new(size, size);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    texture.SetPixel(i, j, Color.black);
                }
            }

            texture.Apply();
            return texture;
        }

        /// <summary>
        ///     刷新所有脏块
        /// </summary>
        public void Flush()
        {
            foreach (Vector2Int dirtyBlock in _dirtyBlocks)
            {
                _mapBlockTexture[dirtyBlock].Apply();
            }

            _dirtyBlocks.Clear();
        }

        public void UpdateColorFormMapBlock(in Vector2Int blockIndex)
        {
            if (_mapBlockTexture.TryGetValue(blockIndex, out Texture2D? texture))
            {
                MapBlock? mapBlock = SparseSandBoxMap.Instance._mapBlocks[blockIndex];
                int mapLength = MapSetting.Instance.MapLocalSizePerUnit;
                for (int y = 0; y < mapLength; y++)
                {
                    for (int x = 0; x < mapLength; x++)
                    {
                        texture.SetPixel(x, y, mapBlock[new Vector2Int(x, y)].Color);
                    }
                }

                texture.Apply();
            }
        }

        #region Instance

        private static SparseSpriteMap? _instance;
        public static  SparseSpriteMap  Instance => _instance ??= new SparseSpriteMap();

        #endregion

        #region MapSetParent

        private string     MapSetParentName = "MapSetParent";
        private Transform? _mapSetParent;
        public  Transform  MapSetParent => _mapSetParent ??= new GameObject(MapSetParentName).transform;

        #endregion

        #region Data

        private HashSet<Vector2Int>               _dirtyBlocks     = new();
        private Dictionary<Vector2Int, Sprite>    _mapBlockSprite  = new();
        public  Dictionary<Vector2Int, Texture2D> _mapBlockTexture = new();

        #endregion
    }
}
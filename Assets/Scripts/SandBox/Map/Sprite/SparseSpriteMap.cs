#nullable enable

using System.Collections.Generic;
using SandBox.Map.SandBox;
using UnityEngine;

namespace SandBox.Map.Sprite
{
    public class SparseSpriteMap
    {
        /// <summary>
        ///     获取或设置颜色
        /// </summary>
        public Color this[in Vector2Int globalIndex]
        {
            get
            {
                Texture2D texture = GetOrNewBlock(globalIndex);
                Vector2Int localIndex = MapOffset.GlobalToLocal(globalIndex);
                return texture.GetPixel(localIndex.x, localIndex.y);
            }
            set
            {
                Texture2D texture = GetOrNewBlock(globalIndex);
                Vector2Int localIndex = MapOffset.GlobalToLocal(globalIndex);
                texture.SetPixel(localIndex.x, localIndex.y, value);
                _dirtyBlocks.Add(MapOffset.GlobalToBlock(globalIndex));
            }
        }

        public void SafeSet(in Vector2Int globalIndex, in Color color)
        {
            Vector2Int blockIndex = MapOffset.GlobalToBlock(globalIndex);
            if (_mapBlockTexture.TryGetValue(blockIndex, out Texture2D? value))
            {
                Vector2Int localIndex = MapOffset.GlobalToLocal(globalIndex);
                value.SetPixel(localIndex.x, localIndex.y, color);
                _dirtyBlocks.Add(blockIndex);
            }
        }

        public void Init()
        {
            _dirtyBlocks.Clear();
            _guiBlocks.Clear();
            _mapBlockSprite.Clear();
            _mapBlockTexture.Clear();
        }

        private Texture2D GetOrNewBlock(in Vector2Int globalIndex)
        {
            Vector2Int blockIndex = MapOffset.GlobalToBlock(globalIndex);
            if (_mapBlockTexture.TryGetValue(blockIndex, out Texture2D? result))
            {
                return result;
            }
            else
            {
                GameObject go = new($"child-map-{blockIndex}", typeof(SpriteRenderer));
                go.transform.SetParent(MapSetParent);
                go.transform.position = MapOffset.BlockToWorld(blockIndex);

                Texture2D tex2D = CreateTexture();
                UnityEngine.Sprite sprite = CreateSprite(tex2D);

                go.GetComponent<SpriteRenderer>().sprite = sprite;

                _mapBlockTexture.Add(blockIndex, tex2D);
                _mapBlockSprite.Add(blockIndex, sprite);

                return tex2D;
            }
        }

        private UnityEngine.Sprite CreateSprite(in Texture2D texture) =>
            UnityEngine.Sprite.Create(texture,
                                      new Rect(0, 0, texture.width, texture.height),
                                      MapSetting.SpritePivot,
                                      MapSetting.MapLocalSizePerUnit);

        private Texture2D CreateTexture()
        {
            Texture2D texture = new(MapSetting.MapLocalSizePerUnit, MapSetting.MapLocalSizePerUnit);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            for (int j = 0; j < MapSetting.MapLocalSizePerUnit; j++)
            {
                for (int i = 0; i < MapSetting.MapLocalSizePerUnit; i++)
                {
                    texture.SetPixel(i, j, Color.black);
                }
            }

            texture.Apply();
            return texture;
        }

        #region FlushColor

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
                // TODO update MapBlock to MapBlock2
                MapBlock? mapBlock = SparseSandBoxMap.Instance._mapBlocks[blockIndex];
                // MapBlock2<IElement> mapBlock = SparseSandBoxMap2<IElement>.Instance._mapBlocks[blockIndex];
                int mapLength = MapSetting.MapLocalSizePerUnit;
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

        #endregion

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

        private HashSet<Vector2Int>                        _dirtyBlocks     = new();
        private HashSet<Vector2Int>                        _guiBlocks       = new();
        private Dictionary<Vector2Int, UnityEngine.Sprite> _mapBlockSprite  = new();
        private Dictionary<Vector2Int, Texture2D>          _mapBlockTexture = new();

        #endregion
    }
}
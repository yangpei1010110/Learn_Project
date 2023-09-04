#nullable enable
using SandBox.Elements.Interface;
using SandBox.Map;
using SandBox.Map.SandBox;
using SandBox.Map.Sprite;
using UnityEngine;

namespace Tools
{
    public static class SandBoxTool
    {
        private static SparseSandBoxMap _cacheSparseSandBoxMap = SparseSandBoxMap.Instance;
        private static SparseSpriteMap  cacheSparseSpriteMap   = SparseSpriteMap.Instance;

        public static void SwapGlobalIndex(ref IElement element1, ref IElement element2, in Vector2Int globalIndex1, in Vector2Int globalIndex2)
        {
            Vector2Int local1 = MapOffset.GlobalToLocal(globalIndex1);
            Vector2Int local2 = MapOffset.GlobalToLocal(globalIndex2);

            element1.Position = local2;
            element2.Position = local1;
        }

        public static void MoveTo(in Vector2Int sourceGlobalIndex, in Vector2Int targetGlobalIndex)
        {
            if (_cacheSparseSandBoxMap.Exist(targetGlobalIndex)
             && _cacheSparseSandBoxMap.Exist(sourceGlobalIndex))
            {
                IElement? source = _cacheSparseSandBoxMap[sourceGlobalIndex];
                IElement? target = _cacheSparseSandBoxMap[targetGlobalIndex];
                _cacheSparseSandBoxMap[targetGlobalIndex] = source;
                _cacheSparseSandBoxMap[sourceGlobalIndex] = target;
                cacheSparseSpriteMap.ReloadColor(targetGlobalIndex);
                cacheSparseSpriteMap.ReloadColor(sourceGlobalIndex);
                _cacheSparseSandBoxMap.SetDirty(targetGlobalIndex);
                _cacheSparseSandBoxMap.SetDirty(sourceGlobalIndex);
            }
        }
    }
}
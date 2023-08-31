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
        public static void SwapGlobalIndex(ref IElement element1, ref IElement element2, in Vector2Int globalIndex1, in Vector2Int globalIndex2)
        {
            Vector2Int local1 = MapOffset.GlobalToLocal(globalIndex1);
            Vector2Int local2 = MapOffset.GlobalToLocal(globalIndex2);

            element1.Position = local2;
            element2.Position = local1;
        }

        public static void MoveTo(in Vector2Int sourceGlobalIndex, in Vector2Int targetGlobalIndex)
        {
            if (SparseSandBoxMap2.Instance.Exist(targetGlobalIndex)
             && SparseSandBoxMap2.Instance.Exist(sourceGlobalIndex))
            {
                var source = SparseSandBoxMap2.Instance[sourceGlobalIndex];
                var target = SparseSandBoxMap2.Instance[targetGlobalIndex];
                SparseSandBoxMap2.Instance[targetGlobalIndex] = source;
                SparseSandBoxMap2.Instance[sourceGlobalIndex] = target;
                SparseSpriteMap.Instance.ReloadColor(targetGlobalIndex);
                SparseSpriteMap.Instance.ReloadColor(sourceGlobalIndex);
                SparseSandBoxMap2.Instance.SetDirty(targetGlobalIndex);
                SparseSandBoxMap2.Instance.SetDirty(sourceGlobalIndex);
            }
        }
    }
}
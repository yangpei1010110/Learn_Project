#nullable enable
using SandBox.Elements.Interface;
using SandBox.Map;
using UnityEngine;

namespace Tools
{
    public static class SandBoxTool
    {
        public static void SwapGlobalIndex(ref IElement element1, ref IElement element2, in Vector2Int globalIndex1, in Vector2Int globalIndex2)
        {
            Vector2Int local1 = MapOffset.GlobalToLocal(globalIndex1, MapSetting.Instance.MapLocalSizePerUnit);
            Vector2Int local2 = MapOffset.GlobalToLocal(globalIndex2, MapSetting.Instance.MapLocalSizePerUnit);

            element1.Position = local2;
            element2.Position = local1;
        }
    }
}
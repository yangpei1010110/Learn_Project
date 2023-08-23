#nullable enable
using SandBox.Elements.Interface;
using SandBox.Map;
using UnityEngine;

namespace Tools
{
    public static class SandBoxTool
    {
        public static void SwapPosition(ref IElement element1, ref IElement element2, Vector2Int global1, Vector2Int global2)
        {
            Vector2Int local1 = MapOffset.GlobalToLocal(global1, MapSetting.Instance.MapLocalSizePerUnit);
            Vector2Int local2 = MapOffset.GlobalToLocal(global2, MapSetting.Instance.MapLocalSizePerUnit);

            element1.Position = local2;
            element2.Position = local1;
        }
    }
}
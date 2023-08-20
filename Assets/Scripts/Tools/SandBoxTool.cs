using SandBox.Elements;
using UnityEngine;

namespace Tools
{
    public static class SandBoxTool
    {
        public static void SwapPosition(ref IElement s1, ref IElement s2)
        {
            Vector2Int temp = s1.Position;
            s1.Position = s2.Position;
            s2.Position = temp;
        }
    }
}
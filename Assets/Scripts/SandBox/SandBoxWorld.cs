using SandBox.Elements.Solid;
using SandBox.Map;
using UnityEngine;

namespace SandBox
{
    public class SandBoxWorld : MonoBehaviour
    {
        public SandBoxMap SandBoxMap;

        private void Start()
        {
            SandBoxMap = new SandBoxMap();
        }

        private void Update()
        {
            UpdateInput();
            UpdateTexture();
        }


        private void UpdateInput()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SandBoxMap[mousePosition] = new Sand();
                Debug.Log($"mousePosition: {mousePosition.x}, {mousePosition.y}");
                var globalPosition = MapOffset.WorldToGlobal(mousePosition, MapSetting.Instance.MapLocalSizePerUnit, MapSetting.Instance.MapWorldSizePerUnit);
                Debug.Log($"globalPosition: {globalPosition.x}, {globalPosition.y}");
                var localPosition = MapOffset.WorldToLocal(mousePosition, MapSetting.Instance.MapLocalSizePerUnit, MapSetting.Instance.MapWorldSizePerUnit);
                Debug.Log($"localPosition: {localPosition.x}, {localPosition.y}");
            }
        }

        private void UpdateTexture()
        {
            SandBoxMap.UpdateSprite();
        }
    }
}
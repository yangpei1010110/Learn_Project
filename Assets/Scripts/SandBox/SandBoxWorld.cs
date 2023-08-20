using SandBox.Elements.Liquid;
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
            }
            else if (Input.GetMouseButton(1))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SandBoxMap[mousePosition] = new Water();
            }
        }

        private void UpdateTexture()
        {
            SandBoxMap.UpdateMap();
        }
    }
}
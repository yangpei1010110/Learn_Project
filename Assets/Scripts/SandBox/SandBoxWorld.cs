using System;
using System.Collections.Generic;
using SandBox.Elements.Solid;
using SandBox.Map;
using UnityEngine;

namespace SandBox
{
    public class SandBoxWorld : MonoBehaviour
    {
        private List<GameObject>               ChildSpriteList;
        // public  Dictionary<Vector2Int, Sprite> SpriteMap;
        public  SandBoxMap                     SandBoxMap;

        public int TempResult;

        private void Start()
        {
            ChildSpriteList = new List<GameObject>();
            // SpriteMap = new Dictionary<Vector2Int, Sprite>();
            SandBoxMap = new SandBoxMap();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);
                ChildSpriteList.Add(child.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInput();
        }


        private void UpdateInput()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SandBoxMap[mousePosition] = new Sand(SandBoxMap.GetLocalPosition(mousePosition));
                // mousePosition *= SandBoxMap.MapSize;
                // var mapIndex = SandBoxMap.GetMapBlockIndex(mousePosition);
                // SandBoxMap.CreateMap(mapIndex);
                // Debug.Log($"mouse: {mousePosition}, mapIndex: {mapIndex}");
            }

            foreach (var kv in SandBoxMap._mapBlockIndex) 
            {
                kv.Value.UpdateTexture();
            }
        }
    }
}
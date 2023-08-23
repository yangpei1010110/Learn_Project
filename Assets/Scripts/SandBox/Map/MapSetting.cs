#nullable enable

using System.IO;
using Environments;
using JetBrains.Annotations;
using UnityEngine;

namespace SandBox.Map
{
    public class MapSetting
    {
        [CanBeNull] private static MapSetting _instance;

        public int   MapDirtyOutRange    = 2;
        public int   MapLocalSizePerUnit = 32;
        public int   MapPixelPerUnit     = 32;
        public float MapWorldSizePerUnit = 1f;

        public        Vector2    SpritePivot = new(0.5f, 0.5f);
        public static MapSetting Instance => _instance ??= new MapSetting();

        public static void Load()
        {
            string json = File.ReadAllText(FilePathEnvironment.MapSettingFile);
            _instance = JsonUtility.FromJson<MapSetting>(json);
        }

        public static void Save()
        {
            string json = JsonUtility.ToJson(Instance);
            File.WriteAllText(FilePathEnvironment.MapSettingFile, json);
        }
    }
}
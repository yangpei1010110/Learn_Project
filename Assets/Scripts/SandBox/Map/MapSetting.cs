using System.IO;
using Environments;
using JetBrains.Annotations;
using UnityEngine;

namespace SandBox.Map
{
    public class MapSetting
    {
        [CanBeNull] private static MapSetting _instance;
        public static              MapSetting Instance => _instance ??= new MapSetting();

        public Vector2 SpritePivot         { get; set; } = new(0.5f, 0.5f);
        public int     MapLocalSizePerUnit { get; set; } = 64;
        public int     MapPixelPerUnit     { get; set; } = 64;
        public float   MapWorldSizePerUnit { get; set; } = 1f;

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
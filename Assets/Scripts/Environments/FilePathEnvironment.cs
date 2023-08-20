using System.IO;
using UnityEngine;

namespace Environments
{
    /// <summary>
    ///     文件相对路径
    /// </summary>
    public static class FilePathEnvironment
    {
        public static string SettingFolder  { get; set; } = Path.Combine(Application.dataPath, "Settings");
        public static string MapSettingFile { get; set; } = Path.Combine(SettingFolder, "MapSetting.json");
    }
}
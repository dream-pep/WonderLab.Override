﻿using MinecraftLaunch.Modules.Models.Launch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    /// <summary>
    /// 启动信息数据模型
    /// </summary>
    public class LaunchInfoDataModel {
        [JsonProperty("gamedirectoryPath")]
        public string GameDirectoryPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),".minecraft");

        [JsonProperty("javaruntimePath")]
        public JavaInfo JavaRuntimePath { get; set; }

        [JsonProperty("selectgameCore")]
        public string SelectGameCore { get; set; }

        [JsonProperty("maxMemory")]
        public int MaxMemory { get; set; } = 1024;

        [JsonProperty("miniMemory")]
        public int MiniMemory { get; set; } = 512;

        [JsonProperty("isautoSelectjava")]
        public bool IsAutoSelectJava { get; set; } = false;

        [JsonProperty("javaRuntimes")]
        public List<JavaInfo> JavaRuntimes { get; set; } = new();

        [JsonProperty("gameDirectorys")]
        public List<string> GameDirectorys { get; set; } = new() { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft") };
    }
}
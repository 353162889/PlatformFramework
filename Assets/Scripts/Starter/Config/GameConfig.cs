using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Launch
{
    public class GameCfg
    {
        public int id;
        public string rootDir;
        public string name;
        public string runClass;
        public string ui;
        public int uiDepth;
        public GameCfg(XmlElement element)
        {
            this.id = int.Parse(element.GetAttribute("id"));
            this.rootDir = element.GetAttribute("rootDir");
            this.rootDir.Replace("\\","/");
            if(this.rootDir.EndsWith("/"))
            {
                this.rootDir = this.rootDir.Substring(0, this.rootDir.Length - 1);
            }
            int index = this.rootDir.IndexOf("/");
            if(index == -1)
            {
                this.name = this.rootDir;
            }
            else
            {
                this.name = this.rootDir.Substring(index + 1, this.rootDir.Length - index - 1);
            }
            this.runClass = element.GetAttribute("runClass");
            if(string.IsNullOrEmpty(this.runClass))
            {
                this.runClass = "Launch.GameRunner";
            }
            this.ui = element.GetAttribute("ui");
            this.uiDepth = int.Parse(element.GetAttribute("uiDepth"));
        }
    }
    //包内的游戏配置
    public class GameConfig
    {
        private static string GameSetting = "Launch/Config/GameConfig";
        public static bool IsResourceLoadMode { get; set; }
        public static bool IsHotUpdateMode { get; set; }
        public static bool IsDebugInfo { get; set; }
        public static bool IsDebugWarn { get; set; }
        public static bool IsDebugError { get; set; }
        public static string ServerUrl { get; set; }
        public static string PkgUpdateUrl { get; set; }
        public static string ShowVersion { get; set; }
        public static int PkgVersion { get; set; }
        public static string VersionCodeFile { get; set; }
        public static string VersionDataFile { get; set; }
        public static string VersionPkgFile { get; set; }
        private static Dictionary<int, GameCfg> _mapGame = new Dictionary<int, GameCfg>();

        public static void Load()
        {
            TextAsset txt = Resources.Load<TextAsset>(GameSetting);
            string files = txt.text;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(files);
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                XmlElement element = (XmlElement)node; 
                if (node.Name == "Resource")
                {
                    IsResourceLoadMode = element.GetAttribute("isResourcesLoadMode") == "true";
                }
                else if(node.Name == "HotUpdate")
                {
                    IsHotUpdateMode = element.GetAttribute("isHotUpdate") == "true";
                }
                else if (node.Name == "Debug")
                {
                    IsDebugInfo = element.GetAttribute("isDebugInfo") == "true";
                    IsDebugWarn = element.GetAttribute("isDebugWarn") == "true";
                    IsDebugError = element.GetAttribute("isDebugError") == "true";
                }
                else if (node.Name == "Server")
                {
                    ServerUrl = element.GetAttribute("url");
                    PkgUpdateUrl = element.GetAttribute("pkgUpdateUrl");
                }
                else if (node.Name == "Version")
                {
                    ShowVersion = element.GetAttribute("showVersion");
                    PkgVersion = int.Parse(element.GetAttribute("pkgVersion"));
                    VersionCodeFile = element.GetAttribute("versionCodeFile");
                    VersionDataFile = element.GetAttribute("versionDataFile");
                    VersionPkgFile = element.GetAttribute("versionPkgFile");
                }
                else if(node.Name == "Games")
                {
                    foreach (XmlElement child in node.ChildNodes)
                    {
                        GameCfg cfg = new GameCfg(child);
                        _mapGame.Add(cfg.id, cfg);
                    }
                }
            }
        }

        public static GameCfg GetGameCfg(int id)
        {
            GameCfg cfg;
            _mapGame.TryGetValue(id,out cfg);
            if(cfg == null)
            {
                CLog.LogError("can not find gameId="+id+" GameCfg");
            }
            return cfg;
        }

        public static List<GameCfg> GetGames()
        {
            return _mapGame.Values.ToList();
        }

        public static void Save()
        {
            TextAsset txt = Resources.Load<TextAsset>(GameSetting);
            string files = txt.text;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(files);
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                XmlElement element = (XmlElement)node;
                if (node.Name == "Resource")
                {
                    element.SetAttribute("isResourcesLoadMode", IsResourceLoadMode.ToString().ToLower());
                }
                else if (node.Name == "HotUpdate")
                {
                    element.SetAttribute("isHotUpdate", IsHotUpdateMode.ToString().ToLower());
                }
                else if (node.Name == "Debug")
                {
                    element.SetAttribute("isDebugInfo", IsDebugInfo.ToString().ToLower());
                    element.SetAttribute("isDebugWarn", IsDebugWarn.ToString().ToLower());
                    element.SetAttribute("isDebugError", IsDebugError.ToString().ToLower());
                }
                else if (node.Name == "Server")
                {
                    element.SetAttribute("url", ServerUrl);
                    element.SetAttribute("pkgUpdateUrl", PkgUpdateUrl);
                }
                else if (node.Name == "Version")
                {
                    element.SetAttribute("showVersion", ShowVersion);
                    element.SetAttribute("pkgVersion", PkgVersion.ToString());
                    element.SetAttribute("versionCodeFile",VersionCodeFile);
                    element.SetAttribute("versionDataFile", VersionDataFile);
                    element.SetAttribute("versionPkgFile", VersionPkgFile);
                }
            }
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.OmitXmlDeclaration = true;
            setting.Encoding = Encoding.ASCII;

            string path = Application.dataPath + "/Resources/" + GameSetting + ".xml";
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            XmlWriter write = XmlWriter.Create(path, setting);
            xmlDoc.Save(write);
            write.Close();
        }
    }
}

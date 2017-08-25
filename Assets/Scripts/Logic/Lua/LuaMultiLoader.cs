using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public class LuaMultiLoader : LuaFileUtils
    {
        public int GameId;
        private static int index = 1;
        public LuaMultiLoader()
        {
            GameId = index++;
            instance = this;
            beZip = false;
        }

        //不同游戏使用的路径是不一样的
        public virtual void BindGame(int id)
        {
            //GameId = id;
        }

//        public override byte[] ReadFile(string fileName)
//        {
//            //Debug.Log("Load file:" + fileName);

//            if (string.IsNullOrEmpty(fileName))
//            {
//                Debug.LogError("[ERROR] Lua file name error, Is null or empty !");
//                return null;
//            }

//            if (!beZip)
//            {
//                if (Application.isEditor)
//                {
//                    string path = FindFile(fileName);
//                    //string path = GetFullPathFileName(fileName);
//                    byte[] str = null;

//                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
//                    {
//#if !UNITY_WEBPLAYER
//                        str = File.ReadAllBytes(path);
//#else
//                        throw new LuaException("can't run in web platform, please switch to other platform");
//#endif
//                    }
//                    return str;
//                }
//                else
//                {
//                    //其他情况手机、或pc包情况下（resource模式）特殊处理,需要预加载所有的Lua文件
//                    if (fileName.EndsWith(".lua"))
//                    {
//                        fileName = fileName.Substring(0, fileName.Length - 4);
//                    }
//                    if (!fileName.EndsWith(".bytes"))
//                    {
//                        fileName = fileName + ".bytes";
//                    }
//                    if (!fileName.StartsWith("LuaCode/"))
//                    {
//                        fileName = "LuaCode/" + fileName;
//                    }
//                    Resource res = ResourceMgr.Instance.GetResource(fileName);
//                    return res.GetBytes();
//                    //Debug.LogError("手机包和pc包中的Resource模式需要特殊处理");
//                    //return null;
//                }
//            }
//            else
//            {
//                return ReadBundleFile(fileName);
//            }
//        }

//        //从bundle读入文件
//        byte[] ReadBundleFile(string fileName)
//        {
//            if (fileName.EndsWith(".lua"))
//            {
//                fileName = fileName.Substring(0, fileName.Length - 4);
//            }
//            if (!fileName.EndsWith(".bytes"))
//            {
//                fileName = fileName + ".bytes";
//            }
//            if (!fileName.StartsWith("LuaCode/"))
//            {
//                fileName = "LuaCode/" + fileName;
//            }
//            //lua文档必须预加载
//            Resource res = ResourceMgr.Instance.GetResource(fileName);
//            TextAsset textAsset = (TextAsset)res.GetAsset(fileName);
//            byte[] buffer = null;
//            if (textAsset != null)
//            {
//                buffer = textAsset.bytes;
//            }
//            return buffer;
//        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Launch
{
    public static class CTLView
    {
        //更改UGUI中Canvas的sortingLayer,lv当前对象的level，越大显示在越前面,index当前对象在总容器中的索引，越大显示在前面
        public static void UpdateUGUILayer(GameObject go,int level,int index)
        {
            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas != null)
            {
                bool oldActive = go.activeSelf;
                if (!oldActive)
                {
                    go.SetActive(true);
                }
                canvas.overrideSorting = true;
                canvas.sortingOrder = level * 10000 + index * 1000;
                if (!oldActive)
                {
                    go.SetActive(oldActive);
                }
            }
        }

        public static void UpdateUGUISort(GameObject parent, GameObject go,int level,int index)
        {
            go.transform.SetAsLastSibling();
        }

        private static List<Transform> _list = new List<Transform>();
        public static GameObject CreateUGUIViewContainer(GameObject parent,int level)
        {
            Transform trans = parent.transform;
            Transform child = trans.FindChild(level.ToString());
            if (child == null)
            {
                GameObject go = new GameObject();
                CTLTools.AddChildToParent(go, parent, false);
                go.name = level.ToString();
                child = go.transform;
                //子容器里面的对象排序
                int count = trans.childCount;
                _list.Clear();
                for (int i = 0; i < count; i++)
                {
                    _list.Add(trans.GetChild(i));
                }
                _list.Sort(delegate(Transform a,Transform b){
                    int aValue = int.Parse(a.name);
                    int bValue = int.Parse(b.name);
                    return aValue - bValue;
                });
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].SetSiblingIndex(i);
                }
            }
            return child.gameObject;
        }

        public static void UpdateUGUISort(GameObject go,int level,int index)
        {
        }
    }
}

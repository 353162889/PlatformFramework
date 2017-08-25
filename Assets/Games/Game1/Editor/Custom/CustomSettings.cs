using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;

using BindType = ToLuaMenu.BindType;
using System.Reflection;

namespace Game1
{
    public class CustomSettings
    {
        //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
        //unity 有些类作为sealed class, 其实完全等价于静态类
        public static List<Type> GetStaticClassTypes()
        {
            var list = new List<Type> {

            };
            return list;
        }

        //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
        public static DelegateType[] GetCustomDelegateList()
        {
            var list = new DelegateType[]{

            };
            return list;
        }

        //在这里添加你要导出注册到lua的类型列表
        public static BindType[] GetCustomTypeList()
        {
            var list = new BindType[] {
                _GT(typeof(Test))
            };
            return list;
        }

        public static List<Type> GetDynamicList()
        {
            var list = new List<Type> {
            };
            return list;
        }

        //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
        //使用方法参见例子14
        public static List<Type> GetOutList()
        {
            var list = new List<Type> {
            };
            return list;
        }

        public static BindType _GT(Type t)
        {
            return new BindType(t);
        }

        public static DelegateType _DT(Type t)
        {
            return new DelegateType(t);
        }
    }
}
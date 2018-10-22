using System;
using System.Collections.Generic;
using UnityEngine;

namespace FollowMachineDll.Utility.Bounder
{
    public static class SupportedTypes
    {
        private static  Dictionary<string, TypeUtils> Types = new Dictionary<string, TypeUtils> ()
        {
#if UNITY_EDITOR
		            {
                "Int32",new TypeUtils
                {
                    Type=typeof(Int32),
                    Default = 0,
                    Convertor = s=> Int32.TryParse(s, out var i) ? i : 0,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.IntField(s,(int)o) :
                        UnityEditor.EditorGUILayout.IntField((int)o)
                }
            } ,
            {
                "Boolean",new TypeUtils
                {
                    Type=typeof(Boolean),
                    Default = false,
                    Convertor = s=> Boolean.TryParse(s, out var f) && f,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.Toggle(s,(bool)o) :
                        UnityEditor.EditorGUILayout.Toggle((bool)o)

                }
            } ,
            {
                "Single",new TypeUtils
                {
                    Type=typeof(Single),
                    Default = 0f,
                    Convertor = s=>Single.TryParse(s,out var f) ? f:0f,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.FloatField(s,(float)o):
                        UnityEditor.EditorGUILayout.FloatField((float)o)
                }
            } ,
            {
                "string",new TypeUtils
                {
                    Type=typeof(string),
                    Default = "",
                    Convertor = s=>s,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.TextField(s,(string)o):
                        UnityEditor.EditorGUILayout.TextField((string)o)
                }
            } ,
            {
                "String",new TypeUtils
                {
                    Type=typeof(String),
                    Default = "",
                    Convertor = s=>s,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.TextField(s,(String)o):
                        UnityEditor.EditorGUILayout.TextField((String)o)
                }
            } ,
            {
                "DateTime",new TypeUtils
                {
                    Type=typeof(DateTime),
                    Default = DateTime.Now,
                    Convertor = s=>DateTime.TryParse(s,out var date) ? date: DateTime.Now,
                    GUI=(s, o) => s=="" ?
                        UnityEditor.EditorGUILayout.TextField(s,o.ToString()):
                        UnityEditor.EditorGUILayout.TextField((string)o)
                }
            } ,

#endif
        };

            #region Functions
        public class TypeUtils
        {
            public Type Type;
            public object Default;
            public Func<string, object> Convertor;
            public Func<string, object, object> GUI;
            public Func<object, object> GUINoLable;
        }

        public static bool IsSupported(Type type)
        {
            return Types.ContainsKey(type.Name);
        }

        public static bool IsSupported(string typeName)
        {
            return Types.ContainsKey(typeName);

        } 
        #endregion

        public static string GUI(string lable,  string value, Type type)
        {
            CheckIsTypeSupporterd(type);

            if (value == null)
                value = Types[type.Name].Default.ToString();

            return Types[type.Name].GUI(lable, Types[type.Name].Convertor(value)).ToString();
        }
        public static string GUI(string lable,  string value, string type)
        {
            CheckIsTypeSupporterd(type);

            if (value == null)
                value = Types[type].Default.ToString();

            return Types[type].GUI(lable, Types[type].Convertor(value)).ToString();
        }

        private static void CheckIsTypeSupporterd(Type type)
        {
            if (!Types.ContainsKey(type.Name))
                throw new Exception($"Type {type.Name} not supported!");
        }

        private static void CheckIsTypeSupporterd(string type)
        {
            if (!Types.ContainsKey(type))
                throw new Exception($"Type {type} not supported!");
        }

        public static object Convert(string value, Type type)
        {
            CheckIsTypeSupporterd(type);

            return Types[type.Name].Convertor(value);
        }

        public static Type GetType(string type)
        {
            CheckIsTypeSupporterd(type);

            return Types[type].Type;
        }

        public static object GetDefault(Type type)
        {
            CheckIsTypeSupporterd(type);

            return Types[type.Name].Default;
        }

        public static object Convert(string value, string type)
        {
            CheckIsTypeSupporterd(type);

            return Types[type].Convertor(value);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FollowMachineDll.Utility.Bounder
{
    public static class SupportedTypes
    {
        public static  Dictionary<string, TypeUtils> Types = new Dictionary<string, TypeUtils>
        {
            {
                "Int32",new TypeUtils
                {
                    Type=typeof(Int32),
                    Default = 0,
                    Convertor = s=> Int32.TryParse(s, out var i) ? i : 0,
                    GUI=(s, o) => s=="" ?
                        EditorGUILayout.IntField(s,(int)o) :
                        EditorGUILayout.IntField((int)o)
                }
            } ,
            {
                "Boolean",new TypeUtils
                {
                    Type=typeof(Boolean),
                    Default = false,
                    Convertor = s=> Boolean.TryParse(s, out var f) && f,
                    GUI=(s, o) => s=="" ? 
                        EditorGUILayout.Toggle(s,(bool)o) :
                        EditorGUILayout.Toggle((bool)o) 

                }
            } ,
            {
                "Single",new TypeUtils
                {
                    Type=typeof(Single),
                    Default = 0f,
                    Convertor = s=>Single.TryParse(s,out var f) ? f:0f,
                    GUI=(s, o) => s=="" ? 
                        EditorGUILayout.FloatField(s,(float)o):
                        EditorGUILayout.FloatField((float)o)
                }
            } ,
            {
                "string",new TypeUtils
                {
                    Type=typeof(string),
                    Default = "",
                    Convertor = s=>s,
                    GUI=(s, o) => s=="" ? 
                        EditorGUILayout.TextField(s,(string)o):
                        EditorGUILayout.TextField((string)o)
                }
            } ,
            {
                "String",new TypeUtils
                {
                    Type=typeof(String),
                    Default = "",
                    Convertor = s=>s,
                    GUI=(s, o) => s=="" ? 
                        EditorGUILayout.TextField(s,(String)o):
                        EditorGUILayout.TextField((String)o)
                }
            } ,
            {
                "DateTime",new TypeUtils
                {
                    Type=typeof(DateTime),
                    Default = DateTime.Now,
                    Convertor = s=>DateTime.TryParse(s,out var date) ? date: DateTime.Now,
                    GUI=(s, o) => s=="" ? 
                        EditorGUILayout.TextField(s,o.ToString()):
                        EditorGUILayout.TextField((string)o)
                }
            } ,
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

        public static TypeUtils Get(string typeName) => Types[typeName];
        public static TypeUtils Get(Type type) => Types[type.Name];

        internal static bool IsSupported(string typeName)
        {
            return Types.ContainsKey(typeName);

        } 
        #endregion
    }
}

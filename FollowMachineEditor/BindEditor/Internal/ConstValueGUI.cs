using System;
using System.Collections.Generic;
using Bind.Internal;
using FollowMachineEditor.CustomInspectors;
using UnityEditor;
using UnityEngine;

namespace BindEditor.Internal
{
    internal static class ConstValueGUI
    {
        private static Dictionary<Type, Func<string, object, object>> _guiDictionary = new Dictionary<Type, Func<string, object, object>>
        {
            {typeof(bool), (string lable, object o) =>
                {
                    return EditorGUILayout.Toggle(lable, (bool) o);
                }
            },
            {typeof(int), (string lable, object o) =>
                {
                    return EditorGUILayout.IntField(lable, (int) o);
                }
            },
            {typeof(float), (string lable, object o) =>
                {
                    return EditorGUILayout.FloatField(lable, (float) o);
                }
            },
            { typeof(string), (string lable, object o) =>
                {

                    return GUIUtil.LanguageField(lable, (string) o);
                }
            },
            { typeof(Color), (string lable, object o) =>
                {
                    return EditorGUILayout.ColorField(lable, (Color) o);
                }
            },
            { typeof(Vector3), (string lable, object o) =>
                {
                    Vector3 v = (Vector3) o ;
                    GUILayout.BeginVertical();
                    v.x = EditorGUILayout.FloatField("X", v.x);
                    v.y = EditorGUILayout.FloatField("Y", v.y);
                    v.z = EditorGUILayout.FloatField("Z", v.z);
                    GUILayout.EndVertical();
                    return v;
                }
            },
        };

        public static void OnGUI(this ConstValue constValue, string lable)
        {
            var type = constValue.Type.Value;

            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                constValue.Value= EditorGUILayout.ObjectField(lable, (UnityEngine.Object)constValue.Value, type, true);

            }
            else if (_guiDictionary.ContainsKey(type))
            {
                constValue.Value = _guiDictionary[type](lable, constValue.Value);
            }
            else
                throw new Exception($"Type ({type.Name}) is not valid const type!");
        }

        public static bool IsSupported(Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                return true;

            return _guiDictionary.ContainsKey(type);
        }
    }
}
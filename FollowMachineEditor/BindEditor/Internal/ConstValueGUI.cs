using System;
using System.Collections.Generic;
using Bind.Internal;
using FollowMachineDll.DataTypes;
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
                    var lText = (LText) o;

                    GUILayout.BeginHorizontal();

                    if (lText.IsConst)
                    {
                        lText.Text = EditorGUILayout.TextField(lable, lText.ToString());
                        if (GUILayout.Button("L", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                            lText.IsConst = false;
                    }
                    else
                    {
                        lText.Text = GUIUtil.LanguageField(lable, lText.Text);
                        if (GUILayout.Button("C", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                            lText.IsConst = true;
                    }
                    GUILayout.EndHorizontal();
                    return lText;
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
            { typeof(LText), (string lable, object o) =>
                {
                    var lText = (LText) o;

                    GUILayout.BeginHorizontal();

                    if (lText.IsConst)
                    {
                        lText.Text = EditorGUILayout.TextField(lable, lText.ToString());
                        if (GUILayout.Button("L", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                            lText.IsConst = false;
                    }
                    else
                    {
                        lText.Text = GUIUtil.LanguageField(lable, lText.Text);
                        if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                            lText.IsConst = true;
                    }

                    GUILayout.EndHorizontal();
                    return lText;
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
                constValue.Value = _guiDictionary[type](lable, constValue.RawValue);
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
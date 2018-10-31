using System;
using System.Collections.Generic;
using FMachine.Editor;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.EditorObjectMapper;
using FollowMachineEditor.Windows.Bounder;
using MgsCommonLib.UI;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.CustomInspectors
{
    public static class GUIUtil
    {

        public static Node TargetNode { get; set; }

        public static bool TextFieldInBox(string lable, ref string text)
        {
            bool b = false;
            var s = text;
            DrawInBox(lable, () =>
            {
                b = TextField("", ref s);
            });
            text = s;
            return b;
        }

        public static bool TextField(string lable, ref string text)
        {
            string s;

            if (lable == "")
                s = GUILayout.TextField(text);
            else
                s = GUILayout.TextField(lable, text);

            if (s == text)
                return false;

            Undo.RecordObject(TargetNode, "Change Info");
            text = s;
            return true;

        }
        public static bool TextFieldVertical(string lable, ref string text)
        {
            if (lable != "")
                GUILayout.Label(lable);

            var s = GUILayout.TextField(text);

            if (s == text)
                return false;

            Undo.RecordObject(TargetNode, "Change Info");
            text = s;
            return true;

        }

        public static void DrawInBox(string title, Action body)
        {
            StartBox();
            {
                if (title != "")
                    BoldLable(title);

                body();
            }
            EndBox();
            GUILayout.Space(5);
        }

        public static void BoldLable(string title)
        {
            GUILayout.Label(title, (GUIStyle)"BoldLabel");
        }

        public static void EndBox()
        {
            GUILayout.EndVertical();
        }

        public static void StartBox()
        {
            GUILayout.BeginVertical((GUIStyle)"box");
        }

        public static void DisplayError(string errorMsg)
        {
            DrawInBox("Error :", () => GUILayout.Box(errorMsg, (GUIStyle)"ErrorStyle"));
        }

        public static void DrawInBox(Action action)
        {
            DrawInBox("",action);
        }

        public static  bool ButtonWithBoldLable(string lable, string buttonText)
        {
            GUILayout.BeginHorizontal();

            BoldLable(lable);

            GUILayout.FlexibleSpace();

            bool flag = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();
            return flag;
        }
        public static bool ButtonWithLable(string lable, string buttonText)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(lable);

            GUILayout.FlexibleSpace();

            bool flag = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();
            return flag;
        }

        public static bool PopupField(ref string text, List<string> textList)
        {
            var indexOf = textList.IndexOf(text);
            if (indexOf == -1)
                indexOf = textList.Count - 1;
            indexOf=EditorGUILayout.Popup(indexOf, textList.ToArray());
            if (textList[indexOf] != text)
            {
                Undo.RecordObject(TargetNode,"Change in node");
                text = textList[indexOf];
                return true;
            }

            return false;
        }

        public static bool PopupFieldInBox(string lable, ref string text, List<string> textList)
        {
            var s = text;
            bool flag = false;
            DrawInBox(lable, () => flag=PopupField(ref s, textList));
            text = s;
            return flag;
        }

        public static void BoundField(BoundData boundData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(boundData.Lable);

            if (boundData.BoundMethod == BoundMethodEnum.GameObject)
                GUILayout.Label(boundData.Value);
            else
                boundData.Value =
                    SupportedTypes.GUI("", boundData.Value, boundData.TypeName);

            if (GUILayout.Button("B", GUILayout.Width(20)))
            {
                var menu = new GenericMenu();

                if (boundData.BoundMethod==BoundMethodEnum.GameObject)
                {
                    menu.AddItem(new GUIContent("Edit"), false, () => BounderWindow.EditBound(
                        boundData.BoundGameObject,
                        boundData.Value,
                        typeof(string),
                        (o, s) =>
                        {
                            boundData.BoundGameObject = o;
                            boundData.Value = s;
                        }));

                    menu.AddItem(new GUIContent("Unbound"), false, () =>
                    {
                        Undo.RecordObject(TargetNode, "Change Bound");
                        boundData.BoundMethod = BoundMethodEnum.Constant;
                        boundData.Value = "";
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Bound"), false, () => BounderWindow.EditBound(
                        boundData.BoundGameObject,
                        boundData.Value,
                        typeof(string),
                        (o, s) =>
                        {
                            boundData.BoundGameObject = o;
                            boundData.Value = s;
                            boundData.BoundMethod = BoundMethodEnum.GameObject;
                        }));
                }

                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();

        }

        public static void RefreshWindow()
        {
            var fmWindow = EditorWindow.GetWindow<FMWindow>();
            fmWindow.Repaint();
        }

        public static void GetProgressBar(ProgressBarInfo pbInfo)
        {
            StartBox();

            GUILayout.Label("Progressbar :", (GUIStyle)"BoldLabel");

            pbInfo.ProgressbarWindow = (MgsProgressWindow)EditorGUILayout.ObjectField(
                new GUIContent("ProgressbarWindow"),
                pbInfo.ProgressbarWindow,
                typeof(MgsProgressWindow), true);

            if (pbInfo.ProgressbarWindow != null)
            {
                EditorTools.Instance.LanguageField(TargetNode, "Progressbar Message", ref pbInfo.ProgressbarMessage);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Display method:");
                    pbInfo.ProgressbarShow = GUILayout.Toggle(pbInfo.ProgressbarShow, "Show");
                    pbInfo.ProgressbarHide = GUILayout.Toggle(pbInfo.ProgressbarHide, "Hide");
                }
                GUILayout.EndHorizontal();

            }
            EndBox();
        }
    }
}

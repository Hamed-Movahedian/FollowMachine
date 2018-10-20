using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Editor;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.Windows.Bounder;
using FollowMachineEditor.Windows.FollowMachineInspector;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.CustomInspectors
{
    public abstract class CustomInspector
    {
        private static readonly Dictionary<Type,CustomInspector> Dic = new Dictionary<Type, CustomInspector>
        {
            {typeof(ServerNode),new ServerNodeEditor() },
        };

        public static void DrawInspector(Node node)
        {
            var nodeType = node.GetType();

            if (Dic.ContainsKey(nodeType))
            {
                var inspector = Dic[nodeType];
                inspector.TargetNode = node;
                inspector.OnInspector(node);
            }
            else
                node.OnInspector();
        }

        public Node TargetNode { get; set; }

        public virtual void OnInspector(Node node)
        {
            if (TextFieldInBox("Info :", ref node.Info))
            {
                node.name = node.Info + " (" + node.GetType().Name + ")";
            }
            
            GUILayout.Space(5);
        }

        protected bool TextFieldInBox(string lable, ref string text)
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

        protected bool TextField(string lable, ref string text)
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
        protected bool TextFieldVertical(string lable, ref string text)
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

        protected void DrawInBox(string title, Action body)
        {
            StartBox();
            {
                if (title != "")
                    BoldLable(title);

                body();
            }
            EndBox();
        }

        protected static void BoldLable(string title)
        {
            GUILayout.Label(title, (GUIStyle)"BoldLabel");
        }

        protected static void EndBox()
        {
            GUILayout.EndVertical();
        }

        protected static void StartBox()
        {
            GUILayout.BeginVertical((GUIStyle)"box");
        }

        protected void DisplayError(string errorMsg)
        {
            DrawInBox("Error :", () =>
            {
                GUILayout.Box(errorMsg, (GUIStyle)"ErrorStyle");
            });
        }

        protected void DrawInBox(Action action)
        {
            DrawInBox("",action);
        }

        protected static bool ButtonWithBoldLable(string lable, string buttonText)
        {
            GUILayout.BeginHorizontal();

            BoldLable(lable);

            GUILayout.FlexibleSpace();

            bool flag = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();
            return flag;
        }
        protected static bool ButtonWithLable(string lable, string buttonText)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(lable);

            GUILayout.FlexibleSpace();

            bool flag = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();
            return flag;
        }

        protected bool PopupField(ref string text, List<string> textList)
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

        protected bool PopupFieldInBox(string lable, ref string text, List<string> textList)
        {
            var s = text;
            bool flag = false;
            DrawInBox(lable, () => flag=PopupField(ref s, textList));
            text = s;
            return flag;
        }

        protected void BoundField(BoundData boundData)
        {
            GUILayout.BeginHorizontal();

            if (boundData.Type==null)
                boundData.IsBound = true;

            if (boundData.IsBound)
                GUILayout.Label(boundData.Value);
            else
            {
                var typeUtils = SupportedTypes.Types[boundData.Type.Name];

                boundData.Value=
                    typeUtils.GUI("", typeUtils.Convertor(boundData.Value)).ToString();
            }

            if (GUILayout.Button("B", GUILayout.Width(20)))
            {
                var menu = new GenericMenu();

                if (boundData.IsBound)
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
                        boundData.IsBound = false;
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
                            boundData.IsBound = true;
                        }));
                }

                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();

        }

        protected void RefreshWindow()
        {
            var fmWindow = EditorWindow.GetWindow<FMWindow>();
            fmWindow.Repaint();
        }
    }
}

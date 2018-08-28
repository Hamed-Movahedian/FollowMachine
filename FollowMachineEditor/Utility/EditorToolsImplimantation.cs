using System;
using System.Collections.Generic;
using System.Reflection;
using FMachine;
using FMachine.Editor;
using FMachine.SettingScripts;
using FMachine.Shapes;
using FollowMachineDll.Utility;
using MgsCommonLib.Theme;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FollowMachineEditor.Utility
{
    public class EditorToolsImplimantation : EditorTools
    {
        private readonly FMWindow _window;
        private readonly EdgeEditor _editorEdge;

        #region Constructor

        public EditorToolsImplimantation(FMWindow window)
        {
            _window = window;
            _editorEdge = new EdgeEditor(window);

        }

        #endregion

        #region PropertyField

        public override bool PropertyField(Object obj, string propertyPath)
        {
            SerializedObject sObj = GetSerializedObject(obj);
            EditorGUILayout.PropertyField(sObj.FindProperty(propertyPath), true);

            return sObj.ApplyModifiedProperties();
        }

        private SerializedObject GetSerializedObject(Object o)
        {
            return new SerializedObject(o);
        }


        #endregion

        public override int Popup(string label, int index, string[] displayedOptions)
        {
            return EditorGUILayout.Popup(label, index, displayedOptions);
        }

        public override void Undo_RecordObject(Object objectToUndo, string name)
        {
            Undo.RecordObject(objectToUndo, name);
        }

        #region GetParameter

        public override string GetParameter(Object objectToUndo, ParameterInfo parameterInfo, string valueString)
        {
            if (parameterInfo.ParameterType == typeof(int))
            {
                return IntFieldAsString(objectToUndo, parameterInfo.Name, valueString);
            }
            else if (parameterInfo.ParameterType == typeof(float))
            {
                return FloatFieldAsString(objectToUndo, parameterInfo.Name, valueString);
            }
            else if (parameterInfo.ParameterType == typeof(bool))
            {
                return BoolFieldAsString(objectToUndo, parameterInfo.Name, valueString);
            }
            else if (parameterInfo.ParameterType == typeof(string))
            {
                return StringField(objectToUndo, parameterInfo.Name, valueString);
            }
            else
            {
                EditorGUILayout.LabelField(parameterInfo.Name, "Not supported type");
                return null;
            }
        }

        #endregion

        #region GetFieldsAsString

        private string BoolFieldAsString(Object objectToUndo, string lable, string valueString)
        {
            if (!bool.TryParse(valueString, out var value))
                value = false;

            return BoolField(objectToUndo, lable, ref value).ToString();
        }

        public string FloatFieldAsString(Object objectToUndo, string lable, string valueString)
        {
            if (!float.TryParse(valueString, out var value))
                value = 0;

            return FloatField(objectToUndo, lable, ref value).ToString();
        }

        public string IntFieldAsString(Object objectToUndo, string lable, string valueString)
        {
            if (!int.TryParse(valueString, out var value))
                value = 0;

            return IntField(objectToUndo, lable, ref value).ToString();
        }


        #endregion

        #region GetFields

        public int IntField(Object objectToUndo, string lable, ref int value)
        {
            var newValue = EditorGUILayout.IntField(lable, value);
            if (newValue != value)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                value = newValue;
            }

            return newValue;
        }
        public float FloatField(Object objectToUndo, string lable, ref float value)
        {
            var newValue = EditorGUILayout.FloatField(lable, value);
            if (newValue != value)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                value = newValue;
            }

            return newValue;
        }
        public bool BoolField(Object objectToUndo, string lable, ref bool value)
        {

            var newValue = EditorGUILayout.Toggle(lable, value);
            if (newValue != value)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                value = newValue;
            }

            return newValue;
        }
        private string StringField(Object objectToUndo, string lable, string value)
        {
            var newValue = EditorGUILayout.TextField(lable, value);
            if (newValue != value)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                value = newValue;
            }

            return value;
        }

        #endregion

        #region Style

        public override GUIStyle TintStyle(GUIStyle baseStyle, Color tintColor)
        {
            var style = new GUIStyle(baseStyle)
            {
                normal = { background = TintTexture(baseStyle.normal.background, tintColor) },
                hover = { background = TintTexture(baseStyle.hover.background, tintColor) },
                focused = { background = TintTexture(baseStyle.focused.background, tintColor) },
                active = { background = TintTexture(baseStyle.active.background, tintColor) }
            };

            return style;
        }

        private static Texture2D TintTexture(Texture2D baseTexture, Color tintColor)
        {
            if (baseTexture == null)
                return null;

            var texture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);
            var pixels = baseTexture.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] *= tintColor;
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }


        #endregion

        #region Draw

        public override void DrawBox(GUIStyle style, Rect rect, GUIContent guiContent, bool b, bool b1, bool b2, bool b3)
        {
            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, guiContent, b, b1, b2, b3);

        }

        #endregion

        public override ScriptableObject GetAsset(string name, Type type)
        {
            return SettingController.Instance.GetAsset(name, type);
        }

        #region Edge

        public override IMouseInteractable IsMouseOverEdge(Edge edge, Vector2 mousePos)
        {
            return _editorEdge.IsMouseOverEdge(edge, mousePos);
        }

        public override void DrawBezierEdge(Vector2 p1, Vector2 p2, Color forgroundColor, Color backColor, float thickness)
        {
            _editorEdge.DrawBezierEdge(p1, p2, forgroundColor, backColor, thickness);
        }

        public override void DrawEdge(Edge edge)
        {
            _editorEdge.DrawBezierEdge(edge);
        }

        #endregion

        public override bool DisplayDialog(string title, string message, string ok, string cancel)
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }

        public override bool LanguageField(Object objectToUndo, string lable, ref string entryName)
        {
            var languagePack = ThemeManager.Instance.LanguagePack;

            if (languagePack == null)
                throw new Exception("Language pack not set!!");

            var entryNameList = languagePack.GetEntryNameList();

            var index = entryNameList.IndexOf(entryName);

            var newIndex = EditorGUILayout.Popup(lable, index, entryNameList.ToArray());

            if (index != newIndex)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                entryName = entryNameList[newIndex];
                return true;
            }

            return false;
        }

        public override void IconField(Object objectToUndo, string lable, ref string entryName)
        {
            var iconPack = ThemeManager.Instance.IconPack;

            if (iconPack == null)
                throw new Exception("Icon pack not set!!");

            var entryNameList = iconPack.GetEntryNameList();

            var index = entryNameList.IndexOf(entryName);

            var newIndex = EditorGUILayout.Popup(lable, index, entryNameList.ToArray());

            if (index != newIndex)
            {
                Undo.RecordObject(objectToUndo, "Change in " + lable);
                entryName = entryNameList[newIndex];
            }

        }

        public override void Space()
        {
            EditorGUILayout.Space();
        }

        public override bool LanguageListField(Object objectToUndo, string lable, List<string> entryNameList)
        {
            bool hasChanged = false;

            GUILayout.Label(lable);
            int size = entryNameList.Count;

            size = EditorGUILayout.IntField("Size", size);

            if (size != entryNameList.Count)
                hasChanged = true;

            while (size < entryNameList.Count)
                entryNameList.RemoveAt(entryNameList.Count - 1);

            while (size > entryNameList.Count)
                if (entryNameList.Count == 0)
                    entryNameList.Add("");
                else
                    entryNameList.Add(entryNameList[entryNameList.Count - 1]);

            for (int i = 0; i < entryNameList.Count; i++)
            {
                var entryName = entryNameList[i];

                if (LanguageField(objectToUndo, lable + " " + i, ref entryName))
                    hasChanged = true;

                entryNameList[i] = entryName;
            }

            return hasChanged;
        }

        public override void DrawTexture(Rect rect, Texture2D texture, Color color, string text = "")
        {
            Texture2D background = GUI.skin.box.normal.background;
            Color backgroundColor = GUI.backgroundColor;

            GUI.skin.box.normal.background = texture;
            GUI.backgroundColor = color;
            GUI.Box(rect, text);

            GUI.skin.box.normal.background = background;
            GUI.backgroundColor = backgroundColor;
        }

        public override void DrawTexture(Rect rect, Texture2D texture, GUIStyle style, Color color, string text = "")
        {
            Texture2D background = style.normal.background;
            Color backgroundColor = GUI.backgroundColor;

            GUI.backgroundColor = color;
            style.normal.background = texture;
            GUI.Box(rect, text, style);

            style.normal.background = background;
            GUI.backgroundColor = backgroundColor;
        }

        public override void SetVerticalResizeMouseCursor(Rect rect)
        {
            rect.xMin = rect.xMax - 10;
            EditorGUIUtility.AddCursorRect(_window.Canvas.WindowRect, MouseCursor.ResizeVertical);
        }

        public override bool TextField(string lable, ref string field)
        {
            var textField = EditorGUILayout.TextField(lable, field);
            if (textField != field)
            {
                field = textField;
                return true;
            }

            return false;
        }

        public override void OpenScript(MonoBehaviour monoBehaviour)
        {
            MonoScript monoScript = MonoScript.FromMonoBehaviour(monoBehaviour);
            AssetDatabase.OpenAsset(monoScript);
        }

        public override void AddFollowMachine(FollowMachine followmachine)
        {
            _window.GraphStack.Add(followmachine);
        }

    }
}

using FMachine.Editor;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.SettingScripts;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.Windows.EntryPoints
{
    public class EntryPointsWindow : EditorWindow
    {
        [SerializeField]
        private FMEntryPointsSetting _setting;

        private Vector2 _scrollPos;

        [MenuItem("Window/FollowMachine/EntryPoints")]
        private static void OnCreateWindow()
        {
            var window = GetWindow<EntryPointsWindow>();
            window.Show();
        }


        public void OnEnable()
        {
            if (_setting == null)
                _setting = (FMEntryPointsSetting)SettingController.Instance.GetAsset("EntryPointsSetting", typeof(FMEntryPointsSetting));
            titleContent = new GUIContent("EntryPoints", _setting.Icon);
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            var entryPointNodes = FindObjectsOfType<EntryPointNode>();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.Space(10);
            foreach (var entryPointNode in entryPointNodes)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(
                    entryPointNode.Graph.name,
                    _setting.Style);

                GUILayout.FlexibleSpace();

                var text = EditorGUILayout.TextField(
                    entryPointNode.Info,
                    GUILayout.ExpandWidth(true));
                if (text != entryPointNode.Info)
                {
                    Undo.RecordObject(entryPointNode, "Change in entry point");
                    entryPointNode.Info = text;
                }


                var toggle = GUILayout.Toggle(entryPointNode.Active, "");
                if (toggle != entryPointNode.Active)
                {
                    Undo.RecordObject(entryPointNode, "Change in entry point");
                    entryPointNode.Active = toggle;
                }

                if (GUILayout.Button(new GUIContent(_setting.FindIcon), EditorStyles.miniButton))
                {
                    Selection.activeGameObject = entryPointNode.Graph.gameObject;
                    entryPointNode.Graph.Editor().DeselectAll();
                    entryPointNode.Editor().Select();
                    var fmWindow = EditorWindow.GetWindow<FMWindow>();
                    if (fmWindow != null)
                    {
                        fmWindow.GraphStack.OnSelectinChange();
                        fmWindow.Canvas.CordinationSystem.Focus(true, false);
                    }

                }

                if (GUILayout.Button(new GUIContent(_setting.DeleteIcon),EditorStyles.miniButton))
                {
                    entryPointNode.Graph.Editor().DeselectAll();
                    entryPointNode.Editor().Select();
                    entryPointNode.Graph.Editor().DeleteSelection();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
            GUILayout.EndScrollView();
        }

    }
}

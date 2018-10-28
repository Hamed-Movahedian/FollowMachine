using FMachine.Editor;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.SettingScripts;
using FollowMachineEditor.CustomInspectors;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.Windows.FollowMachineInspector
{
    public class FollowMachineInspector : EditorWindow
    {
        [SerializeField]
        private FMInspectorSetting _setting;

        private FMWindow _fmWindow;
        private Vector2 _scrollPos;


        [MenuItem("Window/FollowMachine/Inspector")]
        private static void OnCreateWindow()
        {
            FollowMachineInspector window = GetWindow<FollowMachineInspector>();
            window.Show();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
        public void OnEnable()
        {
            if (_setting == null)
                _setting = (FMInspectorSetting)SettingController.Instance.GetAsset("InspectorSetting", typeof(FMInspectorSetting));
            titleContent = new GUIContent("Inspector", _setting.Icon);
        }
        private void OnGUI()
        {

            if (_fmWindow == null)
                _fmWindow = GetWindow<FMWindow>();

            Node selectedNode = null;

            var graph = _fmWindow.GraphStack.CurrentGraph;

            if (!graph)
                return;

            selectedNode = graph.Editor().SelectedNode;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (selectedNode)
            {
                selectedNode.Editor().OnInspector();
            }
            else
            {
                var selectedGroup = graph.Editor().SelectedGroup;
                if (selectedGroup)
                    selectedGroup.Editor().OnInspector();
            }

            EditorGUILayout.EndScrollView();
        }

    }
}

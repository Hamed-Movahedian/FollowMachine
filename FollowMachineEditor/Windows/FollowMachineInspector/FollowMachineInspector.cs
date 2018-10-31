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
            _setting.Style = (GUIStyle)"box";
            //titleContent = new GUIContent("Inspector", _setting.Icon);
            titleContent = GUIContent.none;

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
            GUILayout.BeginVertical(_setting.Style);
            EditorGUILayout.Space();

            //_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

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


            // Set window size to fit content
            if (Event.current.type == EventType.Repaint)
            {
                var lastRect = GUILayoutUtility.GetLastRect();

                var height = lastRect.y + lastRect.height + 5;

                minSize = maxSize = new Vector2(300, height);
            }

            GUILayout.EndVertical();
        }

        public static void ShowInMousePos(Vector2 screenPoint)
        {
            var mousePosition =
                Event.current.mousePosition +
                GetWindow<FMWindow>().position.position;

            CloseAll();

            var window = ScriptableObject.CreateInstance<FollowMachineInspector>();

            var windowPosition = window.position;
            windowPosition.position = screenPoint;
            window.position = windowPosition;
            window.ShowPopup();
        }


        public static void CloseAll()
        {
            foreach (FollowMachineInspector inspector in Resources.FindObjectsOfTypeAll<FollowMachineInspector>())
            {
                inspector.Close();
            }

        }
    }
}

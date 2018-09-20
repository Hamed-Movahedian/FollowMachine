using FMachine.SettingScripts;
using FollowMachineDll.SettingScripts;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FollowMachineEditor.Windows.FollowMachineExplorer
{
    public class FollowMachineTreeViewWindow : EditorWindow
    {
        #region FollowMachineTreeView

        private FollowMachineTreeView _followMachineTreeView;

        public FollowMachineTreeView FollowMachineTreeView =>
            _followMachineTreeView ?? (_followMachineTreeView = new FollowMachineTreeView(TreeViewState));

        #endregion

        #region TreeViewState

        [SerializeField]
        private TreeViewState _treeViewState;

        public TreeViewState TreeViewState =>
            _treeViewState ?? (_treeViewState = new TreeViewState());

        #endregion

        void OnEnable()
        {
            var setting = (FMExpelorerSetting) SettingController.Instance.GetAsset("ExpelorerSetting", typeof(FMExpelorerSetting));

            titleContent = new GUIContent("Explorer", setting.Icon);
        }

        void OnGUI()
        {
            FollowMachineTreeView.OnGUI(new Rect(0,0,position.width,position.height));
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        [MenuItem("Window/FollowMachine/Explorer")]
        static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<FollowMachineTreeViewWindow>();
    
            window.Show();
        }

    }
}

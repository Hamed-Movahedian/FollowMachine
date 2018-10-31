using FMachine.SettingScripts;
using FollowMachineDll.SettingScripts;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FollowMachineEditor.Windows.FollowMachineExplorer
{
    public class FollowMachineTreeViewWindow : PopupWindowContent
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
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 600);
        }

        public override void OnGUI(Rect rect)
        {
            FollowMachineTreeView.OnGUI(rect);
        }
    }
}

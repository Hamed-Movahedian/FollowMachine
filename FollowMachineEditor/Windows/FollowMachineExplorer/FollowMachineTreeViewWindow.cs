using FMachine.SettingScripts;
using FollowMachineDll.SettingScripts;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FollowMachineEditor.Windows.FollowMachineExplorer
{
    public class FollowMachineTreeViewWindow : EditorWindow
    {
        [SerializeField] TreeViewState _treeViewState;

        private FollowMachineTreeView _followMachineTreeView;
        private static FMExpelorerSetting _setting;

        void OnEnable()
        {
            if(_treeViewState==null)
                _treeViewState=new TreeViewState();

            _followMachineTreeView=new FollowMachineTreeView(_treeViewState);
            if (_setting == null)
                _setting = (FMExpelorerSetting)SettingController.Instance.GetAsset("ExpelorerSetting", typeof(FMExpelorerSetting));
            titleContent = new GUIContent("Explorer", _setting.Icon);
        }

        void OnGUI()
        {
            _followMachineTreeView.OnGUI(new Rect(0,0,position.width,position.height));
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

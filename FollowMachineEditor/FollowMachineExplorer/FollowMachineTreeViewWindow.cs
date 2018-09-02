using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FollowMachineEditor.FollowMachineExplorer
{
    public class FollowMachineTreeViewWindow : EditorWindow
    {
        [SerializeField] TreeViewState _treeViewState;

        private FollowMachineTreeView _followMachineTreeView;

        void OnEnable()
        {
            if(_treeViewState==null)
                _treeViewState=new TreeViewState();

            _followMachineTreeView=new FollowMachineTreeView(_treeViewState);
            
        }

        void OnGUI()
        {
            _followMachineTreeView.OnGUI(new Rect(0,0,position.width,position.height));
        }

        [MenuItem("Window/FollowMachine/FM Explorer")]
        static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<FollowMachineTreeViewWindow>();
            window.titleContent = new GUIContent("FM Explorer");
            window.Show();
        }

    }
}

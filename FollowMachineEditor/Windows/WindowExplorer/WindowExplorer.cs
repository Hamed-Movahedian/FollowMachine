using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.SettingScripts;
using FollowMachineDll.SettingScripts;
using FollowMachineEditor.Windows.FollowMachineExplorer;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FollowMachineEditor.Windows.WindowExplorer
{
    public class WindowExplorer : EditorWindow
    {
        [SerializeField] TreeViewState _treeViewState;
        [SerializeField]
        private static WindowExpelorerSetting _setting;

        private WindowTreeView _windowTreeView;

        void OnEnable()
        {
            if (_treeViewState == null)
                _treeViewState = new TreeViewState();

            _windowTreeView = new WindowTreeView(_treeViewState);

            if (_setting == null)
                _setting = (WindowExpelorerSetting)SettingController.Instance.GetAsset("WindowExpelorerSetting", typeof(WindowExpelorerSetting));
            titleContent = new GUIContent("Explorer", _setting.Icon);
        }

        void OnGUI()
        {
            _windowTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
        }

        [MenuItem("Window/FollowMachine/Window Explorer")]
        static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<WindowExplorer>();

            window.Show();
        }

    }
}

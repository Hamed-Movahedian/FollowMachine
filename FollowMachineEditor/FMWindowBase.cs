using FMachine.SettingScripts;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor
{
    public abstract class FMWindowBase : EditorWindow 
    {
        public FMWiondowSettings Settings;

        private bool _isInitialized = false;

        private string _errorMessage = "";

        [MenuItem("Window/FollowMachine/Follow Machine Window")]
        private static void OnCreateWindow()
        {
            FMWindow window = GetWindow<FMWindow>();
            window.Show();
        }


        public void OnEnable()
        {
            BaseInitialize();
            
        }


        public void BaseInitialize()
        {
            _isInitialized = false;

            wantsMouseMove = true;

            SettingController.Instance.Reset();
            Settings = (FMWiondowSettings) SettingController.Instance.GetAsset("WindowSetting", typeof(FMWiondowSettings));
            titleContent = new GUIContent("Follow Mech",Settings.Icon);


            Initialize();

            _isInitialized = true;
        }

        protected abstract void Initialize();

        private void OnGUI()
        {
            if (!_isInitialized)
                GUILayout.Label(_errorMessage);

            PerformOnGUI();

            if (
                Event.current.type == EventType.Layout ||
                Event.current.isMouse ||
                Event.current.isKey)
                Repaint();
        }

        protected abstract void PerformOnGUI();
    }
}
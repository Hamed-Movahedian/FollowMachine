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

        [MenuItem("Window/Follow Machine Window")]
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

            Repaint();
        }

        protected abstract void PerformOnGUI();
    }
}
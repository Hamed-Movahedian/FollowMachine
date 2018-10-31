using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "FollowMachine/InspectorSetting")]
    public class FMInspectorSetting : ScriptableObject
    {
        public Texture2D Icon;
        public GUISkin Skin;
        public GUIStyle Style;
    }
}
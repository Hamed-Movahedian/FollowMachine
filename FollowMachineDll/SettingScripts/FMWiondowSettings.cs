using UnityEngine;

namespace FMachine.SettingScripts
{
    [CreateAssetMenu(fileName = "FMWiondowSettings", menuName = "FollowMachine/WindowSettings", order = 1)]
    public class FMWiondowSettings : ScriptableObject
    {
        public float InspectorWith;
        public GUIStyle BoxSelectionStyle;
        public Texture2D Icon;
    }
}

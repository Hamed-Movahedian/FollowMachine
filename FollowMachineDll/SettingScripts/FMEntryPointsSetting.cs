using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "FollowMachine/EntryPointsSetting")]
    public class FMEntryPointsSetting : ScriptableObject
    {
        public Texture2D Icon;
        public GUISkin Skin;
        public float LableWith=100;
        public GUIStyle Style;
        public Texture2D DeleteIcon;
        public Texture2D FindIcon;
    }
}
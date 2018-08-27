using FMachine.SettingScripts;
using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "Settings/SpcificNodeSetting")]
    public class SpcificNodeSetting: ScriptableObject
    {
        public Color HeaderColor=Color.white;
        public string Title="Title";
        public Texture2D Icon;
        public Color IconColor=Color.white;
    }
}
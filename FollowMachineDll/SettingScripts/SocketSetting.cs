using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "FollowMachine/SocketSetting")]
    public class SocketSetting : ScriptableObject
    {
        public GUIStyle Style;
        public Vector2 Size;
        public float Thickness;
        public Vector2 Offset = new Vector2(10,5);
        public float Space = 5;
        public Color Color;
        public Color AutoHideColor;
        public Texture2D ConnectedTexure;
        public Texture2D DisconnectedTexure;
    }
}
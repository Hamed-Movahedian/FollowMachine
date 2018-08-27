using System;
using UnityEngine;

namespace FMachine.SettingScripts
{
    [CreateAssetMenu(menuName = "Settings/SocketSetting")]
    public class SocketSetting : ScriptableObject
    {
        public GUIStyle Style;
        public Vector2 Size;
        public float Thickness;
        public Vector2 Offset = new Vector2(10,5);
        public float Space = 5;
        public Color Color;
        public Texture2D ConnectedTexure;
        public Texture2D DisconnectedTexure;
    }
}
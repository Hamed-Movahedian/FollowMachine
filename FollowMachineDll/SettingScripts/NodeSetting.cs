﻿using System;
using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "FollowMachine/NodeSetting")]
    public class NodeSetting : ScriptableObject
    {
        public Vector2 Size=new Vector2(300,200);
        public SocketSetting InputSocketSetting;
        public SocketSetting OutputSocketSetting;


        [Header("Icon")]
        public Rect IconRect;
        public Texture2D ShowIcon;


        [Header("Glow")]
        public Color GlowNormal = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color GlowHover = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color GlowSelected = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [Header("Line")]
        public Color LineNormal = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color LineSelected = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color LineRunning = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color LineHover = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [Header("Colors")]
        public Color BodyColor;
        public Color InfoColor;


        [Header("Header")]
        public float HeaderWith;

        public SectionSetting Header;


        [Header("Info")]
        public SectionSetting Info;

        [Header("Body")]
        public SectionSetting Body;

    }

    [Serializable]
    public class SectionSetting
    {
        public Texture2D GlowTexture;
        public Texture2D LineTexture;
        public Texture2D FillTexture;
        public GUIStyle Style;
    }
}
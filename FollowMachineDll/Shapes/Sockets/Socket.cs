using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public abstract class Socket : BoxShape
    {
        public Node Node;
        protected int Index;
        public List<Edge> EdgeList = new List<Edge>();
        public Texture2D Icon; 

        public bool IsConnected { get { return EdgeList.Count > 0; } }

        public abstract SocketSetting SocketSetting { get; }

        public virtual void OnCreate(Graph graph,Node node)
        {
            Graph = graph;
            Node = node;
        }

        public override void Draw()
        {
            base.Draw();

            Rect.size = SocketSetting.Size;

            Color color = SocketSetting.Color;

            if (EdgeList.Count > 0)
                if (EdgeList[0].IsRunning)
                    color = Node.NodeSetting.LineRunning;

            if (Icon != null)
            {
                SocketSetting.Style.CalcMinMaxWidth(new GUIContent(FilterName(Info)),out var min,out var max);

                var iconRect = Rect;

                iconRect.x -= max;

                GUI.DrawTexture(iconRect,Icon);
            }
            EditorTools.Instance.DrawTexture(
                Rect,
                EdgeList.Count == 0 ? SocketSetting.DisconnectedTexure : SocketSetting.ConnectedTexure,
                SocketSetting.Style,
                color,
                FilterName(Info));

        }


        public override void Delete()
        {
            Disconnect();

            DestroyImmediate(this);
        }

        public void Disconnect()
        {
            var edges = new List<Edge>(EdgeList);
            foreach (var edge in edges)
                edge.Delete();
            EdgeList.Clear();
        }
    }
}
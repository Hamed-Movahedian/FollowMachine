using System.Collections.Generic;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorSockets
{
    public abstract class ESocket : EBoxShape
    {
        public Node Node
        {
            get => _socket.Node;
            set => _socket.Node = value;
        }

        public List<Edge> EdgeList
        {
            get => _socket.EdgeList;
            set => _socket.EdgeList = value;
        }

        public Texture2D Icon
        {
            get => _socket.Icon;
            set => _socket.Icon = value;
        }


        private readonly Socket _socket;

        protected ESocket(Socket socket) : base(socket)
        {
            _socket = socket;
        }

        public bool IsConnected => _socket.IsConnected;

        public abstract SocketSetting SocketSetting { get; }

        public virtual void OnCreate(Graph graph, Node node)
        {
            Graph = graph;
            Node = node;
        }

        public override void Draw()
        {
            base.Draw();

            _socket.Rect.size = SocketSetting.Size;

            Color color = SocketSetting.Color;

            if (EdgeList.Count > 0)
                if (EdgeList[0].Editor().IsRunning)
                    color = Node.Editor().NodeSetting.LineRunning;

            if (Icon != null)
            {
                SocketSetting.Style.CalcMinMaxWidth(new GUIContent(FilterName(Info)), out var min, out var max);

                var iconRect = Rect;

                iconRect.x -= max;

                GUI.DrawTexture(iconRect, Icon);
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
            Undo.DestroyObjectImmediate(_socket);
        }

        public void Disconnect()
        {
            var edges = new List<Edge>(EdgeList);
            foreach (var edge in edges)
                edge.Editor().Delete();
            EdgeList.Clear();
        }

        public void RemoveEdge(Edge edge)
        {
            Undo.RegisterCompleteObjectUndo(_socket,"");
            EdgeList.Remove(edge);
        }
    }
}

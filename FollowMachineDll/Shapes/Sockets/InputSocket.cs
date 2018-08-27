using System;
using System.Linq;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public class InputSocket : Socket
    {
        private bool _showDragLine;
        private Vector2 _dragPos;

        public override SocketSetting SocketSetting => Node.NodeSetting.OutputSocketSetting;


        public override void OnCreate(Graph graph, Node node)
        {
            base.OnCreate(graph,node);
            Rect.size = Node.NodeSetting.InputSocketSetting.Size;
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            while (EdgeList.Count > 0)
            {
                EdgeList[0].Delete();
            }
        }


        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            _showDragLine = true;
            _dragPos = mousePosition;
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            _showDragLine = false;

            foreach (var node in Graph.NodeList)
                //if (node != Node)
                    foreach (OutputSocket socket in node.InputSocketList)
                        if (socket.Rect.Contains(mousePosition))
                        {
                            CreateEdge(this, socket);
                            return;
                        }
        }

        public override void Draw()
        {
            base.Draw();
            if (_showDragLine)
                EditorTools.Instance.DrawBezierEdge(Rect.center, _dragPos, SocketSetting.Color, Color.gray,
                    SocketSetting.Thickness);

            EditorTools.Instance.DrawTexture(
                Rect,
                EdgeList.Count==0 ? SocketSetting.DisconnectedTexure : SocketSetting.ConnectedTexure,
                SocketSetting.Style,
                SocketSetting.Color,
                FilterName(Name));

        }
        protected override string FilterName(string name)
        {
            return name.Split('/').Last();
        }

        public Node GetNextNode()
        {
            if (EdgeList.Count > 0)
            {
                var nextNode = EdgeList[0].OutputSocket.Node;
                nextNode.EnteredSocket = EdgeList[0].OutputSocket;
                return nextNode;
            }
            else
                return null;
        }

        public void Disconnect()
        {
            while (EdgeList.Count>0)
            {
                EdgeList[0].Delete();
            }
        }
    }
}
using System;
using System.Linq;
using FMachine.Shapes.Nodes;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public class InputSocket : Socket
    {
        public bool AutoEdgeHide = false;
        private bool _showDragLine;
        private Vector2 _dragPos;
        public bool AutoHide;

        public override SocketSetting SocketSetting => Node.NodeSetting.OutputSocketSetting;


        public override void OnCreate(Graph graph, Node node)
        {
            base.OnCreate(graph, node);
            Rect.size = Node.NodeSetting.InputSocketSetting.Size;
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button==0)
            {
                while (EdgeList.Count > 0)
                {
                    EdgeList[0].Delete();
                } 
            }
            else if (currentEvent.button == 1)
                EditorTools.Instance.ShowContexMenu(this);
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                _showDragLine = true;
                _dragPos = mousePosition; 
            }
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                _showDragLine = false;
                        
                foreach (var node in Graph.NodeList)
                    foreach (OutputSocket socket in node.InputSocketList)
                        if (socket.Rect.Contains(mousePosition))
                        {
                            CreateEdge(this, socket);

                            return;
                        } 
            }
        }

        public override void Draw()
        {
            if (_showDragLine)
                EditorTools.Instance.DrawBezierEdge(Rect.center, _dragPos, SocketSetting.Color, Color.gray,
                    SocketSetting.Thickness);

            if (
                !_showDragLine &&
                AutoHide &&
                !IsConnected &&
                !IsHover
                )
                return;
            base.Draw();
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
            while (EdgeList.Count > 0)
            {
                EdgeList[0].Delete();
            }
        }

        protected  void CreateEdge(InputSocket inputSocket, OutputSocket outputSocket)
        {
            Edge edge = Graph.Repository.CreateEdge(inputSocket);
            edge.InputSocket = inputSocket;
            edge.OutputSocket = outputSocket;
            if(AutoEdgeHide)
            edge.AutoHide = true;

            inputSocket.EdgeList.Add(edge);
            outputSocket.EdgeList.Add(edge);
        }
    }
}
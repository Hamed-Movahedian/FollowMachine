using System.Linq;
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
    public class EInputSocket : ESocket
    {
        public bool AutoEdgeHide
        {
            get => _inputSocket.AutoEdgeHide;
            set => _inputSocket.AutoEdgeHide = value;
        }

        public bool AutoHide
        {
            get => _inputSocket.AutoHide;
            set => _inputSocket.AutoHide = value;
        }

        private InputSocket _inputSocket;
        private bool _showDragLine;
        private Vector2 _dragPos;

        public EInputSocket(InputSocket inputSocket) : base(inputSocket)
        {
            _inputSocket = inputSocket;
        }

        public override SocketSetting SocketSetting => Node.Editor().NodeSetting.OutputSocketSetting;


        public override void OnCreate(Graph graph, Node node)
        {
            base.OnCreate(graph, node);
            _inputSocket.Rect.size = Node.Editor().NodeSetting.InputSocketSetting.Size;
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            switch (currentEvent.button)
            {
                case 0 when currentEvent.control:
                    return;
                case 0:
                    while (EdgeList.Count > 0)
                    {
                        EdgeList[0].Editor().Delete();
                    }

                    break;
                case 1:
                    EditorTools.Instance.ShowContexMenu(_inputSocket);
                    break;
            }
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {

            if (currentEvent.button == 0)
            {
                if (currentEvent.control)
                {
                    if(mousePosition.y<Rect.yMin-Rect.height)
                        Node.Editor().MoveSocket(_inputSocket,-1);

                    if(mousePosition.y>Rect.yMax+Rect.height)
                        Node.Editor().MoveSocket(_inputSocket,1);
                }
                else
                {
                    _showDragLine = true;
                    _dragPos = mousePosition;
                }
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
                            CreateEdge(socket);

                            return;
                        }
            }
        }

        public void CreateEdge(OutputSocket outputSocket)
        {
            CreateEdge(_inputSocket, outputSocket);
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

        public void Disconnect()
        {
            while (EdgeList.Count > 0)
            {
                EdgeList[0].Editor().Delete();
            }
        }

        protected void CreateEdge(InputSocket inputSocket, OutputSocket outputSocket)
        {
            Edge edge = Graph.Editor().Repository.CreateEdge(inputSocket);
            Undo.RegisterCompleteObjectUndo(edge, "");
            edge.InputSocket = inputSocket;
            edge.OutputSocket = outputSocket;
            if (AutoEdgeHide)
                edge.AutoHide = true;
            Undo.RegisterCompleteObjectUndo(inputSocket, "");
            Undo.RegisterCompleteObjectUndo(outputSocket, "");
            inputSocket.EdgeList.Add(edge);
            outputSocket.EdgeList.Add(edge);
        }
    }
}

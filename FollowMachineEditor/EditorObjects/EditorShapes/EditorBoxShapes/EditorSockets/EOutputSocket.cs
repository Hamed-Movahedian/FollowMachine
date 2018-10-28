using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorSockets
{
    public class EOutputSocket : ESocket
    {
        private readonly OutputSocket _outputSocket;
        private Edge _selectedEdge;
        private InputSocket _selectedInputSocket;
        private bool _showDragLine;
        private Vector2 _dragPos;

        public override SocketSetting SocketSetting => Node.Editor().NodeSetting.InputSocketSetting;


        public override void OnCreate(Graph graph, Node node)
        {
            base.OnCreate(graph, node);
            _outputSocket.Rect.size = Node.Editor().NodeSetting.OutputSocketSetting.Size;

        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                _selectedEdge = EdgeList.Count == 0 ? null : EdgeList[EdgeList.Count - 1];

                if (_selectedEdge == null) return;

                _selectedEdge.Editor().Hide();
                _selectedInputSocket = _selectedEdge.InputSocket;
            }
            else if (currentEvent.button == 1)
                EditorTools.Instance.ShowContexMenu(_outputSocket);
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                _dragPos = mousePosition;
                _showDragLine = true;
            }
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            _showDragLine = false;
            foreach (var node in Graph.NodeList)
                foreach (OutputSocket socket in node.InputSocketList)
                    if (socket.Rect.Contains(mousePosition))
                        if (_selectedEdge != null)
                        {
                            _selectedEdge.Editor().SetOutputSocket(socket);
                            _selectedEdge.Editor().Show();
                            return;
                        }

            if (_selectedEdge != null)
                _selectedEdge.Editor().Delete();
        }

        public override void Draw()
        {
            if (_showDragLine)
            {
                if (_selectedInputSocket != null)
                    EditorTools.Instance.DrawBezierEdge(
                    _selectedInputSocket.Rect.center,
                    _dragPos,
                    SocketSetting.Color,
                    Color.gray,
                    SocketSetting.Thickness);
            }

            base.Draw();
        }

        public EOutputSocket(OutputSocket outputSocket) : base(outputSocket)
        {
            _outputSocket = outputSocket;
        }
    }
}

using System.Linq;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public class OutputSocket : Socket
    {
        private Edge _selectedEdge;
        private InputSocket _selectedInputSocket;
        private bool _showDragLine;
        private Vector2 _dragPos;

        public override SocketSetting SocketSetting => Node.NodeSetting.InputSocketSetting;


        public override void OnCreate(Graph graph, Node node)
        {
            base.OnCreate(graph,node);
            Rect.size = Node.NodeSetting.OutputSocketSetting.Size;

        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            _selectedEdge = EdgeList.Count == 0 ? null : EdgeList[EdgeList.Count - 1];

            if (_selectedEdge == null) return;

            _selectedEdge.Hide();
            _selectedInputSocket = _selectedEdge.InputSocket;
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            _dragPos = mousePosition;
            _showDragLine = true;
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            _showDragLine = false;
            foreach (var node in Graph.NodeList)
                foreach (OutputSocket socket in node.InputSocketList)
                    if (socket.Rect.Contains(mousePosition))
                        if (_selectedEdge != null)
                        {
                            _selectedEdge.SetOutputSocket(socket);
                            _selectedEdge.Show();
                            return;
                        }

            if (_selectedEdge != null)
                _selectedEdge.Delete();
        }

        public override void Draw()
        {
            base.Draw();
            if (_showDragLine)
            {
                if(_selectedInputSocket!=null)
                    EditorTools.Instance.DrawBezierEdge(
                    _selectedInputSocket.Rect.center,
                    _dragPos,
                    SocketSetting.Color,
                    Color.gray,
                    SocketSetting.Thickness);
            }

            EditorTools.Instance.DrawTexture(
                Rect,
                EdgeList.Count==0 ? SocketSetting.DisconnectedTexure : SocketSetting.ConnectedTexure,
                SocketSetting.Style,
                SocketSetting.Color,
                FilterName(Name));
        }
    }
}
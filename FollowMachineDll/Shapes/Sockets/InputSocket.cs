using FMachine.Shapes.Nodes;
using UnityEditor;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public class InputSocket : Socket
    {
        public bool AutoEdgeHide = false;
        private bool _showDragLine;
        private Vector2 _dragPos;
        public bool AutoHide;


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
    }
}
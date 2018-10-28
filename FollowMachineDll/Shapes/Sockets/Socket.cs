using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public abstract class Socket : BoxShape
    {
        public Node Node;
        public List<Edge> EdgeList = new List<Edge>();
        public Texture2D Icon;
        public bool IsConnected => EdgeList.Count > 0;


    }
}
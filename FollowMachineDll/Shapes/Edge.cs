using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using UnityEngine;

namespace FMachine.Shapes
{
    public class Edge : Shape
    {
        public InputSocket InputSocket;
        public OutputSocket OutputSocket;
        public List<Vector2> EditPoints = new List<Vector2>();
        public int ClickEditpointIndex;
        public bool AutoHide = false;
        public bool ExcludeInLayout = false;


    }
}
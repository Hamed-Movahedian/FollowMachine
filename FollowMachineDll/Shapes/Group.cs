using System.Collections.Generic;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using UnityEngine;

namespace FollowMachineDll.Shapes
{
    public class Group : BoxShape
    {
        public List<Node> NodeList=new List<Node>();
        public Color Color=Color.white;
    }
}

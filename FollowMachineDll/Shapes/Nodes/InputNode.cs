using System;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Input")]
    public class InputNode : Node
    {
        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle="IO/Jump")]
    public class NullNode : Node
    {
        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}

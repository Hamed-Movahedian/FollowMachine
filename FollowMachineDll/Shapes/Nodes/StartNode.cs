using System.Collections;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Start")]
    class StartNode : Node
    {
        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("Out");
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}
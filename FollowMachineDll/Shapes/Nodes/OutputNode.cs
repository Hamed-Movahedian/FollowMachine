using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Output")]
    public class OutputNode : Node
    {
        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }

        public override Node GetNextNode()
        {
            (Graph as FollowMachine).IsRunning = false;
            return null;
        }
    }
}
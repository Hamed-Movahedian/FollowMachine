using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Output")]
    public class OutputNode : Node
    {

        public override Node GetNextNode()
        {
            (Graph as FollowMachine).IsRunning = false;
            return null;
        }
    }
}
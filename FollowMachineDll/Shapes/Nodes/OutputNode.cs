using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Output")]
    public class OutputNode : Node
    {

        public override Node GetNextNode()
        {
            var followMachine = Graph as FollowMachine;
            followMachine.IsRunning = false;
            return null;
        }
    }
}
using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Output")]
    public class OutputNode : Node
    {

        public override Node GetNextNode()
        {
            return null;
        }
    }
}
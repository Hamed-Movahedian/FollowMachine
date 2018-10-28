using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/Input")]
    public class InputNode : Node
    {
        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}
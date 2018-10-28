using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "IO/Jump(old)")]
    public class NullNode : Node
    {

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}

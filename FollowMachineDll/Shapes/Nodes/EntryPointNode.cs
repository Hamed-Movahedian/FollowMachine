using FollowMachineDll.Attributes;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/EntryPoint")]
    public class EntryPointNode : Node
    {
        public void Start()
        {
            if(Active)
                StartCoroutine((Graph as FollowMachine)?.RunNode(this));
        }

        public override Node GetNextNode()
        {
            if (Active)
                return OutputSocketList[0].GetNextNode();
            else
                return null;
        }
    }
}
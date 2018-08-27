using FMachine.Shapes.Sockets;

namespace FMachine.Shapes.Nodes
{
    public class OutputNode : Node
    {
        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }

        public override Node GetNextNode()
        {
            return null;
        }
    }
}
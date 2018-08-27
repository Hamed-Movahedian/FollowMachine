using System.Collections;
using FMachine.Shapes.Sockets;

namespace FMachine.Shapes.Nodes
{
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
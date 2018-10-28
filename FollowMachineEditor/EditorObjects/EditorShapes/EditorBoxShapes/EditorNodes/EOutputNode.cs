using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EOutputNode : ENode
    {
        private OutputNode _outputNode;

        public EOutputNode(OutputNode outputNode) : base(outputNode)
        {
            _outputNode = outputNode;
        }
        public override bool IsEqualTo(Node node)
        {
            return Info == node.Info;
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }

    }
}

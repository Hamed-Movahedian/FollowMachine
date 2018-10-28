using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EInputNode : ENode
    {
        private InputNode _inputNode;

        public EInputNode(InputNode inputNode) : base(inputNode)
        {
            _inputNode = inputNode;
        }
        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
        }
    }
}

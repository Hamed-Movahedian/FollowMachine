using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class ENullNode : ENode
    {
        private readonly NullNode _nullNode;

        public ENullNode(NullNode nullNode) : base(nullNode)
        {
            _nullNode = nullNode;
        }
        public override void Draw()
        {
            var nextNode = _nullNode.GetNextNode();

            if (nextNode != null)
                Info = nextNode.Info;

            base.Draw();
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
            OutputSocketList[0].AutoEdgeHide = true;
        }

        public override void OnShow()
        {
            base.OnShow();
            OutputSocketList[0].AutoEdgeHide = true;

            if (OutputSocketList[0].EdgeList.Count > 0)
                OutputSocketList[0].EdgeList[0].AutoHide = true;

        }

    }
}

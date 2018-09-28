using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "IO/Jump(old)")]
    public class NullNode : Node
    {
        public override void Draw()
        {
            var nextNode = GetNextNode();

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

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
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

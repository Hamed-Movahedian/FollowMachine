using System.Collections;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "UI/ProgressWindow")]
    public class ProgressNode : Node
    {
        public enum ProgressModeEnum
        {
            Show,
            Hide
        }

        public MgsProgressWindow Window;
        public string Message;
        public ProgressModeEnum ProgressMode;


        protected override IEnumerator Run()
        {
            if (ProgressMode == ProgressModeEnum.Show)
                return Window.Display(Message);
            else
                return Window.Hide();
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}

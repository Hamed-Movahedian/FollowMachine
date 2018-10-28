using System;
using System.Collections;
using System.Collections.Generic;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="UI/Dialog")]
    public class DialogeNode : Node
    {
        public MgsDialougWindow Window;
        public string Title;
        public string Message;
        public string Icon;
        public float Timer = 0;
        public List<string> Buttons = new List<string>();

        protected override IEnumerator Run()
        {
            if (Window == null)
                throw new Exception("Error in dialog node! Window isn't set");

            return Window.Display(Title, Message, Icon, Buttons,Timer);
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (socket.Info == Window.Result)
                    return socket.GetNextNode();
            return null;
        }
    }
}
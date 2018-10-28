using System;
using System.Collections;
using System.Collections.Generic;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle = "UI/WindowNode")]
    public class WindowNode : Node
    {
        public MgsUIWindow Window;
        public bool Refresh = true;
        public List<bool> HideList = new List<bool>();


        protected override IEnumerator Run()
        {
            if (Window == null)
                throw new Exception("Error in window node! Window is'nt set");

            if (Refresh)
                Window.Refresh();

            if (EnteredSocket == InputSocketList[0])
                yield return  Window.WaitForClose(false, false);
            else
                yield return Window.WaitForClose(true, false);


            for (var i = 0; i < OutputSocketList.Count; i++)
                if (OutputSocketList[i].Info == Window.Result)
                    if (HideList[i])
                        yield return Window.Hide();
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
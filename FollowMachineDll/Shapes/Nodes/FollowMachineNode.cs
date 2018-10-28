using System.Collections;
using FollowMachineDll.Attributes;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle = "Follow Machine")]
    public class FollowMachineNode : Node
    {
        public FollowMachine FollowMachine;


        protected override IEnumerator Run()
        {
            if (FollowMachine == null)
                yield break;

            FollowMachine.IsRunning = true;

            StartCoroutine(FollowMachine.RunInputNode(EnteredSocket.Info));

            yield return new WaitWhile(() => FollowMachine.IsRunning);
        }

        public override Node GetNextNode()
        {
            if (FollowMachine != null)
                if (FollowMachine.RunningNode != null)
                {
                    if (FollowMachine.RunningNode is OutputNode)
                        foreach (var socket in OutputSocketList)
                            if (socket.Info == FollowMachine.RunningNode.Info)
                                return socket.GetNextNode();
                }

            return null;
        }
    }
}
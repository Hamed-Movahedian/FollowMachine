using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using FollowMachineDll.Components;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Server connection")]
    public class ServerNode : Node
    {
        public string MethodName;

        public List<BoundData> Parameters;

        public int BodyParamIndex = -1;

        public ServerConnectionMethod ConnectionMethod;

        public ProgressBarInfo ProgressBarInfo;

        protected override IEnumerator Run()
        {
            var url = MethodName;

            var firstParam = true;

            object outData = null;

            for (int i = 0; i < Parameters.Count; i++)
            {
                var parm = Parameters[i];

                if (BodyParamIndex == i)
                {
                    outData = parm.GetValue();
                }
                else
                {
                    url += $"{(firstParam ? "?" : "&")}{parm.Lable}={parm.GetValue()}";

                    firstParam = false;
                }
            }

            yield return ProgressBarInfo.Show();

            yield return ServerControllerBase.Send(ConnectionMethod, url, outData);

            yield return ProgressBarInfo.Hide();

        }

        public override Node GetNextNode()
        {
            var outputFollowControl = ServerControllerBase.OutputFollowControl;

            foreach (var socket in OutputSocketList)
                if (socket.Info.ToLower() == outputFollowControl.ToLower())
                    return socket.GetNextNode();

            return null;
        }
    }
}



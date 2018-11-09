using System.Collections;
using System.Collections.Generic;
using Bind;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Assets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Components;
using FollowMachineDll.Utility;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Server connection")]
    public class ServerNode : Node
    {
        public string RoutePrefix;

        public ServerData.Controller.MethodData MethodData;

        public List<GetValue> Parameters=new List<GetValue>();

        public ProgressBarInfo ProgressBarInfo;

        protected override IEnumerator Run()
        {
            var url = $"{RoutePrefix}/{MethodData.Prefix}";

            var firstParam = true;

            object outData = null;

            for (int i = 0; i < MethodData.Parameters.Count; i++)
            {
                var paramValue = Parameters[i];

                var paramData = MethodData.Parameters[i];

                if (paramData.FormBody)
                {
                    outData = paramValue.Value;
                }
                else
                {
                    url += $"{(firstParam ? "?" : "&")}{paramData.Name}={paramValue.Value}";

                    firstParam = false;
                }
            }

            yield return ProgressBarInfo.Show();

            yield return ServerControllerBase.Send(MethodData.ConnectionMethod, url, outData);

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



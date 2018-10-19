using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Components;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using MgsCommonLib;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Server connection")]
    public class ServerNode : Node
    {
        public string MethodName;

        public List<BoundData> Parameters;

        public int BodyParamIndex = -1;

        public ServerConnectionMethod ConnectionMethod;

        private string _outputFollowControl;

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("Success");
            AddOutputSocket<InputSocket>("Network Error");
            AddOutputSocket<InputSocket>("Http Error");
        }

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
                    url += $"{(firstParam ? "?" : "&")}{parm.Name}={parm.GetValue()}";

                    firstParam = false;
                }
            }

            return ServerControllerBase.Send(
                ConnectionMethod,
                url,
                outData,
                // On Success
                () =>
                {
                    _outputFollowControl = ServerControllerBase.Instance.GetOutputFollowControl() ?? "Success";
                },
                // On Error
                request =>
                {
                    // Network Error !!!!!
                    if (request.isNetworkError)
                        _outputFollowControl = "Network Error";

                    // Http Error !!!!
                    else if (request.isHttpError)
                        _outputFollowControl = "Http Error";
                });
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (socket.Info.ToLower()==_outputFollowControl.ToLower())
                    return socket.GetNextNode();

            return null;
        }

    }
}



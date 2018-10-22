using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using MgsCommonLib.Utilities;
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

        public ProgressBarInfo ProgressBarInfo;

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

        public void UpdateBaseOnMethodName()
        {
            var methodData = ServerControllerBase.Instance.GetMethodData(MethodName);

            Info = string.IsNullOrEmpty(methodData.Info) ?
                MethodName.Split('(').First().Replace("/", ".") :
                methodData.Info;

            ConnectionMethod = methodData.ConnectionMethod;

            // reset node
            OutputSocketList.Clear();
            BodyParamIndex = -1;
            Parameters.Clear();

            // Set outputs
            if (methodData.Outputs.Count == 0)
            {
                AddOutputSocket<InputSocket>("Success");
                AddOutputSocket<InputSocket>("Network Error");
                AddOutputSocket<InputSocket>("Http Error");
            }
            else
            {
                foreach (var output in methodData.Outputs)
                    AddOutputSocket<InputSocket>(output);

                AddOutputSocket<InputSocket>("Network Error");
                AddOutputSocket<InputSocket>("Http Error");
            }

            var parts = MethodName.Split('(', ')', ',')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .ToList();

            Parameters.Resize(parts.Count - 1);

            BodyParamIndex = -1;

            for (int i = 0; i < parts.Count - 1; i++)
            {
                if (Parameters[i] == null)
                    Parameters[i] = new BoundData();

                if (parts[i + 1].Contains("FromBody"))
                {
                    if (BodyParamIndex == -1)
                        BodyParamIndex = i;
                    else if (BodyParamIndex != i)
                        throw new InvalidDataException(" More than one body parameters!!");
                }

                var paramParts = parts[i + 1]
                    .Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => s != "")
                    .ToList();

                if (paramParts.Count > 1)
                {
                    Parameters[i].Name = paramParts[paramParts.Count - 1];

                    var type = paramParts[paramParts.Count - 2];

                    Parameters[i].TypeName = type;

                    if (!SupportedTypes.IsSupported(type))
                    {
                        if (BodyParamIndex == -1)
                            BodyParamIndex = i;
                        else if (BodyParamIndex != i)
                            throw new InvalidDataException(" More than one body parameters!!");
                    }
                }
            }

        }
    }
}



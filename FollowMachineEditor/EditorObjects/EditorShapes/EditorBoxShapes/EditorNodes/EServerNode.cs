using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Components;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.CustomInspectors;
using FollowMachineEditor.EditorObjectMapper;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EServerNode : ENode
    {
        private readonly ServerNode _serverNode;

        public string MethodName
        {
            get => _serverNode.MethodName;
            set => _serverNode.MethodName = value;
        }

        public List<BoundData> Parameters
        {
            get => _serverNode.Parameters;
            set => _serverNode.Parameters = value;
        }


        public int BodyParamIndex
        {
            get => _serverNode.BodyParamIndex;
            set => _serverNode.BodyParamIndex = value;
        }


        public ServerConnectionMethod ConnectionMethod
        {
            get => _serverNode.ConnectionMethod;
            set => _serverNode.ConnectionMethod = value;
        }


        public EServerNode(ServerNode serverNode) : base(serverNode)
        {
            _serverNode = serverNode;
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("Success");
            AddOutputSocket<InputSocket>("Network Error");
            AddOutputSocket<InputSocket>("Http Error");
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
                    Parameters[i].Lable = paramParts[paramParts.Count - 1];

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

        public override void OnInspector()
        {
            base.OnInspector();

            var serverController = ServerControllerBase.Instance;
            
            if (serverController.Data == null)
            {
                GUIUtil.DisplayError(" Server Data not set!!");
                return;
            }

            if (serverController.MethodNames.Count == 0)
            {
                GUIUtil.DisplayError(" Server controller has no controller!!");
                return;
            }

            if (GUIUtil.PopupFieldInBox("Method :", ref _serverNode.MethodName, serverController.MethodNames))
            {
                try
                {
                    UpdateBaseOnMethodName();
                }
                catch (InvalidDataException e)
                {
                    GUIUtil.DisplayError(e.Message);
                    return;
                }

                GUIUtil.RefreshWindow();
            }

            // Parameters
            GUIUtil.DrawInBox(GetParameters);

            // Progress bar
            GUIUtil.GetProgressBar(_serverNode.ProgressBarInfo);
        }


        private void GetParameters()
        {
            GUIUtil.BoldLable("Parameters :");

            for (int i = 0; i < _serverNode.Parameters.Count; i++)
            {
                var boundData = _serverNode.Parameters[i];

                GUILayout.Label($"{boundData.Lable} ({boundData.TypeName}) {(i == _serverNode.BodyParamIndex ? "  [FromBody]" : "")}");

                GUIUtil.BoundField(boundData);
            }

        }

    }
}

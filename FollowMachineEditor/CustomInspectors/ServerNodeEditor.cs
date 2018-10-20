using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Editor;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Components;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.Windows.Bounder;
using FollowMachineEditor.Windows.FollowMachineInspector;
using MgsCommonLib.Utilities;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

namespace FollowMachineEditor.CustomInspectors
{
    public class ServerNodeEditor : CustomInspector
    {
        private ServerNode _serverNode;

        public override void OnInspector(Node node)
        {
            base.OnInspector(node);

            _serverNode = (ServerNode)node;

            var serverController = GameObject.FindObjectOfType<ServerControllerBase>();

            if (serverController == null)
            {
                DisplayError(" Server controller not found!!");
                return;
            }

            if (serverController.Data == null)
            {
                DisplayError(" Server not set!!");
                return;
            }

            if (serverController.Data.Controllers.Count == 0)
            {
                DisplayError(" Server controller has no controller!!");
                return;
            }

            var methodList = serverController
                .Data.Controllers
                .SelectMany(controller => controller.Methods.Select(methodData => new {controller,methodData}))
                .ToList();

            var MethodNameList = methodList.Select(m => m.controller.Name + "/" + m.methodData.Name).ToList();

            if (PopupFieldInBox("Method :", ref _serverNode.MethodName, MethodNameList))
            {
                var method = methodList[MethodNameList.IndexOf(_serverNode.MethodName)];

                if(string.IsNullOrEmpty(method.methodData.Info))
                    node.Info = _serverNode.MethodName.Split('(').First().Replace("/", ".");
                else
                {
                    node.Info = method.methodData.Info;
                }

                _serverNode.OutputSocketList.Clear();

                if (method.methodData.Outputs.Count == 0)
                {
                    _serverNode.AddOutputSocket<InputSocket>("Success");
                    _serverNode.AddOutputSocket<InputSocket>("Network Error");
                    _serverNode.AddOutputSocket<InputSocket>("Http Error");
                }
                else
                {
                    foreach (var output in method.methodData.Outputs)
                        _serverNode.AddOutputSocket<InputSocket>(output);
                    _serverNode.AddOutputSocket<InputSocket>("Network Error");
                    _serverNode.AddOutputSocket<InputSocket>("Http Error");
                }

                RefreshWindow();
            }

            var parts = _serverNode.MethodName.Split('(', ')', ',')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .ToList();

            _serverNode.Parameters.Resize(parts.Count - 1);

            _serverNode.BodyParamIndex = -1;

            for (int i = 0; i < parts.Count - 1; i++)
            {
                if (_serverNode.Parameters[i] == null)
                    _serverNode.Parameters[i] = new BoundData();

                if (parts[i + 1].Contains("FromBody"))
                    if (SetBodyParamIndex(i))
                        return;

                var paramParts = parts[i + 1]
                    .Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => s != "")
                    .ToList();

                if (paramParts.Count > 1)
                {
                    _serverNode.Parameters[i].Name = paramParts[paramParts.Count - 1];

                    var type = paramParts[paramParts.Count - 2];

                    _serverNode.Parameters[i].TypeName = type;

                    if (!SupportedTypes.Types.ContainsKey(type))
                    {
                        if (SetBodyParamIndex(i))
                            return;
                        _serverNode.Parameters[i].Type = null;
                    }
                    else
                        _serverNode.Parameters[i].Type = SupportedTypes.Types[type].Type;
                }
            }

            DrawInBox(GetParameters);
        }

        private bool SetBodyParamIndex(int i)
        {
            if (_serverNode.BodyParamIndex == -1)
                _serverNode.BodyParamIndex = i;
            else if(_serverNode.BodyParamIndex!=i)
            {
                DisplayError(" More than one body parameters!!");
                return true;
            }

            return false;
        }

        private void GetParameters()
        {
            BoldLable("Parameters :");

            for (int i = 0; i < _serverNode.Parameters.Count; i++)
            {
                var boundData = _serverNode.Parameters[i];

                GUILayout.Label($"{boundData.Name} ({boundData.TypeName}) {(i == _serverNode.BodyParamIndex ? "  [FromBody]" : "")}");

                BoundField(boundData);
            }

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Assets;
using FollowMachineDll.Components;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.CustomInspectors;
using FollowMachineEditor.EditorObjectMapper;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EServerNode : ENode
    {
        private readonly ServerNode _serverNode;

        public EServerNode(ServerNode serverNode) : base(serverNode)
        {
            _serverNode = serverNode;
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            SetOutputs(new List<string>{"Success","Network Error","Http Error"});
        }

        public override void OnInspector()
        {
            base.OnInspector();

            if (!ServerControllerBase.Instance.IsValid)
            {
                GUIUtil.DisplayError("Invalid Server Data!!");
                return;
            }

            // Get method
            GUIUtil.DrawInBox("Method: ", GetMethodGUI);

            // Parameters
            GUIUtil.DrawInBox(GetParameters);

            // Progress bar
            GUIUtil.GetProgressBar(_serverNode.ProgressBarInfo);
        }

        private void GetParameters()
        {
            GUIUtil.BoldLable("Parameters :");

            foreach (var parameter in _serverNode.Parameters)
            {
                GUIUtil.BoundField(parameter);
            }
        }

        private void GetMethodGUI()
        {
            var methods = ServerControllerBase
                .Instance
                .Data
                .Controllers
                .SelectMany(controller =>
                    controller
                        .Methods
                        .Select(method => new
                        {
                            controller,
                            method
                        }))
                .ToList();

            int currIndex = methods.FindIndex(m =>
                    m.controller.Prefix == _serverNode.RoutePrefix &&
                    m.method.Equals(_serverNode.MethodData));

            int selIndex = EditorGUILayout.Popup(
                currIndex,
                methods.Select(m => m.controller.Name + "/" + m.method.FullName).ToArray());

            if (currIndex != selIndex)
            {
                var method = methods[selIndex];

                Undo.RegisterCompleteObjectUndo(_serverNode, "");

                _serverNode.MethodData =
                    JObject.FromObject(method.method).ToObject<ServerData.Controller.MethodData>();

                _serverNode.RoutePrefix = method.controller.Prefix;

                _serverNode.Parameters = method
                    .method
                    .Parameters
                    .Select(p => new BoundData(
                        lable: $"{p.Name} ({p.TypeName}) {(p.FormBody ? "[FromBody]" : "")}",
                        boundSource: p.FormBody ? BoundSourceEnum.GameObject : BoundSourceEnum.Constant,
                        value: "",
                        typeName: p.TypeName
                        ))
                    .ToList();

                SetOutputs(method.method.Outputs);

                if (method.method.Info != null)
                    _serverNode.Info = method.method.Info;

                GUIUtil.RefreshWindow();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Bind;
using BindEditor;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Assets;
using FollowMachineDll.Components;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineEditor.CustomInspectors;
using Newtonsoft.Json.Linq;
using UnityEditor;

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
                parameter.GetValueGUI();
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
                    .Select(p => new GetValue(
                        $"{p.Name} ({p.TypeName}) {(p.FormBody ? "[FromBody]" : "")}",p.AssemblyQualifiedName))
                    .ToList();

                SetOutputs(method.method.Outputs);

                if (method.method.Info != null)
                    _serverNode.Info = method.method.Info;

                GUIUtil.RefreshWindow();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Shapes.Nodes;
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

            if(serverController.Data==null)
            {
                DisplayError(" Server not set!!");
                return;
            }

            if(serverController.Data.Controllers.Count==0)
            {
                DisplayError(" Server controller has no controller!!");
                return;
            }

            var methodList = serverController
                .Data.Controllers
                .SelectMany(c => c.Methods.Select(m => c.Name + "/" + m))
                .ToList();

            if (PopupFieldInBox("Method :", ref _serverNode.MethodName, methodList))
            {
                node.Info = _serverNode.MethodName.Split('(').First().Replace("/",".");
            }

            var parts = _serverNode.MethodName.Split('(', ')',',')
                .Select(s=>s.Trim())
                .Where(s=>s!="")
                .ToList();

            _serverNode.Parameters.Resize(parts.Count-1);

            _serverNode.BodyParamIndex = -1;

            for (int i = 0; i < parts.Count-1; i++)
            {
                if(_serverNode.Parameters[i]==null)
                    _serverNode.Parameters[i]=new ServerNode.ParamData();

                if (parts[i + 1].Contains("FromBody"))
                    _serverNode.BodyParamIndex = i;

                var paramParts = parts[i+1]
                    .Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => s != "")
                    .ToList();
                if (paramParts.Count > 1)
                {
                    _serverNode.Parameters[i].Name = paramParts[paramParts.Count - 1];
                    _serverNode.Parameters[i].Type = paramParts[paramParts.Count - 2];
                }
            }

            DrawInBox(GetParameters);
        }

        private void GetParameters()
        {
            BoldLable("Parameters :");

            for (int i = 0; i < _serverNode.Parameters.Count; i++)
            {
                //DrawInBox(() => GetParameter(i));
                var paramData = _serverNode.Parameters[i];

                GUILayout.Label($"{paramData.Name} ({paramData.Type}) {(i == _serverNode.BodyParamIndex ? "  [FromBody]" : "")}");

                BoundField(paramData);
            }

        }

        private void GetParameter(int i)
        {
            var paramData = _serverNode.Parameters[i];

            GUILayout.Label($"{paramData.Name} ({paramData.Type}) {(i == _serverNode.BodyParamIndex ? "  [FromBody]" : "")}");

            BoundField(paramData);
        }

        private void BoundField(ServerNode.ParamData paramData)
        {
            GUILayout.BeginHorizontal();

            if (paramData.IsBound)
                GUILayout.Label(paramData.Value);
            else
                TextField("", ref paramData.Value);

            if (GUILayout.Button("B", GUILayout.Width(20)))
            {
                var menu = new GenericMenu();

                if (paramData.IsBound)
                {
                    menu.AddItem(new GUIContent("Edit"), false, () => BounderWindow.EditBound(
                        paramData.BoundGameObject,
                        paramData.Value,
                        typeof(string),
                        (o, s) =>
                        {
                            paramData.BoundGameObject = o;
                            paramData.Value = s;
                        }));

                    menu.AddItem(new GUIContent("Unbound"), false, () =>
                    {
                        Undo.RecordObject(TargetNode, "Change Bound");
                        paramData.IsBound = false;
                        paramData.Value = "";
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Bound"), false, () => BounderWindow.EditBound(
                        paramData.BoundGameObject,
                        paramData.Value,
                        typeof(string),
                        (o, s) =>
                        {
                            paramData.BoundGameObject = o;
                            paramData.Value = s;
                            paramData.IsBound = true;
                        }));
                }

                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();
        }
    }
}

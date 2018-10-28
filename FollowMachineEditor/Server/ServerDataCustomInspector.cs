using System;
using System.Collections.Generic;
using System.Linq;
using FollowMachineDll.Assets;
using FollowMachineDll.Components;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.Server
{
    [CustomEditor(typeof(ServerData))]
    public class ServerDataCustomInspector : Editor
    {
        private ServerData _serverData;

        public override void OnInspectorGUI()
        {
            _serverData = target as ServerData;

            if (GUILayout.Button("Update Interface"))
            {
                var controllers = ServerEditor.Get(@"Interface/GetControllers", "Download interface", "Download");

                _serverData.Controllers = JArray
                    .Parse(controllers)
                    .Select(ctrl => new ServerData.Controller
                    {
                        Name = (string)ctrl["Name"],
                        Prefix = (string)ctrl["Prefix"],
                        Methods = ctrl["Methods"]
                            .Select(method => new ServerData.Controller.MethodData
                            {
                                Name = (string)method["Name"],
                                Prefix = (string)method["Prefix"],
                                ConnectionMethod = (ServerConnectionMethod)
                                    Enum.Parse(
                                        typeof(ServerConnectionMethod),
                                        (string)method["ConnectionMethod"]),
                                Info = (string)method["Info"],
                                Outputs = method["Outputs"].Select(s => s.ToString()).ToList(),
                                Parameters = method["Parameters"]
                                    .Select(param => new ServerData.Controller.MethodData.ParameterData()
                                    {
                                        Name = (string)param["Name"],
                                        TypeName = (string)param["TypeName"],
                                        FormBody = (bool)param["FormBody"]
                                    })
                                    .ToList(),
                            })
                            .ToList(),
                    })
                    .ToList();
            }

            DrawDefaultInspector();
        }
    }
}

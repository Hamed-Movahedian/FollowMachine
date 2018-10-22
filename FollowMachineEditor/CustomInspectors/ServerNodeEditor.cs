using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FMachine.Editor;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Components;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using FollowMachineEditor.Windows.Bounder;
using FollowMachineEditor.Windows.FollowMachineInspector;
using MgsCommonLib.UI;
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

            var serverController = ServerControllerBase.Instance;


            if (serverController.Data == null)
            {
                DisplayError(" Server Data not set!!");
                return;
            }

            if (serverController.MethodNames.Count == 0)
            {
                DisplayError(" Server controller has no controller!!");
                return;
            }

            if (PopupFieldInBox("Method :", ref _serverNode.MethodName, serverController.MethodNames))
            {
                try
                {
                    _serverNode.UpdateBaseOnMethodName();
                }
                catch (InvalidDataException e)
                {
                    DisplayError(e.Message);
                    return;
                }

                RefreshWindow();
            }

            // Parameters
            DrawInBox(GetParameters);

            // Progress bar
            DrawInBox(() => GetProgressBar(_serverNode.ProgressBarInfo));
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.UI;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "UI/ProgressWindow")]
    public class ProgressNode : Node
    {
        public enum ProgressModeEnum
        {
            Show,
            Hide
        }

        public MgsProgressWindow Window;
        public string Message;
        public ProgressModeEnum ProgressMode;
        
        public override void DrawInspector()
        {
            base.DrawInspector();
            EditorTools.Instance.PropertyField(this, "Window");
            if(Window==null)
                return;
            EditorTools.Instance.LanguageField(this, "Message", ref Message);
            EditorTools.Instance.PropertyField(this, "ProgressMode");
            if (GUILayout.Button("Fill Info"))
            {
                var camaSplit = Message.Split('/');

                Info = camaSplit[camaSplit.Length-1] + " => " + ProgressMode;
            }
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        public override IEnumerator Run()
        {
            return Window.Display(Message);
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}

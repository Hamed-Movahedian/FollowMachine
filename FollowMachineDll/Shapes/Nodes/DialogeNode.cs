using System;
using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="UI/Dialoge")]
    public class DialogeNode : Node
    {
        public MgsDialougWindow Window;
        public string Title;
        public string Message;
        public string Icon;
        public List<string> Buttons = new List<string>();

        public override void OnInspector()
        {
            base.OnInspector();

            EditorTools.Instance.PropertyField(this, "Window");

            if (Window == null)
                return;
            EditorTools.Instance.Space();

            EditorTools.Instance.LanguageField(this, "Title", ref Title);
            EditorTools.Instance.LanguageField(this, "Message", ref Message);
            EditorTools.Instance.IconField(this, "Icon", ref Icon);

            EditorTools.Instance.Space();

            if (EditorTools.Instance.LanguageListField(this, "Buttons", Buttons))
            {
                while (Buttons.Count < OutputSocketList.Count)
                {
                    var socket = OutputSocketList[OutputSocketList.Count - 1];
                    socket.Delete();
                    OutputSocketList.Remove(socket);
                }

                while (Buttons.Count > OutputSocketList.Count)
                {
                    AddOutputSocket<InputSocket>("");
                }

                for (int i = 0; i < Buttons.Count; i++)
                {
                    OutputSocketList[i].Info = Buttons[i];
                }

            }
            if (Message != "")
                if (GUILayout.Button("Fill Info"))
                    Info = Message;

        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }

        protected override IEnumerator Run()
        {
            if (Window == null)
                throw new Exception("Error in dialoge node! Window is'nt set");

            return Window.Display(Title, Message, Icon, Buttons);
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (socket.Info == Window.Result)
                    return socket.GetNextNode();
            return null;
        }
    }
}
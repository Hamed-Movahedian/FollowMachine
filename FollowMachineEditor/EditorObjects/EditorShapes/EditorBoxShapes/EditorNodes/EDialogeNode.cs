using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using MgsCommonLib.UI;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EDialogeNode : ENode
    {
        public MgsDialougWindow Window
        {
            get => _dialogeNode.Window;
            set => _dialogeNode.Window = value;
        }
        public string Title
        {
            get => _dialogeNode.Title;
            set => _dialogeNode.Title = value;
        }

        public string Message
        {
            get => _dialogeNode.Message;
            set => _dialogeNode.Message = value;
        }

        public string Icon
        {
            get => _dialogeNode.Icon;
            set => _dialogeNode.Icon = value;
        }

        public float Timer
        {
            get => _dialogeNode.Timer;
            set => _dialogeNode.Timer = value;
        }

        public List<string> Buttons
        {
            get => _dialogeNode.Buttons;
            set => _dialogeNode.Buttons = value;
        }


        private DialogeNode _dialogeNode;

        public EDialogeNode(DialogeNode dialogeNode) : base(dialogeNode)
        {
            _dialogeNode = dialogeNode;
        }

        public override void OnInspector()
        {
            base.OnInspector();

            EditorTools.Instance.PropertyField(_dialogeNode, "Window");

            if (Window == null)
                return;
            EditorTools.Instance.Space();

            EditorTools.Instance.LanguageField(_dialogeNode, "Title", ref _dialogeNode.Title);
            EditorTools.Instance.LanguageField(_dialogeNode, "Message", ref _dialogeNode.Message);
            EditorTools.Instance.IconField(_dialogeNode, "Icon", ref _dialogeNode.Icon);

            if (EditorTools.Instance.PropertyField(_dialogeNode, "Timer"))
                UpdateOutputSockets();

            EditorTools.Instance.Space();

            if (EditorTools.Instance.LanguageListField(_dialogeNode, "Buttons", Buttons))
                UpdateOutputSockets();

            if (Message != "")
                if (GUILayout.Button("Fill Info"))
                    Info = Message;

        }

        private void UpdateOutputSockets()
        {
            if (Timer > 0)
                Buttons.Add("Time Out");
            while (Buttons.Count < OutputSocketList.Count)
            {
                var socket = OutputSocketList[OutputSocketList.Count - 1];
                socket.Editor().Delete();
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
            if (Timer > 0)
                Buttons.RemoveAt(Buttons.Count - 1);
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }

    }
}

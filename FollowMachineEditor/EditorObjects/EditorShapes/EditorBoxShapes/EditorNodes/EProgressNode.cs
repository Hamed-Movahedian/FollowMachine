using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using MgsCommonLib.UI;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EProgressNode : ENode
    {
        private ProgressNode _progressNode;

        public MgsProgressWindow Window
        {
            get => _progressNode.Window;
            set => _progressNode.Window = value;
        }
        public string Message
        {
            get => _progressNode.Message;
            set => _progressNode.Message = value;
        }

        public ProgressNode.ProgressModeEnum ProgressMode
        {
            get => _progressNode.ProgressMode;
            set => _progressNode.ProgressMode = value;
        }


        public EProgressNode(ProgressNode progressNode) : base(progressNode)
        {
            _progressNode = progressNode;
        }

        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(_progressNode, "Window");
            if (Window == null)
                return;
            EditorTools.Instance.LanguageField(_progressNode, "Message", ref _progressNode.Message);
            EditorTools.Instance.PropertyField(_progressNode, "ProgressMode");
            if (GUILayout.Button("Fill Info"))
            {
                var camaSplit = Message.Split('/');

                Info = camaSplit[camaSplit.Length - 1] + " => " + ProgressMode;
            }
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }
       
    }
}

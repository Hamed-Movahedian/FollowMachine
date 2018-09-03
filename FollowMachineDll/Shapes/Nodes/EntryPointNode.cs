using System.Collections;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="IO/EntryPoint")]
    public class EntryPointNode : Node
    {
        private void Start()
        {
            if(Enable)
                StartCoroutine((Graph as FollowMachine)?.RunNode(this));
        }
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(this, "Enable");
            if (GUILayout.Button("Disable all but this"))
            {
                var entryPointNodes = FindObjectsOfType<EntryPointNode>();

                foreach (var entryPointNode in entryPointNodes)
                    entryPointNode.Enable = false;

                Enable = true;
            }
        }

        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
        }

        public override Node GetNextNode()
        {
            if (Enable)
                return OutputSocketList[0].GetNextNode();
            else
                return null;
        }
    }
}
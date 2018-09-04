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
        public void Start()
        {
            if(Active)
                StartCoroutine((Graph as FollowMachine)?.RunNode(this));
        }
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(this, "Active");
            if (GUILayout.Button("Disable all but this"))
            {
                var entryPointNodes = FindObjectsOfType<EntryPointNode>();

                foreach (var entryPointNode in entryPointNodes)
                    entryPointNode.Active = false;

                Active = true;
            }
        }

        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
        }

        public override Node GetNextNode()
        {
            if (Active)
                return OutputSocketList[0].GetNextNode();
            else
                return null;
        }
    }
}
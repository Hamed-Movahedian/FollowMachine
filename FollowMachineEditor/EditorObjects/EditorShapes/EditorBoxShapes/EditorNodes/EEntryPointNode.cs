using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EEntryPointNode : ENode
    {
        private EntryPointNode _entryPointNode;

        public EEntryPointNode(EntryPointNode entryPointNode) : base(entryPointNode)
        {
            _entryPointNode = entryPointNode;
        }
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(_entryPointNode, "Active");
            if (GUILayout.Button("Disable all but this"))
            {
                var entryPointNodes = Object.FindObjectsOfType<EntryPointNode>();

                foreach (var entryPointNode in entryPointNodes)
                    entryPointNode.Active = false;

                Active = true;
            }
        }

        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
        }

    }
}

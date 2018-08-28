using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    public class FollowMachineNode : Node
    {
        [Node(MenuTitle="Follow Machine")]
        public FollowMachine FollowMachine;

        public override bool IsEqualTo(Node node)
        {
            var followMachineNode = node as FollowMachineNode;

            if (followMachineNode==null)
                return false;

            return followMachineNode.FollowMachine == FollowMachine;
        }

        public override void DrawInspector()
        {
            if (EditorTools.Instance.PropertyField(this, "Info"))
                if (FollowMachine != null)
                    FollowMachine.name = Info;

            if (EditorTools.Instance.PropertyField(this, "FollowMachine"))
                UpdateFollowMachine();
        }

        #region UpdateFollowMachine

        public void UpdateFollowMachine()
        {
            if (FollowMachine == null)
                return;

            Name = FollowMachine.name;

            #region Update outputsockets

            var outputLables = GetOutputLables();

            for (int i = 0; i < outputLables.Count; i++)
            {
                if (i < OutputSocketList.Count)
                    OutputSocketList[i].Name = outputLables[i];
                else
                    AddOutputSocket<InputSocket>(outputLables[i]);
            }

            while (OutputSocketList.Count > outputLables.Count)
            {
                var socket = OutputSocketList[OutputSocketList.Count - 1];
                OutputSocketList.Remove(socket);
                socket.Delete();
            }

            #endregion

            #region Update inputsockets

            var inputLables = GetInputLables();

            for (int i = 0; i < inputLables.Count; i++)
            {
                if (i < InputSocketList.Count)
                    InputSocketList[i].Name = inputLables[i];
                else
                    AddInputSocket<OutputSocket>(inputLables[i]);
            }

            while (InputSocketList.Count > inputLables.Count)
            {
                var socket = InputSocketList[InputSocketList.Count - 1];
                InputSocketList.Remove(socket);
                socket.Delete();
            }

            #endregion

        }
        public List<string> GetInputLables()
        {
            var list = new List<string>();

            foreach (Node node in FollowMachine.NodeList)
                if (node is InputNode)
                    list.Add(node.Name);

            return list;
        }

        public List<string> GetOutputLables()
        {
            var list = new List<string>();

            foreach (Node node in FollowMachine.NodeList)
                if (node is OutputNode)
                    list.Add(node.Name);

            return list;
        }


        #endregion

        protected override void Initialize()
        {
        }

        public override IEnumerator Run()
        {
            if (FollowMachine != null)
                return FollowMachine.RunInputNode(EnteredSocket.Name);
            else
                return null;
        }

        public override void OnShow()
        {
            base.OnShow();

            if (FollowMachine != null)
                Info = FollowMachine.name;

            UpdateFollowMachine();
        }

        public override Node GetNextNode()
        {
            if (FollowMachine != null)
                if (FollowMachine.LastRunningNode != null)
                {
                    if (FollowMachine.LastRunningNode is OutputNode)
                        foreach (var socket in OutputSocketList)
                            if (socket.Name == FollowMachine.LastRunningNode.Name)
                                return socket.GetNextNode();
                }

            return null;
        }

        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            base.DoubleClick(mousePosition, currentEvent);
            EditorTools.Instance.AddFollowMachine(FollowMachine);
        }
    }
}
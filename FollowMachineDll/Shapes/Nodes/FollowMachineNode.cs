using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle = "Follow Machine")]
    public class FollowMachineNode : Node
    {
        public FollowMachine FollowMachine;

        public override bool IsEqualTo(Node node)
        {
            var followMachineNode = node as FollowMachineNode;

            if (followMachineNode == null)
                return false;

            return followMachineNode.FollowMachine == FollowMachine;
        }

        public override void OnInspector()
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

            Info = FollowMachine.name;

            #region Update outputsockets

            var outputLables = GetOutputLables();

            for (int i = 0; i < outputLables.Count; i++)
            {
                if (i < OutputSocketList.Count)
                    OutputSocketList[i].Info = outputLables[i];
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
                    InputSocketList[i].Info = inputLables[i];
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
                    list.Add(node.Info);

            return list;
        }

        public List<string> GetOutputLables()
        {
            var list = new List<string>();

            foreach (Node node in FollowMachine.NodeList)
                if (node is OutputNode)
                    if (!list.Contains(node.Info))
                        list.Add(node.Info);

            return list;
        }


        #endregion

        protected override void Initialize()
        {
        }

        protected override IEnumerator Run()
        {
            if (FollowMachine == null)
                yield break;

            FollowMachine.IsRunning = true;

            StartCoroutine(FollowMachine.RunInputNode(EnteredSocket.Info));

            yield return new WaitWhile(() => FollowMachine.IsRunning);
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
                if (FollowMachine.RunningNode != null)
                {
                    if (FollowMachine.RunningNode is OutputNode)
                        foreach (var socket in OutputSocketList)
                            if (socket.Info == FollowMachine.RunningNode.Info)
                                return socket.GetNextNode();
                }

            return null;
        }

        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            EditorTools.Instance.AddFollowMachine(FollowMachine);
        }
    }
}
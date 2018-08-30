using System;
using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.UI;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle= "UI/WindowNode")]
    public class WindowNode : Node
    {
        public MgsUIWindow Window;
        public bool ShowWindow=true;
        public bool HideWindow=true;

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");
        }
        public override void OnInspector()
        {
            base.OnInspector();
            if(EditorTools.Instance.PropertyField(this, "Window"))
                UpdateOutputSockets();
            if(Window==null)
                return;
            EditorTools.Instance.PropertyField(this, "ShowWindow");
            EditorTools.Instance.PropertyField(this, "HideWindow");

        }
        public override void OnShow()
        {
            base.OnShow();
            UpdateOutputSockets();
            if(Window!=null)
                Window.OnChange += UpdateOutputSockets;

        }

        #region UpdateOutputSockets

        public void UpdateOutputSockets()
        {
            if (Window == null)
                return;

            Name = Window.name;

            #region Update outputsockets

            var outputLables = Window.ActionList;

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
        }

        #endregion

        public override IEnumerator Run()
        {
            if (Window == null)
                throw new Exception("Error in window node! Window is'nt set");
            return Window.WaitForClose(ShowWindow, HideWindow);
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (socket.Name == Window.Result)
                    return socket.GetNextNode();
            return null;
        }
    }
}
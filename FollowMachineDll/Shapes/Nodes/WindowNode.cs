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
        public bool HideWindow=true;

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("Wait only");
            AddInputSocket<OutputSocket>("Show and Wait");
        }
        public override void OnInspector()
        {
            base.OnInspector();
            if(EditorTools.Instance.PropertyField(this, "Window"))
                UpdateOutputSockets();
            if(Window==null)
                return;
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
            if(InputSocketList.Count==1)
            {
                InputSocketList[0].Info = "Wait only";
                AddInputSocket<OutputSocket>("Show and Wait");
            }
            if (Window == null)
                return;

            Info = Window.name;

            #region Update outputsockets

            var outputLables = Window.ActionList;

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
        }

        #endregion

        protected override IEnumerator Run()
        {
            if (Window == null)
                throw new Exception("Error in window node! Window is'nt set");

            if(EnteredSocket==InputSocketList[0]) 
                return Window.WaitForClose(false, HideWindow);
            else
                return Window.WaitForClose(true, HideWindow);
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
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
    [Node(MenuTitle = "UI/WindowNode")]
    public class WindowNode : Node
    {
        public MgsUIWindow Window;
        public bool Refresh = true;
        public List<bool> HideList = new List<bool>();

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("Wait only");
            AddInputSocket<OutputSocket>("Show and Wait");
        }
        public override void OnInspector()
        {
            base.OnInspector();
            if (EditorTools.Instance.PropertyField(this, "Window"))
                UpdateOutputSockets();
            if (Window == null)
                return;
            EditorTools.Instance.PropertyField(this, "Refresh");

            // ****************************** Hide list
            
            SetHideListFromWindowActionList();
            
            // Hide all
            if (GUILayout.Button("Hide all"))
                for (int i = 0; i < HideList.Count; i++)
                    HideList[i] = true;

            // Show all
            if (GUILayout.Button("Show all"))
                for (int i = 0; i < HideList.Count; i++)
                    HideList[i] = false;

            // display Hide list
            GUILayout.Label("Hide window by outputs:");
            for (int i = 0; i < HideList.Count; i++)
                HideList[i] = GUILayout.Toggle(HideList[i], Window.ActionList[i]);
            UpdateOutputSockets();
        }

        private void SetHideListFromWindowActionList()
        {
            // remove extra hidelist elements
            while (HideList.Count > Window.ActionList.Count)
                HideList.RemoveAt(HideList.Count - 1);

            // add shortcoming elements
            while (HideList.Count < Window.ActionList.Count)
            {
                HideList.Add(true);
            }
        }

        public override void OnShow()
        {
            base.OnShow();
            UpdateOutputSockets();
            if (Window != null)
                Window.OnChange += UpdateOutputSockets;

        }

        public override void MoveSocket(InputSocket socket, int delta)
        {
            if(Window==null)
                return;

            var index = OutputSocketList.IndexOf(socket);

            if (index == -1)
                return;

            if(index+delta<0)
                return;

            if(index+delta>=OutputSocketList.Count)
                return;

            OutputSocketList[index] = OutputSocketList[index + delta];
            OutputSocketList[index + delta] = socket;

            var action = Window.ActionList[index];
            Window.ActionList[index] = Window.ActionList[index + delta];
            Window.ActionList[index + delta] = action;

            var hide = HideList[index];
            HideList[index] = HideList[index + delta];
            HideList[index + delta] = hide;

        }

        #region UpdateOutputSockets

        public void UpdateOutputSockets()
        {
            if (InputSocketList.Count == 1)
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

                OutputSocketList[i].Icon = !HideList[i] ? NodeSetting.ShowIcon : null;
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

            if (Refresh)
                Window.Refresh();

            if (EnteredSocket == InputSocketList[0])
                yield return  Window.WaitForClose(false, false);
            else
                yield return Window.WaitForClose(true, false);


            for (var i = 0; i < OutputSocketList.Count; i++)
                if (OutputSocketList[i].Info == Window.Result)
                    if (HideList[i])
                        yield return Window.Hide();
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (socket.Info == Window.Result)
                    return socket.GetNextNode();
            return null;
        }
        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            
            if (Window!=null)
                EditorTools.Instance.OpenScript(Window);
        }
    }
}
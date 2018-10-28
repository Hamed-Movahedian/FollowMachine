using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using MgsCommonLib.UI;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EWindowNode : ENode
    {
        private WindowNode _windowNode;

        public EWindowNode(WindowNode windowNode) : base(windowNode)
        {
            _windowNode = windowNode;
        }

        public MgsUIWindow Window
        {
            get => _windowNode.Window;
            set => _windowNode.Window = value;
        }
        public bool Refresh
        {
            get => _windowNode.Refresh;
            set => _windowNode.Refresh = value;
        }

        public List<bool> HideList
        {
            get => _windowNode.HideList;
            set => _windowNode.HideList = value;
        }
        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("Wait only");
            AddInputSocket<OutputSocket>("Show and Wait");
        }
        public override void OnInspector()
        {
            base.OnInspector();
            if (EditorTools.Instance.PropertyField(_windowNode, "Window"))
                UpdateOutputSockets();
            if (Window == null)
                return;
            EditorTools.Instance.PropertyField(_windowNode, "Refresh");

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
            if (Window == null)
                return;

            var index = OutputSocketList.IndexOf(socket);

            if (index == -1)
                return;

            if (index + delta < 0)
                return;

            if (index + delta >= OutputSocketList.Count)
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
                socket.Editor().Delete();
            }

            #endregion
        }

        #endregion
        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {

            if (Window != null)
                EditorTools.Instance.OpenScript(Window);
        }

    }
}

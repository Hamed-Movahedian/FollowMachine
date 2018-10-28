using System.Collections.Generic;
using System.Linq;
using FMachine;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Shapes;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public abstract class ENode : EBoxShape
    {
        public List<OutputSocket> InputSocketList
        {
            get => _node.InputSocketList;
            set => _node.InputSocketList = value;
        }
        public List<InputSocket> OutputSocketList
        {
            get => _node.OutputSocketList;
            set => _node.OutputSocketList = value;
        }

        public InputSocket DefaultOutputSocket
        {
            get => _node.DefaultOutputSocket;
            set => _node.DefaultOutputSocket = value;
        }

        public bool Active
        {
            get => _node.Active;
            set => _node.Active = value;
        }

        private readonly Node _node;

        #region Privates
        private bool _isDraging;
        private bool _isResizing;

        #endregion

        #region NodeSetting

        private NodeSetting _nodeSetting = null;

        public NodeSetting NodeSetting =>
            _nodeSetting ??
                (_nodeSetting = (NodeSetting)EditorTools.Instance.GetAsset("NodeSetting", typeof(NodeSetting)));

        #endregion

        #region Specific node setting

        private SpcificNodeSetting _spcificNodeSetting;

        public SpcificNodeSetting SpcificNodeSetting => _spcificNodeSetting ??
                                          (_spcificNodeSetting = (SpcificNodeSetting)EditorTools.Instance.GetAsset(_node.GetType().Name, typeof(SpcificNodeSetting)));
        #endregion

        #region Properties
        public bool IsRunningNode => Graph.RunningNode == _node;
        public bool IsLastRunningNode => Graph.LastRunningNode == _node;
        public OutputSocket EnteredSocket
        {
            get => _node.EnteredSocket;
            set => _node.EnteredSocket = value;
        }

        public Group Group
        {
            get
            {
                foreach (var @group in Graph.GroupList)
                {
                    if (group.Editor().ContainNode(_node))
                        return group;
                }

                return null;
            }
        }

        #endregion

        // ************************** Methods
        #region Add Sockets

        public void AddOutputSocket<T>(string label) where T : InputSocket
        {
            var socket = (T)Graph.Editor().Repository.CreateSocket(_node, typeof(T));
            socket.Info = label;
            OutputSocketList.Add(socket);
        }

        protected void AddInputSocket<T>(string success) where T : OutputSocket
        {
            var socket = (T)Graph.Editor().Repository.CreateSocket(_node, typeof(T));
            socket.Info = success;
            InputSocketList.Add(socket);
        }

        #endregion

        #region Mouse Events
        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                _isResizing = Rect.xMax - mousePosition.x < 20;

                if (_isResizing)
                    return;

                if (!IsSelected)
                {
                    if (!currentEvent.shift)
                        Graph.Editor().DeselectAll();

                    Select();
                }
                _isDraging = false;
            }
            else if (currentEvent.button == 1)
            {
                EditorTools.Instance.ShowContexMenu(_node);
            }
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                if (_isResizing)
                {
                    _node.Rect.xMax = mousePosition.x;
                }
                else
                {
                    if (!_isDraging && currentEvent.alt && currentEvent.shift)
                        Graph.Editor().RemoveSelectedNodesFromAllGroups();

                    _isDraging = true;

                    Graph.Editor().MoveSelectedNodes(delta);
                }
            }
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                if (!_isDraging)
                {
                    if (!currentEvent.shift)
                        Graph.Editor().DeselectAll();

                    if (!IsSelected)
                        Select();
                }
                else
                {
                    Graph.Editor().EndNodeMove();
                    if (currentEvent.alt && currentEvent.shift)
                        Graph.Editor().AddSelectedNodeToGroups(mousePosition);
                }
            }
        }

        #endregion

        #region Hover

        public override void MouseEnter(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = true;
            foreach (var node in Graph.NodeList)
            {
                if (node.Editor().IsEqualTo(_node))
                    node.IsHover = true;
            }
        }

        public override void MouseExit(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = false;
            foreach (var node in Graph.NodeList)
            {
                if (node.Editor().IsEqualTo(_node))
                    node.IsHover = false;
            }
        }


        public virtual bool IsEqualTo(Node node)
        {
            return false;
        }
        #endregion

        #region Draw
        public override void Draw()
        {
            var pos = Rect.position;
            float headerHeight;
            float infoHeight;
            float bodyHeight;

            // **************  Header
            NodeSetting.Header.Style.CalcMinMaxWidth(new GUIContent(SpcificNodeSetting.Title), out var minWith, out var headerWidth);
            _node.Rect.width = Mathf.Max(Rect.width, headerWidth);
            headerHeight = DrawSection(pos, headerWidth, SpcificNodeSetting.HeaderColor, NodeSetting.Header, SpcificNodeSetting.Title, -1);
            pos.y += headerHeight;

            // **************  Info
            infoHeight = DrawSection(pos, Rect.width, NodeSetting.InfoColor, NodeSetting.Info, Info, -1);
            pos.y += infoHeight;

            // Input Sockets
            if (InputSocketList.Count == 1 && InputSocketList[0].Info == "")
            {
                InputSocketList[0].Rect.position = new Vector2(
                    pos.x - NodeSetting.InputSocketSetting.Offset.x,
                    pos.y - infoHeight / 2 - InputSocketList[0].Rect.height / 2);

                bodyHeight = NodeSetting.Body.Style.border.vertical;
            }
            else
            {
                for (int i = 0; i < InputSocketList.Count; i++)
                {
                    InputSocketList[i].Rect.position = new Vector2(
                        pos.x - NodeSetting.InputSocketSetting.Offset.x,
                        pos.y + NodeSetting.InputSocketSetting.Space * (i + 0.5f) - InputSocketList[0].Rect.height / 2);
                }

                bodyHeight = NodeSetting.InputSocketSetting.Space * (InputSocketList.Count + 0.5f);
            }

            // Output Sockets
            if (OutputSocketList.Count == 1 && OutputSocketList[0].Info == "")
            {
                OutputSocketList[0].Rect.position = new Vector2(
                    pos.x + Rect.width + NodeSetting.OutputSocketSetting.Offset.x,
                    pos.y - infoHeight / 2 - OutputSocketList[0].Rect.height / 2);

                bodyHeight = Mathf.Max(bodyHeight, NodeSetting.Body.Style.border.vertical);
            }
            else
            {
                for (int i = 0; i < OutputSocketList.Count; i++)
                {
                    OutputSocketList[i].Rect.position = new Vector2(
                        pos.x + Rect.width + NodeSetting.OutputSocketSetting.Offset.x,
                        pos.y + NodeSetting.OutputSocketSetting.Space * (i + 0.5f) - OutputSocketList[0].Rect.height / 2);
                }

                bodyHeight = Mathf.Max(bodyHeight, NodeSetting.OutputSocketSetting.Space * (OutputSocketList.Count + 0.5f));
            }

            // *************** Body
            _node.Rect.yMax = pos.y + DrawSection(pos, Rect.width, NodeSetting.BodyColor, NodeSetting.Body, "", bodyHeight);

            // *************** Icon
            if (SpcificNodeSetting.Icon != null)
            {
                var iconRect = new Rect(NodeSetting.IconRect);
                iconRect.position += Rect.position;
                EditorTools.Instance.DrawTexture(iconRect, SpcificNodeSetting.Icon, SpcificNodeSetting.IconColor);

            }

            if (DefaultOutputSocket == null)
            {
                DefaultOutputSocket = (InputSocket)Graph.Editor().Repository.CreateSocket(_node, typeof(InputSocket));
            }

            DefaultOutputSocket.Rect.position = new Vector2(
                Rect.x + NodeSetting.Body.Style.border.left - NodeSetting.OutputSocketSetting.Offset.x * 2,
                Rect.yMax + NodeSetting.OutputSocketSetting.Offset.y);

        }

        private float DrawSection(Vector2 pos, float with, Color fillColor, SectionSetting setting, string text,
            float height)
        {

            var rect = new Rect(pos, Rect.size)
            {
                position = pos,
                width = with,
                height =
                    height != -1 ?
                        height :
                        text == "" ?
                            setting.Style.border.vertical :
                            setting.Style.CalcHeight(new GUIContent(text), Rect.size.x)
            };



            EditorTools.Instance.DrawTexture(rect, setting.GlowTexture, setting.Style,
                IsSelected ?
                    NodeSetting.GlowSelected :
                    IsHover ?
                        NodeSetting.GlowHover :
                        NodeSetting.GlowNormal);

            EditorTools.Instance.DrawTexture(rect, setting.LineTexture, setting.Style,
                IsRunningNode ?
                    NodeSetting.LineRunning :
                    IsSelected ?
                        NodeSetting.LineSelected :
                        IsHover ?
                            NodeSetting.LineHover :
                            NodeSetting.LineNormal);

            EditorTools.Instance.DrawTexture(rect, setting.FillTexture, setting.Style, fillColor);

            if (text != "")
                GUI.Box(rect, text, setting.Style);

            return rect.height;
        }



        #endregion


        #region Initialize
        public override void OnCreate(Graph graph, Vector2 position)
        {
            base.OnCreate(graph, position);

            _node.Rect.size = NodeSetting.Size;

            _node.name = base.Info + " (" + _node.GetType().Name + ")";

            DefaultOutputSocket = (InputSocket)Graph.Editor().Repository.CreateSocket(_node, typeof(InputSocket));
            DefaultOutputSocket.AutoHide = true;
            Initialize();
        }

        public override void Move(Vector2 delta)
        {
            base.Move(delta);
            InputSocketList.ForEach(socket => socket.Editor().Move(delta));
            OutputSocketList.ForEach(socket => socket.Editor().Move(delta));
        }

        protected abstract void Initialize();
        #endregion

        public virtual void OnShow()
        {
            DefaultOutputSocket.AutoHide = true;
        }

        #region Delete, Duplicate
        public override void Delete()
        {
            InputSocketList.ForEach(socket => socket.Editor().Delete());
            OutputSocketList.ForEach(socket => socket.Editor().Delete());

            Graph.Editor().RemoveNode(_node);

            Undo.DestroyObjectImmediate(_node);
        }
        public Node Duplicate()
        {
            // duplicate node game object (with node, input/outputsockets, edges)
            Node newNode = Object.Instantiate(_node.gameObject).GetComponent<Node>();

            // parent new node to repository
            Undo.SetTransformParent(newNode.transform,_node.transform.parent,"");

            // move new node a bit
            newNode.Editor().Move(Vector2.one * 150);

            // clear output socket, delete output edges
            newNode.OutputSocketList.ForEach(s => s.Editor().Disconnect());

            // clear input sockets
            newNode.InputSocketList.ForEach(inSocket => inSocket.EdgeList.Clear());

            // Deselect source node
            IsSelected = false;

            return newNode;
        }
        #endregion

        public virtual void MoveSocket(InputSocket socket, int i)
        {

        }

        public void SetGraph(Graph graph)
        {
            Undo.RegisterCompleteObjectUndo(_node,"");
            Undo.RegisterCompleteObjectUndo(InputSocketList.Cast<Object>().ToArray(),"");
            Undo.RegisterCompleteObjectUndo(OutputSocketList.Cast<Object>().ToArray(),"");

            Graph = graph;
            InputSocketList.ForEach(socket => socket.Graph = graph);
            OutputSocketList.ForEach(socket => socket.Graph = graph);
        }

        protected ENode(Node node) : base(node)
        {
            _node = node;
        }

    }
}

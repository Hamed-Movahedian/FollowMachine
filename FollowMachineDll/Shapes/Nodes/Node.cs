using System;
using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FMachine.Utility;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Shapes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    public abstract class Node : BoxShape
    {
        #region Public fields

        public List<OutputSocket> InputSocketList = new List<OutputSocket>();
        public List<InputSocket> OutputSocketList = new List<InputSocket>();
        public InputSocket DefaultOutputSocket;


        #endregion

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
        public bool Active = true;

        public SpcificNodeSetting SpcificNodeSetting => _spcificNodeSetting ??
                                          (_spcificNodeSetting = (SpcificNodeSetting)EditorTools.Instance.GetAsset(GetType().Name, typeof(SpcificNodeSetting)));
        #endregion

        #region Properties
        public bool IsRunningNode => Graph.RunningNode == this;
        public bool IsLastRunningNode => Graph.LastRunningNode == this;
        public OutputSocket EnteredSocket { get; set; }

        public Group Group
        {
            get
            {
                foreach (var @group in Graph.GroupList)
                {
                    if (group.ContainNode(this))
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
            var socket = (T)Graph.Repository.CreateSocket(this, typeof(T));
            socket.Info = label;
            OutputSocketList.Add(socket);
        }

        protected void AddInputSocket<T>(string success) where T : OutputSocket
        {
            var socket = (T)Graph.Repository.CreateSocket(this, typeof(T));
            socket.Info = success;
            InputSocketList.Add(socket);
        }

        #endregion

        #region Mouse Events
        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button==0)
            {
                _isResizing = Rect.xMax - mousePosition.x < 20;

                if (_isResizing)
                    return;

                if (!IsSelected)
                {
                    if (!currentEvent.shift)
                        Graph.DeselectAll();

                    Select();
                }
                _isDraging = false; 
            }
            else if (currentEvent.button == 1)
            {
                EditorTools.Instance.ShowContexMenu(this);
            }
        }
        
        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button==0)
            {
                if (_isResizing)
                {
                    Rect.xMax = mousePosition.x;
                }
                else
                {
                    if (!_isDraging && currentEvent.alt && currentEvent.shift)
                        Graph.RemoveSelectedNodesFromAllGroups();

                    _isDraging = true;

                    Graph.MoveSelectedNodes(delta);
                } 
            }
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button==0)
            {
                if (!_isDraging)
                {
                    if (!currentEvent.shift)
                        Graph.DeselectAll();

                    if (!IsSelected)
                        Select();
                }
                else
                {
                    Graph.EndNodeMove();
                    if (currentEvent.alt && currentEvent.shift)
                        Graph.AddSelectedNodeToGroups(mousePosition);
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
                if (node.IsEqualTo(this))
                    node.IsHover = true;
            }
        }

        public override void MouseExit(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = false;
            foreach (var node in Graph.NodeList)
            {
                if (node.IsEqualTo(this))
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
            Rect.width = Mathf.Max(Rect.width, headerWidth);
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
            Rect.yMax = pos.y + DrawSection(pos, Rect.width, NodeSetting.BodyColor, NodeSetting.Body, "", bodyHeight);

            // *************** Icon
            if (SpcificNodeSetting.Icon != null)
            {
                var iconRect = new Rect(NodeSetting.IconRect);
                iconRect.position += Rect.position;
                EditorTools.Instance.DrawTexture(iconRect, SpcificNodeSetting.Icon, SpcificNodeSetting.IconColor);

            }

            if (DefaultOutputSocket==null)
            {
                DefaultOutputSocket = (InputSocket)Graph.Repository.CreateSocket(this, typeof(InputSocket));
            }

            DefaultOutputSocket.Rect.position = new Vector2(
                Rect.x + NodeSetting.Body.Style.border.left - NodeSetting.OutputSocketSetting.Offset.x*2,
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

            Rect.size = NodeSetting.Size;

            name = base.Info + " (" + GetType().Name + ")";

            DefaultOutputSocket = (InputSocket)Graph.Repository.CreateSocket(this, typeof(InputSocket));
            DefaultOutputSocket.AutoHide = true;
            Initialize();
        }

        public override void Move(Vector2 delta)
        {
            base.Move(delta);
            InputSocketList.ForEach(socket=>socket.Move(delta));
            OutputSocketList.ForEach(socket=>socket.Move(delta));
        }

        protected abstract void Initialize();
        #endregion

        #region Run
        internal IEnumerator RunBase()
        {
            Graph.RunningNode = this;
            yield return Run();

            if (DefaultOutputSocket.IsConnected)
                yield return (Graph as FollowMachine).RunNode(DefaultOutputSocket.GetNextNode());

            
        }
        protected virtual IEnumerator Run()
        {
            return null;
        }
        public abstract Node GetNextNode();


        #endregion
        public virtual void OnShow()
        {
            DefaultOutputSocket.AutoHide = true;
        }

        #region Delete, Duplicate
        public override void Delete()
        {
            InputSocketList.ForEach(socket => socket.Delete());
            OutputSocketList.ForEach(socket => socket.Delete());

            Graph.RemoveNode(this);

            DestroyImmediate(gameObject);
        }
        public Node Duplicate()
        {
            // duplicate node game object (with node, input/outputsockets, edges)
            Node newNode = Instantiate(gameObject).GetComponent<Node>();

            // parent new node to repository
            newNode.transform.parent = transform.parent;

            // move new node a bit
            newNode.Move(Vector2.one * 150);

            // clear output socket, delete output edges
            newNode.OutputSocketList.ForEach(s => s.Disconnect());

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
            Graph = graph;
            InputSocketList.ForEach(socket=>socket.Graph=graph);
            OutputSocketList.ForEach(socket=>socket.Graph=graph);
        }
    }
}
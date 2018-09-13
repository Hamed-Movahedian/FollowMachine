using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FollowMachineDll.Shapes
{
    public class Group : BoxShape
    {
        #region Publics

        public List<Node> NodeList=new List<Node>();
        public Color Color=Color.white;

        #endregion

        #region Privates
        private bool _isDraging;

        #endregion

        #region GroupSetting

        private GroupSetting _groupSetting = null;

        public GroupSetting GroupSetting =>
            _groupSetting ??
            (_groupSetting = (GroupSetting)EditorTools.Instance.GetAsset("GroupSetting", typeof(GroupSetting)));

        #endregion

        #region Delete


        public override void Delete()
        {
            Graph.GroupList.Remove(this);

            DestroyImmediate(gameObject);
        }
        #endregion

        #region Draw
        public override void Draw()
        {
            if (NodeList.Count>0)
            {
                Rect.size=Vector2.zero;
                foreach (var node in NodeList)
                {
                    if (Rect.size == Vector2.zero)
                        Rect = node.Rect;
                    else
                        Rect = Rect.Expand(node.Rect);
                }
                Rect.min-=Vector2.one*50;
                Rect.max+=Vector2.one*50;
                
            }
            else
            {
                Rect.size=Vector2.one*200;
            }

            GUI.Box(Rect, Info, GroupSetting.Style);
        }

        #endregion

        #region OnInspector
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(this, "Color");
        }

        #endregion

        #region Mouse Events

        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            var height = Mathf.Abs(GroupSetting.Style.padding.top);

            if (Rect.xMin<mousePosition.x && mousePosition.x<Rect.xMax)
                if (Rect.y - height < mousePosition.y && mousePosition.y < Rect.y)
                    return this;

            return null;
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (!IsSelected)
            {
                Graph.DeselectAllNodes();

                if (!currentEvent.shift)
                    Graph.DeselectAllGroups();

                Select();
            }
            _isDraging = false;

        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            _isDraging = true;
            Graph.MoveNodes(NodeList,delta);
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            if (!_isDraging)
            {
                if (!currentEvent.shift)
                    Graph.DeselectAllGroups();

                if (!IsSelected)
                    Select();

            }
            else
            {
                Graph.EndGroupMove();
            }
        }

        #endregion

        #region Move

        public override void Move(Vector2 delta)
        {
            base.Move(delta);
            Graph.MoveNodes(NodeList,delta);
        }

        public override void EndMove()
        {
            base.EndMove();
        }

        #endregion

        #region OnCreate
        public void OnCreate(Graph graph, List<Node> nodes)
        {
            base.OnCreate(graph);
            Info = "Group";
            NodeList = nodes;
            name = base.Info + " (" + GetType().Name + ")";
        }

        #endregion

        #region Add/Remove nodes

        public void RemoveNode(Node node)
        {
            if (NodeList.Contains(node))
                NodeList.Remove(node);
        }
        #endregion

    }
}

using System.Collections.Generic;
using FMachine;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.SettingScripts;
using FollowMachineDll.Shapes;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes
{
    public class EGroup : EBoxShape
    {
        private readonly Group _group;

        public EGroup(Group @group) : base(group)
        {
            _group = @group;
        }

        #region Publics

        public List<Node> NodeList
        {
            get => _group.NodeList;
            set => _group.NodeList = value;
        }
        public Color Color
        {
            get => _group.Color;
            set => _group.Color = value;
        }

        #endregion

        #region Privates

        private bool _isDraging;

        #endregion
        #region GroupSetting

        private GroupSetting _groupSetting = null;

        public GroupSetting GroupSetting =>
            _groupSetting ??
            (_groupSetting = SettingController.Instance.GetAsset<GroupSetting>("GroupSetting"));

        #endregion

        #region Delete


        public override void Delete()
        {
            Undo.RegisterCompleteObjectUndo(Graph,"Delete Group");

            Graph.GroupList.Remove(_group);

            Undo.DestroyObjectImmediate(_group.gameObject);
        }
        #endregion

        #region Draw
        public override void Draw()
        {
            if (NodeList.Count > 0)
            {
                _group.Rect.size = Vector2.zero;
                foreach (var node in NodeList)
                {
                    if (Rect.size == Vector2.zero)
                        Rect = node.Rect;
                    else
                        Rect = Rect.Expand(node.Rect);
                }
                _group.Rect.min -= Vector2.one * 50;
                _group.Rect.max += Vector2.one * 50;

            }
            else
            {
                _group.Rect.size = Vector2.one * 200;
            }

            var backgroundColor = GUI.backgroundColor;
            var contentColor = GUI.contentColor;

            GUI.backgroundColor = Color;
            GUI.contentColor = IsSelected ? Color.red : Color;

            GroupSetting.SetupZoom(Graph.Zoom);
            GUI.Box(Rect, Info, GroupSetting.TempStyle);

            GUI.contentColor = contentColor;
            GUI.backgroundColor = backgroundColor;
        }

        #endregion

        #region OnInspector
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(_group, "Color");
        }

        #endregion

        #region Mouse Events

        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            var height = Mathf.Abs(GroupSetting.TempStyle.padding.top);

            if (Rect.xMin < mousePosition.x && mousePosition.x < Rect.xMax)
                if (Rect.y - height < mousePosition.y && mousePosition.y < Rect.y)
                    return this;

            return null;
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (!IsSelected)
            {
                Graph.Editor().DeselectAll();

                if (!currentEvent.shift)
                    Graph.Editor().DeselectAllGroups();

                Select();
            }
            _isDraging = false;

        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            _isDraging = true;
            Graph.Editor().MoveNodes(NodeList, delta);
        }

        public override void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            if (!_isDraging)
            {
                if (!currentEvent.shift)
                    Graph.Editor().DeselectAllGroups();

                if (!IsSelected)
                    Select();

            }
            else
            {
                Graph.Editor().EndGroupMove();
            }
        }

        #endregion

        #region Move

        public override void Move(Vector2 delta)
        {
            base.Move(delta);
            Graph.Editor().MoveNodes(NodeList, delta);
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
            _group.name = base.Info + " (" + GetType().Name + ")";
            IsSelected = true;
        }

        #endregion

        #region Add/Remove nodes

        public void RemoveNode(Node node)
        {
            if (NodeList.Contains(node))
            {
                Undo.RegisterCompleteObjectUndo(_group,"");
                NodeList.Remove(node);
            }
        }

        public void AddNode(Node node)
        {
            if (!NodeList.Contains(node))
                NodeList.Add(node);
        }

        #endregion

        public bool ContainNode(Node node)
        {
            return NodeList.Contains(node);
        }

    }
}

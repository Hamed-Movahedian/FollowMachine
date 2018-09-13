using System;
using System.Collections.Generic;
using System.Linq;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Shapes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine
{
    public class Graph : MonoBehaviour
    {
        public Vector2 Position = new Vector2(-50000, -50000);
        public float Zoom = 1;

        #region ShapeRepository

        private ShapeRepository _repository = null;

        public ShapeRepository Repository
        {
            get { return _repository ?? (_repository = new ShapeRepository(this)); }
        }

        public Node SelectedNode
        {
            get
            {
                foreach (var node in NodeList)
                    if (node.IsSelected)
                        return node;
                return null;
            }
        }

        #endregion

        public List<Node> NodeList = new List<Node>();
        public List<Group> GroupList = new List<Group>();

        #region RunningNode
        private Node _runningNode;
        public Node RunningNode
        {
            get { return _runningNode; }
            set
            {
                LastRunningNode = _runningNode;
                _runningNode = value;
            }
        }
        public Node LastRunningNode;

        #endregion

        public void DeselectAllNodes()
        {
            NodeList.ForEach(node => node.Deselect());
        }
        public void DeselectAllGroups()
        {
            GroupList.ForEach(g => g.Deselect());
        }


        public void MoveSelectedNodes(Vector2 delta)
        {
            foreach (Node node in NodeList)
            {
                if (node.IsSelected)
                {
                    EditorTools.Instance.Undo_RecordObject(node, "Move Nodes");
                    node.Move(delta);
                }
            }

        }
        public void MoveSelectedGroups(Vector2 delta)
        {
            foreach (var @group in GroupList)
            {
                if (@group.IsSelected)
                {
                    EditorTools.Instance.Undo_RecordObject(@group, "Move Nodes");
                    @group.Move(delta);
                }
            }

        }

        public void MoveNodes(List<Node> nodes, Vector2 delta)
        {
            foreach (Node node in nodes)
            {
                EditorTools.Instance.Undo_RecordObject(node, "Move Group");
                node.Move(delta);
            }
        }


        public void EndNodeMove()
        {
            foreach (Node node in NodeList)
            {
                if (node.IsSelected)
                {
                    EditorTools.Instance.Undo_RecordObject(node, "Move Nodes");
                    node.EndMove();
                }

            }
        }
        public void EndGroupMove()
        {
            foreach (var @group in GroupList)
            {
                if (@group.IsSelected)
                {
                    EditorTools.Instance.Undo_RecordObject(@group, "Move Group");
                    @group.EndMove();
                }

            }

        }

        public void BringToFront(Node node)
        {
            NodeList.Remove(node);
            NodeList.Add(node);
        }

        public void DeleteSelection()
        {
            //EditorTools.Instance.Undo_RecordObject(this, "Delete Node");

            var selectedNodes = new List<Node>();

            foreach (Node node in NodeList)
                if (node.IsSelected)
                    selectedNodes.Add(node);

            selectedNodes.ForEach(node => node.Delete());
        }

        public void OnShow()
        {
            foreach (var node in NodeList)
            {
                node.OnShow();
            }
        }

        public void DuplicateSelection()
        {
            var addedNodes = new List<Node>();
            foreach (var node in NodeList)
                if (node.IsSelected)
                {
                    // add new node to list
                    addedNodes.Add(node.Duplicate());
                }

            NodeList.AddRange(addedNodes);

            OnShow();
        }


        public void GroupSelection()
        {
            var selectedNodes = new List<Node>();

            foreach (var node in NodeList)
                if (node.IsSelected)
                    selectedNodes.Add(node);

            if (selectedNodes.Count>0)
                Repository.CreateGroup(selectedNodes);
        }
    }
}
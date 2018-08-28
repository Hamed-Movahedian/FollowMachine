using System;
using System.Collections.Generic;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
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
        public Node RunningNode;
        public Node LastRunningNode;

        public void DeselectAll()
        {
            NodeList.ForEach(s => s.Deselect());
        }

        public void MoveSelectedNode(Vector2 delta)
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

        public void EndMove()
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

        public void BringToFront(Node node)
        {
            NodeList.Remove(node);
            NodeList.Add(node);
        }

        public void DeleteSelection()
        {
            EditorTools.Instance.Undo_RecordObject(this, "Delete Node");

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

    }
}
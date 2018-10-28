using System;
using System.Collections.Generic;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Base;
using FollowMachineDll.Shapes;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorGraph
{
    public class EGraph : EObject
    {

        public Vector2 Position
        {
            get => _graph.Position;
            set => _graph.Position = value;
        }
        public float Zoom
        {
            get => _graph.Zoom;
            set => _graph.Zoom = value;
        }

        public List<Node> NodeList
        {
            get => _graph.NodeList;
            set => _graph.NodeList = value;
        }

        public List<Group> GroupList
        {
            get => _graph.GroupList;
            set => _graph.GroupList = value;
        }

        public Node LastRunningNode
        {
            get => _graph.LastRunningNode;
            set => _graph.LastRunningNode = value;
        }


        private readonly Graph _graph;

        #region ShapeRepository

        private ShapeRepository _repository = null;

        public ShapeRepository Repository => _repository ?? (_repository = new ShapeRepository(_graph));

        #endregion

        #region Selected Node/Graph
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
        public Group SelectedGroup
        {
            get
            {
                foreach (var @group in GroupList)
                    if (@group.IsSelected)
                        return @group;
                return null;
            }
        }
        #endregion


        public void DeselectAll()
        {
            NodeList.ForEach(node => node.Editor().Deselect());
            DeselectAllGroups();
        }
        public void DeselectAllGroups()
        {
            GroupList.ForEach(g => g.Editor().Deselect());
        }


        public void MoveSelectedNodes(Vector2 delta)
        {
            foreach (Node node in NodeList)
            {
                if (node.IsSelected)
                {
                    Undo.RecordObject(node, "Move Nodes");
                    node.Editor().Move(delta);
                }
            }

        }
        public void MoveSelectedGroups(Vector2 delta)
        {
            foreach (var @group in GroupList)
            {
                if (@group.IsSelected)
                {
                    Undo.RecordObject(@group, "Move Nodes");
                    @group.Editor().Move(delta);
                }
            }

        }

        public void MoveNodes(List<Node> nodes, Vector2 delta)
        {
            foreach (Node node in nodes)
            {
                Undo.RecordObject(node, "Move Group");
                node.Editor().Move(delta);
            }
        }


        public void EndNodeMove()
        {
            foreach (Node node in NodeList)
            {
                if (node.IsSelected)
                {
                    Undo.RecordObject(node, "Move Nodes");
                    node.Editor().EndMove();
                }

            }
        }
        public void EndGroupMove()
        {
            foreach (var @group in GroupList)
            {
                if (@group.IsSelected)
                {
                    Undo.RecordObject(@group, "Move Group");
                    @group.Editor().EndMove();
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
            var selectedNodes = SelectedNodes;

            if (selectedNodes.Count > 0)
                selectedNodes.ForEach(node => node.Editor().Delete());
            else
            {
                var selectedGroup = SelectedGroup;
                if (selectedGroup)
                    selectedGroup.Editor().Delete();
            }
        }

        public void OnShow()
        {
            foreach (var node in NodeList)
            {
                node.Editor().OnShow();
            }
        }

        public void DuplicateSelection()
        {
            var addedNodes = new List<Node>();
            foreach (var node in NodeList)
                if (node.IsSelected)
                {
                    // add new node to list
                    addedNodes.Add(node.Editor().Duplicate());
                }

            NodeList.AddRange(addedNodes);

            OnShow();
        }


        public void GroupSelection()
        {
            var selectedNodes = SelectedNodes;
            if (selectedNodes.Count > 0)
            {
                DeselectAll();
                Repository.CreateGroup(selectedNodes);
                EditorTools.Instance.FocusOnInspector();
            }
        }

        public List<Node> SelectedNodes
        {
            get
            {
                var selectedNodes = new List<Node>();

                foreach (var node in NodeList)
                    if (node.IsSelected)
                        selectedNodes.Add(node);

                return selectedNodes;
            }
        }

        public void RemoveNode(Node node)
        {
            Undo.RegisterCompleteObjectUndo(_graph,"");
            NodeList.Remove(node);

            RemoveFromGroups(node);

        }

        public void RemoveFromGroups(Node node)
        {
            // remove from groups
            foreach (var @group in GroupList)
                group.Editor().RemoveNode(node);
        }

        public void RemoveSelectedNodesFromAllGroups()
        {
            var selectedNodes = SelectedNodes;

            foreach (var selectedNode in selectedNodes)
            {
                RemoveFromGroups(selectedNode);
            }
        }

        public void AddSelectedNodeToGroups(Vector2 position)
        {
            var selectedNodes = SelectedNodes;

            foreach (var @group in GroupList)
                if (group.Rect.Contains(position))
                    foreach (var selectedNode in selectedNodes)
                        group.Editor().AddNode(selectedNode);
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                foreach (var node in NodeList)
                    foreach (var socket in node.OutputSocketList)
                        foreach (var edge in socket.EdgeList)
                        {
                            if (edge == null)
                                throw new Exception($"In node \"{node.name}.{node.Info}\" socket {socket.GetType().Name} has null edge!");
                            yield return edge;
                        }
            }
        }
        public EGraph(Graph graph)
        {
            _graph = graph;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FollowMachineEditor.Utility
{
    public class ColapseToFollowMachine
    {
        #region Instance
        private static ColapseToFollowMachine _instance;
        public static ColapseToFollowMachine Instance => _instance ?? (_instance = new ColapseToFollowMachine());

        #endregion

        #region privates

        private List<Edge> _inputEdges = new List<Edge>();
        private List<Edge> _outputEdges = new List<Edge>();
        private List<Node> _selectedNodes;
        private InputNode _inputNode;
        private OutputNode _outputNode;
        private FollowMachine _fMachine;
        private FollowMachineNode _fmNode;

        #endregion

        public void Collapse(Graph graph)
        {
            // Get selected nodes
            _selectedNodes = graph.SelectedNodes;

            // Return if no node is selected
            if (_selectedNodes.Count == 0)
                return;

            // Identify input output edges
            SetInputOutputEdges(graph);


            // Create follow machine
            CreateFollowMachine(graph);

            // Transfer selected nodes to new machine
            TransferToNewMachine(graph);


            // Connect input edges output socket to follow machine node
            foreach (var inputEdge in _inputEdges)
            {
                _inputNode.OutputSocketList[0].CreateEdge(inputEdge.OutputSocket);
                inputEdge.SetOutputSocket(_fmNode.InputSocketList[0]);
            }

            // connect output edges input socket to follow machine node
            foreach (var outputEdge in _outputEdges)
            {
                outputEdge.InputSocket.CreateEdge(_outputNode.InputSocketList[0]);

                outputEdge.SetInputSocket(_fmNode.OutputSocketList[0]);
            }

        }

        #region TransferToNewMachine
        private void TransferToNewMachine(Graph graph)
        {
            foreach (var node in _selectedNodes)
            {
                graph.Repository.Remove(node);
                _fMachine.Repository.Add(node);
            }
        }

        #endregion

        #region Create follow machine
        private void CreateFollowMachine(Graph graph)
        {
            var bRect = graph.SelectedNodes.BoundigRect();

            _fmNode =
                (FollowMachineNode)graph.Repository.CreateNode(
                    typeof(FollowMachineNode),
                    bRect.center);

            if (_fmNode != null)
            {
                _fMachine = graph.Repository.CreateFollowMachine("Follow Machine");

                _fmNode.FollowMachine = _fMachine;

                _inputNode = (InputNode)_fMachine.Repository.CreateNode(typeof(InputNode),bRect.center);


                _outputNode = (OutputNode)_fMachine.Repository.CreateNode(typeof(OutputNode),bRect.center);

                _inputNode.Move(Vector2.left* (bRect.width/2+ 300));

                _outputNode.Move(Vector2.right* (bRect.width / 2 + 200));

                _fMachine.Position = graph.Position;
                _fMachine.Zoom = graph.Zoom;

                _fmNode.OnShow();
            }
        }
        #endregion

        #region Identify input output edges
        private void SetInputOutputEdges(Graph graph)
        {
            _inputEdges.Clear();
            _outputEdges.Clear();

            foreach (var edge in graph.Edges)
            {
                var input = _selectedNodes.Contains(edge.InputNode);
                var output = _selectedNodes.Contains(edge.OutputNode);

                if (!input && output)
                    _inputEdges.Add(edge);

                if (input && !output)
                    _outputEdges.Add(edge);
            }
        }

        #endregion

    }
}

using System.Collections.Generic;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
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
            _selectedNodes = graph.Editor().SelectedNodes;

            // Return if no node is selected
            if (_selectedNodes.Count == 0)
                return;

            // Identify input output edges
            GetInputOutputEdges(graph);

            Undo.RegisterCompleteObjectUndo(graph, "Collapse");

            // remove nodes from graph
            _selectedNodes.ForEach(n=>graph.NodeList.Remove(n));

            // Create follow machine
            CreateFollowMachine(graph);

            // add node to new follow machine
            _selectedNodes.ForEach(n =>
            {
                _fMachine.NodeList.Add(n);
                n.Editor().SetGraph(_fMachine);
            });


            // Connect input edges output socket to follow machine node
            foreach (var inputEdge in _inputEdges)
            {
                _inputNode.OutputSocketList[0].Editor().CreateEdge(inputEdge.OutputSocket);
                inputEdge.Editor().SetOutputSocket(_fmNode.InputSocketList[0]);
            }

            // connect output edges input socket to follow machine node
            foreach (var outputEdge in _outputEdges)
            {
                outputEdge.InputSocket.Editor().CreateEdge(_outputNode.InputSocketList[0]);

                outputEdge.Editor().SetInputSocket(_fmNode.OutputSocketList[0]);

            }

        }

        #region Create follow machine
        private void CreateFollowMachine(Graph graph)
        {
            var bRect = _selectedNodes.BoundigRect();

            _fmNode =
                (FollowMachineNode)graph.Editor().Repository.CreateNode(
                    typeof(FollowMachineNode),
                    bRect.center);

            if (_fmNode != null)
            {
                _fMachine = graph.Editor().Repository.CreateFollowMachine("Follow Machine");

                _fmNode.FollowMachine = _fMachine;

                _inputNode = (InputNode)_fMachine.Editor().Repository.CreateNode(typeof(InputNode), bRect.center);


                _outputNode = (OutputNode)_fMachine.Editor().Repository.CreateNode(typeof(OutputNode), bRect.center);

                _inputNode.Editor().Move(Vector2.left * (bRect.width / 2 + 300));

                _outputNode.Editor().Move(Vector2.right * (bRect.width / 2 + 200));


                _fMachine.Position = graph.Position;
                _fMachine.Zoom = graph.Zoom;

                _fmNode.Editor().OnShow();
            }
        }
        #endregion

        #region Identify input output edges
        private void GetInputOutputEdges(Graph graph)
        {
            _inputEdges.Clear();
            _outputEdges.Clear();

            foreach (var edge in graph.Editor().Edges)
            {
                var input = _selectedNodes.Contains(edge.Editor().InputNode);
                var output = _selectedNodes.Contains(edge.Editor().OutputNode);

                if (!input && output)
                    _inputEdges.Add(edge);

                if (input && !output)
                    _outputEdges.Add(edge);
            }
        }

        #endregion

    }
}

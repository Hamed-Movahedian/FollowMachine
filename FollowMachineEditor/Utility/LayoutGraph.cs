using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.Utility
{
    public static class LayoutGraph
    {
        private static readonly int RearangeIterations = 70;
        private static readonly float GapSize=100;
        private static readonly float MoveStep=5;
        private static readonly Vector2 NodeBorder=new Vector2(20,20);

        public static void ReLayout(Graph graph)
        {
            // safety code
            if (graph == null)
                return;

            // get nodes and filter edges
            var nodes = graph.NodeList;
            var edges = nodes
                .SelectMany(n => n.InputSocketList.SelectMany(s => s.EdgeList))
                .Where(e => !e.AutoHide && !e.ExcludeInLayout && e.InputNode.Group == e.OutputNode.Group)
                .ToList();


            // Record undo for all nodes
            nodes.ForEach(n => Undo.RecordObject(n, "Re-layout"));

            // Rearrange X times
            for (int i = 0; i < RearangeIterations; i++)
            {
                Rarrange(nodes, edges);
            }
        }

        private static void Rarrange(List<Node> nodes, List<Edge> edges)
        {
            // process each edge 
            edges.ForEach(ProccessEdge);

            // Check collision detection
            for (int i = 0; i < nodes.Count - 1; i++)
                for (int j = i + 1; j < nodes.Count; j++)
                    CheckCollision(nodes[i], nodes[j]);
        }

        private static void CheckCollision(Node n1, Node n2)
        {
            var r1 = n1.Rect;
            var r2 = n2.Rect;

            r1.min -= NodeBorder;
            r1.max += NodeBorder;
            r2.min -= NodeBorder;
            r2.max += NodeBorder;

            if (!r1.Overlaps(r2))
                return;

            var c1 = r1.center;
            var c2 = r2.center;

            var dx = ((r1.width + r2.width) / 2 - Mathf.Abs(c1.x - c2.x)) / 2;
            var dy = ((r1.height + r2.height) / 2 - Mathf.Abs(c1.y - c2.y)) / 2;

            if (dx < dy)
            {
                if (c2.x > c1.x)
                    dx *= -1;
                n1.Move(new Vector2(dx,0));
                n2.Move(new Vector2(-dx,0));
            }
            else
            {
                if (c2.y > c1.y)
                    dy *= -1;

                n1.Move(new Vector2(0,dy));
                n2.Move(new Vector2(0,-dy));
            }
        }

        private static void ProccessEdge(Edge edge)
        {
            // get center of input and output socket
            var p1 = edge.InputSocket.Rect.center;
            var p2 = edge.OutputSocket.Rect.center;

            var c = (p1 + p2) / 2;
            var g = Vector2.right * GapSize;

            var v1 = c - g - p1;
            var v2 = c + g - p2;

            v1 = v1.normalized * Mathf.Min(v1.magnitude, MoveStep);
            v2 = v2.normalized * Mathf.Min(v2.magnitude, MoveStep);

            edge.InputNode.Move(v1);
            edge.OutputNode.Move(v2);
        }

    }
}

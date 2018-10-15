using System.Collections.Generic;
using System.Linq;
using FMachine.Shapes;
using FMachine.Shapes.Sockets;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor
{
    internal class EdgeEditor
    {
        private FMWindow _window;
        private float _tSize=100;

        public EdgeEditor(FMWindow window)
        {
            _window = window;
        }

        public IMouseInteractable IsMouseOverEdge(Edge edge, Vector2 mousePos)
        {
            Vector2 t1, t2;
            var p = new List<Vector2>();
            p.Add(edge.InputSocket.Rect.center);
            p.AddRange(edge.EditPoints.Select(ep => edge.ToAbsPos(ep)));
            p.Add(edge.OutputSocket.Rect.center);

            for (int i = 0; i < p.Count - 1; i++)
            {
                if (i == 0)
                    t1 = Vector2.right;
                else
                    t1 = (p[i + 1] - p[i - 1]).normalized;

                if (i == p.Count - 2)
                    t2 = Vector2.left;
                else
                    t2 = (p[i] - p[i + 2]).normalized;

                var source = IsOverLine(edge,mousePos,
                    p[i],
                    p[i + 1],
                    p[i] + t1 * _tSize,
                    p[i + 1] + t2 * _tSize);
                if (source != null)
                {
                    edge.ClickEditpointIndex = i;
                    return source;
                }
            }

            return null;

        }

        private static IMouseInteractable IsOverLine(Edge edge, Vector2 mousePos, Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2)
        {
            var points = Handles.MakeBezierPoints(p1, p2, t1, t2, (int) ((p1-p2).magnitude/10));
            foreach (Vector2 point in points)
                if (Vector2.SqrMagnitude(point - mousePos) < 100)
                    return edge;

            return null;
        }

        public void DrawBezierEdge(Edge edge)
        {
            DrawEdge(edge);

            if (edge.IsHover)
                foreach (var editPoint in edge.EditPoints)

                    Handles.DrawSolidDisc(edge.ToAbsPos(editPoint), Vector3.forward,
                        edge.InputSocket.SocketSetting.Thickness);
        }

        private void DrawEdge(Edge edge)
        {
            Vector2 t1, t2;
            var p = new List<Vector2>();
            p.Add(edge.InputSocket.Rect.center);
            p.AddRange(edge.EditPoints.Select(ep=> edge.ToAbsPos(ep)));
            p.Add(edge.OutputSocket.Rect.center);

            for (int i = 0; i < p.Count-1; i++)
            {
                if (i == 0)
                    t1 = Vector2.right;
                else
                    t1 = (p[i + 1] - p[i - 1]).normalized;

                if (i == p.Count - 2)
                    t2 = Vector2.left;
                else
                    t2 = (p[i ] - p[i + 2]).normalized;

                DrawBezierLine(
                    p[i],
                    p[i + 1],
                    p[i] + t1 * _tSize,
                    p[i + 1] + t2 * _tSize,
                    edge);
            }
        }


        private void DrawBezierLine(Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2, Edge edge)
        {
            bool isRunning = edge.IsRunning;

            var bColor = 
                edge.IsHover ?
                    Color.gray :
                    isRunning ? 
                        edge.InputSocket.Node.NodeSetting.LineRunning :
                        Color.black;

            Handles.DrawBezier(p1, p2, t1, t2, bColor, null, edge.InputSocket.SocketSetting.Thickness + 4);

            Handles.DrawBezier(p1, p2, t1, t2, edge.Color, null, edge.InputSocket.SocketSetting.Thickness);
        }

        public void DrawBezierEdge(Vector2 p1, Vector2 p2, Color forgroundColor, Color backColor, float thickness)
        {
            var tangent = Vector2.right * Mathf.Sign(p2.x - p1.x) * 50;

            Handles.DrawBezier(p1, p2, p1 + tangent, p2 - tangent, backColor, null, thickness + 4);
            Handles.DrawBezier(p1, p2, p1 + tangent, p2 - tangent, forgroundColor, null, thickness);
        }
    }
}
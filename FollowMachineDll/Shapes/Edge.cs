using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes
{
    public class Edge : Shape
    {
        public InputSocket InputSocket;
        public OutputSocket OutputSocket;
        public List<Vector2> EditPoints = new List<Vector2>();
        private int _dragIndex = -1;
        public int ClickEditpointIndex;

        public bool IsRunning { get { return OutputSocket.Node.IsRunningNode && InputSocket.Node.IsLastRunningNode; } }

        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            return EditorTools.Instance.IsMouseOverEdge(this, mousePosition);
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            base.MouseDown(mousePosition, currentEvent);

            for (var i = 0; i < EditPoints.Count; i++)
            {
                if ((ToAbsPos(EditPoints[i]) - mousePosition).sqrMagnitude < 100)
                {
                    if (currentEvent.alt)
                    {
                        EditPoints.RemoveAt(i);
                        _dragIndex = -1;
                    }
                    else
                        _dragIndex = i;

                    return;
                }
            }

            EditPoints.Insert(ClickEditpointIndex, ToRelativePos(mousePosition));
            _dragIndex = ClickEditpointIndex;
        }

        public Vector2 ToRelativePos(Vector3 x)
        {
            Vector3 p1 = InputSocket.Rect.center;
            Vector3 p2 = OutputSocket.Rect.center;
            var v = p2 - p1;
            var r = Quaternion.FromToRotation(v, Vector3.right);
            return r * (x - p1) / v.magnitude;

        }

        public Vector2 ToAbsPos(Vector3 x)
        {
            Vector3 p1 = InputSocket.Rect.center;
            Vector3 p2 = OutputSocket.Rect.center;
            var v = p2 - p1;
            var r = Quaternion.FromToRotation(Vector3.right, v);
            return p1 + r * x * v.magnitude;
        }

        public override void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            base.MouseDrag(delta, mousePosition, currentEvent);
            if (_dragIndex != -1)
                EditPoints[_dragIndex] = ToRelativePos(mousePosition);
        }

        public override void Draw()
        {
            if (IsHidden)
                return;
            EditorTools.Instance.DrawEdge(this);
        }

        public void SetOutputSocket(OutputSocket socket)
        {
            if (OutputSocket != null) OutputSocket.EdgeList.Remove(this);
            OutputSocket = socket;
            OutputSocket.EdgeList.Add(this);
        }

        public override void Delete()
        {
            if (InputSocket != null)
                InputSocket.EdgeList.Remove(this);

            if (OutputSocket != null)
                OutputSocket.EdgeList.Remove(this);

            DestroyImmediate(this);
        }
    }
}
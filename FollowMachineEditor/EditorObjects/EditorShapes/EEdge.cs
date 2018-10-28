using System.Collections.Generic;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes
{
    public class EEdge : EShape
    {
        private readonly Edge _edge;

        public InputSocket InputSocket
        {
            get => _edge.InputSocket;
            set => _edge.InputSocket = value;
        }
        public OutputSocket OutputSocket
        {
            get => _edge.OutputSocket;
            set => _edge.OutputSocket = value;
        }

        public List<Vector2> EditPoints
        {
            get => _edge.EditPoints;
            set => _edge.EditPoints = value;
        }

        public int ClickEditpointIndex
        {
            get => _edge.ClickEditpointIndex;
            set => _edge.ClickEditpointIndex = value;
        }

        public bool AutoHide
        {
            get => _edge.AutoHide;
            set => _edge.AutoHide = value;
        }

        public bool ExcludeInLayout
        {
            get => _edge.ExcludeInLayout;
            set => _edge.ExcludeInLayout = value;
        }

        public EEdge(Edge edge) : base(edge)
        {
            _edge = edge;
        }
        private int _dragIndex = -1;

        public bool IsRunning => OutputSocket.Node.Editor().IsRunningNode && InputSocket.Node.Editor().IsLastRunningNode;
        public Node InputNode => InputSocket.Node;
        public Node OutputNode => OutputSocket.Node;

        public Color Color => IsRunning ?
                                InputSocket.Node.Editor().NodeSetting.LineRunning :
                                !AutoHide ||
                                IsHover ||
                                InputSocket.Node.IsHover ||
                                OutputSocket.Node.IsHover ||
                                InputSocket.Node.IsSelected ||
                                OutputSocket.Node.IsSelected ?
                                    InputSocket.Editor().SocketSetting.Color :
                                    InputSocket.Editor().SocketSetting.AutoHideColor;

        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            return EditorTools.Instance.IsMouseOverEdge(_edge, mousePosition);
        }

        public override void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
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
            else if (currentEvent.button == 1)
            {
                EditorTools.Instance.ShowContexMenu(_edge);
            }
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
            if (currentEvent.button == 0)
                if (_dragIndex != -1)
                    EditPoints[_dragIndex] = ToRelativePos(mousePosition);
        }

        public override void Draw()
        {
            if (IsHidden)
                return;

            EditorTools.Instance.DrawEdge(_edge);
        }

        public void SetOutputSocket(OutputSocket socket)
        {

            if (OutputSocket != null)
            {
                Undo.RegisterCompleteObjectUndo(OutputSocket, "");
                OutputSocket.EdgeList.Remove(_edge);
            }
            Undo.RegisterCompleteObjectUndo(_edge,"");
            OutputSocket = socket;
            Undo.RegisterCompleteObjectUndo(OutputSocket, "");
            OutputSocket.EdgeList.Add(_edge);
        }

        public override void Delete()
        {
            if (InputSocket != null)
                InputSocket.Editor().RemoveEdge(_edge);

            if (OutputSocket != null)
                OutputSocket.Editor().RemoveEdge(_edge);

            Undo.DestroyObjectImmediate(_edge);
        }

        public void SetInputSocket(InputSocket socket)
        {
            if (InputSocket != null)
            {
                Undo.RegisterCompleteObjectUndo(InputSocket, "");
                InputSocket.EdgeList.Remove(_edge);
            }
            InputSocket = socket;
            Undo.RegisterCompleteObjectUndo(InputSocket, "");
            InputSocket.EdgeList.Add(_edge);
        }
    }
}

using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineEditor.CustomInspectors;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes
{
    public abstract class EBoxShape : EShape
    {
        private readonly BoxShape _boxShape;

        protected EBoxShape(BoxShape boxShape) : base(boxShape)
        {
            _boxShape = boxShape;
        }

        public Rect Rect
        {
            get => _boxShape.Rect;
            set => _boxShape.Rect = value;
        }

        public string Info
        {
            get => _boxShape.Info;
            set => _boxShape.Info = value;
        }

        // ******************* Methods

        public virtual void OnCreate(Graph graph, Vector2 position)
        {
            base.OnCreate(graph);
            _boxShape.Rect.position = position;
        }



        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            if (Rect.Contains(mousePosition))
                return this;

            return null;
        }


        protected virtual string FilterName(string name)
        {
            return name;
        }

        public override bool IsInBox(Rect rect)
        {
            return rect.Contains(Rect.min) && rect.Contains(Rect.max);
        }

        public virtual void Move(Vector2 delta)
        {
            Undo.RecordObject(_boxShape,"");
            _boxShape.Rect.position += delta;
        }

        public virtual void EndMove()
        {
            _boxShape.Rect.position = new Vector2(Mathf.Round(Rect.position.x / 10) * 10, Mathf.Round(Rect.position.y / 10) * 10);

        }

        public virtual void OnInspector()
        {
            GUIUtil.TargetNode = (Node) _boxShape;

            if (GUIUtil.TextFieldInBox("Info :", ref _boxShape.Info))
            {
                Undo.RecordObject(_boxShape,"");
                _boxShape.name = _boxShape.Info + " (" + _boxShape.GetType().Name + ")";
            }

        }
    }
}

using FMachine;
using FMachine.Shapes;
using FollowMachineDll.Base;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes
{
    public abstract class EShape : EObject, IMouseInteractable
    {
        #region Constructor
        private readonly Shape _shape;

        protected EShape(Shape shape)
        {
            _shape = shape;
        }

        #endregion

        public Graph Graph
        {
            get => _shape.Graph;
            set => _shape.Graph = value;
        }

        public bool IsSelected
        {
            get => _shape.IsSelected;
            set => _shape.IsSelected = value;
        }

        public bool IsHover
        {
            get => _shape.IsHover;
            set => _shape.IsHover = value;
        }

        protected bool IsHidden;



        public virtual void OnCreate(Graph graph)
        {
            Graph = graph;
        }
        public void Select()
        {
            if (IsSelected)
                return;

            Undo.RecordObject(_shape, "Selection Change");

            IsSelected = true;

        }

        public void Deselect()
        {
            if (!IsSelected)
                return;

            Undo.RecordObject(_shape, "Selection Change");

            IsSelected = false;
        }

        public virtual IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            return null;
        }

        public virtual void MouseDown(Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseUp(Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseEnter(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = true;
        }


        public virtual void MouseExit(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = false;
        }
        public virtual void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {

        }

        public virtual void MouseMove(Vector2 mousePosition, Event currentEvent)
        {

        }

        public virtual void Draw() { }

        public virtual bool IsInBox(Rect rect)
        {
            return false;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        public void Show()
        {
            IsHidden = false;
        }

        public abstract void Delete();

    }
}

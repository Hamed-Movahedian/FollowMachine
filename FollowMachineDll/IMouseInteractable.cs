using UnityEngine;

namespace FMachine
{
    public interface IMouseInteractable
    {
        IMouseInteractable IsMouseOver(Vector2 mousePosition);

        void MouseDown(Vector2 mousePosition, Event currentEvent);
        void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent);
        void MouseUp(Vector2 mousePosition, Event currentEvent);
        void MouseEnter(Vector2 mousePosition, Event currentEvent);
        void MouseExit(Vector2 mousePosition, Event currentEvent);
        void DoubleClick(Vector2 mousePosition, Event currentEvent);
        void MouseMove(Vector2 mousePosition, Event currentEvent);
    }
}
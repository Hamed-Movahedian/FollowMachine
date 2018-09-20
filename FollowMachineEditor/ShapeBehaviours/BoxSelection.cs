using FMachine.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Editor.ShapeBehaviours
{
    public class BoxSelection : IMouseInteractable
    {
        private Vector2 _startPos;
        private bool _show;
        private Rect _rect = new Rect();
        private FMCanvas _canvas;

        public BoxSelection(FMCanvas canvas)
        {
            _canvas = canvas;
        }

        public IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            return null;
        }

        public void MouseDown(Vector2 mousePosition, Event currentEvent)
        {
            _startPos = mousePosition;
            _canvas.Graph.DeselectAllGroups();
            MouseDrag(Vector2.zero,mousePosition,currentEvent);
        }

        public void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent)
        {
            _show = true;
            _rect.xMin = Mathf.Min(_startPos.x, mousePosition.x);
            _rect.xMax = Mathf.Max(_startPos.x, mousePosition.x);
            _rect.yMin = Mathf.Min(_startPos.y, mousePosition.y);
            _rect.yMax = Mathf.Max(_startPos.y, mousePosition.y);

            SelectionShapeInBox(_rect);
        }

        private void SelectionShapeInBox(Rect rect)
        {
            foreach (var shape in _canvas.Graph.NodeList)
            {
                bool inBox = shape.IsInBox(rect);
                if (inBox)
                    shape.Select();
                else
                    shape.Deselect();
            }
        }

        public void MouseUp(Vector2 mousePosition, Event currentEvent)
        {
            _show = false;
        }

        public void MouseEnter(Vector2 mousePosition, Event currentEvent)
        {

        }

        public void MouseExit(Vector2 mousePosition, Event currentEvent)
        {

        }

        public void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            
        }

        public void MouseMove(Vector2 mousePosition, Event currentEvent)
        {
        }

        public void Draw()
        {
            if (_show)
                EditorTools.Instance.DrawBox(_canvas.Window.Settings.BoxSelectionStyle,_rect, new GUIContent(), false, false, false, false);

        }
    }
}
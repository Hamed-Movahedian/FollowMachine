using UnityEngine;

namespace FMachine.Editor
{
    public class CordianationSystem
    {
        private const float MinZoom = 0.1f;
        private const float MaxZoom = 1.0f;

        private FMCanvas _canvas;

        public CordianationSystem(FMCanvas canvas)
        {
            _canvas = canvas;
        }

        public void Zoom(float zoomDelta)
        {
            Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);

            _canvas.Zoom = Mathf.Clamp(_canvas.Zoom + zoomDelta, MinZoom, MaxZoom);

            Vector2 newzoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);


            _canvas.Position+= newzoomCoordsMousePos - zoomCoordsMousePos;
/*
            var nextTranlationPosition = _canvas.Position + newzoomCoordsMousePos - zoomCoordsMousePos;

            if (nextTranlationPosition.x >= 0) nextTranlationPosition.x = 0;
            if (nextTranlationPosition.y >= 0) nextTranlationPosition.y = 0;

            _canvas.Position = nextTranlationPosition;
*/
        }

        private void Pan()
        {
            Vector2 delta = Event.current.delta;
            delta /= _canvas.Zoom;

            _canvas.Position += delta;

/*
            var nextTranlationPosition = _canvas.Position + delta;

            if (nextTranlationPosition.x >= 0) nextTranlationPosition.x = 0;
            if (nextTranlationPosition.y >= 0) nextTranlationPosition.y = 0;

            _canvas.Position = nextTranlationPosition;
*/
        }

        public Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
        {
            //return (screenCoords - _canvas.WindowRect.min) / _canvas.Zoom - _canvas.Position;
            var offset = new Vector2(
                screenCoords.x - _canvas.WindowRect.min.x,
                screenCoords.y - _canvas.WindowRect.min.y-2
                );
            return offset / _canvas.Zoom - _canvas.Position;
        }

        public void HandleZoomAndPan()
        {
            //******************  Zoom
            if (Event.current.type == EventType.ScrollWheel)
            {
                Zoom(-Event.current.delta.y / 150.0f);
                
                Event.current.Use();
            }

            //****************** Pan
            if (Event.current.type == EventType.MouseDrag &&
                (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
                Event.current.button == 2)
            {
                Pan();
                Event.current.Use();
            }
        }

        public void BeginDraw()
        {
            EditorZoomArea.Begin(_canvas.Zoom, _canvas.WindowRect);
            Rect drawArea= 
                new Rect(_canvas.Position.x, _canvas.Position.y, _canvas.CanvasSize, _canvas.CanvasSize);
            GUILayout.BeginArea(drawArea);

        }

        public void EndDraw()
        {
            GUILayout.EndArea();
            EditorZoomArea.End();
        }
    }
}
﻿using System.Collections.Generic;
using FMachine.Shapes.Nodes;
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
            Vector2 zoomCoordsMousePos = ScreenCoordsToZoomCoords(Event.current.mousePosition);

            _canvas.Zoom = Mathf.Clamp(_canvas.Zoom + zoomDelta, MinZoom, MaxZoom);

            Vector2 newzoomCoordsMousePos = ScreenCoordsToZoomCoords(Event.current.mousePosition);


            _canvas.Position += newzoomCoordsMousePos - zoomCoordsMousePos;
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

        }

        public Vector2 ScreenCoordsToZoomCoords(Vector2 screenCoords)
        {
            return screenCoords / _canvas.Zoom - _canvas.Position;
        }

        public Vector2 ZoomCoordsToScreenCoords(Vector2 zoomCoords)
        {
            return (zoomCoords+ _canvas.Position) * _canvas.Zoom ;
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
            Rect drawArea =
                new Rect(
                    _canvas.Position - _canvas.WindowRect.position / _canvas.Zoom,
                    Vector2.one * _canvas.CanvasSize);
            GUILayout.BeginArea(drawArea);

        }

        public void EndDraw()
        {
            GUILayout.EndArea();
            EditorZoomArea.End();
        }

        public void Focus(bool toSelection, bool zooming)
        {
            if (_canvas.Graph == null)
                return;

            var nodes = new List<Node>();
            foreach (var node in _canvas.Graph.NodeList)
                if (!toSelection || node.IsSelected)
                    nodes.Add(node);

            Focus(nodes, zooming);
        }

        public void Focus(Node node, bool zooming)
        {
            Focus(new List<Node> { node }, zooming);
        }

        public void Focus(List<Node> nodes, bool zooming)
        {
            if (_canvas.Graph == null)
                return;

            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (var node in nodes)
                if (bounds.center == Vector3.zero)
                    bounds = new Bounds(node.Rect.center, node.Rect.size);
                else
                {
                    bounds.Encapsulate(node.Rect.min);
                    bounds.Encapsulate(node.Rect.max);
                }

            if (bounds.center == Vector3.zero)
                return;


            if (zooming)
            {
                _canvas.Zoom = Mathf.Min(
                    _canvas.WindowRect.height / bounds.size.y,
                    _canvas.WindowRect.width / bounds.size.x);

                _canvas.Zoom = Mathf.Max(MinZoom, _canvas.Zoom);
                _canvas.Zoom = Mathf.Min(MaxZoom, _canvas.Zoom);
            }
            _canvas.Position = -(Vector2)bounds.center + _canvas.WindowRect.size * 0.5f / _canvas.Zoom;
        }
    }
}
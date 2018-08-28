using System;
using System.Collections.Generic;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor.ShapeBehaviours
{
    public class EventProcessor
    {
        private FMCanvas _canvas;
        private Vector2 _mousePosition;
        private bool _isMouseInCanvas;
        private IMouseInteractable _interactionSource;

        public EventProcessor(FMCanvas canvas)
        {
            _canvas = canvas;
        }

        public void ProcessEvents()
        {
            var currentEvent = Event.current;

            if (currentEvent.isMouse || _canvas.DropNode.IsCorrectEvent(currentEvent))
                ProcessMouseEvents(currentEvent);

            if (currentEvent.isKey)
                ProcessKeyboardEvents(currentEvent);

        }

        private void ProcessKeyboardEvents(Event currentEvent)
        {
            switch (currentEvent.keyCode)
            {
                case KeyCode.Delete:
                    if (!EditorGUIUtility.editingTextField)
                        _canvas.Graph.DeleteSelection();
                    break;
                case KeyCode.D:
                    if (currentEvent.control)
                        _canvas.Graph.DuplicateSelection();
                    break;
            }
        }

        private void ProcessMouseEvents(Event currentEvent)
        {
            // set isInCanvas, mousePos
            SetupMouseInfo();

            switch (currentEvent.type)
            {
                // *********************************** Mouse Down
                case EventType.MouseDown:
                    // exit if mouse not in canvas
                    if (!_isMouseInCanvas)
                        return;

                    // get interaction source
                    SetMouseInteractionSource();

                    if (currentEvent.clickCount < 2)
                        _interactionSource.MouseDown(_mousePosition, currentEvent);
                    else
                        _interactionSource.DoubleClick(_mousePosition, currentEvent);

                    Event.current.type = EventType.Layout;
                    break;

                // *********************************** Mouse Drag
                case EventType.MouseDrag:
                    if (_interactionSource != null)
                        _interactionSource.MouseDrag(
                            currentEvent.delta / _canvas.Zoom,
                            _mousePosition,
                            currentEvent);
                    break;

                // *********************************** Mouse Up
                case EventType.MouseUp:
                    if (_interactionSource != null)
                    {
                        _interactionSource.MouseUp(_mousePosition, currentEvent);
                        _interactionSource = null;
                    }

                    break;

                // *********************************** Mouse Move
                case EventType.MouseMove:
                    // exit if mouse not in canvas
                    if (!_isMouseInCanvas)
                        return;

                    var source = GetMouseInteractionSource(_mousePosition);

                    if (source != _interactionSource)
                    {
                        if (source != null)
                            source.MouseEnter(_mousePosition, currentEvent);
                        if (_interactionSource != null)
                            _interactionSource.MouseExit(_mousePosition, currentEvent);
                        _interactionSource = source;
                    }

                    if (source != null)
                        source.MouseMove(_mousePosition, currentEvent);

                    break;

                // ******************************* Drag and drop
                case EventType.DragUpdated:
                    if (_isMouseInCanvas)
                        _canvas.DropNode.Update(_mousePosition);
                    break;
                case EventType.DragPerform:
                    if (_isMouseInCanvas)
                        _canvas.DropNode.Perform(_mousePosition);
                    break;
            }
        }

        private IMouseInteractable GetMouseInteractionSource(Vector2 mousePosition)
        {
            IMouseInteractable source = null;

            foreach (var socket in _canvas.GetSockets())
            {
                source = socket.IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }
            foreach (Node node in _canvas.Graph.NodeList)
            {
                source = node.IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }

            foreach (var edge in _canvas.GetEdges())
            {
                source = edge.IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }


            return null;
        }

        private void SetMouseInteractionSource()
        {
            _interactionSource = GetMouseInteractionSource(_mousePosition);
            if (_interactionSource == null)
                _interactionSource = _canvas.BoxSelection;
        }

        private void SetupMouseInfo()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            _isMouseInCanvas = _canvas.WindowRect.Contains(mousePosition);
            _mousePosition = _canvas.CordinationSystem.ConvertScreenCoordsToZoomCoords(mousePosition);
        }
    }
}
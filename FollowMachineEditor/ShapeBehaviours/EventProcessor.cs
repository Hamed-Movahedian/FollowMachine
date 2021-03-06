﻿using FMachine.Shapes.Nodes;
using FollowMachineEditor.EditorObjectMapper;
using FollowMachineEditor.Utility;
using FollowMachineEditor.Windows.FollowMachineInspector;
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
                    if (currentEvent.type == EventType.KeyDown)
                        if (!EditorGUIUtility.editingTextField)
                            _canvas.Graph.Editor().DeleteSelection();
                    break;
                case KeyCode.D:
                    if (currentEvent.control)
                        _canvas.Graph.Editor().DuplicateSelection();
                    break;
                case KeyCode.G:
                    if (currentEvent.type == EventType.KeyDown)
                        if (currentEvent.control)
                            _canvas.Graph.Editor().GroupSelection();
                    break;
                case KeyCode.M:
                    if (currentEvent.type == EventType.KeyDown)
                        if (currentEvent.control)
                            ColapseToFollowMachine.Instance.Collapse(_canvas.Graph);
                    break;
                case KeyCode.L:
                    if (currentEvent.type == EventType.KeyDown)
                        if (currentEvent.control)
                        {
                            LayoutGraph.ReLayout(_canvas.Graph);
                        }
                    break;
                case KeyCode.F:
                    if (currentEvent.type == EventType.KeyDown)
                        if (_canvas.Graph.Editor().SelectedNode == null)
                            _canvas.CordinationSystem.Focus(false, true);
                        else
                            _canvas.CordinationSystem.Focus(true, true);
                    break;
            }
        }

        private void ProcessMouseEvents(Event currentEvent)
        {
            // set isInCanvas, mousePos
            SetupMouseInfo();

            if (!_isMouseInCanvas)
                return;


            switch (currentEvent.type)
            {
                // *********************************** Mouse Down
                case EventType.MouseDown:
                    FollowMachineInspector.CloseAll();

                    // get interaction source
                    _interactionSource = GetMouseInteractionSource(_mousePosition);

                    if (_interactionSource == null)
                        if (currentEvent.button == 0)
                            _interactionSource = _canvas.BoxSelection;
                        else
                            _canvas.CreationMenu.Show(_mousePosition);

                    if (currentEvent.clickCount < 2)
                        _interactionSource?.MouseDown(_mousePosition, currentEvent);
                    else
                        _interactionSource?.DoubleClick(_mousePosition, currentEvent);

                    Event.current.Use();
                    break;

                // *********************************** Mouse Drag
                case EventType.MouseDrag:
                    _interactionSource?.MouseDrag(
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
                        source?.MouseEnter(_mousePosition, currentEvent);
                        _interactionSource?.MouseExit(_mousePosition, currentEvent);
                        _interactionSource = source;
                    }

                    source?.MouseMove(_mousePosition, currentEvent);

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
                source = socket.Editor().IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }
            foreach (Node node in _canvas.Graph.NodeList)
            {
                source = node.Editor().IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }

            foreach (var edge in _canvas.Graph.Editor().Edges)
            {
                source = edge.Editor().IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }

            foreach (var @group in _canvas.Graph.GroupList)
            {
                source = @group.Editor().IsMouseOver(mousePosition);
                if (source != null)
                    return source;
            }

            return null;
        }

        private void SetMouseInteractionSource()
        {
            _interactionSource = GetMouseInteractionSource(_mousePosition);
        }

        private void SetupMouseInfo()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            _isMouseInCanvas = _canvas.WindowRect.Contains(mousePosition);
            _mousePosition = _canvas.CordinationSystem.ScreenCoordsToZoomCoords(mousePosition);
        }
    }
}
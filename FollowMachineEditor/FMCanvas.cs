using System;
using System.Collections.Generic;
using FMachine.Editor.ShapeBehaviours;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor
{
    public class FMCanvas
    {
        public float CanvasSize = 100000;
        public FMWindow Window => _window;

        private FMWindow _window;

        // ********************* Properties

        #region Position

        public Vector2 Position
        {
            get
            {
                return Graph != null ? Graph.Position : Vector2.zero;
            }
            set
            {
                if (Graph == null)
                    return;
                Graph.Position = value;
            }
        }

        #endregion

        #region Zoom

        public float Zoom
        {
            get
            {
                return Graph != null ? Graph.Zoom : 1;
            }
            set
            {
                if (Graph == null)
                    return;
                Graph.Zoom = value;
            }
        }

        #endregion

        #region Graph

        public Graph Graph
        {
            get { return _window.GraphStack.CurrentGraph; }
        }

        #endregion

        #region GridBackground

        private GridBackground _gridBackground = null;

        public GridBackground GridBackground
        {
            get { return _gridBackground ?? (_gridBackground = new GridBackground()); }
        }

        #endregion

        #region CreationMenu

        private CreationMenu _creationMenu = null;

        public CreationMenu CreationMenu
        {
            get { return _creationMenu ?? (_creationMenu = new CreationMenu(this)); }
        }

        #endregion

        #region WindowRect

        private Rect _windowRect;

        public Rect WindowRect
        {
            get { return _windowRect; }
            set { _windowRect = value; }
        }

        #endregion

        #region CordinationSystem

        private CordianationSystem _cordinationSystem;

        public CordianationSystem CordinationSystem
        {
            get
            {
                if (_cordinationSystem == null)
                    _cordinationSystem = new CordianationSystem(this);
                return _cordinationSystem;

            }
        }

        #endregion

        #region EventProcessor

        private EventProcessor _eventProcessor;

        public EventProcessor EventProcessor
        {
            get
            {
                if (_eventProcessor == null)
                    _eventProcessor = new EventProcessor(this);
                return _eventProcessor;

            }
        }

        #endregion

        #region BoxSelection

        private BoxSelection _boxSelection;

        public BoxSelection BoxSelection
        {
            get
            {
                if (_boxSelection == null)
                    _boxSelection = new BoxSelection(this);
                return _boxSelection;

            }
        }

        #endregion

        #region DropNode

        private DropNode _dropNode = null;

        public DropNode DropNode
        {
            get { return _dropNode = new DropNode(this); }
        }


        #endregion

        // ********************* Methods

        public FMCanvas(FMWindow fmWindow)
        {
            _window = fmWindow;

        }

        public void OnGUI()
        {
            // *********************** Set Rect
            var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));

            if (Event.current.type == EventType.Repaint)
                WindowRect = rect;


            // ********************** Draw background
            GridBackground.Draw(this);

            // *************** Exit if no graph
            if (Graph == null)
                return;

            //*************** Process Events

            if (
                Event.current.type!=EventType.Repaint &&
                Event.current.type!=EventType.Layout)
            {
                // Zoom & pan
                CordinationSystem.HandleZoomAndPan();

                // creation menu
                CreationMenu.HandleUserInput();

                // Process Events
                EventProcessor.ProcessEvents();
            }
            else
            {
                // ************** Draw Shapes

                CordinationSystem.BeginDraw();

                BoxSelection.Draw();

                // Draw Edges
                DrawEdges();

                // Draw Nodes
                Graph.NodeList.ForEach(shape => shape.Draw());

                // Draw Sockets
                DrawSockets();

                CordinationSystem.EndDraw();
                
            }
        }

        private void DrawSockets()
        {
            var sockets = GetSockets();
            foreach (var socket in sockets)
                socket.Draw();
        }

        public IEnumerable<Socket> GetSockets()
        {
            foreach (var node in Graph.NodeList)
            {
                foreach (Socket socket in node.InputSocketList)
                    yield return socket;

                foreach (Socket socket in node.OutputSocketList)
                    yield return socket;
            }
        }

        public IEnumerable<Edge> GetEdges()
        {
            foreach (var node in Graph.NodeList)
                foreach (var socket in node.InputSocketList)
                    foreach (var edge in socket.EdgeList)
                        yield return edge;
        }

        private void DrawEdges()
        {
            foreach (Node node in Graph.NodeList)
                foreach (var socket in node.OutputSocketList)
                    foreach (var edge in socket.EdgeList)
                        edge.Draw();
        }

        public void Repaint()
        {
            _window.Repaint();
        }
    }
}
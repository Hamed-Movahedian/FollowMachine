using System;
using FMachine.Shapes.Nodes;
using FollowMachineEditor.EditorObjectMapper;
using MgsCommonLib.UI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FMachine.Editor
{
    public class DropNode
    {
        private FMCanvas _canvas;
        private GameObject _dragGameObject;
        private GenericMenu _dragMenu;
        private Vector2 _lastMousePosition;

        public DropNode(FMCanvas canvas)
        {
            _canvas = canvas;
        }

        public void Update(Vector2 mousePosition)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        public void Perform(Vector2 mousePosition)
        {
            _lastMousePosition = mousePosition;

            DragAndDrop.AcceptDrag();

            foreach (Object o in DragAndDrop.objectReferences)
                if (o is GameObject)
                {
                    _dragGameObject = (GameObject)o;

                    if (_dragGameObject == null)
                        return;
                    CreateDragMenu();

                    _dragMenu.ShowAsContext();

                    Event.current.Use();
                    return;
                }
                else if(o is FollowMachine)
                {
                    _dragGameObject = (o as FollowMachine).gameObject;
                    CreateDragMenu();

                    _dragMenu.ShowAsContext();

                    Event.current.Use();
                    return;
                }
        }
        private void CreateDragMenu()
        {
            _dragMenu = new GenericMenu();

            _dragMenu.AddItem(new GUIContent("Action"), false, OnDragMenuClick, typeof(ActionNode));

            if (_dragGameObject.GetComponent<FollowMachine>() != null)
                _dragMenu.AddItem(new GUIContent("FollowMachine"), false, OnDragMenuClick, typeof(FollowMachineNode));
            else
                _dragMenu.AddDisabledItem(new GUIContent("FollowMachine"));

            if (_dragGameObject.GetComponent<MgsUIWindow>() != null)
                _dragMenu.AddItem(new GUIContent("UI/Window"), false, OnDragMenuClick, typeof(WindowNode));
            else
                _dragMenu.AddDisabledItem(new GUIContent("UI/Window"));

            if (_dragGameObject.GetComponent<MgsDialougWindow>() != null)
                _dragMenu.AddItem(new GUIContent("UI/Dialogue"), false, OnDragMenuClick, typeof(DialogeNode));
            else
                _dragMenu.AddDisabledItem(new GUIContent("UI/Dialogue"));

        }

        private void OnDragMenuClick(object o)
        {
            Type type = (Type) o;
            if (_canvas == null)
                return;

            if (_canvas.Graph == null)
                return;

            if (type == typeof(ActionNode))
            {
                ActionNode actionNode =
                    (ActionNode) _canvas.Graph.Editor().Repository.CreateNode(type, _lastMousePosition); ;

                actionNode.TargetGameObject = (GameObject)_dragGameObject;
            }

            if (type == typeof(FollowMachineNode))
            {
                FollowMachineNode followMachineNode =
                    (FollowMachineNode) _canvas.Graph.Editor().Repository.CreateNode(type, _lastMousePosition); ;

                followMachineNode.FollowMachine = _dragGameObject.GetComponent<FollowMachine>();
                followMachineNode.Editor().UpdateFollowMachine();
            }

            if (type == typeof(WindowNode))
            {
                WindowNode windowNode =
                    (WindowNode) _canvas.Graph.Editor().Repository.CreateNode(type, _lastMousePosition); ;

                windowNode.Window = _dragGameObject.GetComponent<MgsUIWindow>();
                windowNode.Editor().OnShow();
            }

            if (type == typeof(DialogeNode))
            {
                DialogeNode dialogeNode =
                    (DialogeNode) _canvas.Graph.Editor().Repository.CreateNode(type, _lastMousePosition); ;

                dialogeNode.Window = _dragGameObject.GetComponent<MgsDialougWindow>();
                dialogeNode.Editor().OnShow();
            }



        }

        public bool IsCorrectEvent(Event currentEvent)
        {
            return currentEvent.type==EventType.DragUpdated || currentEvent.type==EventType.DragPerform;
        }
    }
}
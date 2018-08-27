using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor
{
    public class CreationMenu
    {
        private FMCanvas _canvas;
        private GenericMenu _menu;
        private Vector2 _mousePosition;

        public CreationMenu(FMCanvas canvas)
        {
            _canvas = canvas;
            _menu = CreateGenericMenu();
        }

        private Dictionary<string, Type> CreateMenuEntries()
        {
            Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();

            Type shapeType = typeof(Node);

            IEnumerable<Type> classesExtendingNode =
                Assembly
                    .GetAssembly(shapeType)
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(shapeType));

            foreach (Type type in classesExtendingNode)
                menuEntries.Add(type.Name, type);

            menuEntries.OrderBy(x => x.Key);

            return menuEntries;
        }

        private GenericMenu CreateGenericMenu()
        {
            GenericMenu m = new GenericMenu();

            var entries = CreateMenuEntries();

            foreach (KeyValuePair<string, Type> entry in entries)
                m.AddItem(new GUIContent(entry.Key), false, OnGenericMenuClick, entry.Value);

            return m;
        }

        private void OnGenericMenuClick(object item)
        {
            if (_canvas == null)
                return;

            Node node=null;

            if (_canvas.Graph != null)
                node = _canvas.Graph.Repository.CreateNode((Type) item, _mousePosition);

            var followMachineNode = node as FollowMachineNode;

            if(followMachineNode!=null)
                if (EditorTools.Instance.DisplayDialog("Create Follow Machine node", "Create new Follow Machine ?", "Ok",
                    "Cancel"))
                {
                    followMachineNode.FollowMachine = _canvas.Graph.Repository.CreateFollowMachine("Follow Machine");
                    followMachineNode.FollowMachine.Repository.CreateNode(typeof(InputNode), _mousePosition);
                    followMachineNode.FollowMachine.Repository.CreateNode(typeof(OutputNode), 
                        _mousePosition+Vector2.right*followMachineNode.Rect.width*2);
                    followMachineNode.OnShow();
                }

        }

        private void Show(Vector2 position)
        {
            _mousePosition = position;
            _menu.ShowAsContext();
        }

        public void HandleUserInput()
        {
            if (Event.current.type == EventType.ContextClick)
            {
                var mousePosition = Event.current.mousePosition;

                if (_canvas.WindowRect.Contains(mousePosition))
                    Show(_canvas.CordinationSystem.ConvertScreenCoordsToZoomCoords(mousePosition));

                Event.current.Use();
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using FollowMachineEditor.EditorObjectMapper;
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
            {
                var customAttributes = type.GetCustomAttributes(typeof(NodeAttribute), true);

                if (customAttributes.Length>0)
                    menuEntries.Add(((NodeAttribute) customAttributes[0]).MenuTitle, type);
            }

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
                node = _canvas.Graph.Editor().Repository.CreateNode((Type) item, _mousePosition);

            var followMachineNode = node as FollowMachineNode;

            if(followMachineNode!=null)
                if (EditorTools.Instance.DisplayDialog("Create Follow Machine node", "Create new Follow Machine ?", "Ok",
                    "Cancel"))
                {
                    followMachineNode.FollowMachine = _canvas.Graph.Editor().Repository.CreateFollowMachine("Follow Machine");
                    followMachineNode.FollowMachine.Editor().Repository.CreateNode(typeof(InputNode), _mousePosition);
                    followMachineNode.FollowMachine.Editor().Repository.CreateNode(typeof(OutputNode), 
                        _mousePosition+Vector2.right*followMachineNode.Rect.width*2);
                    followMachineNode.Editor().OnShow();
                }

        }

        public void Show(Vector2 position)
        {
            _mousePosition = position;
            _menu.ShowAsContext();
        }

    }
}
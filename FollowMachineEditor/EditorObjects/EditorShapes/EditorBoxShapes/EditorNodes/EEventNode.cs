using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EEventNode :ENode
    {
        public GameObject TargetGameObject
        {
            get => _eventNode.TargetGameObject;
            set => _eventNode.TargetGameObject = value;
        }

        public string ComponentTypeName
        {
            get => _eventNode.ComponentTypeName;
            set => _eventNode.ComponentTypeName = value;
        }


        public string FieldName
        {
            get => _eventNode.FieldName;
            set => _eventNode.FieldName = value;
        }


        private EventNode _eventNode;

        public EEventNode(EventNode eventNode) : base(eventNode)
        {
            _eventNode = eventNode;
        }

        public override void OnInspector()
        {
            base.OnInspector();

            #region Get TargetGameObject

            EditorTools.Instance.PropertyField(_eventNode, "TargetGameObject");

            if (TargetGameObject == null)
                return;

            #endregion

            #region Get ComponentType

            List<Type> componentsTypes =
                TargetGameObject
                    .GetComponents<Component>()
                    .Select(c => c.GetType()).ToList();

            // Remove Transform component
            componentsTypes.RemoveAt(0);

            List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

            int currentTypeIndex = componentTypeNames.IndexOf(ComponentTypeName);

            if (currentTypeIndex == -1)
                currentTypeIndex = 0;

            string componentTypeName =
                componentTypeNames[EditorTools.Instance.Popup("Component", currentTypeIndex, componentTypeNames.ToArray())];

            if (ComponentTypeName != componentTypeName)
            {
                EditorTools.Instance.Undo_RecordObject(_eventNode, "Change Component type in eventNode");
                ComponentTypeName = componentTypeName;
            }

            Type componentType = componentsTypes[currentTypeIndex];

            #endregion

            #region get property or field

            List<PropertyInfo> propertyInfos = componentType
                .GetProperties()
                .Where(mi => mi.PropertyType == typeof(UnityEvent) || mi.PropertyType.BaseType == typeof(UnityEvent))
                .ToList();

            List<FieldInfo> fieldInfos = componentType.GetFields()
                .Where(mi => mi.FieldType == typeof(UnityEvent) || mi.FieldType.BaseType == typeof(UnityEvent))
                .ToList();

            List<string> fieldNames = propertyInfos.Select(mi => mi.Name).ToList();
            fieldNames.AddRange(fieldInfos.Select(fi => fi.Name));

            if (fieldNames.Count > 0)
            {
                int selectedIndex = fieldNames.IndexOf(FieldName);

                if (selectedIndex == -1)
                    selectedIndex = 0;


                selectedIndex = EditorTools.Instance.Popup("Field", selectedIndex, fieldNames.ToArray());

                string methodName = fieldNames[selectedIndex];

                if (methodName != FieldName)
                {
                    EditorTools.Instance.Undo_RecordObject(_eventNode, "Change field in eventNode");
                    FieldName = methodName;
                }

                if (GUILayout.Button("Fill Info"))
                {
                    Info = TargetGameObject.name + " => " + FieldName;
                }
            }

            #endregion
        }

        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");

        }
        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            if (TargetGameObject == null)
                return;

            Type componentType = _eventNode.GetComponentType();

            if (componentType == null)
                return;

            if (componentType.IsSubclassOf(typeof(MonoBehaviour)))
                EditorTools.Instance.OpenScript((MonoBehaviour)TargetGameObject.GetComponent(componentType));

        }

    }
}

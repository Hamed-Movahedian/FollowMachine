using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FMachine;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle="IO/Event")]
    public class EventNode : Node
    {
        #region Public

        public GameObject TargetGameObject;

        public string ComponentTypeName;

        public string FieldName;

        #endregion

        #region Private

        private Type _componentType;
        private PropertyInfo _propertyInfo;
        private FieldInfo _fieldInfo;

        private Component _component;
        private UnityEvent _unityEvent;

        #endregion

        // **************

        #region Awake 

        public void Awake()
        {
            #region Get componentType

            if (_componentType == null)
            {
                _componentType = GetComponentType();

                if (_componentType == null)
                {
                    Debug.LogError("Component " + ComponentTypeName + " not found !!!");
                    return;
                }
            }

            #endregion

            #region Get component

            if (_component == null)
            {
                _component = TargetGameObject.GetComponent(_componentType);

                if (_component == null)
                {
                    Debug.LogErrorFormat("Can't get component of type {0} in {1} not found !!!",
                        _componentType.Name,
                        TargetGameObject.name);
                    return;
                }

            }
            #endregion

            #region Get _propertyInfo or _fieldInfo

            if (_propertyInfo == null && _fieldInfo == null)
            {
                _propertyInfo = GetPropertyInfo(_componentType);

                if (_propertyInfo == null)
                    _fieldInfo = GetFieldInfo(_componentType);

                if (_propertyInfo == null && _fieldInfo == null)
                {
                    Debug.LogError("Property or field --" + Info + " not found !!!");
                    return;
                }
            }

            #endregion

            #region Get _unityEvent

            if (_unityEvent == null)
            {
                if (_fieldInfo != null)
                    _unityEvent = _fieldInfo.GetValue(_component) as UnityEvent;

                if (_propertyInfo != null)
                    _unityEvent = _propertyInfo.GetValue(_component, null) as UnityEvent;

            }
            #endregion

            #region Add Listener

            _unityEvent.AddListener(() => { StartCoroutine((Graph as FollowMachine)?.RunNode(this));});
            #endregion
        }

        #endregion

        public override void OnInspector()
        {
            base.OnInspector();

            #region Get TargetGameObject

            EditorTools.Instance.PropertyField(this, "TargetGameObject");

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
                EditorTools.Instance.Undo_RecordObject(this, "Change Component type in eventNode");
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
                    EditorTools.Instance.Undo_RecordObject(this, "Change field in eventNode");
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

 
        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }

        #region GetPropertyInfo

        private PropertyInfo GetPropertyInfo(Type componentType)
        {
            return componentType
                .GetProperties()
                .FirstOrDefault(
                    mi =>
                        mi.Name == FieldName &&
                        (mi.PropertyType == typeof(UnityEvent) || mi.PropertyType.BaseType == typeof(UnityEvent)));
        }

        #endregion

        #region GetFieldInfo

        private FieldInfo GetFieldInfo(Type componentType)
        {
            return componentType
                .GetFields()
                .FirstOrDefault(
                    mi =>
                        mi.Name == FieldName &&
                        (mi.FieldType == typeof(UnityEvent) || mi.FieldType.BaseType == typeof(UnityEvent)));

        }

        #endregion

        #region GetComponentType

        private Type GetComponentType()
        {
            List<Type> componentsTypes =
                TargetGameObject
                    .GetComponents<Component>()
                    .Select(c =>
                    {
                        if (c == null)
                            Debug.LogError("component error in " + TargetGameObject.name);
                        return c.GetType();
                    }).ToList();

            List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

            int currentTypeIndex = componentTypeNames.IndexOf(ComponentTypeName);

            Type componentType = null;

            if (currentTypeIndex != -1)
                componentType = componentsTypes[currentTypeIndex];

            return componentType;
        }

        #endregion

    }
}

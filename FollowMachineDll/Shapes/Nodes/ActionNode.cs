using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine.SettingScripts;
using FMachine.Shapes.Sockets;
using FollowMachineDll;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle="Action")]
    public class ActionNode : Node
    {
        #region Public

        [Header("Output Lables:")]
        public List<string> Lables = new List<string>();

        [Header("Action Settings:")]
        public GameObject TargetGameObject;

        public string ComponentTypeName;

        public string MethodName;

        public List<string> ParameterValueStrings = new List<string>();

        #endregion

        #region Private

        private MethodInfo _methodInfo;
        private object _object;
        private object[] _parameters;


        #endregion

        public override bool IsEqualTo(Node node)
        {
            var actionNode = node as ActionNode;

            if (actionNode == null)
                return false;

            if (
                actionNode.TargetGameObject != TargetGameObject || 
                actionNode.ComponentTypeName != ComponentTypeName ||
                actionNode.MethodName != MethodName ||
                actionNode.ParameterValueStrings.Count != ParameterValueStrings.Count)
                return false;

            for (int i = 0; i < ParameterValueStrings.Count; i++)
                if (actionNode.ParameterValueStrings[i] != ParameterValueStrings[i])
                    return false;

            return true;
        }

        public override void OnInspector()
        {
            base.OnInspector();

            #region Output lables

            Lables = OutputSocketList.Select(s => s.Info).ToList();

            if (EditorTools.Instance.PropertyField(this, "Lables"))
                UpdateOutputSocketsWithLables();

            #endregion

            EditorTools.Instance.PropertyField(this, "TargetGameObject");

            if (TargetGameObject == null)
                return;

            #region Get ComponentType

            List<Type> componentsTypes =
                TargetGameObject
                    .GetComponents<Component>()
                    .Select(c => c.GetType()).ToList();

            // add game object
            componentsTypes.Insert(0, typeof(GameObject));


            List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

            int currentTypeIndex = componentTypeNames.IndexOf(ComponentTypeName);

            if (currentTypeIndex == -1)
                currentTypeIndex = 0;


            string componentTypeName =
                componentTypeNames[EditorTools.Instance.Popup("Component", currentTypeIndex, componentTypeNames.ToArray())];

            if (ComponentTypeName != componentTypeName)
            {
                EditorTools.Instance.Undo_RecordObject(this, "Change Component type");
                ComponentTypeName = componentTypeName;

            }

            Type componentType = componentsTypes[currentTypeIndex];

            #endregion

            #region get ComponentMethodInfo

            List<MethodInfo> methods = componentType.GetMethods()
                //.Where(mi => mi.DeclaringType == componentType)
                .Where(mi => mi.ReturnType.Name == "Void" || mi.ReturnType.Name == "IEnumerator")
                .ToList();

            MethodInfo methodInfo = null;

            if (methods.Count > 0)
            {
                List<string> methodNames = methods.Select(mi => mi.ToString()).ToList();

                int selectedMethodIndex = methodNames.IndexOf(MethodName);

                if (selectedMethodIndex == -1)
                    selectedMethodIndex = 0;


                selectedMethodIndex = EditorTools.Instance.Popup("Method", selectedMethodIndex, methodNames.ToArray());

                string methodName = methodNames[selectedMethodIndex];
                methodInfo = methods[selectedMethodIndex];

                if (methodName != MethodName)
                {
                    EditorTools.Instance.Undo_RecordObject(this, "Change method in eventNode");
                    MethodName = methodName;
                }

                var attributes = methodInfo.GetCustomAttributes(typeof(FollowMachineAttribute), false);
                if (attributes.Length > 0)
                {
                    var machineAttribute = attributes[0] as FollowMachineAttribute;
                    Info = machineAttribute.Info;
                    Lables = machineAttribute.Outputs.Split(',').ToList();
                    UpdateOutputSocketsWithLables();
                }
                else
                {
                    if (GUILayout.Button("Fill Info"))
                    {
                        Info = componentTypeName + " => " + methodInfo.Name;
                    }
                }
            }

            #endregion

            #region Get Parameters

            if (methodInfo == null)
                return;

            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length > 0)
            {
                #region Remove extra parameters

                while (ParameterValueStrings.Count > parameters.Length)
                    ParameterValueStrings.RemoveAt(ParameterValueStrings.Count - 1);

                #endregion

                #region Add shortcoming parameters

                while (ParameterValueStrings.Count < parameters.Length)
                    ParameterValueStrings.Add("");

                #endregion

                #region Get parameters

                for (int i = 0; i < parameters.Length; i++)
                {
                    var attributes = parameters[i].GetCustomAttributes(typeof(RefrenceAttribute), true);
                    if (attributes.Length > 0)
                    {
                        GUILayout.Label("Refrence");
                    }
                    else
                        ParameterValueStrings[i]=EditorTools.Instance.GetParameter(this, parameters[i], ParameterValueStrings[i]);
                }

                #endregion
            }
            #endregion
        }

        private void UpdateOutputSocketsWithLables()
        {
            for (int i = 0; i < Lables.Count; i++)
                if (i < OutputSocketList.Count)
                    OutputSocketList[i].Info = Lables[i];
                else
                    AddOutputSocket<InputSocket>(Lables[i]);

            while (OutputSocketList.Count > Lables.Count)
            {
                var socket = OutputSocketList[OutputSocketList.Count - 1];
                OutputSocketList.Remove(socket);
                socket.Delete();
            }
        }

        #region Run 

        protected override IEnumerator Run()
        {
            FollowMachine.SetOutput("");

            #region Get componentType

            Type componentType = GetComponentType();

            if (componentType == null)
                throw new Exception("Error in Action node " + Info);

            #endregion

            #region Get methodInfo

            _methodInfo = GetMethodInfo(componentType);

            if (_methodInfo == null)
                throw new Exception("Error in Action node " + Info);

            #endregion

            #region Get component

            if (componentType != typeof(GameObject))
            {
                _object = TargetGameObject.GetComponent(componentType);

                if (_object == null)
                    throw new Exception("Error in Action node " + Info);
            }
            else
                _object = TargetGameObject;

            #endregion

            #region Get parameters

            ParameterInfo[] parameterInfos = _methodInfo.GetParameters();

            _parameters = new object[parameterInfos.Length];

            for (int i = 0; i < _parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                string valueString = ParameterValueStrings[i];

                if (parameterInfo.ParameterType == typeof(int)) //*** int
                {
                    int value;
                    if (!int.TryParse(valueString, out value))
                        value = 0;

                    _parameters[i] = value;
                }
                else if (parameterInfo.ParameterType == typeof(float)) //*** float
                {
                    float value;
                    if (!float.TryParse(valueString, out value))
                        value = 0;

                    _parameters[i] = value;
                }
                else if (parameterInfo.ParameterType == typeof(bool)) //*** bool
                {
                    bool value;
                    if (!bool.TryParse(valueString, out value))
                        value = false;

                    _parameters[i] = value;
                }
                else if (parameterInfo.ParameterType == typeof(string)) //*** string
                {
                    _parameters[i] = valueString;
                }
                else //*** Others
                {
                    _parameters[i] = null;
                }
            }

            #endregion

            #region Invoke


            if (_methodInfo.ReturnType.Name == "IEnumerator")
                return (IEnumerator)_methodInfo.Invoke(_object, _parameters);
            else
            {
                _methodInfo.Invoke(_object, _parameters);
                return null;
            }

            #endregion

        }

        #endregion

        public override void OnShow()
        {
            base.OnShow();

            if (TargetGameObject == null)
                return;

            Type componentType = GetComponentType();

            if (componentType == null)
                return;

            _methodInfo = GetMethodInfo(componentType);

            if (_methodInfo == null)
                return;

            var attributes = _methodInfo.GetCustomAttributes(typeof(FollowMachineAttribute), false);
            if (attributes.Length > 0)
            {
                var machineAttribute = attributes[0] as FollowMachineAttribute;
                Info = machineAttribute.Info;
                Lables = machineAttribute.Outputs.Split(',').ToList();
                UpdateOutputSocketsWithLables();
            }
        }

        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (FollowMachine.CheckOutputLable(socket.Info))
                    return socket.GetNextNode();

            return null;
        }

        #region GetMethodInfo

        private MethodInfo GetMethodInfo(Type componentType)
        {
            List<MethodInfo> methods = componentType.GetMethods()
                .Where(mi => mi.ReturnType.Name == "Void" || mi.ReturnType.Name == "IEnumerator")
                .ToList();

            List<string> methodNames = methods.Select(mi => mi.ToString()).ToList();

            int selectedMethodIndex = methodNames.IndexOf(MethodName);

            MethodInfo methodInfo = null;

            if (selectedMethodIndex != -1)
                methodInfo = methods[selectedMethodIndex];

            return methodInfo;
        }

        #endregion

        #region GetComponentType

        private Type GetComponentType()
        {
            Type _componentType = null;

            if (TargetGameObject != null)
            {
                if (ComponentTypeName == "GameObject")
                    return typeof(GameObject);

                List<Type> componentsTypes =
                    TargetGameObject
                        .GetComponents<Component>()
                        .Select(c => c.GetType()).ToList();

                List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

                int currentTypeIndex = componentTypeNames.IndexOf(ComponentTypeName);

                if (currentTypeIndex != -1)
                    _componentType = componentsTypes[currentTypeIndex];

            }

            return _componentType;
        }

        #endregion

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            if (TargetGameObject == null)
                return;

            Type componentType = GetComponentType();

            if (componentType == null)
                return;

            _methodInfo = GetMethodInfo(componentType);

            if (_methodInfo == null)
                return;

            if(componentType.IsSubclassOf(typeof(MonoBehaviour)))
                EditorTools.Instance.OpenScript((MonoBehaviour)TargetGameObject.GetComponent(componentType));
        }
    }
}
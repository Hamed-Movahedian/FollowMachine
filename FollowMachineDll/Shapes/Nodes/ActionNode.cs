using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine.Shapes.Sockets;
using FollowMachineDll;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Action (static parameters)")]
    public class ActionNode : Node
    {
        #region Public

        public List<string> Lables = new List<string>();

        public GameObject TargetGameObject;

        public string ComponentTypeName;

        public string MethodName;

        public List<string> ParameterValueStrings = new List<string>();

        public MgsProgressWindow ProgressbarWindow;
        public string ProgressbarMessage;
        public bool ProgressbarShow = true;
        public bool ProgressbarHide = true;
        #endregion

        #region Private

        protected MethodInfo _methodInfo;
        private object _object;
        protected object[] _parameters;
        protected ParameterInfo[] _parameterInfos;

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


            GUILayout.BeginVertical((GUIStyle)"box");
            {
                EditorTools.Instance.BoldLabel("Method:");
                // Get GameObject
                EditorTools.Instance.PropertyField(this, "TargetGameObject");

                if (TargetGameObject == null)
                    return;

                _methodInfo = EditorTools.Instance.GetMethodInfo(
                    TargetGameObject, ref ComponentTypeName, ref MethodName);

                if (_methodInfo == null)
                    return;
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Get parameters
            GetParameters();

            GUILayout.Space(5);

            #region Check follow machine attributes
            var attributes = _methodInfo.GetCustomAttributes(typeof(FollowMachineAttribute), false);
            if (attributes.Length > 0)
            {
                var machineAttribute = attributes[0] as FollowMachineAttribute;
                Info = machineAttribute.Info;
                Lables = machineAttribute.Outputs.Split(',').ToList();
                UpdateOutputSocketsWithLables();
            }
            else
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Fill Info"))
                {
                    Info = _methodInfo.DeclaringType.Name + " => " + _methodInfo.Name;
                }
            }
            #endregion

            // if not coroutine don't show progress bar
            if (_methodInfo.ReturnType != typeof(IEnumerator))
                return;

            // Get Progress bar 

            GUILayout.BeginVertical((GUIStyle)"box");
            {
                GUILayout.Label("Progressbar :", (GUIStyle)"BoldLabel");

                EditorTools.Instance.PropertyField(this, "ProgressbarWindow");

                if (ProgressbarWindow == null)
                    return;

                EditorTools.Instance.LanguageField(this, "Progressbar Message", ref ProgressbarMessage);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Display method:");
                    ProgressbarShow = GUILayout.Toggle(ProgressbarShow, "Show");
                    ProgressbarHide = GUILayout.Toggle(ProgressbarHide, "Hide");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        protected virtual void GetParameters()
        {
            GUILayout.BeginVertical((GUIStyle)"box");

            GUILayout.Label("Progressbar :", (GUIStyle)"BoldLabel");

            ParameterInfo[] parameters = _methodInfo.GetParameters();

            if (parameters.Length > 0)
            {
                ParameterValueStrings.Resize(parameters.Length);

                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterValueStrings[i] =
                        EditorTools.Instance.GetParameter(this, parameters[i].Name, parameters[i].ParameterType, ParameterValueStrings[i]);
                }
            }
            GUILayout.EndVertical();

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

            Type componentType = GetComponentType(ComponentTypeName, TargetGameObject);

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

            GetParametersObjects();

            #region Invoke


            if (_methodInfo.ReturnType.Name == "IEnumerator")
            {
                if (ProgressbarWindow)
                    yield return ProgressbarWindow.Display(ProgressbarMessage, ProgressbarShow);

                yield return (IEnumerator)_methodInfo.Invoke(_object, _parameters);

                if (ProgressbarWindow && ProgressbarHide)
                    yield return ProgressbarWindow.Hide();
            }
            else
            {
                _methodInfo.Invoke(_object, _parameters);
                yield return null;
            }

            #endregion

        }

        protected virtual void GetParametersObjects()
        {
            _parameterInfos = _methodInfo.GetParameters();

            _parameters = new object[_parameterInfos.Length];

            for (int i = 0; i < _parameters.Length; i++)
                SetStaticParamerterObject(i);
        }

        protected void SetStaticParamerterObject(int i)
        {
            ParameterInfo parameterInfo = _parameterInfos[i];
            string valueString = ParameterValueStrings[i];

            if (parameterInfo.ParameterType == typeof(int)) //*** int
            {
                if (!int.TryParse(valueString, out var value))
                    value = 0;

                _parameters[i] = value;
            }
            else if (parameterInfo.ParameterType == typeof(float)) //*** float
            {
                if (!float.TryParse(valueString, out var value))
                    value = 0;

                _parameters[i] = value;
            }
            else if (parameterInfo.ParameterType == typeof(bool)) //*** bool
            {
                if (!bool.TryParse(valueString, out var value))
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

        public override void OnShow()
        {
            base.OnShow();

            if (TargetGameObject == null)
                return;

            Type componentType = GetComponentType(ComponentTypeName, TargetGameObject);

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

        protected Type GetComponentType(string cName, GameObject tGameObject)
        {
            Type _componentType = null;

            if (tGameObject != null)
            {
                if (cName == "GameObject")
                    return typeof(GameObject);

                List<Type> componentsTypes =
                    tGameObject
                        .GetComponents<Component>()
                        .Select(c => c.GetType()).ToList();

                List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

                int currentTypeIndex = componentTypeNames.IndexOf(cName);

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

            Type componentType = GetComponentType(ComponentTypeName, TargetGameObject);

            if (componentType == null)
                return;

            if (componentType.IsSubclassOf(typeof(MonoBehaviour)))
                EditorTools.Instance.OpenScript((MonoBehaviour)TargetGameObject.GetComponent(componentType));
        }
    }
}
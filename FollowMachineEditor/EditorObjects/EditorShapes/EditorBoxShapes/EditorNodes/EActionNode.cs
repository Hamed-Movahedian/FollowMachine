﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bind;
using BindEditor;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EActionNode : ENode
    {
        #region Public

        public GameObject TargetGameObject
        {
            get => _actionNode.TargetGameObject;
            set => _actionNode.TargetGameObject = value;
        }


        public string ComponentTypeName
        {
            get => _actionNode.ComponentTypeName;
            set => _actionNode.ComponentTypeName = value;
        }


        public string MethodName
        {
            get => _actionNode.MethodName;
            set => _actionNode.MethodName = value;
        }


        public MgsProgressWindow ProgressbarWindow
        {
            get => _actionNode.ProgressbarWindow;
            set => _actionNode.ProgressbarWindow = value;
        }

        public string ProgressbarMessage
        {
            get => _actionNode.ProgressbarMessage;
            set => _actionNode.ProgressbarMessage = value;
        }

        public bool ProgressbarShow
        {
            get => _actionNode.ProgressbarShow;
            set => _actionNode.ProgressbarShow = value;
        }

        public bool ProgressbarHide
        {
            get => _actionNode.ProgressbarHide;
            set => _actionNode.ProgressbarHide = value;
        }

        #endregion

        #region Private

        protected MethodInfo _methodInfo;
        private object _object;
        protected object[] _parameters;
        protected ParameterInfo[] _parameterInfos;

        #endregion

        private readonly ActionNode _actionNode;

        public EActionNode(ActionNode actionNode) : base(actionNode)
        {
            _actionNode = actionNode;
        }
        public override bool IsEqualTo(Node node)
        {
            var actionNode = node as ActionNode;

            if (actionNode == null)
                return false;

            if (
                actionNode.TargetGameObject != TargetGameObject ||
                actionNode.ComponentTypeName != ComponentTypeName ||
                actionNode.MethodName != MethodName ||
                actionNode.ParameterGetValues.Count != _actionNode.ParameterGetValues.Count)
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
                if (EditorTools.Instance.PropertyField(_actionNode, "TargetGameObject"))
                    _actionNode.ParameterGetValues.Clear();

                if (TargetGameObject != null)
                {
                    var componentTypeName = ComponentTypeName;
                    var methodName = MethodName;
                    _methodInfo = EditorTools.Instance.GetMethodInfo(
                        TargetGameObject, ref componentTypeName, ref methodName);
                    if (ComponentTypeName != componentTypeName || MethodName != methodName)
                    {
                        ComponentTypeName = componentTypeName;
                        MethodName = methodName;
                        _actionNode.ParameterGetValues.Clear();
                    }
                }
                else
                {
                    _methodInfo = null;
                }

            }
            GUILayout.EndVertical();

            if (_methodInfo == null)
                return;

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
                SetOutputs(machineAttribute.Outputs.Split(',').ToList());
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

                EditorTools.Instance.PropertyField(_actionNode, "ProgressbarWindow");

                if (ProgressbarWindow != null)
                {
                    EditorTools.Instance.LanguageField(_actionNode, "Progressbar Message",
                        ref _actionNode.ProgressbarMessage);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Display method:");
                        ProgressbarShow = GUILayout.Toggle(ProgressbarShow, "Show");
                        ProgressbarHide = GUILayout.Toggle(ProgressbarHide, "Hide");
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        protected virtual void GetParameters()
        {
            GUILayout.BeginVertical((GUIStyle)"box");

            GUILayout.Label("Parameters :", (GUIStyle)"BoldLabel");

            ParameterInfo[] parameters = _methodInfo.GetParameters();

            if (parameters.Length > 0)
            {
                _actionNode.ParameterGetValues.Resize(parameters.Length);

                for (int i = 0; i < parameters.Length; i++)
                {
                    GUILayout.Label(parameters[i].Name);

                    if (_actionNode.ParameterGetValues[i] == null)
                        _actionNode.ParameterGetValues[i] = BindEditorGateway.CreateGetValue(parameters[i].ParameterType);

                    _actionNode.ParameterGetValues[i].GetValueGUI();

                }
            }
            GUILayout.EndVertical();

        }

        public override void OnShow()
        {
            base.OnShow();

            if (TargetGameObject == null)
                return;

            Type componentType = _actionNode.GetComponentType(ComponentTypeName, TargetGameObject);

            if (componentType == null)
                return;

            _methodInfo = _actionNode.GetMethodInfo(componentType);

            if (_methodInfo == null)
                return;

            var attributes = _methodInfo.GetCustomAttributes(typeof(FollowMachineAttribute), false);
            if (attributes.Length > 0)
            {
                var machineAttribute = attributes[0] as FollowMachineAttribute;
                Info = machineAttribute?.Info;
                SetOutputs(machineAttribute?.Outputs.Split(',').ToList());
            }
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        public override void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            if (TargetGameObject == null)
                return;

            Type componentType = _actionNode.GetComponentType(ComponentTypeName, TargetGameObject);

            if (componentType == null)
                return;

            if (componentType.IsSubclassOf(typeof(MonoBehaviour)))
                EditorTools.Instance.OpenScript((MonoBehaviour)TargetGameObject.GetComponent(componentType));
        }
    }
}

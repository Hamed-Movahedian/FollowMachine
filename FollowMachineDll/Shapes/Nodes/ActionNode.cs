using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bind;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Action (static parameters)")]
    public class ActionNode : Node
    {
        #region Public

        public GameObject TargetGameObject;

        public string ComponentTypeName;

        public string MethodName;

        public List<string> ParameterValueStrings = new List<string>();

        public List<GetValue> ParameterGetValues = new List<GetValue>();

        public MgsProgressWindow ProgressbarWindow;
        public string ProgressbarMessage;
        public bool ProgressbarShow = true;
        public bool ProgressbarHide = true;
        #endregion

        protected virtual void OnValidate()
        {
            if (ParameterValueStrings.Count > 0)
            {

                ParameterGetValues.Clear();

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

                var parameterInfos = _methodInfo.GetParameters();

                for (int i = 0; i < parameterInfos.Length; i++)
                    ParameterGetValues.Add(new GetValue(false, null, ParameterValueStrings[i],
                        parameterInfos[i].ParameterType));

                ParameterValueStrings.Clear();
                Debug.Log("ActionNode Transferd");

            }


        }

        #region Private

        protected MethodInfo _methodInfo;
        private object _object;

        #endregion

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

            #region Invoke

            var parameters = ParameterGetValues
                .Select(gv => gv.Value)
                .ToArray();

            if (_methodInfo.ReturnType.Name == "IEnumerator")
            {
                if (ProgressbarWindow)
                    yield return ProgressbarWindow.Display(ProgressbarMessage, ProgressbarShow);

                yield return (IEnumerator)_methodInfo.Invoke(_object, parameters);

                if (ProgressbarWindow && ProgressbarHide)
                    yield return ProgressbarWindow.Hide();
            }
            else
            {
                _methodInfo.Invoke(_object, parameters);
                yield return null;
            }

            #endregion

        }

        #endregion
        public override Node GetNextNode()
        {
            foreach (var socket in OutputSocketList)
                if (FollowMachine.CheckOutputLable(socket.Info))
                    return socket.GetNextNode();

            return null;
        }

        #region GetMethodInfo

        public MethodInfo GetMethodInfo(Type componentType)
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

        public Type GetComponentType(string cName, GameObject tGameObject)
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


    }
}
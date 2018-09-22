using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.MgsCommonLib.Utilities;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Action (Dynamic parameters)")]
    public class DynamicActionNode : ActionNode
    {

        public List<bool> DynamicParameter = new List<bool>();
        public List<GameObject> ParameterGameObjects = new List<GameObject>();

        protected override void GetParameters()
        {
            _parameterInfos = _methodInfo.GetParameters();

            if (_parameterInfos.Length == 0)
                return;

            // Resize parameter lists
            ParameterValueStrings.Resize(_parameterInfos.Length);
            ParameterGameObjects.Resize(_parameterInfos.Length);
            DynamicParameter.Resize(_parameterInfos.Length);

            // Bold label for parameters
            //GUILayout.Label("Parameters:");

            for (int i = 0; i < _parameterInfos.Length; i++)
            {
                GUILayout.Space(10);

                // Parameter Name and dynamic toggle
                GUILayout.BeginHorizontal();
                // parameter name
                EditorTools.Instance.BoldLabel(_parameterInfos[i].Name.ToFristLetterUpperCase() + " (" + _parameterInfos[i].ParameterType.Name + ")");
                GUILayout.FlexibleSpace();
                // Dynamic toggle
                var toggle = GUILayout.Toggle(DynamicParameter[i], "Dynamic");
                if (toggle != DynamicParameter[i])
                {
                    EditorTools.Instance.Undo_RecordObject(this, "Change Parameter");
                    DynamicParameter[i] = toggle;
                    ParameterValueStrings[i] = "";
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);
                    GUILayout.BeginVertical();
                    {
                        if (DynamicParameter[i])
                        {
                            var pGameObject = ParameterGameObjects[i];
                            var pText = ParameterValueStrings[i];
                            EditorTools.Instance.GetDynamicParameter(gameObject, ref pGameObject, ref pText,
                                _parameterInfos[i].ParameterType);
                            ParameterGameObjects[i] = pGameObject;
                            ParameterValueStrings[i] = pText;
                        }
                        else
                        {
                            GUILayout.Space(5);
                            ParameterValueStrings[i] =
                                EditorTools.Instance.GetParameter(this, "", _parameterInfos[i].ParameterType, ParameterValueStrings[i]);

                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

            }
        }

        protected override void GetParametersObjects()
        {
            _parameterInfos = _methodInfo.GetParameters();

            if (_parameterInfos.Length == 0)
                return;

            // Resize parameter lists
            ParameterValueStrings.Resize(_parameterInfos.Length);
            ParameterGameObjects.Resize(_parameterInfos.Length);
            DynamicParameter.Resize(_parameterInfos.Length);

            _parameters = new object[_parameterInfos.Length];

            for (int i = 0; i < _parameters.Length; i++)
                if (DynamicParameter[i])
                    _parameters[i]=SetDynamicParameterObject(ParameterGameObjects[i],ParameterValueStrings[i]);
                else
                    SetStaticParamerterObject(i);

        }

        private  object SetDynamicParameterObject(GameObject pGameObject, string pText)
        {
            // check game object isn't empty
            if (pGameObject == null)
                return null;

            // Extract component and property name
            if (pText == null) pText = "";

            var tList = pText.Split('.').ToList();

            if (tList.Count != 2)
                return null;

            var cName = tList[0];
            var pName = tList[1];

            // Get component
            var componentType = GetComponentType(cName, pGameObject);

            if (componentType == null)
                return null;

            // Get property
            var propertyInfo = componentType
                .GetProperties()
                .FirstOrDefault(pi => pi.Name == pName);

            if (propertyInfo == null)
                return null;

            // get property object
            object pObject = null;

            if (componentType == typeof(GameObject))
                pObject = pGameObject;
            else
                pObject = pGameObject.GetComponent(componentType);

            if (pObject == null)
                return null;

            // get value object
            return propertyInfo.GetValue(pObject,null);
        }

        private void SetDynamicParameterObject(int i)
        {
            
        }
    }
}

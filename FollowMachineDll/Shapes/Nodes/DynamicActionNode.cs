using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using FollowMachineDll.Utility.Bounder;
using MgsCommonLib.MgsCommonLib.Utilities;
using MgsCommonLib.Utilities;
using UnityEditor;
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

            GUILayout.BeginVertical((GUIStyle)"box");

            EditorTools.Instance.BoldLabel("Parameters :");

            for (int i = 0; i < _parameterInfos.Length; i++)
            {

                // Parameter Name and dynamic toggle
                GUILayout.BeginVertical((GUIStyle)"box");
                // parameter name
                EditorTools.Instance.BoldLabel(_parameterInfos[i].Name.ToFristLetterUpperCase() + " (" + _parameterInfos[i].ParameterType.Name + ")");
                {
                    {
                        GUILayout.BeginHorizontal();
                        if (DynamicParameter[i])
                        {

                            GUILayout.Label(ParameterValueStrings[i]);

                            if (GUILayout.Button(".", GUILayout.Width(20)))
                            {
                                var menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Edit"), false, (indexObj) =>
                                 {
                                     var index = (int)indexObj;
                                     EditorTools.Instance
                                         .EditBoundData(
                                             ParameterGameObjects[index],
                                             ParameterValueStrings[index],
                                             _parameterInfos[index].ParameterType,
                                             (o, s) =>
                                             {
                                                 ParameterGameObjects[index] = o;
                                                 ParameterValueStrings[index] = s;
                                             });
                                 }, i);

                                menu.AddItem(new GUIContent("Unbound"), false, (indexObj) =>
                                {
                                    var index = (int)indexObj;
                                    EditorTools.Instance.Undo_RecordObject(this, "Change Parameter");
                                    DynamicParameter[index] = false;
                                    ParameterValueStrings[index] = "";
                                }, i);

                                menu.ShowAsContext();
                            }

                        }
                        else
                        {
                            ParameterValueStrings[i] =
                                EditorTools.Instance.GetParameter(this, "", _parameterInfos[i].ParameterType, ParameterValueStrings[i]);
                            if (GUILayout.Button(".", GUILayout.Width(20)))
                            {
                                var menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Bound"), false, (indexObj) =>
                                {
                                    var index = (int)indexObj;
                                    ParameterValueStrings[index] = "";
                                    EditorTools.Instance
                                        .EditBoundData(
                                            ParameterGameObjects[index],
                                            ParameterValueStrings[index],
                                            _parameterInfos[index].ParameterType,
                                            (o, s) =>
                                            {
                                                ParameterGameObjects[index] = o;
                                                ParameterValueStrings[index] = s;
                                                DynamicParameter[index] = true;

                                            });
                                }, i);
                                menu.ShowAsContext();
                            }


                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();

            }

            GUILayout.EndVertical();
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
                    _parameters[i] = SetDynamicParameterObject(ParameterGameObjects[i], ParameterValueStrings[i]);
                else
                    SetStaticParamerterObject(i);

        }

        private object SetDynamicParameterObject(GameObject pGameObject, string pText)
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
            return propertyInfo.GetValue(pObject, null);
        }

    }
}

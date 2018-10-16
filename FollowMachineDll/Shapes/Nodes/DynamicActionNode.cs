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
        private List<BoundData> _parameterBoundDatas = new List<BoundData>();

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

            GUILayout.Label("Parameters :",(GUIStyle)"BoldLabel");

            for (int i = 0; i < _parameterInfos.Length; i++)
            {
                GUILayout.BeginVertical((GUIStyle)"box");
                {
                    GUILayout.BeginHorizontal();
                    // parameter name
                    GUILayout.Label(
                        _parameterInfos[i].Name.ToFristLetterUpperCase() + " (" +_parameterInfos[i].ParameterType.Name + ")",
                        (GUIStyle)"BoldLabel");

                    if (GUILayout.Button("B", (GUIStyle)"Button", GUILayout.Width(20)))
                    {
                        var menu = new GenericMenu();

                        if (DynamicParameter[i])
                        {
                            menu.AddItem(new GUIContent("Edit"), false, (indexObj) =>
                            {
                                var index = (int) indexObj;
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
                                var index = (int) indexObj;
                                EditorTools.Instance.Undo_RecordObject(this, "Change Parameter");
                                DynamicParameter[index] = false;
                                ParameterValueStrings[index] = "";
                            }, i);
                        }
                        else
                        {
                            menu.AddItem(new GUIContent("Bound"), false, (indexObj) =>
                            {
                                var index = (int) indexObj;
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
                        }

                        menu.ShowAsContext();
                    }

                    GUILayout.EndHorizontal();

                    if (DynamicParameter[i])
                        GUILayout.Label(ParameterValueStrings[i]);
                    else
                        ParameterValueStrings[i] =
                            EditorTools.Instance.GetParameter(this, "", _parameterInfos[i].ParameterType,
                                ParameterValueStrings[i]);
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
            _parameterBoundDatas.Resize(_parameterInfos.Length);

            _parameters = new object[_parameterInfos.Length];

            for (int i = 0; i < _parameters.Length; i++)
                if (DynamicParameter[i])
                    _parameters[i] = SetDynamicParameterObject(i);
                else
                    SetStaticParamerterObject(i);

        }

        private object SetDynamicParameterObject(int i)
        {
            if (_parameterBoundDatas[i] == null)
                _parameterBoundDatas[i] = new BoundData(ParameterGameObjects[i], ParameterValueStrings[i]);

            return _parameterBoundDatas[i].GetValue(_parameterInfos[i].ParameterType);
        }

    }
}

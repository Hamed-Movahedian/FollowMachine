using System.Collections.Generic;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using MgsCommonLib.MgsCommonLib.Utilities;
using MgsCommonLib.Utilities;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EDynamicActionNode :EActionNode
    {
        private readonly DynamicActionNode _dynamicActionNode;

        public EDynamicActionNode(DynamicActionNode dynamicActionNode) : base(dynamicActionNode)
        {
            _dynamicActionNode = dynamicActionNode;
        }

        public List<bool> DynamicParameter
        {
            get => _dynamicActionNode.DynamicParameter;
            set => _dynamicActionNode.DynamicParameter = value;
        }
        public List<GameObject> ParameterGameObjects
        {
            get => _dynamicActionNode.ParameterGameObjects;
            set => _dynamicActionNode.ParameterGameObjects = value;
        }

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

            GUILayout.Label("Parameters :", (GUIStyle)"BoldLabel");

            for (int i = 0; i < _parameterInfos.Length; i++)
            {
                GUILayout.BeginVertical((GUIStyle)"box");
                {
                    GUILayout.BeginHorizontal();
                    // parameter name
                    GUILayout.Label(
                        _parameterInfos[i].Name.ToFristLetterUpperCase() + " (" + _parameterInfos[i].ParameterType.Name + ")",
                        (GUIStyle)"BoldLabel");

                    if (GUILayout.Button("B", (GUIStyle)"Button", GUILayout.Width(20)))
                    {
                        var menu = new UnityEditor.GenericMenu();
                        
                        if (DynamicParameter[i])
                        {
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
                                Undo.RecordObject(_dynamicActionNode, "Change Parameter");
                                DynamicParameter[index] = false;
                                ParameterValueStrings[index] = "";
                            }, i);
                        }
                        else
                        {
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
                        }

                        menu.ShowAsContext();
                    }

                    GUILayout.EndHorizontal();

                    if (DynamicParameter[i])
                        GUILayout.Label(ParameterValueStrings[i]);
                    else
                        ParameterValueStrings[i] =
                            EditorTools.Instance.GetParameter(_dynamicActionNode, "", _parameterInfos[i].ParameterType,
                                ParameterValueStrings[i]);
                }
                GUILayout.EndVertical();

            }

            GUILayout.EndVertical(); 
        }

    }
}

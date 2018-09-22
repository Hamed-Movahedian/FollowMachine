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
            ParameterInfo[] pInfos = _methodInfo.GetParameters();

            if (pInfos.Length == 0)
                return;

            // Resize parameter lists
            ParameterValueStrings.Resize(pInfos.Length);
            ParameterGameObjects.Resize(pInfos.Length);
            DynamicParameter.Resize(pInfos.Length);

            // Bold label for parameters
            //GUILayout.Label("Parameters:");

            for (int i = 0; i < pInfos.Length; i++)
            {
                GUILayout.Space(10);

                // Parameter Name and dynamic toggle
                GUILayout.BeginHorizontal();
                // parameter name
                EditorTools.Instance.BoldLabel(pInfos[i].Name.ToFristLetterUpperCase()+" ("+pInfos[i].ParameterType.Name+")");
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
                                pInfos[i].ParameterType);
                            ParameterGameObjects[i] = pGameObject;
                            ParameterValueStrings[i] = pText;
                        }
                        else
                        {
                            GUILayout.Space(5);
                            ParameterValueStrings[i] =
                                EditorTools.Instance.GetParameter(this,"", pInfos[i], ParameterValueStrings[i]);

                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

            }
        }

    }
}

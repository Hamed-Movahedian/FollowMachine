using System;
using System.Collections.Generic;
using Bind;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Action (Dynamic parameters)")]
    public class DynamicActionNode : ActionNode
    {

        public List<bool> DynamicParameter = new List<bool>();
        public List<GameObject> ParameterGameObjects = new List<GameObject>();

        protected override void OnValidate()
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
                {
                    ParameterGetValues.Add(new GetValue(DynamicParameter[i], ParameterGameObjects[i], ParameterValueStrings[i],
                        parameterInfos[i].ParameterType));

                }

/*
                ParameterGameObjects.Clear();
                ParameterValueStrings.Clear();
                DynamicParameter.Clear();

                Debug.Log("DynamicActionNode Transferd");
*/

            }
        }
    }
}

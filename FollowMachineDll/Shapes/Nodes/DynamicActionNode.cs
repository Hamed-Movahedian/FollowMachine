using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility.Bounder;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Action (Dynamic parameters)")]
    public class DynamicActionNode : ActionNode
    {

        public List<bool> DynamicParameter = new List<bool>();
        public List<GameObject> ParameterGameObjects = new List<GameObject>();
        private readonly List<Bounder> _parameterBoundDatas = new List<Bounder>();


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
                _parameterBoundDatas[i] = new Bounder(ParameterGameObjects[i], ParameterValueStrings[i]);

            return _parameterBoundDatas[i].GetValue(_parameterInfos[i].ParameterType);
        }

    }
}

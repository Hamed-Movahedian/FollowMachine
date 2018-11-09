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
    }
}

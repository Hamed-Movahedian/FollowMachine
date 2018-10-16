using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib;
using UnityEngine;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/Server connection")]
    public class ServerNode : Node
    {
        public string MethodName;

        public List<ParamData> Parameters;

        public int BodyParamIndex = -1;

        #region ParamData class
        [Serializable]
        public class ParamData
        {
            public string Name;
            public string Value;
            public bool IsBound;
            public GameObject BoundGameObject;
        }

        #endregion

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("Success");
            AddOutputSocket<InputSocket>("Network Error");
            AddOutputSocket<InputSocket>("Http Error");
        }

        public override Node GetNextNode()
        {
            return null;
        }
    }

    
}

public abstract class ServerControllerBase : MgsSingleton<ServerControllerBase>
{
    public ServerStateEnum ServerState = ServerStateEnum.Remote;

    public List<Controller> Controllers;

    public enum ServerStateEnum
    {
        Local, Remote
    }

    public abstract IEnumerator SendRequest(string methodName, List<string> paramNames, List<object> paramObjects);


    [Serializable]
    public class Controller
    {
        public string Name;
        public List<string> Methods;
    }

}

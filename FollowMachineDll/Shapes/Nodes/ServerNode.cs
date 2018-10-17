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
            public string Type;
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
    public ServerData Data;
    public string URL
    {
        get
        {
            if (Data == null)
                throw new Exception($"Server Data Not set!!!");
            return Data.URL;

        }
    }
    public abstract IEnumerator SendRequest(string methodName, List<string> paramNames, List<object> paramObjects);


}

[CreateAssetMenu(menuName = "ServerData")]
public class ServerData : ScriptableObject
{
    public ServerStateEnum ServerState = ServerStateEnum.Remote;

    public string LocalURL = "http://localhost:52391";
    public string RemoteURL = "http://charsoogame.ir";

    public List<Controller> Controllers;
    public string URL
    {
        get
        {
            if (ServerState == ServerStateEnum.Local && Application.isEditor)
                return LocalURL;
            else
                return RemoteURL;

        }
    }

    public enum ServerStateEnum
    {
        Local, Remote
    }

    [Serializable]
    public class Controller
    {
        public string Name;
        public List<string> Methods;
    }
}

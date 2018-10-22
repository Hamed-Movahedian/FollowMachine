using System;
using System.Collections.Generic;
using FollowMachineDll.Components;
using UnityEngine;

namespace FollowMachineDll.Assets
{
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
            [Serializable]
            public class MethodData
            {
                public string Name;
                public string Info;
                public ServerConnectionMethod ConnectionMethod;
                public List<string> Outputs;
            }

            public string Name;
            public List<MethodData> Methods;
        }
    }
}
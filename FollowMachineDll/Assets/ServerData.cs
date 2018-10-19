using System;
using System.Collections.Generic;
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
            public string Name;
            public List<string> Methods;
        }
    }
}
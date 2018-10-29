using System;
using System.Collections.Generic;
using System.Linq;
using FollowMachineDll.Components;
using Newtonsoft.Json.Linq;
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
            public string Prefix;
            public List<MethodData> Methods;

            [Serializable]
            public class MethodData
            {
                public bool Equals(MethodData other)
                {
                    return
                        string.Equals(Name, other.Name) &&
                        string.Equals(Prefix, other.Prefix) &&
                        string.Equals(Info, other.Info) &&
                        ConnectionMethod == other.ConnectionMethod &&
                        Outputs.Count==other.Outputs.Count &&
                        Outputs.TrueForAll(o=>other.Outputs.Contains(o)) &&
                        Parameters.Count == other.Parameters.Count &&
                        Parameters.TrueForAll(p => other.Parameters.Any(po =>
                              po.Name == p.Name &&
                              po.TypeName == p.TypeName &&
                              po.FormBody == p.FormBody));
                }

                public string Name;
                public string Prefix;
                public string Info;
                public ServerConnectionMethod ConnectionMethod;
                public List<string> Outputs;
                public List<ParameterData> Parameters;

                [Serializable]
                public class ParameterData
                {
                    public string Name;
                    public bool FormBody;
                    public string TypeName;
                }

                public string FullName => Name + "(" + Parameters.Aggregate("",
                                              (c, p) =>
                                                  $"{c}{(c != "" ? ", " : "")}{p.TypeName} {p.Name}") + ")";
            }
        }
    }
}
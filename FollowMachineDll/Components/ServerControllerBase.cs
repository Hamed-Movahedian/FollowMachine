using System;
using System.Collections;
using System.Collections.Generic;
using FollowMachineDll.Assets;
using MgsCommonLib;
using UnityEngine.Networking;

namespace FollowMachineDll.Components
{
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

        public static IEnumerator Send(ServerConnectionMethod method,string url, object outData, Action onSuccess, Action<UnityWebRequest> OnError)
        {
            throw new NotImplementedException();
        }

        public  abstract string GetOutputFollowControl();
    }

    public enum ServerConnectionMethod
    {
        Get,Post
    }
}
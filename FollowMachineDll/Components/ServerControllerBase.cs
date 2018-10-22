using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FollowMachineDll.Assets;
using MgsCommonLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace FollowMachineDll.Components
{
    public abstract class ServerControllerBase : MgsSingleton<ServerControllerBase>
    {
        public ServerData Data;
        public static string OutputFollowControl { get; private set; }

        #region PostRequst

        public static UnityWebRequest PostRequest(string url, object bodyData)
        {
            UnityWebRequest request = new UnityWebRequest(
                (Instance).URL + @"/api/" + url,
                "POST",
                new DownloadHandlerBuffer(),
                (bodyData == null) ? null : new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyData))));

            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        #endregion

        #region GetRequest

        public static UnityWebRequest GetRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(Instance.URL + @"/api/" + url);

            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
        #endregion

        #region Post


        public static IEnumerator Post<TReturnType>(string url, object bodyData,
            Action<TReturnType> onSuccess,
            Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = PostRequest(url, bodyData);

            request.Send();

            while (!request.isDone)
                yield return null;

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (onSuccess != null)
                    onSuccess(JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
            }
            else
            {
                if (onError != null)
                    onError(request);
            }
        }

        #endregion

        #region Get

        public static IEnumerator Get<TReturnType>(string url, Action<object> onSuccess)
        {
            UnityWebRequest request = GetRequest(url);

            request.Send();

            while (!request.isDone)
                yield return null;

            if (!request.isHttpError && !request.isNetworkError)
                if (onSuccess != null)
                    onSuccess(JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
        }


        #endregion

        public string URL
        {
            get
            {
                if (Data == null)
                    throw new Exception($"Server Data Not set!!!");
                return Data.URL;

            }
        }

        public List<string> MethodNames =>
            Data
                .Controllers
                .SelectMany(controller => controller.Methods.Select(methodData => controller.Name + "/" + methodData.Name))
                .ToList();

        public ServerData.Controller.MethodData GetMethodData(string methodFullName)
        {
            var i = methodFullName.IndexOf("/");

            var controllerName = methodFullName.Substring(0, i);
            var methodName = methodFullName.Substring(i + 1);

            return Data
                .Controllers
                .FirstOrDefault(c => c.Name == controllerName)
                .Methods
                .FirstOrDefault(m => m.Name == methodName);

        }

        public static IEnumerator Send(ServerConnectionMethod method, string url, object bodyData)
        {

            UnityWebRequest request =
                method == ServerConnectionMethod.Post ?
                PostRequest(url, bodyData) :
                GetRequest(url, bodyData);

            request.Send();

            while (!request.isDone)
                yield return null;

            if (request.isNetworkError)
                OutputFollowControl = "Network Error";
            else if (request.isHttpError)
                OutputFollowControl = "Network Error";
            else
            {
                Instance.Output = JObject.Parse(request.downloadHandler.text);
                OutputFollowControl = (string)Instance.Output["ControlOutput"] ?? "Success";
            }
        }

        public  JObject Output { get; set; }

        private static UnityWebRequest GetRequest(string url, object bodyData)
        {
            UnityWebRequest request = new UnityWebRequest(
                (Instance).URL + @"/api/" + url,
                "GET",
                new DownloadHandlerBuffer(),
                (bodyData == null) ? null : new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyData))));

            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

    }

    public enum ServerConnectionMethod
    {
        Get, Post
    }
}
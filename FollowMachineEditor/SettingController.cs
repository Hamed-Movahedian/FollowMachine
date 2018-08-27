using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FMachine.SettingScripts
{
    public class SettingController
    {
        #region Instance

        private static SettingController _instance;


        public static SettingController Instance
        {
            get { return _instance ?? (_instance = new SettingController()); }
        }

        #endregion

        private Dictionary<string, ScriptableObject> _dic;

        private SettingController()
        {
            _dic = new Dictionary<string, ScriptableObject>();
        }

        public ScriptableObject GetAsset(string name,Type type) 
        {
            if(_dic==null)
                _dic = new Dictionary<string, ScriptableObject>();

            if (!_dic.ContainsKey(name))
                _dic.Add(name,Load(name,type));

            return _dic[name];
        }

        private ScriptableObject Load(string name, Type type) 
        {
            var assets = AssetDatabase
                .FindAssets(name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path=>AssetDatabase.LoadAssetAtPath(path,type))
                .Where(a => a != null)
                .ToList();

            if (assets.Count == 0)
                throw new Exception(
                    string.Format("Can't find {0} Settings of type {1} !!!", name, type.Name));

            if (assets.Count > 1)
                throw new Exception(
                    string.Format("More than one {0} Settings of type {1} !!!", name, type.Name));

            return (ScriptableObject) assets[0];
        }

        public void Reset()
        {
            _dic = null;
        }
    }
}
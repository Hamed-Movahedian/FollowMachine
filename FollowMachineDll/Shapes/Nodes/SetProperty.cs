using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMachine;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using MgsCommonLib.Theme;
using UnityEngine;
using Object = System.Object;

namespace FollowMachineDll.Shapes.Nodes
{
    [Node(MenuTitle = "Action/SetProperty")]
    public class SetProperty : Node
    {
        public int PropertyCount = 1;

        public List<bool> DynamicValue = new List<bool>();

        public List<GameObject> PropertyGameObjects = new List<GameObject>();
        public List<string> PropertyString = new List<string>();

        public List<GameObject> ValueGameObjects = new List<GameObject>();
        public List<string> ValueString = new List<string>();


        protected override IEnumerator Run()
        {
            FollowMachine.SetOutput("");

            // set each property
            for (int i = 0; i < PropertyCount; i++)
            {
                // Get property
                PropertyInfo pInfo = GetPropetyInfo(PropertyGameObjects[i], PropertyString[i], out var pObject);

                if (pObject == null || pInfo == null)
                    continue;

                // Get Value
                object value = null;

                if (DynamicValue[i]) // Get Dynamic value
                {
                    value = GetDynamicValue(ValueGameObjects[i], ValueString[i]);
                }
                else // Get Static Value
                {
                    value = GetStaticValue(ValueString[i], pInfo.PropertyType);
                }

                // Set value to property
                pInfo.SetValue(pObject,value,null);
            }

            return null;
        }

        private object GetDynamicValue(GameObject valueGameObject, string valueText)
        {
            PropertyInfo pInfo = GetPropetyInfo(valueGameObject, valueText, out var pObject);

            if (pObject == null || pInfo == null)
                return null;

            return pInfo.GetValue(pObject, null);
        }

        private object GetStaticValue(string valueString, Type type)
        {

            if (type == typeof(int)) //*** int
            {
                if (!int.TryParse(valueString, out var value))
                    value = 0;

                return value;
            }
            else if (type == typeof(float)) //*** float
            {
                if (!float.TryParse(valueString, out var value))
                    value = 0;

                return value;
            }
            else if (type == typeof(bool)) //*** bool
            {
                if (!bool.TryParse(valueString, out var value))
                    value = false;

                return value;
            }
            else if (type == typeof(string)) //*** string
            {
                return ThemeManager.Instance.LanguagePack.GetLable(valueString);
            }
            else //*** Others
            {
                return null;
            }
        }

        private PropertyInfo GetPropetyInfo(GameObject pGameObject, string pText, out Object pObject)
        {
            pObject = null;

            // check game object isn't empty
            if (pGameObject == null)
                return null;

            // Extract component and property name
            if (pText == null) pText = "";

            var tList = pText.Split('.').ToList();

            if (tList.Count != 2)
                return null;

            var cName = tList[0];
            var pName = tList[1];

            // Get component
            var componentType = GetComponentType(cName, pGameObject);

            if (componentType == null)
                return null;

            // Get property
            var propertyInfo = componentType
                .GetProperties()
                .FirstOrDefault(pi => pi.Name == pName);

            if (propertyInfo == null)
                return null;

            // get property object

            if (componentType == typeof(GameObject))
                pObject = pGameObject;
            else
                pObject = pGameObject.GetComponent(componentType);

            if (pObject == null)
                return null;

            // get value object
            return propertyInfo;
        }

        #region GetComponentType

        protected Type GetComponentType(string cName, GameObject tGameObject)
        {
            Type _componentType = null;

            if (tGameObject != null)
            {
                if (cName == "GameObject")
                    return typeof(GameObject);

                List<Type> componentsTypes =
                    tGameObject
                        .GetComponents<Component>()
                        .Select(c => c.GetType()).ToList();

                List<string> componentTypeNames = componentsTypes.Select(ct => ct.Name).ToList();

                int currentTypeIndex = componentTypeNames.IndexOf(cName);

                if (currentTypeIndex != -1)
                    _componentType = componentsTypes[currentTypeIndex];

            }

            return _componentType;
        }

        #endregion

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }

    }
}

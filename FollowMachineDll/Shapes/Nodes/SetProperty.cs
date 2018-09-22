using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FMachine;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Attributes;
using FollowMachineDll.Utility;
using MgsCommonLib.Theme;
using MgsCommonLib.Utilities;
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

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

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

        public override void OnInspector()
        {
            base.OnInspector();

            // Set count
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("-"))
                    PropertyCount--;
                if (GUILayout.Button("+"))
                    PropertyCount++;

                if (PropertyCount < 0)
                    PropertyCount = 0;
            }
            GUILayout.EndHorizontal();

            // set list sizes
            PropertyGameObjects.Resize(PropertyCount);
            PropertyString.Resize(PropertyCount);

            ValueGameObjects.Resize(PropertyCount);
            ValueString.Resize(PropertyCount);

            DynamicValue.Resize(PropertyCount);

            // Display each property
            for (int i = 0; i < PropertyCount; i++)
            {
                GUILayout.Space(10);
                //GUILayout.Label("--------------------");
                GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(4));
                GUILayout.Space(10);

                // ************** Get property
                var propertyGameObject = PropertyGameObjects[i];
                var pText = PropertyString[i];

                var propertyInfo = EditorTools.Instance.GetDynamicParameter(
                    gameObject, ref propertyGameObject, ref pText, null);

                PropertyGameObjects[i] = propertyGameObject;
                PropertyString[i] = pText;

                if (propertyInfo == null)
                    continue;

                // ************** Get value

                // Dynamic Toggle
                var toggle = GUILayout.Toggle(DynamicValue[i], "Dynamic");
                if (toggle != DynamicValue[i])
                {
                    EditorTools.Instance.Undo_RecordObject(this, "Change Parameter");
                    DynamicValue[i] = toggle;
                    ValueString[i] = "";
                }

                if (toggle) // Dynamic Value
                {
                    propertyGameObject = ValueGameObjects[i];
                    pText = ValueString[i];

                    EditorTools.Instance.GetDynamicParameter(
                        gameObject, ref propertyGameObject, ref pText, propertyInfo.PropertyType);

                    ValueGameObjects[i] = propertyGameObject;
                    ValueString[i] = pText;
                }
                else // Static Value
                {
                    ValueString[i] =
                        EditorTools.Instance.GetParameter(this, "Value", propertyInfo.PropertyType, ValueString[i]);
                }

            }
        }
    }
}

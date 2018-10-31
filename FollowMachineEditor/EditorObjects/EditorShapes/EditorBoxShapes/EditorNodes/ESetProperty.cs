using System.Collections.Generic;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using MgsCommonLib.Utilities;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class ESetProperty : ENode
    {
        private readonly SetProperty _setProperty;

        public ESetProperty(SetProperty setProperty) : base(setProperty)
        {
            _setProperty = setProperty;
        }

        public int PropertyCount
        {
            get => _setProperty.PropertyCount;
            set => _setProperty.PropertyCount = value;
        }

        public List<bool> DynamicValue
        {
            get => _setProperty.DynamicValue;
            set => _setProperty.DynamicValue = value;
        }


        public List<GameObject> PropertyGameObjects
        {
            get => _setProperty.PropertyGameObjects;
            set => _setProperty.PropertyGameObjects = value;
        }

        public List<string> PropertyString
        {
            get => _setProperty.PropertyString;
            set => _setProperty.PropertyString = value;
        }


        public List<GameObject> ValueGameObjects
        {
            get => _setProperty.ValueGameObjects;
            set => _setProperty.ValueGameObjects = value;
        }

        public List<string> ValueString
        {
            get => _setProperty.ValueString;
            set => _setProperty.ValueString = value;
        }
        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }
        public override void OnInspector()
        {
            base.OnInspector();

            return;
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
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(4));
                GUILayout.Space(10);

                // ************** Get property
                var propertyGameObject = PropertyGameObjects[i];
                var pText = PropertyString[i];

                var propertyInfo = EditorTools.Instance.GetDynamicParameter(
                    _setProperty.gameObject, ref propertyGameObject, ref pText, null);

                PropertyGameObjects[i] = propertyGameObject;
                PropertyString[i] = pText;

                if (propertyInfo == null)
                    continue;

                // ************** Get value

                // Dynamic Toggle
                var toggle = GUILayout.Toggle(DynamicValue[i], "Dynamic");
                if (toggle != DynamicValue[i])
                {
                    EditorTools.Instance.Undo_RecordObject(_setProperty, "Change Parameter");
                    DynamicValue[i] = toggle;
                    ValueString[i] = "";
                }

                if (toggle) // Dynamic Value
                {
                    propertyGameObject = ValueGameObjects[i];
                    pText = ValueString[i];

                    EditorTools.Instance.GetDynamicParameter(
                        _setProperty.gameObject, ref propertyGameObject, ref pText, propertyInfo.PropertyType);

                    ValueGameObjects[i] = propertyGameObject;
                    ValueString[i] = pText;
                }
                else // Static Value
                {
                    ValueString[i] =
                        EditorTools.Instance.GetParameter(_setProperty, "Value", propertyInfo.PropertyType, ValueString[i]);
                }

            }
        }

    }
}

using System.Collections.Generic;
using Bind;
using BindEditor;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineEditor.CustomInspectors;
using MgsCommonLib.Utilities;
using UnityEditor;
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

/*            for (int i = 0; i < PropertyCount; i++)
            {
                GUILayout.Label("property");
                PropertyGameObjects[i] = (GameObject)EditorGUILayout.ObjectField("", PropertyGameObjects[i], typeof(GameObject), true);
                GUILayout.Label(PropertyGameObjects[i].name + "." + PropertyString[i]);
                GUILayout.Label("Value");
                if (DynamicValue[i])
                    ValueGameObjects[i] = (GameObject)EditorGUILayout.ObjectField("", ValueGameObjects[i], typeof(GameObject), true);
                GUILayout.Label(DynamicValue[i] ?
                        $"Bound to {ValueGameObjects[i].name}.{ValueString[i]}" :
                        $"Const {ValueString[i]}");
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }*/

            foreach (var assinment in _setProperty.Assinments)
            {
                assinment.AssinmentGUI();
                if (GUILayout.Button("Remove"))
                {
                    _setProperty.Assinments.Remove(assinment);
                    return;
                }
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add"))
                _setProperty.Assinments.Add(new Assinment());
        }

    }
}

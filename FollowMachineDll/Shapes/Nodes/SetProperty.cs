using System.Collections;
using System.Collections.Generic;
using Bind;
using FMachine;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Attributes;
using UnityEngine;

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

        public List<Assinment> Assinments = new List<Assinment>();

        void OnValidate()
        {
            if (PropertyCount > 0)
            {
                Assinments.Clear();

                for (int i = 0; i < PropertyCount; i++)
                {
                    Assinments.Add(new Assinment(
                        PropertyGameObjects[i],
                        PropertyString[i],
                        DynamicValue[i],
                        ValueGameObjects[i],
                        ValueString[i]));
                }

/*
                PropertyCount = 0;
                PropertyGameObjects.Clear();
                PropertyString.Clear();
                DynamicValue.Clear();
                ValueGameObjects.Clear();
                ValueString.Clear();

                Debug.Log("SetProperty Transferd");
*/
            }
        }
        protected override IEnumerator Run()
        {
            FollowMachine.SetOutput("");

            Assinments.ForEach(a=>a.Assign());

            return null;
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }

    }
}

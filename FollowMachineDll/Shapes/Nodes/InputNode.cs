using System;
using FMachine.SettingScripts;
using FMachine.Shapes.Sockets;
using UnityEngine;

namespace FMachine.Shapes.Nodes
{
    public class InputNode : Node
    {
        protected override void Initialize()
        {
            AddOutputSocket<InputSocket>("");
            Info = "Input";
        }

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();
        }
    }
}
using FMachine.Shapes.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FollowMachineDll.Attributes;
using FMachine.Shapes.Sockets;
using System.Collections;
using UnityEngine;
using FollowMachineDll.Utility;

namespace FollowMachineDll.Shapes.Nodes
{

    [Node(MenuTitle = "Animation")]
    public class AnimationNode : Node
    {
        public Animator Animator;

        public override Node GetNextNode()
        {
            return OutputSocketList[0].GetNextNode();

        }

        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(this, "Animator");

            if (Animator==null)
            {
                return;
            }
            string[] animationStates = EditorTools.Instance.GetAnimationStates(Animator);

            EditorTools.Instance.Popup("States", 0,
                animationStates);
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        protected override IEnumerator Run()
        {
            return base.Run();
        }
    }
}

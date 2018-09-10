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

        public string StartState;
        public string EndState;

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
            List<string> animationStates = EditorTools.Instance.GetAnimationStates(Animator).ToList();


            // *************** Start state
            int startStateIndex = animationStates.IndexOf(StartState);

            if (startStateIndex == -1)
                startStateIndex = 0;

            string index =
                animationStates[EditorTools.Instance.Popup("Start State", startStateIndex, animationStates.ToArray())];

            if (StartState != index)
            {
                EditorTools.Instance.Undo_RecordObject(this, "Change Start State");
                StartState = index;

            }

            // *************** End state
            int endStateIndex = animationStates.IndexOf(EndState);

            if (endStateIndex == -1)
                endStateIndex = 0;

            index =
                animationStates[EditorTools.Instance.Popup("End State", endStateIndex, animationStates.ToArray())];

            if (EndState != index)
            {
                EditorTools.Instance.Undo_RecordObject(this, "Change End State");
                EndState = index;

            }
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }

        protected override IEnumerator Run()
        {
            if (Animator == null)
                throw new Exception("Animator is not set in " + Info);

            Animator.Play(StartState);

            yield return new WaitWhile(() =>
                    !Animator.GetCurrentAnimatorStateInfo(0).IsName(StartState));

            yield return new WaitWhile(() =>
                    !Animator.GetCurrentAnimatorStateInfo(0).IsName(EndState));

        }
    }
}

using FMachine.Shapes.Nodes;
using System;
using FollowMachineDll.Attributes;
using System.Collections;
using UnityEngine;

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

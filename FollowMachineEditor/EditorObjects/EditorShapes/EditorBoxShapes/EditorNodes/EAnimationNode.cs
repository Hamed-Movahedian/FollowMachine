using System.Collections.Generic;
using System.Linq;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class EAnimationNode : ENode
    {
        public Animator Animator
        {
            get => _animationNode.Animator;
            set => _animationNode.Animator = value;
        }

        public string StartState
        {
            get => _animationNode.StartState;
            set => _animationNode.StartState = value;
        }

        public string EndState
        {
            get => _animationNode.EndState;
            set => _animationNode.EndState = value;
        }


        private AnimationNode _animationNode;

        public EAnimationNode(AnimationNode animationNode) : base(animationNode)
        {
            _animationNode = animationNode;
        }
        public override void OnInspector()
        {
            base.OnInspector();
            EditorTools.Instance.PropertyField(_animationNode, "Animator");

            if (Animator == null)
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
                EditorTools.Instance.Undo_RecordObject(_animationNode, "Change Start State");
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
                EditorTools.Instance.Undo_RecordObject(_animationNode, "Change End State");
                EndState = index;

            }
        }

        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }


    }
}

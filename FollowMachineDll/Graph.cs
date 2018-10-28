using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Base;
using FollowMachineDll.Shapes;
using UnityEngine;

namespace FMachine
{
    public class Graph : MonoBehaviour
    {
        public EObject EObject;
        #region Public

        [HideInInspector]
        public Vector2 Position = new Vector2(-50000, -50000);
        [HideInInspector]
        public float Zoom = 1;
        [HideInInspector]
        public List<Node> NodeList = new List<Node>();
        [HideInInspector]
        public List<Group> GroupList = new List<Group>();
        [HideInInspector]
        public Node LastRunningNode;

        #endregion

        #region RunningNode
        private Node _runningNode;
        public Node RunningNode
        {
            get => _runningNode;
            set
            {
                LastRunningNode = _runningNode;
                _runningNode = value;
            }
        }
        #endregion

    }
}
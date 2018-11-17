using System.Collections;
using FMachine.Shapes.Nodes;
using UnityEngine;

namespace FMachine
{
    public class FollowMachine : Graph
    {
        private static string _outputLable;

        [HideInInspector]
        public bool IsRunning => _threadCount>0;

        private int _threadCount=0;

        public IEnumerator RunNode(Node node)
        {
            _threadCount++;

            while (node != null)
            {
                yield return node.RunBase();
                node = node.GetNextNode();
            }

            if (_threadCount > 0)
                _threadCount--;
        }

        public static void SetOutput(string outputLable)
        {
            _outputLable = outputLable.ToLower().Trim();
        }

        public static bool CheckOutputLable(string lable)
        {
            return _outputLable == lable.ToLower().Trim();
        }

        public IEnumerator RunInputNode(string inputLable)
        {
            foreach (Node node in NodeList)
                if (node is InputNode)
                    if(node.Info==inputLable)
                        return RunNode(node);
            return null;
        }
    }
}
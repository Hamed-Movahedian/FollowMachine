using System;
using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Nodes;

namespace FMachine
{
    public class FollowMachine : Graph
    {
        private static string _outputLable;


        private void Start()
        {
            foreach (Node node in NodeList)
                if (node is StartNode)
                    StartCoroutine(RunNode(node));
        }

        public IEnumerator RunNode(Node node)
        {
            RunningNode = node;
            while (RunningNode != null)
            {
                yield return RunningNode.RunBase();
                LastRunningNode = RunningNode;
                RunningNode = RunningNode.GetNextNode();
            }
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
                    if(node.Name==inputLable)
                        return RunNode(node);
            return null;
        }
    }
}